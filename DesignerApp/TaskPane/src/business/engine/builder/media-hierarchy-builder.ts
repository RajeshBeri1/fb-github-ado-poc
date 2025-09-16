import {
    CalendarType,
    FlightRange,
    ReferencedCalendarDefinition,
    ReferencedHeaderDefinition,
    ReferencedMediaHierarchyDefinition,
    Styling,
} from '@omniflow/omni-webapi';
import * as API from '@omniflow/omni-webapi';
import { find, findIndex, sortBy, sum, filter } from 'lodash';
import moment from 'moment';

import { BuildEngine } from '../models/build-engine';
import { Block } from '../models/block';
import LeftMenuLevelBlock from '../blocks/media-hierarchy/left-menu-level-block';
import LeftMenuSubLevelBlock from '../blocks/media-hierarchy/left-menu-sub-level-block';
import LeftMenuSettingBlock from '../blocks/media-hierarchy/left-menu-setting-block';
import LeftMenuSubTotalBlock from '../blocks/media-hierarchy/left-menu-sub-total-block';
import FlightBarWallBackgroundBlock from '../blocks/media-hierarchy/flight-bar-wall-background-block';
import FlightBarWallBarBlock from '../blocks/media-hierarchy/flight-bar-wall-bar-block';
import FlightBarWallSubTotalBlock from '../blocks/media-hierarchy/flight-bar-wall-sub-total-block';
import { CalendarHelper, normalizeCalendarDateRange } from '../helpers/calendar-helper';
import {
    getPeriods,
    normalizeFlightRangePeriod,
} from '../helpers/period-helper';
import { UsdMetrics } from '../constant/metric';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { StyleMapper } from '../models/styles';
import { TFlowchartDataLevelFlat } from '../helpers/level-helper';
import {
    findNumberFormat,
    formatNumberWithMaxLength,
} from '../helpers/number-format-helper';
import FlightBarWallInflightOverlayBlock from '../blocks/media-hierarchy/flight-bar-wall-inflight-overlay-block';
import { NumberFormat } from '../../../enums/number-format.enum';
import { BriefedCtcMetricColumnName } from '../constant/common';
import SpacerBlock from '../blocks/calendar/spacer-block';
import GenericBlock from '../blocks/calendar/generic-block';
import { AppContext } from '../../../../src/taskpane/contexts/AppContext';
import { useContext } from 'react';
import { precisionValue } from '../../../enums/precision-value.enum';
import { getExtraWeekForSpace } from '../helpers/week-count-helper';
import { mergeAndSortMetrics } from '../helpers/merge-sort-helper';

export interface IMediaHierarchyBuilderProps {
    clientId: string;
    columns: TColumn[];
    ReferencedMediaHierarchyDefinition?: ReferencedMediaHierarchyDefinition;
    FlatLevelData?: TFlowchartDataLevelFlat[];
    ReferencedCalendarDefinition?: ReferencedCalendarDefinition;
    CalendarHelper?: CalendarHelper;
    ReferencedHeaderDefinition?: ReferencedHeaderDefinition;
    BriefedctcColumnSelected?: boolean;
    precisedValue?: boolean;
    isDecimal?: boolean;
    ApplyToCurrencyMetric: boolean,
    ApplyToNonCurrencyMetric: boolean,

    currencyCode?: string;
    showSubtotalsAtBottom?: boolean;
    themeLegendColumn?: string;
    inflightOverlayMetrics?: string[];
    NormalCalendarHelper?: CalendarHelper;
}

export type TLegendData = {
    name: string;
    color: string;
}[];

export class MediaHierarchyBuilder extends BuildEngine {
    clientId: string;
    columns: TColumn[];
    ReferencedMediaHierarchyDefinition: ReferencedMediaHierarchyDefinition;
    FlatLevelData: TFlowchartDataLevelFlat[];
    ReferencedCalendarDefinition: ReferencedCalendarDefinition;
    CalendarHelper: CalendarHelper;
    ReferencedHeaderDefinition: ReferencedHeaderDefinition;
    BriefedctcColumnSelected?: boolean;

    leftMenuColumnLength: number = 0;
    leftMenuRowLength: number = 0;
    flightBarWallColumnLength: number = 0;
    flightBarWallRowLength: number = 0;

    legendData: TLegendData = [];
    precisedValue: boolean;
    isDecimal: boolean;
    ApplyToCurrencyMetric: boolean;
    ApplyToNonCurrencyMetric: boolean;
    currencyCode?: string;
    showSubtotalsAtBottom?: boolean;
    themeLegendColumn?: string;
    inflightOverlayMetrics?: string[];
    NormalCalendarHelper?: CalendarHelper;
    constructor({
        clientId,
        columns,
        ReferencedMediaHierarchyDefinition,
        FlatLevelData,
        ReferencedCalendarDefinition,
        CalendarHelper,
        ReferencedHeaderDefinition,
        BriefedctcColumnSelected,
        precisedValue,
        isDecimal,
        ApplyToCurrencyMetric,
        ApplyToNonCurrencyMetric,
        currencyCode,
        showSubtotalsAtBottom,
        themeLegendColumn,
        inflightOverlayMetrics,
        NormalCalendarHelper
    }: IMediaHierarchyBuilderProps) {
        super();

        this.clientId = clientId;
        this.columns = columns;
        this.ReferencedMediaHierarchyDefinition =
            ReferencedMediaHierarchyDefinition;
        this.FlatLevelData = FlatLevelData;
        this.ReferencedCalendarDefinition = ReferencedCalendarDefinition;
        this.CalendarHelper = CalendarHelper;
        this.ReferencedHeaderDefinition = ReferencedHeaderDefinition;
        this.BriefedctcColumnSelected = BriefedctcColumnSelected;
        this.precisedValue = precisedValue;
        this.isDecimal = isDecimal;
        this.ApplyToCurrencyMetric = ApplyToCurrencyMetric;
        this.ApplyToNonCurrencyMetric = ApplyToNonCurrencyMetric;
        this.currencyCode = currencyCode;
        this.showSubtotalsAtBottom = showSubtotalsAtBottom;
        this.themeLegendColumn = themeLegendColumn;
        this.inflightOverlayMetrics = inflightOverlayMetrics;
        this.NormalCalendarHelper = NormalCalendarHelper;
    }

    build = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];

        if (
            this.ReferencedMediaHierarchyDefinition?.Definition &&
            this.FlatLevelData &&
            this.ReferencedCalendarDefinition?.Definition &&
            this.CalendarHelper
        ) {
            const leftMenuRows = await this.buildLeftMenu();
            const flightBarWallRows = await this.buildFlightBarWall();

            rows = this.merge([leftMenuRows, flightBarWallRows], 0);
        }

        return rows;
    };

    buildLeftMenu = async (): Promise<Block[][]> => {
        const rows: Block[][] = [];
        const maxDepth = Math.max(1, ...this.FlatLevelData.map((o) => o.Depth));
        const subTotalStack: [Block[]] = [[]];
        const subTotalSummaryStack: [Block[]] = [[]];
        let depthCursor = 0;

        this.FlatLevelData.forEach(
            ({ Name, Metrics, SubTotals, Depth, Order, LevelDefinition, SubTotalSummary }, index) => {
                /* if (Depth === maxDepth) {
                   depthCursor = maxDepth
                 }
                 else if (depthCursor > 0) {
                     depthCursor = depthCursor - 1;
                 }*/
                const menuBlockProps = {
                    value: Name,
                    columnStart: Depth - 1,
                    columnLength: maxDepth - Depth + 1,
                    rowLength: Metrics?.[0]?.InflightOverlays?.length ? Metrics?.[0]?.InflightOverlays.filter((val, index) => Metrics.some(c => c.InflightOverlays && c.InflightOverlays[index] && JSON.parse(c.InflightOverlays[index]) != null))?.length + 1 : 1,
                };
                if (this.showSubtotalsAtBottom) {
                    if (subTotalStack.length && Depth <= depthCursor) {
                        for (let i = depthCursor; i >= Depth; i--) {
                            if (subTotalStack.length && subTotalStack[i]?.length) {
                                while (subTotalStack[i].length) {
                                    rows.push([subTotalStack[i].pop()])
                                }
                            }
                        }


                    }
                }

                if (this.showSubtotalsAtBottom) {
                    if (subTotalSummaryStack.length && Depth <= depthCursor) {
                        for (let i = depthCursor; i >= Depth; i--) {
                            if (subTotalSummaryStack.length && subTotalSummaryStack[i]?.length) {
                                while (subTotalSummaryStack[i].length) {
                                    rows.push([subTotalSummaryStack[i].pop()])
                                }
                            }
                        }


                    }
                }

                if (Depth === 1) {
                    const block = new LeftMenuLevelBlock(menuBlockProps);
                    block.getDirectStyles = () =>
                        StyleMapper.mapStyleToExcelStyle(
                            LevelDefinition.Styling,
                            false
                        );

                    rows.push([block]);
                } else if (Depth === 2) {
                    const block = new LeftMenuSettingBlock(menuBlockProps);
                    block.getDirectStyles = () =>
                        StyleMapper.mapStyleToExcelStyle(
                            LevelDefinition.Styling,
                            false
                        );

                    rows.push([block]);
                } else if (Depth > 2) {
                    const block = new LeftMenuSubLevelBlock(menuBlockProps);

                    block.getDirectStyles = () =>
                        StyleMapper.mapStyleToExcelStyle(
                            LevelDefinition.Styling,
                            false
                        );

                    rows.push([block]);
                }

                const settingsObj = Array.isArray(LevelDefinition.Settings)
                    ? LevelDefinition.Settings.find(s => s.Order === Order)
                    : undefined;
                if (SubTotals && SubTotals.length) {

                    SubTotals.forEach((subTotal, subTotalIndex) => {
                        const columnData = find(this.columns, {
                            Name: subTotal.ColumnName,
                            TableId: subTotal.TableId,
                        });
                        const foundUsdMetric = findIndex(UsdMetrics, {
                            MetricColumnName: subTotal.ColumnName,
                            MetricTableId: subTotal.TableId,
                        });

                        const block = new LeftMenuSubTotalBlock({
                            value: `${Name} ${subTotal.FlightRange} ${columnData
                                ? columnData.DisplayName
                                : subTotal.ColumnName
                                }${foundUsdMetric >= 0 ? ' (USD)' : ''}`,
                            columnLength: maxDepth,
                        });
                       
                        const subTotalStyling = settingsObj?.SubTotals?.[subTotalIndex]?.TitleStyling ?? ({} as Styling);

                        block.getDirectStyles = () =>
                            StyleMapper.mapStyleToExcelStyle(subTotalStyling, false);
                        if (this.showSubtotalsAtBottom) {
                            if (!subTotalStack[Depth]) {
                                subTotalStack[Depth] = [];
                            }
                            subTotalStack[Depth].push(block);
                        }
                        else {
                            rows.push([block]);
                        }


                    });

                }

                if (SubTotalSummary && SubTotalSummary.length) {
                    let addedOrdersForSubToatlSummary = [];
                    SubTotalSummary.forEach((subTotal, subTotalIndex) => {
                        if (addedOrdersForSubToatlSummary.length && addedOrdersForSubToatlSummary.findIndex(c => c === subTotal.Order) > -1)
                            return;
                        const columnData = find(this.columns, {
                            Name: subTotal.ColumnName,
                            TableId: subTotal.TableId,
                        });
                        const foundUsdMetric = findIndex(UsdMetrics, {
                            MetricColumnName: subTotal.ColumnName,
                            MetricTableId: subTotal.TableId,
                        });

                        const subTotalSummaryArr = Array.isArray(settingsObj?.SubTotalSummary) ? settingsObj.SubTotalSummary : [];
                        const displayNames = subTotalSummaryArr?.[subTotal.Order]?.DisplayNames ?? {};

                        if (Object.entries(displayNames).length > 0) {
                            addedOrdersForSubToatlSummary.push(subTotal.Order);
                            const subTotals = filter(SubTotalSummary, { Order: subTotal.Order }) || [];
                            Object.entries(displayNames).forEach(displayName => {
                                const exists = subTotals.findIndex(subTotal =>
                                    displayName[0].includes(subTotal.ColumnName) &&
                                    displayName[0].includes(subTotal.DisplayName)
                                ) !== -1;

                                if (exists) {
                                    const block = new LeftMenuSubTotalBlock({
                                        value: displayName[1],
                                        columnLength: maxDepth,
                                    });

                                    const subTotalSummaryStyling = settingsObj?.SubTotalSummary?.[subTotalIndex]?.TitleStyling ?? ({} as Styling);

                                    block.getDirectStyles = () =>
                                        StyleMapper.mapStyleToExcelStyle(subTotalSummaryStyling, false);
                                    if (this.showSubtotalsAtBottom) {
                                        if (!subTotalSummaryStack[Depth]) {
                                            subTotalSummaryStack[Depth] = [];
                                        }
                                        subTotalSummaryStack[Depth].push(block);
                                    }
                                    else {
                                        rows.push([block]);
                                    }
                                }
                            });
                        }
                        else {

                            let label = `${Name} ${subTotal.DisplayName} ${subTotal.FlightRange} ${columnData
                                ? columnData.DisplayName
                                : subTotal.ColumnName
                                }${foundUsdMetric >= 0 ? ' (USD)' : ''}`;

                            let labelToSearch = `${Name} ${subTotal.DisplayName} ${subTotal.FlightRange} ${columnData
                                ? columnData.Name
                                : subTotal.ColumnName
                                }${foundUsdMetric >= 0 ? ' (USD)' : ''}`;

                            if (displayNames && Object.keys(displayNames).length) {
                                label = displayNames[labelToSearch] ? displayNames[labelToSearch] : label;
                            }
                            const block = new LeftMenuSubTotalBlock({
                                value: label,
                                columnLength: maxDepth,
                            });

                            const subTotalSummaryStyling = settingsObj?.SubTotalSummary?.[subTotalIndex]?.TitleStyling ?? ({} as Styling);

                            block.getDirectStyles = () =>
                                StyleMapper.mapStyleToExcelStyle(subTotalSummaryStyling, false);
                            if (this.showSubtotalsAtBottom) {
                                if (!subTotalSummaryStack[Depth]) {
                                    subTotalSummaryStack[Depth] = [];
                                }
                                subTotalSummaryStack[Depth].push(block);
                            }
                            else {
                                rows.push([block]);
                            }
                        }

                    });

                }
                if (this.showSubtotalsAtBottom) {
                    depthCursor = Depth;
                }


            }
        );
        if (subTotalStack.length) {
            for (let i = subTotalStack.length - 1; i >= 1; i--) {
                if (subTotalStack.length && subTotalStack[i]?.length) {
                    while (subTotalStack[i].length) {
                        rows.push([subTotalStack[i].pop()])
                    }
                }
            }
        }

        if (subTotalSummaryStack.length) {
            for (let i = subTotalSummaryStack.length - 1; i >= 1; i--) {
                if (subTotalSummaryStack.length && subTotalSummaryStack[i]?.length) {
                    while (subTotalSummaryStack[i].length) {
                        rows.push([subTotalSummaryStack[i].pop()])
                    }
                }
            }
        }

        this.leftMenuColumnLength = this.getColumnLength(rows);
        this.leftMenuRowLength = this.getRowLength(rows);
        return rows;
    };

    buildFlightBarWall = async (): Promise<Block[][]> => {
        const rows: Block[][] = [];
        let weeklyFlightRange = this.FlatLevelData.some(levelTotal => levelTotal.FlightRange == FlightRange.Weekly || levelTotal.FlightRange == FlightRange.WeeklyBroadcast || levelTotal.SubTotals?.some(subTotals => subTotals.FlightRange == FlightRange.Weekly || subTotals.FlightRange == FlightRange.WeeklyBroadcast) || levelTotal.SubLevels?.some(subLevelTotal => subLevelTotal.FlightRange == FlightRange.Weekly || subLevelTotal.FlightRange == FlightRange.WeeklyBroadcast));
        let isValidCurrency = this.currencyCode && this.currencyCode != 'LLL';
        let dateFormat = "MM/DD";

        // Continue only if there are weeks, startDate and endDate
        const isNormalCalendarEnabled = (this.ReferencedCalendarDefinition?.Definition?.Configuration?.Type == CalendarType.Broadcast
            || this.ReferencedCalendarDefinition?.Definition?.Configuration?.Type == CalendarType.Standard) && this.ReferencedCalendarDefinition?.Definition?.Configuration?.IsNormalizedEnabled;
        const additionalColumnForCalendar = this.BriefedctcColumnSelected ? 1 : 0;
        let actualStartDate = this.ReferencedCalendarDefinition?.Definition?.Configuration?.CustomStartDate ?? this.CalendarHelper.dataFrom;
        let actualEndDate = this.ReferencedCalendarDefinition?.Definition?.Configuration?.CustomEndDate ?? this.CalendarHelper.dataTo;
        const isStandardAndUseCustomStartDay = this.ReferencedCalendarDefinition?.Definition?.Configuration?.Type == CalendarType.Standard && this.ReferencedCalendarDefinition?.Definition?.Configuration?.UseCustomStartDay;
        const isStandardCalendar = this.ReferencedCalendarDefinition?.Definition?.Configuration?.Type == CalendarType.Standard;
        const isBroadcastCalendar = this.ReferencedCalendarDefinition?.Definition?.Configuration?.Type == CalendarType.Broadcast;

        const dates = { dateFrom: null, dateTo: null };
        if (isNormalCalendarEnabled || isStandardAndUseCustomStartDay) {
            dates.dateFrom = this.NormalCalendarHelper?.dataFrom;
            dates.dateTo = this.NormalCalendarHelper?.dataTo;
            const startDay = !isStandardAndUseCustomStartDay && !isNormalCalendarEnabled ? 0 : 1;
            const normalizedDates = normalizeCalendarDateRange(
                actualStartDate,
                actualEndDate,
                isNormalCalendarEnabled || isStandardAndUseCustomStartDay,
                startDay
            );
            actualStartDate = normalizedDates.normalizedStartDate;
            actualEndDate = normalizedDates.normalizedEndDate;
        }

        const extraWeeks = isNormalCalendarEnabled ? getExtraWeekForSpace(actualStartDate, actualEndDate) : 0;

        const countWeeks = (this.CalendarHelper.data?.length || 0) + additionalColumnForCalendar + extraWeeks;
        const actualCountWeeks = (this.CalendarHelper.data?.length || 0) + additionalColumnForCalendar;
        const startDate = this.CalendarHelper.dataFrom;
        const endDate = this.CalendarHelper.dataTo;
        const optionMetricNone: string = 'None';
        const subTotalStack: [[Block[]]] = [[[]]];
        const subTotalSummaryStack: [[Block[]]] = [[[]]];
        const inflightOverlayMetrics = this.inflightOverlayMetrics;
        let depthCursor = 0;
        if (countWeeks && startDate && endDate) {
            const levelOrder = sortBy(this.ReferencedMediaHierarchyDefinition.Definition.Levels, "Order").map(c => c.Order);
            const themeLegendColumn = this.themeLegendColumn;
            // get all top level columns
            const getPreviousDepthLegend = (depth: number, name: string, columnName: string, index: number) => {
                const previousLevel = [];
                for (let i = depth - 1; i >= 1; i--) {
                    const previousLevelData = this.FlatLevelData.filter((x, flatIndex) => (x.Depth == i && flatIndex < index));
                    if (previousLevelData.length) {
                        previousLevel.push(previousLevelData[previousLevelData.length - 1]);
                    }
                }
                return this.FlatLevelData.filter((x, flatIndex) => (x.Depth == depth && x.Name == name && x.ColumnName == columnName && flatIndex == index)).flatMap(c => `${c.ColumnName}_${c.Name}`).concat(previousLevel.flatMap(c => `${c.ColumnName}_${c.Name}`)).join('$');
            }

            this.FlatLevelData.forEach((data, levelIndex, LevelDefinition) => {

                // identify quarter
                const isNormalQuarter = isNormalCalendarEnabled && data.FlightRange == API.FlightRange.Quarterly;
                const isNormalCalendarViewEnabledAndDifferentFlight = isNormalCalendarEnabled && !isStandardCalendar
                    && (data.FlightRange == API.FlightRange.Quarterly
                        || data.FlightRange == API.FlightRange.Weekly
                        || data.FlightRange == API.FlightRange.Annually
                        || data.FlightRange == API.FlightRange.Monthly);

                // metric inflightoverlay columns
                const levelInflightOverlays = data.LevelDefinition?.Settings.find(c =>
                    c.Order == data.Order)?.InflightOverlays;
                const inflightOverlayIndexes = levelInflightOverlays && levelInflightOverlays.length ? levelInflightOverlays.map((c, index) => {
                    return inflightOverlayMetrics.includes(c.ColumnName) ? index : -1;
                }) : [];

                //legend key
                const legendKey = data.Depth == 1 ? `${data.ColumnName}_${data.Name}` : getPreviousDepthLegend(data.Depth, data.Name, data.ColumnName, levelIndex);
                // Merge flight bar metrics
                const flightBars = mergeAndSortMetrics(
                    data?.Metrics,
                    data?.FlightRange,
                    inflightOverlayIndexes
                );
                if (this.showSubtotalsAtBottom) {
                    if (subTotalStack.length && data.Depth <= depthCursor) {
                        for (let i = depthCursor; i >= data.Depth; i--) {
                            if (subTotalStack.length && subTotalStack[i]?.length) {
                                while (subTotalStack[i].length) {
                                    const tempValue = subTotalStack[i].pop();
                                    if (tempValue.length) {
                                        rows.push(tempValue);

                                    }
                                }
                            }
                        }
                    }

                    if (subTotalSummaryStack.length && data.Depth <= depthCursor) {
                        for (let i = depthCursor; i >= data.Depth; i--) {
                            if (subTotalSummaryStack.length && subTotalSummaryStack[i]?.length) {
                                while (subTotalSummaryStack[i].length) {
                                    const tempValue = subTotalSummaryStack[i].pop();
                                    if (tempValue.length) {
                                        rows.push(tempValue);

                                    }
                                }
                            }
                        }
                    }

                }
                // Build flight bar periods
                const flightBarPeriods =
                    data.FlightRange !== API.FlightRange.FlightTotal &&
                        data.FlightRange !== API.FlightRange.None
                        ? getPeriods(
                            startDate,
                            endDate,
                            normalizeFlightRangePeriod(data.FlightRange),
                            this.CalendarHelper.calendarType,
                            isNormalCalendarViewEnabledAndDifferentFlight
                        ).map((flightBarPeriod, index) => {
                            let flightBarSum = 0;
                            let inflightOverlay;
                            let inflightOverlays: string[][] = [];
                            let legend;
                            var flightStartDate;
                            var flightEndDate;
                            flightBars.forEach(
                                ({
                                    EffectiveDate,
                                    ValueJson,
                                    InflightOverlayValueJson,
                                    InflightOverlays,
                                    Legend,
                                }) => {
                                    const metricStartDate = moment
                                        .utc(EffectiveDate)
                                        .toDate();
                                    const metricEndDate = moment
                                        .utc(EffectiveDate)
                                        .subtract(1, 'second')
                                        .toDate();

                                    if (

                                        //metricStartDate >=
                                        //    flightBarPeriod.startDate &&
                                        //metricEndDate <=
                                        //flightBarPeriod.endDate
                                        metricStartDate.getTime() >= flightBarPeriod.startDate.getTime() && metricStartDate.getTime() <= flightBarPeriod.endDate.getTime()
                                    ) {
                                        flightBarSum += ValueJson;

                                        if (!inflightOverlay) {
                                            inflightOverlay =
                                                InflightOverlayValueJson;
                                        }
                                        inflightOverlays.push(InflightOverlays);
                                        if (!legend) {
                                            legend =
                                                Legend;
                                        }
                                        if (!flightStartDate) {
                                            flightStartDate = EffectiveDate;
                                        } else {
                                            flightStartDate = flightStartDate.getTime() > EffectiveDate.getTime() ? EffectiveDate : flightStartDate;
                                        }
                                        if (!flightEndDate) {
                                            flightEndDate = EffectiveDate;
                                        } else {
                                            flightEndDate = flightEndDate.getTime() < EffectiveDate.getTime() ? EffectiveDate : flightEndDate;
                                        }

                                    }
                                }
                            );

                            return {
                                startDate: flightBarPeriod.actualStartDate ?? flightBarPeriod.startDate,
                                endDate: flightBarPeriod.actualEndDate ?? flightBarPeriod.endDate,
                                data: {
                                    flightBarSum,
                                    inflightOverlay: inflightOverlay ?? "InflightOverlayNA",
                                    isMetricNone: data.MetricColumnName == optionMetricNone,
                                    currency: data.LocalCurrency,
                                    inflightOverlays: inflightOverlays,
                                    legend: legend,
                                    flightStartDate: flightStartDate,
                                    flightEndDate: flightEndDate
                                },
                            };
                        })
                        : flightBars.map((flightBar) => ({
                            startDate: flightBar.FlightStart,
                            endDate: flightBar.FlightEnd,
                            data: {
                                flightBarSum: flightBar.ValueJson,
                                inflightOverlay:
                                    flightBar.InflightOverlayValueJson,
                                isMetricNone: data.MetricColumnName == optionMetricNone,
                                currency: data.LocalCurrency,
                                inflightOverlays: [flightBar.InflightOverlays],
                                legend: flightBar.Legend,
                                flightStartDate: flightBar.FlightStart,
                                flightEndDate: flightBar.FlightEnd
                            },
                        }));

                // push briefed ctc values for channel
                const briefedCtcColumns: Block[] = [];
                if (this.BriefedctcColumnSelected) {
                    let menuBlockProps = {
                        columnStart: -countWeeks,
                        columnLength: 1,
                        value: ' '
                    }
                    if (data.ColumnName.toLowerCase() == BriefedCtcMetricColumnName) {
                        let totals = 0;
                        if (data?.BriefedCtcTotals && data?.BriefedCtcTotals.length > 0) {
                            data?.BriefedCtcTotals[0].ValuesJson.forEach(value => {
                                totals += parseFloat(value);
                            });
                        }
                        menuBlockProps.
                            value = formatNumberWithMaxLength(
                                totals,
                                NumberFormat.Currency,
                                1,
                                weeklyFlightRange,
                                isValidCurrency ? this.currencyCode : data?.LocalCurrency,
                                this.precisedValue,
                                this.isDecimal,
                                this.ApplyToCurrencyMetric,
                                this.ApplyToNonCurrencyMetric,
                            );
                    }
                    if (data.Depth == 1) {
                        const block = new LeftMenuLevelBlock(menuBlockProps);
                        block.getDirectStyles = () =>
                            StyleMapper.mapStyleToExcelStyle(
                                data.LevelDefinition.Styling,
                                false
                            );
                        briefedCtcColumns.push(block);
                    } else if (data.Depth == 2) {
                        const block = new LeftMenuSettingBlock(menuBlockProps);
                        block.getDirectStyles = () =>
                            StyleMapper.mapStyleToExcelStyle(
                                data.LevelDefinition.Styling,
                                false
                            );
                        briefedCtcColumns.push(block);
                    } else if (data.Depth > 2) {
                        const block = new LeftMenuSubLevelBlock(menuBlockProps);
                        block.getDirectStyles = () =>
                            StyleMapper.mapStyleToExcelStyle(
                                data.LevelDefinition.Styling,
                                false
                            );
                        briefedCtcColumns.push(block);
                    }

                }

                // when the briefed column is selected start the column start from zero
                const actualColumnLength = countWeeks - additionalColumnForCalendar;
                const actualColumnStart = this.BriefedctcColumnSelected ? 0 : -countWeeks;
                const lastLevel = data.Order + 1 >= levelOrder.length && (data.SubLevels == null || data.SubLevels?.length == 0);

                // Build inflight overlay rows
                const getInflightOverlaySum = (inflightOverlays: string[][], inflightOverlayIndex: number): number => {
                    return sum(inflightOverlays.map(c => c && c[inflightOverlayIndex] && JSON.parse(c[inflightOverlayIndex]) != null ? parseFloat(JSON.parse(c[inflightOverlayIndex])) : 0).flat());
                }

                const def = LevelDefinition.find(
                    (x) => x.Depth === data.Depth && x.Order === data.Order && x.Name === data.Name && x.SettingIndex === data.SettingIndex
                );

                if (flightBarPeriods?.filter(flightBarPeriod => flightBarPeriod?.data?.inflightOverlay && flightBarPeriod?.data?.inflightOverlay != "InflightOverlayNA")?.length || flightBarPeriods?.filter(flightBarPeriod => flightBarPeriod?.data?.inflightOverlays && flightBarPeriod?.data?.inflightOverlays.length)?.length) {
                    data.Metrics[0]?.InflightOverlays.forEach((inflightOverlay, inflightOverlayIndex) => {
                        let isDataExists = data.Metrics.some(c => c.InflightOverlays && c.InflightOverlays[inflightOverlayIndex] && JSON.parse(c.InflightOverlays[inflightOverlayIndex]) != null);
                        if (isDataExists) {// check if the data exists for the inflight overlay
                            const inflightOverlayColumns: Block[] = [];

                            // Build and push flight bar wall inflight overlay background column
                            const inflightOverlayBackgroundColumn: Block =
                                new FlightBarWallBackgroundBlock({
                                    columnLength: actualColumnLength,
                                    merge: false,
                                });
                            if (this.BriefedctcColumnSelected) {
                                inflightOverlayColumns.push(new SpacerBlock());
                            }
                            inflightOverlayColumns.push(
                                inflightOverlayBackgroundColumn
                            );

                            const inflightOverlaySelected = data.LevelDefinition?.Settings.find(c =>
                                c.Order == data.Order)?.InflightOverlays?.[inflightOverlayIndex];
                            const isImpressionMetric = data.LevelDefinition?.Settings.find(c =>
                                c.Order == data.Order)?.InflightOverlays?.[inflightOverlayIndex].ColumnName &&
                                inflightOverlayMetrics.includes(
                                    data.LevelDefinition?.Settings.find(c =>
                                        c.Order == data.Order)?.InflightOverlays?.[inflightOverlayIndex].ColumnName.toLowerCase()
                                );
                            const inflightOverlayNumberFormat = isImpressionMetric ? findNumberFormat(
                                inflightOverlaySelected.TableId,
                                inflightOverlaySelected.ColumnName,
                                this.columns
                            ) : null;

                            // Build and push flight bar wall bar columns
                            const inflightOverlayValueColumns =
                                this.buildPeriodicBlocks({
                                    startDate,
                                    endDate,
                                    columnStart: actualColumnStart,
                                    columnLength: actualColumnLength,
                                    periodicData: flightBarPeriods,
                                    calendarType: this.CalendarHelper.calendarType,
                                    calendarStartDate: this.ReferencedCalendarDefinition?.Definition.Configuration.CustomStartDate ?? startDate,
                                    isFilterRequired: false,
                                    isFlightRangeSelected: data?.FlightRange != FlightRange.None && data?.FlightRange != FlightRange.FlightTotal,
                                    isLastLevel: lastLevel,
                                    isNormalCalendarEnabled: isNormalCalendarEnabled,
                                    flightRange: data?.FlightRange,
                                    normalizedStartDate: actualStartDate,
                                    extraWeeks,
                                    referencedCalendarDefinition: this.ReferencedCalendarDefinition,
                                    blockCb: ({
                                        columnStart,
                                        columnLength,
                                        data: blockData,
                                    }) => {

                                        const isInflightOverlayExists = blockData?.inflightOverlays && blockData?.inflightOverlays.length > 0 && blockData?.inflightOverlays.some(c => c[inflightOverlayIndex] && JSON.parse(c[inflightOverlayIndex]) != null);// && blockData?.inflightOverlays[0][inflightOverlayIndex] && JSON.parse(blockData?.inflightOverlays[0][inflightOverlayIndex]) != null;
                                        let formattedValue = null;
                                        const metricSumInflight = isImpressionMetric && isInflightOverlayExists ? getInflightOverlaySum(blockData?.inflightOverlays, inflightOverlayIndex) : 0;
                                        if (isImpressionMetric && isInflightOverlayExists) {
                                            formattedValue = formatNumberWithMaxLength(
                                                metricSumInflight,
                                                inflightOverlayNumberFormat,
                                                1,
                                                weeklyFlightRange,
                                                isValidCurrency ? this.currencyCode : blockData?.currency,
                                                this.precisedValue,
                                                this.isDecimal,
                                                this.ApplyToCurrencyMetric,
                                                this.ApplyToNonCurrencyMetric,
                                            );
                                        }



                                        let currentInflightOverlay = isInflightOverlayExists ? blockData?.inflightOverlays[0][inflightOverlayIndex] : null;
                                        const isCustomFlightRange = data.LevelDefinition?.Settings.find(c => c.Order == data.Order)?.InflightOverlays?.length && data.LevelDefinition?.Settings.find(c => c.Order == data.Order)?.InflightOverlays?.[inflightOverlayIndex].ColumnName == "CustomFlightRange";
                                        const isFlightRange = data?.FlightRange != FlightRange.None && data?.FlightRange != FlightRange.FlightTotal;
                                        const customStartEndDate = isCustomFlightRange ? `'${moment.utc(blockData?.flightStartDate).format(dateFormat)} - ${moment.utc(blockData?.flightEndDate).format(dateFormat)}` : '';

                                        return (lastLevel || blockData?.flightBarSum > 0) && (isInflightOverlayExists || isImpressionMetric)
                                            ? new FlightBarWallInflightOverlayBlock({
                                                legendColumn: blockData?.legend != null ? `${legendKey}$${themeLegendColumn}_${blockData?.legend}` : legendKey,
                                                columnStart,
                                                columnLength,
                                                value: (isImpressionMetric ? formattedValue || '' :
                                                    (isCustomFlightRange ? customStartEndDate :
                                                        currentInflightOverlay != "InflightOverlayNA" ?
                                                            moment(JSON.parse(currentInflightOverlay), true).isValid() ?
                                                                `'${moment.utc(JSON.parse(currentInflightOverlay)).format(dateFormat)}` :
                                                                JSON.parse(currentInflightOverlay) : undefined))

                                            })
                                            : null;
                                    },
                                });

                            inflightOverlayValueColumns?.map(
                                (x, index) =>
                                (x.getDirectStyles = () =>
                                    StyleMapper.mapStyleToExcelStyle(
                                        // @ts-ignore
                                        def?.LevelDefinition?.Settings.find(c => c.Order == data.Order)
                                            .InflightOverlays[inflightOverlayIndex].Styling ?? {},
                                        false
                                    ))
                            );

                            inflightOverlayColumns.push(...inflightOverlayValueColumns);
                            rows.push(inflightOverlayColumns);
                        }
                    });
                }

                const flightBarColumns: Block[] = [];

                // Build and push flight bar wall bar background column
                const barBackgroundColumn: Block =
                    new FlightBarWallBackgroundBlock({
                        columnLength: countWeeks,
                        merge: false,
                    });
                flightBarColumns.push(barBackgroundColumn);

                const numberFormat = findNumberFormat(
                    data.TableId,
                    data.MetricColumnName,
                    this.columns
                );


                // Build and push flight bar wall bar columns
                const barColumns = this.buildPeriodicBlocks({
                    startDate,
                    endDate,
                    columnStart: actualColumnStart,
                    columnLength: actualColumnLength === 1 ? 1 : actualColumnLength,
                    periodicData: flightBarPeriods,
                    calendarType: this.CalendarHelper.calendarType,
                    calendarStartDate: this.ReferencedCalendarDefinition?.Definition.Configuration.CustomStartDate ?? startDate,
                    isFilterRequired: false,
                    isFlightRangeSelected: data?.FlightRange != FlightRange.None && data?.FlightRange != FlightRange.FlightTotal,
                    isLastLevel: lastLevel,
                    isNormalCalendarEnabled: isNormalCalendarEnabled,
                    flightRange: data?.FlightRange,
                    normalizedStartDate: actualStartDate,
                    extraWeeks,
                    referencedCalendarDefinition: this.ReferencedCalendarDefinition,
                    blockCb: ({
                        columnStart,
                        columnLength,
                        data: blockData,
                    }) => {
                        return lastLevel || blockData?.flightBarSum > 0
                            ? blockData?.isMetricNone ? new FlightBarWallBarBlock({
                                columnStart,
                                columnLength,
                                value: ' ',
                            }) : new FlightBarWallBarBlock({
                                legendColumn: blockData?.legend != null ? `${legendKey}$${themeLegendColumn}_${blockData?.legend}` : legendKey,
                                columnStart,
                                columnLength,
                                value:
                                    data.FlightRange !== API.FlightRange.None
                                        ? formatNumberWithMaxLength(
                                            blockData?.flightBarSum,
                                            numberFormat,
                                            1,
                                            weeklyFlightRange,
                                            isValidCurrency ? this.currencyCode : blockData?.currency,
                                            this.precisedValue,
                                            this.isDecimal,
                                            this.ApplyToCurrencyMetric,
                                            this.ApplyToNonCurrencyMetric,

                                        )
                                        : ' ',
                            })
                            : null;
                    },
                });

                barColumns?.map(
                    (x, index) =>
                    (x.getDirectStyles = () =>
                        StyleMapper.mapStyleToExcelStyle(
                            // @ts-ignore
                            def?.LevelDefinition?.Settings.find(c => c.Order == data.Order)
                                .Styling ?? {},
                            false
                        ))
                );

                if (barColumns[0]) {
                    // save legend entry with styling if it does not yet exist or if legendData is undefined
                    if (
                        (this.legendData &&
                            !this.legendData.find(
                                (l) => l.name === data.Name
                            )) ||
                        !this.legendData
                    ) {
                        this.legendData.push({
                            name: data.Name,
                            color: barColumns[0].getStyles().fill?.color,
                        });
                    }
                }
                flightBarColumns.push(...briefedCtcColumns);
                flightBarColumns.push(...barColumns);
                rows.push(flightBarColumns);

                // Build subtotals rows
                if (data.SubTotals && data.SubTotals.length) {
                    data.SubTotals.forEach((subTotal, subTotalIndex) => {

                        // Merge sub total bar metrics
                        const subTotalBars = mergeAndSortMetrics(
                            subTotal?.Metrics,
                            subTotal?.FlightRange,
                            []
                        );
                        const isSubTotalNormalQuarter = !isStandardCalendar && isNormalCalendarEnabled && subTotal.FlightRange == API.FlightRange.Quarterly;
                        // Build subtotals periods
                        const subTotalsPeriods = subTotal.FlightRange != FlightRange.FlightTotal ?
                            getPeriods(
                                isSubTotalNormalQuarter ? dates?.dateFrom ?? startDate : startDate,
                                isSubTotalNormalQuarter ? dates?.dateTo ?? endDate : endDate,
                                normalizeFlightRangePeriod(subTotal.FlightRange),
                                this.CalendarHelper.calendarType
                            ).map((subTotalsPeriod) => {
                                let metricSum = 0;
                                let legend;

                                subTotalBars.forEach(
                                    ({ EffectiveDate, ValueJson, Legend }) => {
                                        const metricStartDate = moment
                                            .utc(EffectiveDate)
                                            .toDate();
                                        const metricEndDate = moment
                                            .utc(EffectiveDate)
                                            .subtract(1, 'second')
                                            .toDate();

                                        if (
                                            //metricStartDate >=
                                            //    subTotalsPeriod.startDate &&
                                            //metricEndDate <= subTotalsPeriod.endDate
                                            metricStartDate.getTime() >= subTotalsPeriod.startDate.getTime() && metricStartDate.getTime() <= subTotalsPeriod.endDate.getTime()
                                        ) {
                                            metricSum += ValueJson;
                                        }
                                        if (!legend) {
                                            legend =
                                                Legend;
                                        }
                                    }
                                );

                                return {
                                    startDate: subTotalsPeriod.actualStartDate ?? subTotalsPeriod.startDate,
                                    endDate: subTotalsPeriod.actualEndDate ?? subTotalsPeriod.endDate,
                                    data: {
                                        metricSum,
                                        isMetricNone: subTotal.ColumnName == optionMetricNone,
                                        currency: data.LocalCurrency,
                                        legend: legend
                                    },
                                };
                            }) : subTotalBars.map((flightBar) => ({
                                startDate: flightBar.FlightStart,
                                endDate: flightBar.FlightEnd,
                                data: {
                                    metricSum: flightBar.ValueJson,
                                    isMetricNone: subTotal.ColumnName == optionMetricNone,
                                    currency: data.LocalCurrency,
                                    legend: flightBar.Legend
                                },
                            }));

                        const numberFormat = findNumberFormat(
                            data.TableId,
                            data.SubTotals[subTotalIndex].ColumnName,
                            this.columns
                        );

                        /* to apply the formatting for flight total subtotals  ends */
                        const subTotalBarColumns: Block[] = [];
                        const subTotalBackgroundColumn: Block =
                            new FlightBarWallBackgroundBlock({
                                columnLength: actualColumnLength,
                                merge: false,
                            });
                        // briefed ctc columns
                        if (this.BriefedctcColumnSelected) {
                            subTotalBarColumns.push(new SpacerBlock());
                        }
                        subTotalBarColumns.push(subTotalBackgroundColumn);
                        // Build and push subtotals row
                        const subTotalsColumns = this.buildPeriodicBlocks({
                            startDate,
                            endDate,
                            columnStart: -countWeeks + additionalColumnForCalendar,
                            columnLength: actualColumnLength,
                            periodicData: subTotalsPeriods,
                            calendarType: this.CalendarHelper.calendarType,
                            calendarStartDate: this.ReferencedCalendarDefinition?.Definition.Configuration.CustomStartDate ?? startDate,
                            isFilterRequired: false,
                            isFlightRangeSelected: subTotal?.FlightRange != FlightRange.None && subTotal?.FlightRange != FlightRange.FlightTotal,
                            isLastLevel: false,
                            isNormalCalendarEnabled: isNormalCalendarEnabled,
                            flightRange: subTotal?.FlightRange,
                            normalizedStartDate: actualStartDate,
                            extraWeeks,
                            referencedCalendarDefinition: this.ReferencedCalendarDefinition,
                            blockCb: ({
                                columnStart,
                                columnLength,
                                data: blockData,
                            }) => {
                                let block: Block;
                                const isFlightTotal = subTotal.FlightRange == FlightRange.FlightTotal;
                                const value = blockData.isMetricNone ? ' ' : formatNumberWithMaxLength(
                                    blockData.metricSum > 0 || !isFlightTotal ? blockData.metricSum : 0,
                                    numberFormat,
                                    1,
                                    weeklyFlightRange,
                                    isValidCurrency ? this.currencyCode : blockData?.currency,
                                    this.precisedValue,
                                    this.isDecimal,
                                    this.ApplyToCurrencyMetric,
                                    this.ApplyToNonCurrencyMetric,
                                );

                                if (blockData.metricSum > 0 || !isFlightTotal) {
                                    block = new FlightBarWallSubTotalBlock({
                                        legendColumn: blockData?.legend != null ? `${legendKey}$${themeLegendColumn}_${blockData?.legend}` : legendKey,
                                        columnStart,
                                        columnLength,
                                        value,
                                        isMetricNone: blockData.isMetricNone,
                                    });
                                } else {
                                    block = new FlightBarWallSubTotalBlock({
                                        columnStart,
                                        columnLength,
                                        value: '',
                                        isMetricNone: blockData.isMetricNone
                                    });
                                }

                                return block;
                            },
                        });

                        subTotalsColumns?.map((x) => {
                            if (!x) {
                                return x;
                            }
                            return (x.getDirectStyles = () =>
                                x.value?.length < 1 ? {} : StyleMapper.mapStyleToExcelStyle(
                                    // @ts-ignore
                                    this.ReferencedMediaHierarchyDefinition
                                        .Definition?.Levels[data.Depth - 1]
                                        ?.Settings.find(c => c.Order == data.Order)?.SubTotals[
                                        subTotalIndex
                                    ].Styling ?? {},
                                    false
                                ));
                        });

                        subTotalBarColumns.push(...subTotalsColumns);
                        if (this.showSubtotalsAtBottom) {
                            if (!subTotalStack[data.Depth]) {
                                subTotalStack[data.Depth] = [[]];
                            }
                            subTotalStack[data.Depth].push(subTotalBarColumns);
                        }
                        else {
                            rows.push(subTotalBarColumns);
                        }
                    });
                }

                // Build sub total summary
                const settingsObj = data.LevelDefinition?.Settings?.find(
                    (s) => s.Order === data.Order
                );

                if (data.SubTotalSummary && data.SubTotalSummary.length) {
                    let addedOrdersForSubToatlSummary = new Set<Number>();

                    const createSubTotalBlocks = (subTotal: any) => {

                        const subTotalBars = mergeAndSortMetrics(
                            subTotal?.Metrics,
                            subTotal?.FlightRange,
                            []
                        );
                        const isSubTotalSummaryNormalQuarter = isNormalCalendarEnabled && subTotal.FlightRange == API.FlightRange.Quarterly;

                        // Build subtotals periods
                        const subTotalsSummaryPeriods = subTotal.FlightRange != FlightRange.FlightTotal ?
                            getPeriods(
                                isSubTotalSummaryNormalQuarter ? dates?.dateFrom ?? startDate : startDate,
                                isSubTotalSummaryNormalQuarter ? dates?.dateTo ?? endDate : endDate,
                                normalizeFlightRangePeriod(subTotal.FlightRange),
                                this.CalendarHelper.calendarType
                            ).map((subTotalsPeriod) => {
                                let metricSum = 0;
                                let legend;

                                subTotalBars.forEach(
                                    ({ EffectiveDate, ValueJson, Legend }) => {
                                        const metricStartDate = moment
                                            .utc(EffectiveDate)
                                            .toDate();
                                        const metricEndDate = moment
                                            .utc(EffectiveDate)
                                            .subtract(1, 'second')
                                            .toDate();

                                        if (
                                            metricStartDate.getTime() >= subTotalsPeriod.startDate.getTime() && metricStartDate.getTime() <= subTotalsPeriod.endDate.getTime()
                                        ) {
                                            metricSum += ValueJson;
                                        }
                                        if (!legend) {
                                            legend =
                                                Legend;
                                        }
                                    }
                                );

                                return {
                                    startDate: subTotalsPeriod.actualStartDate ?? subTotalsPeriod.startDate,
                                    endDate: subTotalsPeriod.actualEndDate ?? subTotalsPeriod.endDate,
                                    data: {
                                        metricSum,
                                        isMetricNone: subTotal.ColumnName == optionMetricNone,
                                        currency: data.LocalCurrency,
                                        legend: legend
                                    },
                                };
                            }) : subTotalBars.map((flightBar) => ({
                                startDate: flightBar.FlightStart,
                                endDate: flightBar.FlightEnd,
                                data: {
                                    metricSum: flightBar.ValueJson,
                                    isMetricNone: subTotal.ColumnName == optionMetricNone,
                                    currency: data.LocalCurrency,
                                    legend: flightBar.Legend
                                },
                            }));

                        const numberFormat = findNumberFormat(
                            subTotal.TableId,
                            subTotal.ColumnName,
                            this.columns
                        );

                        /* to apply the formatting for flight total subtotals  ends */
                        const subTotalBarColumns: Block[] = [];
                        const subTotalBackgroundColumn: Block =
                            new FlightBarWallBackgroundBlock({
                                columnLength: actualColumnLength,
                                merge: false,
                            });
                        // briefed ctc columns
                        if (this.BriefedctcColumnSelected) {
                            subTotalBarColumns.push(new SpacerBlock());
                        }
                        subTotalBarColumns.push(subTotalBackgroundColumn);
                        // Build and push subtotals row
                        const subTotalsColumns = this.buildPeriodicBlocks({
                            startDate,
                            endDate,
                            columnStart: -countWeeks + additionalColumnForCalendar,
                            columnLength: actualColumnLength,
                            periodicData: subTotalsSummaryPeriods,
                            calendarType: this.CalendarHelper.calendarType,
                            calendarStartDate: this.ReferencedCalendarDefinition?.Definition.Configuration.CustomStartDate ?? startDate,
                            isFilterRequired: false,
                            isFlightRangeSelected: subTotal?.FlightRange != FlightRange.None && subTotal?.FlightRange != FlightRange.FlightTotal,
                            isLastLevel: false,
                            isNormalCalendarEnabled: isNormalCalendarEnabled,
                            flightRange: subTotal?.FlightRange,
                            extraWeeks,
                            normalizedStartDate: actualStartDate,
                            referencedCalendarDefinition: this.ReferencedCalendarDefinition,
                            blockCb: ({
                                columnStart,
                                columnLength,
                                data: blockData,
                            }) => {
                                let block: Block;
                                const isFlightTotal = subTotal.FlightRange == FlightRange.FlightTotal;
                                const value = blockData.isMetricNone ? ' ' : formatNumberWithMaxLength(
                                    blockData.metricSum > 0 || !isFlightTotal ? blockData.metricSum : 0,
                                    numberFormat,
                                    1,
                                    weeklyFlightRange,
                                    isValidCurrency ? this.currencyCode : blockData?.currency,
                                    this.precisedValue,
                                    this.isDecimal,
                                    this.ApplyToCurrencyMetric,
                                    this.ApplyToNonCurrencyMetric,
                                );

                                if (blockData.metricSum > 0 || !isFlightTotal) {
                                    block = new FlightBarWallSubTotalBlock({
                                        legendColumn: blockData?.legend != null ? `${legendKey}$${themeLegendColumn}_${blockData?.legend}` : legendKey,
                                        columnStart,
                                        columnLength,
                                        value,
                                        isMetricNone: blockData.isMetricNone,
                                    });
                                } else {
                                    block = new FlightBarWallSubTotalBlock({
                                        columnStart,
                                        columnLength,
                                        value: '',
                                        isMetricNone: blockData.isMetricNone
                                    });
                                }

                                return block;
                            },
                        });

                        subTotalsColumns?.map((x) => {
                            if (!x) {
                                return x;
                            }
                            return (x.getDirectStyles = () =>
                                x.value?.length < 1 ? {} : StyleMapper.mapStyleToExcelStyle(
                                    // @ts-ignore
                                    this.ReferencedMediaHierarchyDefinition
                                        .Definition?.Levels[data.Depth - 1]
                                        ?.Settings.find(c => c.Order == data.Order)?.SubTotalSummary[
                                        subTotal.Order
                                    ].Styling ?? {},
                                    false
                                ));
                        });

                        subTotalBarColumns.push(...subTotalsColumns);
                        if (this.showSubtotalsAtBottom) {
                            if (!subTotalSummaryStack[data.Depth]) {
                                subTotalSummaryStack[data.Depth] = [[]];
                            }
                            subTotalSummaryStack[data.Depth].push(subTotalBarColumns);
                        }
                        else {
                            //subTotalBarColumns.push(...subTotalsColumns);
                            rows.push(subTotalBarColumns);
                        }
                    }
                    data.SubTotalSummary.forEach((subTotal, subTotalIndex) => {
                        if (addedOrdersForSubToatlSummary.size && addedOrdersForSubToatlSummary.has(subTotal.Order))
                            return;
                        const displayNames = settingsObj?.SubTotalSummary[subTotal.Order]?.DisplayNames ?? {};

                        if (Object.entries(displayNames).length > 0) {
                            const subTotals = filter(data?.SubTotalSummary, { Order: subTotal.Order }) || [];
                            Object.entries(displayNames).forEach(displayName => {
                                addedOrdersForSubToatlSummary.add(subTotal.Order);
                                // Use find to get the first matching subTotal, or undefined if not found
                                const subTotalP = subTotals.find(subTotal =>
                                    displayName[0].includes(subTotal.ColumnName) &&
                                    displayName[0].includes(subTotal.DisplayName)
                                );

                                if (subTotalP) {
                                    createSubTotalBlocks(subTotalP);
                                }
                            });

                        }
                        else {
                            createSubTotalBlocks(subTotal);
                        }
                    });
                }

                if (this.showSubtotalsAtBottom) {
                    depthCursor = data.Depth;
                }

            });
            if (subTotalStack.length) {
                for (let i = subTotalStack.length - 1; i >= 1; i--) {
                    if (subTotalStack.length && subTotalStack[i]?.length) {
                        while (subTotalStack[i].length) {
                            const tempValue = subTotalStack[i].pop();
                            if (tempValue.length) {
                                rows.push(tempValue);

                            }
                        }
                    }
                }
            }

            if (subTotalSummaryStack.length) {
                for (let i = subTotalSummaryStack.length - 1; i >= 1; i--) {
                    if (subTotalSummaryStack.length && subTotalSummaryStack[i]?.length) {
                        while (subTotalSummaryStack[i].length) {
                            const tempValue = subTotalSummaryStack[i].pop();
                            if (tempValue.length) {
                                rows.push(tempValue);

                            }
                        }
                    }
                }
            }
        }

        this.flightBarWallColumnLength = this.getColumnLength(rows);
        this.flightBarWallRowLength = this.getRowLength(rows);
        return rows;
    };

}
