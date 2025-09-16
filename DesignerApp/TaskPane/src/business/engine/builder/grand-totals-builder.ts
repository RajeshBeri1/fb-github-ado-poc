import {
    CalendarType,
    FlightRange,
    ReferencedCalendarDefinition,
    ReferencedGrandTotalDefinition,
    ReferencedMediaHierarchyDefinition,
} from '@omniflow/omni-webapi';
import * as API from '@omniflow/omni-webapi';
import { find, isEmpty } from 'lodash';

import { BuildEngine } from '../models/build-engine';
import { Block } from '../models/block';
import ValueBlock from '../blocks/grand-totals/value-block';
import { CalendarHelper } from '../helpers/calendar-helper';
import { getPeriod, getPeriods, PeriodUnit } from '../helpers/period-helper';
import CalendarOverlayLeftMenuLevelBlock from '../blocks/calendar-overlay/calendar-overlay-left-menu-level-block';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { StyleMapper } from '../models/styles';
import {
    findNumberFormat,
    formatNumberWithMaxLength,
} from '../helpers/number-format-helper';
import moment from 'moment';
import { getExtraWeekForSpace } from '../helpers/week-count-helper';
import { normalizeCalendarDateRange } from '../helpers/calendar-helper';
import { checkCompositeKeyInAncestors } from '../helpers/level-helper';
export interface IGrandTotalsBuilderProps {
    clientId: string;
    columns: TColumn[];
    ReferencedGrandTotalDefinition?: ReferencedGrandTotalDefinition;
    ReferencedCalendarDefinition?: ReferencedCalendarDefinition;
    CalendarHelper?: CalendarHelper;
    ReferencedMediaHierarchyDefinition?: ReferencedMediaHierarchyDefinition;
    LevelFlowchartData?: API.FlowchartData;
    leftMenuColumnLength: number;
    totalsBlocks: Block[][];
    BriefedctcColumnSelected?: boolean;
    precisedValue?: boolean;
    isDecimal?: boolean;
    ApplyToCurrencyMetric?:boolean;
    ApplyToNonCurrencyMetric?: boolean;
    currencyCode?: string;
    NormalCalendarHelper?: CalendarHelper;
    ancestorSet?: Set<string>;

}

export class GrandTotalsBuilder extends BuildEngine {
    clientId: string;
    ReferencedGrandTotalDefinition: ReferencedGrandTotalDefinition;
    ReferencedCalendarDefinition: ReferencedCalendarDefinition;
    CalendarHelper: CalendarHelper;
    ReferencedMediaHierarchyDefinition: ReferencedMediaHierarchyDefinition;
    LevelFlowchartData: API.FlowchartData;
    BriefedctcColumnSelected?: boolean;
    leftMenuColumnLength: number = 0;
    grandTotalsColumnLength: number = 0;
    grandTotalsRowLength: number = 0;
    columns: TColumn[];
    totalsBlocks: Block[][];
    precisedValue: boolean;
    isDecimal: boolean;
    ApplyToCurrencyMetric: boolean;
    ApplyToNonCurrencyMetric: boolean;
    currencyCode?: string;
    NormalCalendarHelper?: CalendarHelper;
    ancestorSet?: Set<string> = new Set<string>();
    constructor({
        clientId,
        ReferencedGrandTotalDefinition,
        ReferencedCalendarDefinition,
        CalendarHelper,
        ReferencedMediaHierarchyDefinition,
        LevelFlowchartData,
        leftMenuColumnLength,
        columns,
        totalsBlocks,
        BriefedctcColumnSelected,
        precisedValue,
        isDecimal,
        ApplyToCurrencyMetric,
        ApplyToNonCurrencyMetric,
        currencyCode,
        NormalCalendarHelper,
        ancestorSet
    }: IGrandTotalsBuilderProps) {
        super();

        this.clientId = clientId;
        this.ReferencedGrandTotalDefinition = ReferencedGrandTotalDefinition;
        this.ReferencedCalendarDefinition = ReferencedCalendarDefinition;
        this.CalendarHelper = CalendarHelper;
        this.ReferencedMediaHierarchyDefinition =
            ReferencedMediaHierarchyDefinition;
        this.LevelFlowchartData = LevelFlowchartData;
        this.leftMenuColumnLength = leftMenuColumnLength;
        this.columns = columns;
        this.totalsBlocks = totalsBlocks;
        this.BriefedctcColumnSelected = BriefedctcColumnSelected;
        this.precisedValue = precisedValue;
        this.isDecimal = isDecimal;
        this.ApplyToCurrencyMetric = ApplyToCurrencyMetric;
        this.ApplyToNonCurrencyMetric = ApplyToNonCurrencyMetric;
        this.currencyCode = currencyCode;
        this.NormalCalendarHelper = NormalCalendarHelper;
        this.ancestorSet = ancestorSet;
    }

    build = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];

        if (
            this.ReferencedGrandTotalDefinition?.Definition &&
            this.ReferencedCalendarDefinition?.Definition &&
            this.CalendarHelper &&
            this.ReferencedMediaHierarchyDefinition?.Definition &&
            this.LevelFlowchartData
        ) {
            rows = await this.buildGrandTotals();
        }
        return rows;
    };

    buildGrandTotals = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];
        let rowStart = 0;
        let weeklyFlightRange = this.LevelFlowchartData.GrandTotals?.some(grandTotal => grandTotal.FlightRange == FlightRange.Weekly || grandTotal.FlightRange == FlightRange.WeeklyBroadcast);
        let isValidCurrency = this.currencyCode && this.currencyCode != 'LLL';
        const isNormalCalendarEnabled = (this.ReferencedCalendarDefinition?.Definition?.Configuration?.Type == CalendarType.Broadcast || this.ReferencedCalendarDefinition?.Definition?.Configuration?.Type == CalendarType.Standard)&& this.ReferencedCalendarDefinition?.Definition?.Configuration?.IsNormalizedEnabled;
        this.LevelFlowchartData.GrandTotals.forEach(
            (grandTotal, grandTotalIndex) => {
                const totals: {
                    StartDate: Date;
                    EndDate: Date;
                    Text: number;
                    ActualStartDate?: Date;
                    ActualEndDate?: Date;
                    LocalCurrency?: string;
                }[] = [];
                let periods = [];
                let selectedPeriodUnit: PeriodUnit;
                const columnData = find(this.columns, {
                    Name: grandTotal.ColumnName,
                    TableId: grandTotal.TableId,
                });

                if (grandTotalIndex == 0 && !isEmpty(this.totalsBlocks)) {
                    this.totalsBlocks.forEach(totalBlock => {
                        rowStart = Math.min(rowStart, totalBlock[0]?.rowStart)
                    })
                }
                else {
                    rowStart = 0;
                }

                const row = [];
                const leftMenuBlock = new CalendarOverlayLeftMenuLevelBlock({
                    columnLength: this.leftMenuColumnLength,
                    rowStart,
                    value: `${grandTotal.FlightRange} ${columnData
                        ? columnData.DisplayName
                        : grandTotal.ColumnName
                        }`,
                });
                leftMenuBlock.getDirectStyles = () =>
                    StyleMapper.mapStyleToExcelStyle(
                        this.ReferencedGrandTotalDefinition.Definition
                            .Selections[grandTotalIndex].LeftMenuStyling,
                        false
                    );

                row.push(leftMenuBlock);
                if (this.BriefedctcColumnSelected) {

                    row.push(new ValueBlock({ value: ' ', columnStart: 0, rowStart: rowStart, columnLength: 1 }));
                }
                const dates = { dateFrom: null, dateTo: null };
                let actualStartDate = this.ReferencedCalendarDefinition?.Definition?.Configuration?.CustomStartDate ?? this.CalendarHelper.dataFrom;
                let actualEndDate = this.ReferencedCalendarDefinition?.Definition?.Configuration?.CustomEndDate ?? this.CalendarHelper.dataTo;
                const isStandardAndUseCustomStartDay = this.ReferencedCalendarDefinition?.Definition?.Configuration?.Type == CalendarType.Standard && this.ReferencedCalendarDefinition?.Definition?.Configuration?.UseCustomStartDay;
                function getAnnualPeriodUnit(calendarType: CalendarType): PeriodUnit {
                    return calendarType === CalendarType.Broadcast ? PeriodUnit.BC_YEAR : PeriodUnit.YEAR;
                }
                if (isNormalCalendarEnabled) {
                    dates.dateFrom = this.NormalCalendarHelper?.dataFrom;
                    dates.dateTo = this.NormalCalendarHelper?.dataTo;
                    const startDay = !isStandardAndUseCustomStartDay && !isNormalCalendarEnabled ? 0 : 1;
                    const { normalizedStartDate, normalizedEndDate } = normalizeCalendarDateRange(
                        actualStartDate,
                        actualEndDate,
                        isNormalCalendarEnabled,
                        startDay
                    );

                    actualStartDate = normalizedStartDate;
                    actualEndDate = normalizedEndDate;
                }
                switch (grandTotal.FlightRange) {
                    case FlightRange.Monthly:
                        periods = getPeriods(
                            this.CalendarHelper.dataFrom,
                            this.CalendarHelper.dataTo,
                            PeriodUnit.MONTH,
                            this.CalendarHelper.calendarType
                        );
                        selectedPeriodUnit = PeriodUnit.MONTH;
                        break;
                    case FlightRange.MonthlyBroadcast:
                        periods = getPeriods(
                            this.CalendarHelper.dataFrom,
                            this.CalendarHelper.dataTo,
                            PeriodUnit.BC_MONTH,
                            this.CalendarHelper.calendarType
                        );
                        selectedPeriodUnit = PeriodUnit.BC_MONTH;
                        break;
                    case FlightRange.Weekly:
                        periods = getPeriods(
                            this.CalendarHelper.dataFrom,
                            this.CalendarHelper.dataTo,
                            PeriodUnit.WEEK,
                            this.CalendarHelper.calendarType
                        );
                        selectedPeriodUnit = PeriodUnit.WEEK;
                        break;
                    case FlightRange.WeeklyBroadcast:
                        periods = getPeriods(
                            this.CalendarHelper.dataFrom,
                            this.CalendarHelper.dataTo,
                            PeriodUnit.BC_WEEK,
                            this.CalendarHelper.calendarType
                        );
                        selectedPeriodUnit = PeriodUnit.BC_WEEK;
                        break;
                    case FlightRange.Quarterly:
                        periods = getPeriods(
                            dates?.dateFrom ?? this.CalendarHelper.dataFrom,
                            dates?.dateTo ?? this.CalendarHelper.dataTo,
                            PeriodUnit.QUARTER,
                            this.CalendarHelper.calendarType
                        );
                        selectedPeriodUnit = PeriodUnit.QUARTER;
                        console.log('totals for periods quarter', JSON.stringify(periods));
                        break;
                    case FlightRange.QuarterlyBroadcast:
                        periods = getPeriods(
                            this.CalendarHelper.dataFrom,
                            this.CalendarHelper.dataTo,
                            PeriodUnit.BC_QUARTER,
                            this.CalendarHelper.calendarType
                        );
                        selectedPeriodUnit = PeriodUnit.BC_QUARTER;
                        break;
                    case FlightRange.Annually: {
                        const annualPeriodUnit = getAnnualPeriodUnit(this.CalendarHelper.calendarType);
                        periods = getPeriods(
                            this.CalendarHelper.dataFrom,
                            this.CalendarHelper.dataTo,
                            annualPeriodUnit,
                            this.CalendarHelper.calendarType
                        );
                        selectedPeriodUnit = annualPeriodUnit;
                        break;
                    }
                    case FlightRange.AnnuallyBroadcast:
                        periods = getPeriods(
                            this.CalendarHelper.dataFrom,
                            this.CalendarHelper.dataTo,
                            PeriodUnit.BC_YEAR,
                            this.CalendarHelper.calendarType
                        );
                        selectedPeriodUnit = PeriodUnit.BC_YEAR;
                        break;
                    case (FlightRange.None, FlightRange.FlightTotal):
                        // Ignore
                        break;
                }

                //remove the dates greate than end date
                if (periods && periods.length) {
                    periods = periods.filter(period => moment(period.startDate).isSameOrBefore(moment(this.CalendarHelper.dataTo)) && (!period.actualStartDate || moment(period.actualStartDate).isSameOrBefore(moment(this.CalendarHelper.dataTo))));
                }

                // Prepare array for calculation
                periods.forEach((period) => {
                    totals.push({
                        StartDate: isNormalCalendarEnabled ? period.actualStartDate ?? period.startDate : period.startDate,
                        EndDate: isNormalCalendarEnabled ? period.actualEndDate ?? period.endDate : period.endDate,
                        Text: 0,
                        ActualEndDate: period.actualEndDate ?? period.endDate,
                        ActualStartDate: period.actualStartDate ?? period.startDate,
                        LocalCurrency: grandTotal.LocalCurrency,
                    });
                });

                // Calculate
                let metrics=[];
                if(!isEmpty(this.ancestorSet)){
                    metrics = grandTotal?.Metrics?.filter(g=> checkCompositeKeyInAncestors(g.CompositeKey,this.ancestorSet));
                    
                }
                else{
                    metrics = grandTotal?.Metrics;
                }
                metrics.forEach((metric) => {
                    const metricStartDate = moment.utc(metric.EffectiveDate).toDate();
                    let currentDataItem = totals.find(
                        (f) => metricStartDate >= f.StartDate && metricStartDate <= f.EndDate
                    );
                    if (currentDataItem) {
                        currentDataItem.Text += Number(metric.ValueJson);
                    }
                });




                const extraWeeks = isNormalCalendarEnabled ? getExtraWeekForSpace(actualStartDate, actualEndDate) : 0;

                // Count calendar weeks and get start and end date of calendar
                const countWeeks = (this.CalendarHelper.data?.length || 0) + extraWeeks;

                const dataForRendering = totals.map((flightBar) => ({
                    startDate: new Date(flightBar.ActualStartDate),
                    endDate: new Date(flightBar.ActualEndDate),
                    data: flightBar,
                }));

                const numberFormat = findNumberFormat(
                    grandTotal.TableId,
                    grandTotal.ColumnName,
                    this.columns
                );
                const flightBarColumns = this.buildPeriodicBlocks({
                    startDate: this.CalendarHelper.dataFrom,
                    endDate: this.CalendarHelper.dataTo,
                    columnStart: 0,
                    columnLength: countWeeks === 1 ? 1 : countWeeks,
                    periodicData: dataForRendering,
                    calendarType: this.CalendarHelper.calendarType,
                    calendarStartDate: this.ReferencedCalendarDefinition?.Definition.Configuration.CustomStartDate ?? this.CalendarHelper.dataFrom,
                    isFilterRequired: true,
                    isFlightRangeSelected: grandTotal.FlightRange != FlightRange.None && grandTotal.FlightRange != FlightRange.FlightTotal,
                    isLastLevel: false,
                    isNormalCalendarEnabled: isNormalCalendarEnabled,
                    flightRange: grandTotal.FlightRange,
                    extraWeeks,
                    normalizedStartDate: actualStartDate,
                    referencedCalendarDefinition: this.ReferencedCalendarDefinition,
                    blockCb: ({
                        columnStart,
                        columnLength,
                        data: blockData,
                    }) => {
                        const valueBlock = new ValueBlock({
                            autofitColumns: false,
                            columnStart,
                            columnLength,
                            rowStart: rowStart,
                            value: formatNumberWithMaxLength(
                                blockData?.Text,
                                numberFormat,
                                1,
                                weeklyFlightRange,
                                isValidCurrency ? this.currencyCode : blockData?.LocalCurrency,
                                this.precisedValue,
                                this.isDecimal,
                                this.ApplyToCurrencyMetric,
                                this.ApplyToNonCurrencyMetric,
                            ),
                        });

                        valueBlock.getDirectStyles = () =>
                            StyleMapper.mapStyleToExcelStyle(
                                this.ReferencedGrandTotalDefinition.Definition
                                    .Selections[grandTotalIndex].Styling,
                                false
                            );

                        return valueBlock;
                    },
                });
                flightBarColumns.forEach((f) => {
                    row.push(f);
                });

                rows.push(row);
            }
        );
        this.grandTotalsColumnLength = this.getColumnLength(rows);
        this.grandTotalsRowLength = this.getRowLength(rows);

        return rows;
    };
}
