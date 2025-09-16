import { useState, useContext, useCallback, useEffect, useRef } from 'react';
import { deleteSplitWorksheets, deleteNamedSplitWorksheets, deleteAllWorksheetsExceptReport } from './excelSheetUtils';
import {
    CalendarDefinition,
    CalendarOverlayDefinition,
    HeaderDefinition,
    MediaHierarchyDefinition,
    GrandTotalDefinition,
    TotalsDefinition,
    ThemeDefinition,
    FooterDefinition,
    ReferencedMediaHierarchyDefinition,
    ReferencedCalendarDefinition,
    ReferencedTotalsDefinition,
    ReferencedHeaderDefinition,
    ReferencedGrandTotalDefinition,
    RunRestriction,
    FlowchartTemplateDetailsDTO,
    HeaderLogoAlignment,
    FlightRange, ReferencedThemeDefinition, CalendarType,
} from '@omniflow/omni-webapi';
import * as API from '@omniflow/omni-webapi';

import { Block } from '../business/engine/models/block';
import { BuildEngine } from '../business/engine/models/build-engine';
import { HeaderBuilder } from '../business/engine/builder/header-builder';
import { HeaderDetailsBuilder } from '../business/engine/builder/header-details-builder';
import { CalendarBuilder } from '../business/engine/builder/calendar-builder';
import { CalendarOverlayBuilder } from '../business/engine/builder/calendar-overlay-builder';
import { MediaHierarchyBuilder } from '../business/engine/builder/media-hierarchy-builder';
import { MediaHierarchyDefinitionWithId } from '../pages/media-hierarchy/states/MediaHierarchyState';
import { RightHandTotalsBuilder } from '../business/engine/builder/right-hand-totals-builder';
import { GrandTotalsBuilder } from '../business/engine/builder/grand-totals-builder';
import { StyleMapper } from '../business/engine/models/styles';
import { ExcelRenderer } from '../business/engine/renderer/excel-renderer';
import useNotification, {
    NotificationType,
} from '../components/notification/useNotification';
import { AppContext } from '../taskpane/contexts/AppContext';
import { FooterBuilder } from '../business/engine/builder/footer-builder';
import { FooterLegendBuilder } from '../business/engine/builder/footer-legend-builder';
import { TDefinition, TDefinitionName } from '../interfaces/definition.type';
import HeadBlock from '../business/engine/blocks/right-hand-totals/head-block';
import ValueBlock from '../business/engine/blocks/right-hand-totals/value-block';
import TotalBlock from '../business/engine/blocks/right-hand-totals/total-block';
import LeftMenuSubTotalBlock from '../business/engine/blocks/media-hierarchy/left-menu-sub-total-block';
import FlightBarWallSubTotalBlock from '../business/engine/blocks/media-hierarchy/flight-bar-wall-sub-total-block';
import LeftMenuSettingBlock from '../business/engine/blocks/media-hierarchy/left-menu-setting-block';
import { CalendarHelper } from '../business/engine/helpers/calendar-helper';
import { dataApi } from '../lib/api';
import { getDataCached } from '../lib/data-cache';
import {
    flatLevels,
    TFlowchartDataLevelFlat,
} from '../business/engine/helpers/level-helper';
import * as HeaderHelper from '../business/engine/helpers/header-helper';
import { THeaderRowData } from '../business/engine/helpers/header-helper';
import { DataContext } from '../taskpane/contexts/DataContext';
import CalendarOverlayLeftMenuLevelBlock
    from '../business/engine/blocks/calendar-overlay/calendar-overlay-left-menu-level-block';
import CalendarOverlayEntryBlock from '../business/engine/blocks/calendar-overlay/calendar-overlay-entry-block';
import FlightBarWallBarBlock from '../business/engine/blocks/media-hierarchy/flight-bar-wall-bar-block';
import LeftMenuLevelBlock from '../business/engine/blocks/media-hierarchy/left-menu-level-block';
import LeftMenuSubLevelBlock from '../business/engine/blocks/media-hierarchy/left-menu-sub-level-block';
import FlightBarWallInflightOverlayBlock
    from '../business/engine/blocks/media-hierarchy/flight-bar-wall-inflight-overlay-block';
import { BriefedCtcMetricColumnName } from '../business/engine/constant/common';
import { convertLetterToNumber } from '../business/convert-letter-to-number';
import { precisionValue } from '../enums/precision-value.enum';
import { groupBy, isEmpty } from 'lodash';
import MainHeadBlock from '../business/engine/blocks/right-hand-totals/main-head-block';
import { splitFlightBars } from '../business/engine/helpers/media-hierarchy-overlap-data-helper';
import { LegendBuilder } from '../business/engine/builder/legend-builder';
import moment from 'moment';
import { filterFlatLevelData } from './filterFlatLevelData';
interface IDefsAndRefs {
    headerDefinition: HeaderDefinition;
    calendarDefinition: CalendarDefinition;
    calendarOverlayDefinition: CalendarOverlayDefinition;
    mediaHierarchyDefinition: MediaHierarchyDefinition;
    totalsDefinition: TotalsDefinition;
    grandTotalsDefinition: GrandTotalDefinition;
    themeDefinition: ThemeDefinition;
    footerDefinition: FooterDefinition;
    mediaHierarchyRef: ReferencedMediaHierarchyDefinition;
    calendarRef: ReferencedCalendarDefinition;
    totalsRef: ReferencedTotalsDefinition;
    headerRef: ReferencedHeaderDefinition;
    grandTotalsRef: ReferencedGrandTotalDefinition;
    themeRef: ReferencedThemeDefinition;
}


const useRenderWithDefinition = () => {
    const customStylesKey = "customstyles";
    const runConfigCustomStylesKey = "runCustomStyles";
    const [isRenderingWithDefinition, setIsRenderingWithDefinition] = useState(false);
    const appContext = useContext(AppContext);
    const dataContext = useContext(DataContext);
    const {
        setCurrentHeaderRowData,
        setCurrentCalendarData,
        setCurrentLevelFlowchartData,
        setCurrentFlatLevelData,
        briefedctcColumnSelected,
        setBriefedctcColumnSelected,
        setIsGlobalRendering,
        currentLevelFlowchartData,
    } = dataContext;
    const [persistedLevelFlowchartData, setPersistedLevelFlowchartData] = useState<API.FlowchartData | null>(null);

    const { columns, setCurrentExcelContext, currentMediaHierarchyDetails } = appContext;
    const { OmniClientId: clientId } = appContext.flowchartTemplateDefinition;
    const pushNotification = useNotification();
    const [flowchartTemplateDefinition, setFlowchartTemplateDefinition] = useState<FlowchartTemplateDetailsDTO>(null);
    const prevSplitColumnNameRef = useRef<string>(appContext?.currentThemeTemplateDetails?.Definition?.SplitByColumn?.ColumnName ?? "");
    useEffect(() => {
        setPersistedLevelFlowchartData(null);
    }, [appContext.currentMediaHierarchyDetails, appContext.currentHeaderTemplateDetails, appContext.currentCalendarTemplateDetails, appContext.currentTotalsDetails, appContext.currentGrandTotalsDetails, appContext.currentThemeTemplateDetails, appContext.currentFooterTemplateDetails]);
    const _getReferencedDefinition = (contextDefinition: any) => {
        return {
            Definition: contextDefinition?.Definition,
            TemplateId: contextDefinition?.Id,
            TemplateVersion: contextDefinition?.Version,
        };
    };

    const _getDefAndRef = (contextData: any, flowChartData: any) => {
        const def = contextData?.Definition ?? flowChartData?.Definition;
        const ref =
            contextData?.Definition && contextData?.Id
                ? _getReferencedDefinition({ ...contextData, Definition: def })
                : flowChartData;
        return { def, ref };
    };

    const overwriteDef = (
        overwriteWith: TDefinition | MediaHierarchyDefinitionWithId,
        currDef: TDefinition | MediaHierarchyDefinitionWithId,
        currRef?: any
    ) => {
        currDef = overwriteWith ?? currDef;
        if (currDef && currRef) {
            currRef = {
                ...currRef,
                Definition: currDef,
            };
        } else {
            currRef = undefined;
        }
        return { newDef: currDef, newRef: currRef };
    };

    const loadCurrentDefinitions = useCallback(
        (
            definitionName?: TDefinitionName,
            definition?: TDefinition | MediaHierarchyDefinitionWithId
        ): IDefsAndRefs => {
            const flowchart = {
                ...appContext.flowchartTemplateDefinition?.Definition,
            };

            let { def: headerDefinition, ref: headerRef } = _getDefAndRef(
                appContext.currentHeaderTemplateDetails,
                flowchart?.HeaderDefinition
            );
            let { def: calendarDefinition, ref: calendarRef } = _getDefAndRef(
                appContext.currentCalendarTemplateDetails,
                flowchart?.CalendarDefinition
            );
            let { def: calendarOverlayDefinition } = _getDefAndRef(
                appContext.currentCalendarOverlayTemplateDetails,
                flowchart?.CalendarOverlayDefinition
            );
            let { def: mediaHierarchyDefinition, ref: mediaHierarchyRef } =
                _getDefAndRef(
                    appContext.currentMediaHierarchyDetails,
                    flowchart?.MediaHierarchyDefinition
                );
            let { def: totalsDefinition, ref: totalsRef } = _getDefAndRef(
                appContext.currentTotalsDetails,
                flowchart?.TotalsDefinition
            );
            let { def: grandTotalsDefinition, ref: grandTotalsRef } =
                _getDefAndRef(
                    appContext.currentGrandTotalsDetails,
                    flowchart?.GrandTotalDefinition
                );
            let { def: themeDefinition, ref: themeRef } = _getDefAndRef(
                appContext.currentThemeTemplateDetails,
                flowchart?.ThemeDefinition
            );
            let { def: footerDefinition } = _getDefAndRef(
                appContext.currentFooterTemplateDetails,
                flowchart?.FooterDefinition
            );

            // Overwrite definition i.e. rendering during edit
            switch (definitionName) {
                case 'calendarDefinition':
                    const { newDef: calDef, newRef: calRef } = overwriteDef(
                        definition,
                        calendarDefinition,
                        calendarRef
                    );
                    calendarDefinition = calDef;
                    calendarRef = calRef;
                    break;
                case 'calendarOverlayDefinition':
                    const { newDef: overlayDef } = overwriteDef(
                        definition,
                        calendarOverlayDefinition
                    );
                    calendarOverlayDefinition = overlayDef;
                    break;
                case 'headerDefinition':
                    const { newDef: hDef, newRef: hRef } = overwriteDef(
                        definition,
                        headerDefinition,
                        headerRef
                    );
                    headerDefinition = hDef;
                    headerRef = hRef;
                    break;
                case 'mediaHierarchyDefinition':
                    const { newDef: mhDef, newRef: mhRef } = overwriteDef(
                        definition,
                        mediaHierarchyDefinition,
                        mediaHierarchyRef
                    );
                    mediaHierarchyDefinition = mhDef;
                    mediaHierarchyRef = mhRef;
                    //debugger;
                    break;
                case 'totalsDefinition':
                    const { newDef: tDef, newRef: tRef } = overwriteDef(
                        definition,
                        totalsDefinition,
                        totalsRef
                    );
                    totalsDefinition = tDef;
                    totalsRef = tRef;
                    break;
                case 'grandTotalsDefinition':
                    const { newDef: gtDef, newRef: gtRef } = overwriteDef(
                        definition,
                        grandTotalsDefinition,
                        grandTotalsRef
                    );
                    grandTotalsDefinition = gtDef;
                    grandTotalsRef = gtRef;
                    break;
                case 'themeDefinition':
                    const { newDef: themeDef, newRef: thRef } = overwriteDef(
                        definition,
                        themeDefinition,
                        themeRef,
                    );
                    themeDefinition = themeDef;
                    themeRef = thRef;
                    break;
                case 'footerDefinition':
                    const { newDef: footerDef } = overwriteDef(
                        definition,
                        footerDefinition
                    );
                    footerDefinition = footerDef;
                    break;
            }

            return {
                calendarDefinition,
                calendarOverlayDefinition,
                headerDefinition,
                mediaHierarchyDefinition,
                totalsDefinition,
                grandTotalsDefinition,
                themeDefinition,
                footerDefinition,
                mediaHierarchyRef,
                calendarRef,
                totalsRef,
                headerRef,
                grandTotalsRef,
                themeRef,
            };
        },
        [appContext]
    );

    const getAllBlocks = useCallback(
        async (
            definitionName?: TDefinitionName,
            definition?: TDefinition | MediaHierarchyDefinitionWithId,
            restrictions?: RunRestriction[],
            splitLevelIds?: string[],
        ): Promise<[allBlocks: Block[][],
            precisedValue: boolean, isDecimal: boolean, levelFlowchartData: API.FlowchartData]> => {
            // Init builder
            const builder = new BuildEngine();
            const latestMediaHierarchyDetails = appContext.getCurrentMediaHierarchyDetails();
            const showSubtotalsAtBottom = latestMediaHierarchyDetails?.ShowSubTotalsAtBottom ??
                appContext.mediaHierarchyTemplate?.ShowSubTotalsAtBottom;

            // Load definitions
            const {
                headerDefinition,
                calendarDefinition,
                calendarOverlayDefinition,
                themeDefinition,
                footerDefinition,
                mediaHierarchyRef,
                calendarRef,
                totalsRef,
                headerRef,
                grandTotalsRef,
                mediaHierarchyDefinition,
                themeRef,
            } = loadCurrentDefinitions(definitionName, definition);
            const flowchartDefinition = flowchartTemplateDefinition;

            // Load header row data
            let headerRowData: THeaderRowData = null;
            if (clientId && headerDefinition) {
                headerRowData = await HeaderHelper.getRowValues(
                    clientId,
                    { ...flowchartDefinition?.Definition, MediaHierarchyDefinition: mediaHierarchyRef }
                );
                setCurrentHeaderRowData(headerRowData);
            }

            // Init Calendar Helper and load calendar data
            let calendarHelper: CalendarHelper = null;
            if (clientId && calendarRef) {
                calendarHelper = new CalendarHelper(
                    clientId,
                    calendarRef?.Definition?.Configuration
                );
                const calendarData = await calendarHelper.getData();
                setCurrentCalendarData(calendarData);
            }

            let normalCalendarHelper: CalendarHelper = null;
            const normalCalendarDefinition: API.CalendarDefinition = {
                ...calendarDefinition,
                Configuration: {
                    ...calendarDefinition?.Configuration,
                    Type: API.CalendarType.Standard,
                    ReportingTimeFrame: calendarDefinition?.Configuration?.ReportingTimeFrame,
                    IsReportingTimeFrame: calendarDefinition?.Configuration?.IsReportingTimeFrame,
                    Rows: [
                        {
                            ColumnName: "Week",
                            StandardCalenderRowType: API.StandardCalenderRowType.Week,
                            Order: 0,
                            Style: API.CalendarRowStyle.WeekDD
                        },
                        {
                            ColumnName: "Month",
                            StandardCalenderRowType: API.StandardCalenderRowType.Month,
                            Order: 1,
                            Style: API.CalendarRowStyle.MonthMMM
                        },
                    ]
                }
            };
            if (clientId && calendarRef) {

                //const data = { ...calendarRef?.Definition?.Configuration, Type: CalendarType.Standard};
                normalCalendarHelper = new CalendarHelper(
                    clientId,
                    normalCalendarDefinition?.Configuration
                );

                const calendarData = await normalCalendarHelper.getData();
            }

            const getBriefedCtcColumnSelected = () => {
                if ((appContext?.currentThemeTemplateDetails?.Definition?.Styling?.ShowBriefedCTC?.ShowBriefedCTCValue === undefined && (appContext?.flowchartTemplateDefinition?.Definition?.ThemeDefinition?.Definition?.Styling?.ShowBriefedCTC?.ShowBriefedCTCValue === undefined || appContext?.flowchartTemplateDefinition?.Definition?.ThemeDefinition?.Definition?.Styling?.ShowBriefedCTC?.ShowBriefedCTCValue)) || (appContext?.currentThemeTemplateDetails?.Definition?.Styling?.ShowBriefedCTC?.ShowBriefedCTCValue === undefined && appContext?.flowchartTemplateDefinition?.Definition?.ThemeDefinition?.Definition?.Styling?.ShowBriefedCTC?.ShowBriefedCTCValue)) {
                    return true;
                }
                return appContext?.currentThemeTemplateDetails?.Definition?.Styling?.ShowBriefedCTC?.ShowBriefedCTCValue || (appContext?.currentThemeTemplateDetails?.Definition?.Styling?.ShowBriefedCTC?.ShowBriefedCTCValue === undefined && appContext?.flowchartTemplateDefinition?.Definition?.ThemeDefinition?.Definition?.Styling?.ShowBriefedCTC?.ShowBriefedCTCValue);
            }

            // Load level data and flat it
            let levelFlowchartData: API.FlowchartData;
            let flatLevelData: TFlowchartDataLevelFlat[];
            let currentRenderBriefedCtcSelected = briefedctcColumnSelected;
            let weeklyFlightRange = false;
            let precisedValue = themeDefinition?.Styling?.Precision?.PrecisionValue === precisionValue.Summarized;
            let isDecimal = themeDefinition?.Styling?.Precision?.PrecisionValue === precisionValue.Decimal;
            const themeLegendColumn = themeDefinition?.LegendTheme?.ColumnName;
            if (clientId && calendarRef && mediaHierarchyRef) {

                if (levelFlowchartData == null) {
                    const levelApiData = await getDataCached({
                        OmniClientId: clientId,
                        FlowchartDefinition: {
                            HeaderDefinition: headerRef,
                            CalendarDefinition: calendarRef,
                            MediaHierarchyDefinition: mediaHierarchyRef,
                            TotalsDefinition: totalsRef,
                            GrandTotalDefinition: grandTotalsRef,
                            ThemeDefinition: themeRef,
                        },
                        RunRestrictions: restrictions ?? [] as RunRestriction[],
                        Currency: currentMediaHierarchyDetails?.Currency || "LLL",
                        ShowBriefedCTC: getBriefedCtcColumnSelected(),
                        PreviousData: null,
                        UseStaticFilter: false
                    });
                    //Added data check according to calendar startdate and enddate
                    const handleEmptyData = (calendarHelper: CalendarHelper) => {
                        const startDate = calendarHelper.calendarFrom;
                        const endDate = calendarHelper.calendarTo;

                        const message = `No data available for the selected date range (${startDate.toLocaleDateString()} - ${endDate.toLocaleDateString()}). Please adjust dates and/or media hierarchy to see the data.`;
                        pushNotification(message, NotificationType.WARNING, 10000);
                    }
                    if (!levelApiData?.data?.Levels || levelApiData.data.Levels.length === 0) {
                        const calendarHelper = new CalendarHelper(clientId, calendarRef?.Definition?.Configuration);
                        handleEmptyData(calendarHelper);
                    }
                    levelFlowchartData = levelApiData?.data;
                    setCurrentLevelFlowchartData(levelFlowchartData);
                    setPersistedLevelFlowchartData(levelFlowchartData);
                    flatLevelData = flatLevels(
                        levelFlowchartData?.Levels || [],
                        mediaHierarchyRef
                    );

                    // split if legend selected
                    const allColumns = new Set(flatLevelData.map(c => c.ColumnName));

                    const legendTheme = appContext.currentThemeTemplateDetails?.Definition?.LegendTheme;
                    if (legendTheme?.Settings?.length > 0 && allColumns.size > 0 && !allColumns.has(legendTheme.ColumnName)) {
                        flatLevelData = splitFlightBars(flatLevelData);
                    }

                    setCurrentFlatLevelData(flatLevelData);
                    currentRenderBriefedCtcSelected = flatLevelData && flatLevelData.findIndex(l => l.ColumnName.toLowerCase() == BriefedCtcMetricColumnName) > -1;
                    setBriefedctcColumnSelected(currentRenderBriefedCtcSelected);
                    weeklyFlightRange = (flatLevelData.some(levelTotal => levelTotal.FlightRange == FlightRange.Weekly || levelTotal.FlightRange == FlightRange.WeeklyBroadcast || levelTotal.SubTotals?.some(subTotals => subTotals.FlightRange == FlightRange.Weekly || subTotals.FlightRange == FlightRange.WeeklyBroadcast) || levelTotal.SubLevels?.some(subLevelTotal => subLevelTotal.FlightRange == FlightRange.Weekly || subLevelTotal.FlightRange == FlightRange.WeeklyBroadcast)) ||
                        levelFlowchartData.GrandTotals?.some(grandTotal => grandTotal.FlightRange == FlightRange.Weekly || grandTotal.FlightRange == FlightRange.WeeklyBroadcast)) ?? false;
                }
            }
            const briefedCtcSelected = currentRenderBriefedCtcSelected && getBriefedCtcColumnSelected();

            // BEWARE: Order of builders is relevant here!
            // First call media hierarchy, calendar overlay, totals, then calendar, then header, then grand totals

            // Build media hierarchy blocks
            const { filteredFlatLevelData, ancestorSet } = filterFlatLevelData(flatLevelData, splitLevelIds);
            const mediaHierarchyBuilder = new MediaHierarchyBuilder({
                clientId,
                columns,
                ReferencedMediaHierarchyDefinition: mediaHierarchyRef,
                FlatLevelData: splitLevelIds?.length ? filteredFlatLevelData : flatLevelData,
                ReferencedCalendarDefinition: calendarRef,
                CalendarHelper: calendarHelper,
                ReferencedHeaderDefinition: headerRef,
                BriefedctcColumnSelected: briefedCtcSelected,
                precisedValue: themeDefinition?.Styling?.Precision?.PrecisionValue === precisionValue.Summarized,
                isDecimal: themeDefinition?.Styling?.Precision?.PrecisionValue === precisionValue.Decimal,
                ApplyToCurrencyMetric: themeDefinition?.Styling?.Precision?.ApplyToCurrencyMetric ?? true,
                ApplyToNonCurrencyMetric: themeDefinition?.Styling?.Precision?.ApplyToNonCurrencyMetric ?? true,
                currencyCode: currentMediaHierarchyDetails?.Currency,
                showSubtotalsAtBottom: showSubtotalsAtBottom,
                themeLegendColumn,
                inflightOverlayMetrics: appContext.inflightOverlayMetrics,
                NormalCalendarHelper: normalCalendarHelper,
            });
            let mediaHierarchyBlocks = await mediaHierarchyBuilder.build();
            const {
                leftMenuColumnLength,
                flightBarWallColumnLength,
                legendData,
            } = mediaHierarchyBuilder;

            // Build calendar overlay blocks
            const calendarOverlayBuilder = new CalendarOverlayBuilder({
                clientId,
                CalendarOverlayDefinition: calendarOverlayDefinition,
                CalendarDefinition: calendarDefinition,
                CalendarHelper: calendarHelper,
                titleColumns: leftMenuColumnLength,
                BriefedctcColumnSelected: briefedCtcSelected,
            });
            let calendarOverlayBlocks = await calendarOverlayBuilder.build();
            const {
                overlayFlightBarWallRowLength,
                overlayLeftMenuColumnLength,
            } = calendarOverlayBuilder;

            const leftSpace =
                overlayLeftMenuColumnLength > leftMenuColumnLength
                    ? overlayLeftMenuColumnLength
                    : leftMenuColumnLength;

            // Build totals blocks
            const totalsBuilder = new RightHandTotalsBuilder({
                clientId,
                ReferencedTotalsDefinition: totalsRef,
                ReferencedMediaHierarchyDefinition: mediaHierarchyRef,
                FlatLevelData: splitLevelIds?.length ? filteredFlatLevelData : flatLevelData,
                ReferencedCalendarDefinition: calendarRef,
                CalendarHelper: calendarHelper,
                calendarOverlayHeight: overlayFlightBarWallRowLength,
                dataColumns: appContext.columns,
                currencyCode: currentMediaHierarchyDetails?.Currency,
                inflightOverlayMetrics: appContext.inflightOverlayMetrics,
                showSubtotalsAtBottom: showSubtotalsAtBottom,
                isDecimal: themeDefinition?.Styling?.Precision?.PrecisionValue === precisionValue.Decimal,
                ApplyToCurrencyMetric: themeDefinition?.Styling?.Precision?.ApplyToCurrencyMetric ?? true,
                ApplyToNonCurrencyMetric: themeDefinition?.Styling?.Precision?.ApplyToNonCurrencyMetric ?? true,
            });
            let totalsBlocks = await totalsBuilder.build();
            const { rightHandTotalsColumnLength } = totalsBuilder;
            const centerSpace = (leftSpace + rightHandTotalsColumnLength) / 2;

            // Merge media hierarchy blocks with totals blocks
            const mediaHierarchyTotalsBlocks = flightBarWallColumnLength
                ? builder.merge([mediaHierarchyBlocks, totalsBlocks], leftMenuColumnLength)
                : [];


            // Build calendar blocks
            const NormalCalendarEnabled = calendarDefinition?.Configuration?.Type == CalendarType.Broadcast && calendarDefinition?.Configuration?.IsNormalizedEnabled;

            const calendarBuilder = new CalendarBuilder({
                clientId,
                CalendarDefinition: calendarDefinition,
                CalendarHelper: calendarHelper,
                BriefedctcColumnSelected: briefedCtcSelected,
                NormalCalendarEnabled: NormalCalendarEnabled,
                CalendarFullLengthForNormalView: 0,
            });
            let calendarBlocks = await calendarBuilder.build();
            const { calendarColumnLength, calendarFullLength } = calendarBuilder;


            const calendarBuilderSecondRow = new CalendarBuilder({
                clientId,
                CalendarDefinition: normalCalendarDefinition,
                CalendarHelper: normalCalendarHelper,
                BriefedctcColumnSelected: briefedCtcSelected,
                IsNormalCalendarRow: true,
                NormalCalendarEnabled: NormalCalendarEnabled,
                CalendarFullLengthForNormalView: calendarFullLength,
            });
            let calendarBlocksSecondRow = await calendarBuilderSecondRow.build();

            // Move calendar next to left menu
            calendarBlocks = builder.move(calendarBlocks, {
                left: leftSpace,
            });

            calendarBlocksSecondRow = builder.move(calendarBlocksSecondRow, {
                left: leftSpace,
            });

            // Does the header contains a logo?
            const containsLogo =
                !!headerDefinition?.Configuration?.Logos?.length;
            const logoCount = headerDefinition?.Configuration?.Logos.length > 1;

            const isLeftlogoDirection = headerDefinition?.Configuration?.Logos?.length && headerDefinition?.Configuration?.Logos[0].Image?.Content && headerDefinition?.Configuration?.Logos[0]?.Alignment == HeaderLogoAlignment.Left;

            const headerColumnsLength = containsLogo ? logoCount ? leftSpace == 0 ? calendarFullLength - 6 : rightHandTotalsColumnLength == 0 ? calendarFullLength - 3 : calendarFullLength : isLeftlogoDirection ? leftSpace != 0 ? calendarFullLength : calendarFullLength - 3 : rightHandTotalsColumnLength == 0 ? calendarFullLength - 3 : calendarFullLength : calendarFullLength;

            const headerDetailsColumnsLength = containsLogo ? logoCount ? leftSpace == 0 ? Math.floor((calendarFullLength ?? 20) - 6) : rightHandTotalsColumnLength == 0 ? Math.floor((calendarFullLength ?? 20) - 3) : Math.floor((calendarFullLength ?? 20)) : isLeftlogoDirection ? leftSpace != 0 ? Math.floor((calendarFullLength ?? 20)) : Math.floor((calendarFullLength ?? 20) - 3) : rightHandTotalsColumnLength == 0 ? Math.floor((calendarFullLength ?? 20) - 3) : Math.floor((calendarFullLength ?? 20)) : Math.floor((calendarFullLength ?? 20));

            // Build header blocks
            const headerBuilder = new HeaderBuilder({
                clientId,
                HeaderDefinition: headerDefinition,
                headerRowData,
                headerColumns: headerColumnsLength,
                logoLeftColumns: leftSpace,
                logoRightColumns: rightHandTotalsColumnLength,
                logoCenterColumns: centerSpace,
                dataColumns: appContext.columns,
            });
            let headerBlocks = await headerBuilder.build();
            const { logoLeftColumns } = headerBuilder;

            // Build header details blocks
            let headerDetailsBuilder = new HeaderDetailsBuilder({
                clientId,
                HeaderDefinition: headerDefinition,
                headerRowData,
                headerDetailsLength: headerDetailsColumnsLength,
                dataColumns: appContext.columns,
            });
            let headerDetailsBlocks = await headerDetailsBuilder.build();

            // Move header & header details next to left menu
            headerBlocks = builder.move(headerBlocks, {
                left: leftSpace && !containsLogo ? leftSpace : 0,
            });
            headerDetailsBlocks = builder.move(headerDetailsBlocks, {
                left: leftSpace ? leftSpace : logoLeftColumns,
            });

            // Build grand totals blocks
            const grandTotalsBuilder = new GrandTotalsBuilder({
                clientId,
                ReferencedGrandTotalDefinition: grandTotalsRef,
                ReferencedCalendarDefinition: calendarRef,
                CalendarHelper: calendarHelper,
                ReferencedMediaHierarchyDefinition: mediaHierarchyRef,
                LevelFlowchartData: levelFlowchartData,
                leftMenuColumnLength: leftSpace,
                columns,
                totalsBlocks,
                BriefedctcColumnSelected: briefedCtcSelected,
                precisedValue: themeDefinition?.Styling?.Precision?.PrecisionValue === precisionValue.Summarized,
                isDecimal: themeDefinition?.Styling?.Precision?.PrecisionValue === precisionValue.Decimal,
                ApplyToCurrencyMetric: themeDefinition?.Styling?.Precision?.ApplyToCurrencyMetric ?? true,
                ApplyToNonCurrencyMetric: themeDefinition?.Styling?.Precision?.ApplyToNonCurrencyMetric ?? true,
                currencyCode: currentMediaHierarchyDetails?.Currency,
                NormalCalendarHelper: normalCalendarHelper,
                ancestorSet: splitLevelIds?.length ? ancestorSet : null,
            });
            let grandTotalsBlocks = await grandTotalsBuilder.build();

            // Build footer & legend blocks
            const footerBuilder = new FooterBuilder({
                clientId,
                FooterDefinition: footerDefinition,
                templateColumnLength:
                    leftSpace +
                    calendarColumnLength +
                    rightHandTotalsColumnLength,
            });
            let footerBlocks = await footerBuilder.build();


            // Build theme legend blocks
            const legendBuilder = new LegendBuilder({
                clientId,
                ReferencedCalendarDefinition: calendarRef,
                ReferencedMediaHierarchyDefinition: mediaHierarchyRef,
                leftMenuColumnLength: leftSpace,
                ThemeDefinition: themeDefinition,
                ReferencedThemeDefinition: themeRef,
                calendarFullLength: calendarFullLength,
                rightHandTotalsColumnLength: rightHandTotalsColumnLength,
            });
            let legendBlocks = await legendBuilder.build();

            // Override global style method and return general theme definition style.
            const mappedGeneralStyle = themeDefinition?.Styling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.Styling,
                    false
                )
                : {};
            const mappedHeaderStyle = themeDefinition?.HeaderTheme?.Styling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.HeaderTheme.Styling,
                    false
                )
                : {};
            const mappedHeaderRowStyle = themeDefinition?.HeaderTheme
                ?.RowStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.HeaderTheme.RowStyling,
                    false
                )
                : {};
            const mappedCalendarStyle = themeDefinition?.CalendarTheme?.Styling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.CalendarTheme.Styling,
                    false
                )
                : {};
            const mappedCalendarOverlaySectionStyle = themeDefinition
                ?.CalendarOverlayTheme?.SectionStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.CalendarOverlayTheme.SectionStyling,
                    false
                )
                : {};
            const mappedCalendarOverlayRowStyle = themeDefinition
                ?.CalendarOverlayTheme?.RowStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.CalendarOverlayTheme.RowStyling,
                    false
                )
                : {};
            const mappedMediaHierarchyStyle = themeDefinition
                ?.MediaHierarchyTheme?.Styling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.MediaHierarchyTheme.Styling,
                    false
                )
                : {};
            const mappedMediaHierarchyFlightBarStyle = themeDefinition
                ?.MediaHierarchyTheme?.FlightBarStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.MediaHierarchyTheme.FlightBarStyling,
                    false
                )
                : {};
            const mappedMediaHierarchyInflightOverlayStyle = themeDefinition
                ?.MediaHierarchyTheme?.InflightOverlayStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.MediaHierarchyTheme
                        .InflightOverlayStyling,
                    false
                )
                : {};
            const mappedMediaHierarchyLeftMenuStyle = themeDefinition
                ?.MediaHierarchyTheme?.LeftMenuStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.MediaHierarchyTheme.LeftMenuStyling,
                    false
                )
                : {};
            const mappedMediaHierarchySubTotalTitleStyle = themeDefinition
                ?.MediaHierarchyTheme?.SubTotalTitleStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.MediaHierarchyTheme.SubTotalTitleStyling,
                    false
                )
                : {};
            const mappedMediaHierarchySubTotalStyle = themeDefinition
                ?.MediaHierarchyTheme?.SubTotalStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.MediaHierarchyTheme.SubTotalStyling,
                    false
                )
                : {};
            const mappedMediaHierarchyLegendStyle = themeDefinition
                ?.LegendTheme?.ColumnName && themeDefinition
                    ?.LegendTheme?.Settings?.length > 0
                ? themeDefinition
                    ?.LegendTheme?.Settings?.filter(s => s?.Enabled)?.map(legend => {
                        return {
                            key: `${themeDefinition?.LegendTheme?.ColumnName}_${legend.Name}`
                            , FlightBarStyling: StyleMapper.mapStyleToExcelStyle(
                                legend.Styling,
                                false
                            )
                            , SubTotalStyling: StyleMapper.mapStyleToExcelStyle(
                                legend["SubTotalStyling"],
                                false
                            ), InflightOverlayStyling: StyleMapper.mapStyleToExcelStyle(
                                legend["InflightOverlayStyling"],
                                false
                            )
                        }
                    })
                : [];
            const mappedGrandTotalStyle = themeDefinition?.GrandTotalTheme
                ?.Styling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.GrandTotalTheme.Styling,
                    false
                )
                : {};
            const mappedGrandTotalLeftMenuStyle = themeDefinition
                ?.GrandTotalTheme?.LeftMenuStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.GrandTotalTheme.LeftMenuStyling,
                    false
                )
                : {};
            const mappedTotalsTitleStyle = themeDefinition?.TotalsTheme
                ?.MainHeaderStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.TotalsTheme.MainHeaderStyling,
                    false
                )
                : {};
            const mappedTotalsHeaderStyle = themeDefinition?.TotalsTheme
                ?.HeaderStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.TotalsTheme.HeaderStyling,
                    false
                )
                : {};
            const mappedTotalsStyle = themeDefinition?.TotalsTheme?.Styling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.TotalsTheme.Styling,
                    false
                )
                : {};
            const mappedTotalsSumStyle = themeDefinition?.TotalsTheme
                ?.SumStyling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.TotalsTheme.SumStyling,
                    false
                )
                : {};
            const mappedFooterStyle = themeDefinition?.FooterTheme?.Styling
                ? StyleMapper.mapStyleToExcelStyle(
                    themeDefinition.FooterTheme.Styling,
                    false
                )
                : {};

            headerBlocks = headerBlocks.map((block) =>
                block.map((blockContent) => {
                    if (!blockContent.containsImage) {
                        blockContent.getGeneralStyles = () => mappedGeneralStyle;
                        blockContent.getComponentStyles = () => mappedHeaderStyle;
                    }
                    return blockContent;
                })
            );
            headerDetailsBlocks = headerDetailsBlocks.map((block) =>
                block.map((blockContent) => {
                    blockContent.getGeneralStyles = () => mappedGeneralStyle;
                    blockContent.getComponentStyles = () =>
                        mappedHeaderRowStyle;
                    return blockContent;
                })
            );

            calendarBlocks = calendarBlocks.map((block) =>
                block.map((blockContent) => {
                    blockContent.getGeneralStyles = () => mappedGeneralStyle;
                    blockContent.getComponentStyles = () => mappedCalendarStyle;

                    return blockContent;
                })
            );

            calendarBlocksSecondRow = calendarBlocksSecondRow.map((block) =>
                block.map((blockContent) => {
                    blockContent.getGeneralStyles = () => mappedGeneralStyle;
                    blockContent.getComponentStyles = () => mappedCalendarStyle;

                    return blockContent;
                })
            );

            calendarOverlayBlocks = calendarOverlayBlocks.map((block) =>
                block.map((blockContent) => {
                    blockContent.getGeneralStyles = () => mappedGeneralStyle;
                    if (blockContent instanceof CalendarOverlayEntryBlock)
                        return blockContent;

                    if (
                        blockContent instanceof
                        CalendarOverlayLeftMenuLevelBlock
                    ) {
                        blockContent.getComponentStyles = () =>
                            mappedCalendarOverlaySectionStyle;
                    } else {
                        blockContent.getComponentStyles = () =>
                            mappedCalendarOverlayRowStyle;
                    }

                    return blockContent;
                })
            );

            mediaHierarchyBlocks = mediaHierarchyBlocks.map(
                (block, blockIndex) =>
                    block.map((blockContent, blockContentIndex) => {
                        blockContent.getGeneralStyles = () =>
                            mappedGeneralStyle;

                        if (blockContent instanceof LeftMenuSubTotalBlock) {
                            blockContent.getComponentStyles = () =>
                                mappedMediaHierarchySubTotalTitleStyle;
                        } else if (
                            blockContent instanceof FlightBarWallSubTotalBlock
                        ) {
                            blockContent.getComponentStyles = () =>
                                mappedMediaHierarchySubTotalStyle;

                        } else if (
                            blockContent instanceof LeftMenuSettingBlock ||
                            blockContent instanceof LeftMenuLevelBlock ||
                            blockContent instanceof LeftMenuSubLevelBlock
                        ) {
                            blockContent.getComponentStyles = () =>
                                mappedMediaHierarchyLeftMenuStyle;
                        } else if (
                            blockContent instanceof FlightBarWallBarBlock
                        ) {
                            blockContent.getComponentStyles = () =>
                                mappedMediaHierarchyFlightBarStyle;
                        } else if (
                            blockContent instanceof
                            FlightBarWallInflightOverlayBlock
                        ) {
                            blockContent.getComponentStyles = () =>
                                mappedMediaHierarchyInflightOverlayStyle;
                        } else {
                            blockContent.getComponentStyles = () =>
                                mappedMediaHierarchyStyle;
                        }

                        // Legend style assignment
                        if (
                            blockContent instanceof FlightBarWallSubTotalBlock ||
                            blockContent instanceof FlightBarWallBarBlock ||
                            blockContent instanceof FlightBarWallInflightOverlayBlock
                        ) {
                            const legendStyle = mappedMediaHierarchyLegendStyle?.find(l => l.key === blockContent.legendColumn || blockContent.legendColumn.includes(l.key));
                            if (legendStyle) {
                                const styleType = blockContent instanceof FlightBarWallSubTotalBlock
                                    ? "SubTotalStyling"
                                    : blockContent instanceof FlightBarWallBarBlock
                                        ? "FlightBarStyling"
                                        : "InflightOverlayStyling";
                                blockContent.getLegendStyles = () => legendStyle[styleType];
                            }
                        }

                        return blockContent;
                    })
            );
            grandTotalsBlocks = grandTotalsBlocks.map((block) =>
                block.map((blockContent) => {
                    blockContent.getGeneralStyles = () => mappedGeneralStyle;

                    if (
                        blockContent instanceof
                        CalendarOverlayLeftMenuLevelBlock
                    ) {
                        blockContent.getComponentStyles = () =>
                            mappedGrandTotalLeftMenuStyle;
                    } else {
                        blockContent.getComponentStyles = () =>
                            mappedGrandTotalStyle;
                    }

                    return blockContent;
                })
            );

            totalsBlocks = totalsBlocks.map((block, blockIndex) =>
                block.map((blockContent, blockContentIndex) => {
                    blockContent.getGeneralStyles = () => mappedGeneralStyle;

                    if (blockContent instanceof HeadBlock) {
                        blockContent.getComponentStyles = () =>
                            mappedTotalsHeaderStyle;
                    }
                    else if (blockContent instanceof MainHeadBlock) {
                        blockContent.getComponentStyles = () =>
                            mappedTotalsTitleStyle;
                    }
                    else if (blockContent instanceof ValueBlock) {
                        blockContent.getComponentStyles = () =>
                            mappedTotalsStyle;
                    } else if (blockContent instanceof TotalBlock) {
                        blockContent.getComponentStyles = () =>
                            mappedTotalsSumStyle;
                    }

                    return blockContent;
                })
            );

            footerBlocks = footerBlocks.map((block) =>
                block.map((blockContent) => {
                    blockContent.getGeneralStyles = () => mappedGeneralStyle;
                    blockContent.getComponentStyles = () => mappedFooterStyle;

                    return blockContent;
                })
            );

            // Put all blocks together and return them

            const combinedCalendarBlocks = NormalCalendarEnabled ? [...calendarBlocks, ...calendarBlocksSecondRow] : [...calendarBlocks];
            return [[
                ...headerBlocks,
                ...headerDetailsBlocks,
                ...[...combinedCalendarBlocks],
                ...calendarOverlayBlocks,
                ...mediaHierarchyTotalsBlocks,
                ...grandTotalsBlocks,
                ...footerBlocks,
                ...legendBlocks
            ], precisedValue, isDecimal, levelFlowchartData];
        },
        [loadCurrentDefinitions, clientId, columns, appContext]
    );

    const showWarning = (
        definitionName?: TDefinitionName,
        definition?: TDefinition
    ) => {
        const flowchartDefinition =
            appContext.flowchartTemplateDefinition?.Definition;
        const calendarAvailable =
            !!appContext.currentCalendarTemplateDetails?.Id ||
            !!flowchartDefinition?.CalendarDefinition?.TemplateId;
        const hierarchyAvailable =
            !!appContext.currentMediaHierarchyDetails?.Id ||
            !!flowchartDefinition?.MediaHierarchyDefinition?.TemplateId;

        const hierarchyNeeded = [
            'grandTotalsDefinition',
            'totalsDefinition',
        ].includes(definitionName);
        const calendarNeeded = [
            'mediaHierarchyDefinition',
            'calendarOverlayDefinition',
            'grandTotalsDefinition',
            'totalsDefinition',
        ].includes(definitionName);

        if (!calendarAvailable && calendarNeeded && definition) {
            pushNotification(
                'A calendar is required in order to display this definition. Configure a calendar component and try again.',
                NotificationType.WARNING
            );
        }
        if (!hierarchyAvailable && hierarchyNeeded && definition) {
            pushNotification(
                'A media hierarchy is required in order to display this definition. Configure a media hierarchy component and try again.',
                NotificationType.WARNING
            );
        }

    };
    const getStorageKey = (key: string) => {
        return sessionStorage.getItem(key);
    };
    const setStorageKey = (key: string, value: any) => {
        return sessionStorage.setItem(key, value);
    };

    const onWorkSheetFormatChanged = async (event: Excel.WorksheetFormatChangedEventArgs) => {

        const cellFormats: Array<{ address: string, format: Excel.RangeFormat }> = [];
        const propertiesToLoadInFormat = [
            'fill'
            , 'font'
            , 'borders'
            , 'columnWidth'
            , 'horizontalAlignment'
            , 'indentLevel'
            , 'rowHeight'
            , 'verticalAlignment'
            , 'wrapText'];
        await Excel.run(async (context) => {
            const app = context.workbook.application;
            app.suspendScreenUpdatingUntilNextSync();
            await context.sync();
            const usedRange = event.getRangeOrNullObject(context);
            await context.sync();
            usedRange.load(["rowCount", "columnCount"]);
            await context.sync();
            if (!usedRange || usedRange.rowCount === 0 || usedRange.columnCount === 0) {

                return;
            }

            // get row count
            const rowCount = usedRange?.rowCount;
            // get column count
            const columnCount = usedRange?.columnCount;
            // Get all cells in the used range

            let cells = new Array<Excel.Range>();

            // add all cells to the array
            for (let i = 0; i < rowCount; i++) {
                for (let j = 0; j < columnCount; j++) {
                    cells.push(usedRange.getCell(i, j));
                    await context.sync();
                }
            }


            // Loop through each cell and get its format
            for (const cell of cells) {
                cell.load();
                await context.sync();
                const cellAddress = cell.address;
                const cellFormat = cell.format;
                cellFormat.load(propertiesToLoadInFormat);//  Excel.Interfaces.AggregationFunction);                    
                await context.sync();
                cellFormat.fill.load();
                cellFormat.font.load();
                cellFormat.borders.load('items');
                await context.sync();
                //cellFormat.
                cellFormats.push({ address: cellAddress, format: cellFormat });
            }
            if (!appContext?.onRun) {
                const currentStyles = [...appContext?.manualFormattingStyles, ...cellFormats];
                appContext.setManualFormattingStyles(currentStyles);
            }
            else {
                const currentStylesRunConfig = [...appContext?.manualFormattingStylesRunConfig, ...cellFormats];
                appContext.setManualFormattingStylesRunConfig(currentStylesRunConfig);
            }
        });

    };

    const render =
        async (definitionName?: TDefinitionName, definition?: TDefinition, restrictions?: RunRestriction[]) => {
            const value = getStorageKey('isRenderingWithDefinition');
            const { mediaHierarchyRef } = loadCurrentDefinitions(definitionName, definition);
            const getUpdatedFlowchartData = (levels?: any[]) => {
                return flatLevels(
                    levels ||
                    appContext.flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition?.Definition?.Levels ||
                    [],
                    mediaHierarchyRef
                );
            };
            if (isRenderingWithDefinition || (value && value == "true")) return;
            setStorageKey('isRenderingWithDefinition', true);
            setIsRenderingWithDefinition(true);
            setIsGlobalRendering(true);
            try {
                setCalendarWeekStartDay();
                let columnStart = 0;
                let rowStart = 0;
                let blocksData;
                blocksData = await getAllBlocks(
                    definitionName,
                    definition,
                    restrictions
                );
                 const levelFlowchartData = blocksData[3];
                const {
                    themeDefinition,
                } = loadCurrentDefinitions(definitionName, definition);
                if (themeDefinition?.Styling?.Position?.Start) {
                    const positions = themeDefinition.Styling.Position.Start.split(/^([A-Z]+)(\d+)$/);
                    columnStart = convertLetterToNumber(positions[1]);
                    rowStart = parseInt(positions[2]) - 1;
                    if (rowStart < 0) rowStart = 0;
                }

                // Render template blocks
                const excelRenderer = new ExcelRenderer();

                let customStylesOnLoadValue = appContext?.flowchartTemplateDefinition?.TrackFormatChanges;
                const renderOptions = {
                    deleteSheet: true,
                    columnStart,
                    rowStart,
                    precisedValue: blocksData[1],
                    setCurrentExcelContext,
                    onWorkSheetFormatChanged
                };

                await excelRenderer.render(blocksData[0], renderOptions);
                const prevSplitColumnName = prevSplitColumnNameRef.current;
                const splitColumnName = themeDefinition?.SplitByColumn?.ColumnName || "";
                const levels =
                        levelFlowchartData?.Levels ||
                        currentLevelFlowchartData?.Levels ||
                        appContext.flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition?.Definition?.Levels ||
                        [];
                    const flowchartData = getUpdatedFlowchartData(levels);
                    if (isEmpty(flowchartData)) {
                        await deleteAllWorksheetsExceptReport(); // Handle case where splitColumnName is not defined
                    }
                   

                    const flowchartColumnNames = new Set(flowchartData.map(l => l.ColumnName));
                    const hasSplitColumn = splitColumnName && flowchartColumnNames.has(splitColumnName);

                    if (
                        !isEmpty(flowchartData) &&
                        prevSplitColumnName &&
                        prevSplitColumnName !== "" &&
                        (
                            prevSplitColumnName !== splitColumnName ||
                            (splitColumnName && !hasSplitColumn)
                        )
                    ) {
                        await deleteSplitWorksheets(prevSplitColumnName);
                    }


                    if (!isEmpty(flowchartData) && splitColumnName && hasSplitColumn) {
                        await deleteNamedSplitWorksheets(splitColumnName, flowchartData);

                        const visitedLevelIds = new Set<string>();
                        for (const level of flowchartData) {
                            if (level.ColumnName == splitColumnName && !visitedLevelIds.has(`${splitColumnName}-${level.Name}`)) {
                                const sheetName = `${splitColumnName}-${level.Name}`;
                                visitedLevelIds.add(sheetName);
                                let splitLevelIds = flowchartData.filter(l => l.levelId.startsWith(`${level.ColumnName}_${level.Name}`)).map(l => l.levelId);
                                blocksData = await getAllBlocks(
                                    definitionName,
                                    definition,
                                    restrictions,
                                    splitLevelIds
                                );
                                const renderOptions = {
                                    name: sheetName,
                                    levelName: level.Name,
                                    deleteSheet: false,
                                    columnStart,
                                    rowStart,
                                    precisedValue: blocksData[1],
                                    setCurrentExcelContext,
                                    onWorkSheetFormatChanged
                                };
                                const excelRenderer = new ExcelRenderer();
                                await excelRenderer.render(blocksData[0], renderOptions);
                            }
                        }
                    }

                    // Update ref for next render
                    prevSplitColumnNameRef.current = splitColumnName;

                if (!appContext.onRun) {
                    if (!appContext.initialRenderDone) {
                        const customStyles = appContext?.flowchartTemplateDefinition?.CustomStyleSettings
                            ? JSON.parse(appContext.flowchartTemplateDefinition.CustomStyleSettings) as Array<{ address: string, format: Excel.RangeFormat }>
                            : null;

                        if (customStyles) {
                            sessionStorage.setItem(customStylesKey, JSON.stringify(customStyles));
                            await excelRenderer.applySavedStyles(customStyles).finally(() => {
                                appContext.setManualFormattingStyles([...customStyles]);

                            });
                        }
                        if (customStylesOnLoadValue || appContext.isTrackFormatting) {
                            await excelRenderer.setUpEvents(onWorkSheetFormatChanged);
                        }
                        appContext.setIsTrackFormatting(customStylesOnLoadValue);

                    } else if (appContext.isTrackFormatting) {
                        if (!isEmpty(sessionStorage.getItem(customStylesKey))) {
                            await excelRenderer.applySavedStyles(JSON.parse(sessionStorage.getItem(customStylesKey)));
                        }
                        await excelRenderer.setUpEvents(onWorkSheetFormatChanged);
                    }
                } else {
                    if (!appContext.initialRenderDone) {

                        const customStylesRunConfig = appContext?.flowchartRunConfiguration?.CustomStyleSettings
                            ? JSON.parse(appContext.flowchartRunConfiguration.CustomStyleSettings)
                            : null;

                        if (customStylesRunConfig) {
                            sessionStorage.setItem(runConfigCustomStylesKey, JSON.stringify(customStylesRunConfig));
                            await excelRenderer.applySavedStyles(customStylesRunConfig).finally(() => {
                                appContext.setManualFormattingStylesRunConfig([...customStylesRunConfig]);
                            });
                        }

                        if (appContext?.flowchartRunConfiguration?.TrackFormatChanges || appContext.isTrackFormattingRunConfig) {
                            await excelRenderer.setUpEvents(onWorkSheetFormatChanged);
                        }

                        appContext.setIsTrackFormattingRunConfig(appContext?.flowchartRunConfiguration?.TrackFormatChanges);
                    } else if (appContext.isTrackFormattingRunConfig) {
                        if (!isEmpty(sessionStorage.getItem(runConfigCustomStylesKey))) {
                            await excelRenderer.applySavedStyles(JSON.parse(sessionStorage.getItem(runConfigCustomStylesKey)));
                        }
                        await excelRenderer.setUpEvents(onWorkSheetFormatChanged);
                    }
                }
                setStorageKey('isRenderingWithDefinition', false);
                setIsRenderingWithDefinition(false);
                setIsGlobalRendering(false);
            } catch (e) {
                appContext.initialRenderDone &&
                    pushNotification(
                        'Render could not be completed. Please try again or change the settings.',
                        NotificationType.DANGER
                    );
                setStorageKey('isRenderingWithDefinition', false);
                setIsRenderingWithDefinition(false);
                setIsGlobalRendering(false);
                console.error(e);
            }
        };

    const setCalendarWeekStartDay = () => {
        const flowchart = {
            ...appContext.flowchartTemplateDefinition?.Definition,
        };

        const calendarDetails = appContext?.currentCalendarTemplateDetails?.Definition ?? flowchart?.CalendarDefinition?.Definition;

        const currentCalendarType = calendarDetails?.Configuration?.Type;
        const isUseCustomStartDayEnabled = calendarDetails?.Configuration?.UseCustomStartDay;
        if (currentCalendarType == CalendarType.Standard && isUseCustomStartDayEnabled) {
            moment.updateLocale('en', {
                week: {
                    dow: 1
                }
            });
        } else {
            moment.updateLocale('en', {
                week: {
                    dow: 0
                }
            });
        }
    }

    return { isRenderingWithDefinition, render, getAllBlocks, setFlowchartTemplateDefinition };
};

export default useRenderWithDefinition;
