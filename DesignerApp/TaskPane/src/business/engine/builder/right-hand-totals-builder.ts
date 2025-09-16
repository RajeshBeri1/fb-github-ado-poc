import {
    FlowchartDataMetric,
    FlowchartDataSubTotal,
    FlowchartDataTotal,
    ReferencedCalendarDefinition,
    ReferencedMediaHierarchyDefinition,
    ReferencedTotalsDefinition,
} from '@omniflow/omni-webapi';
import { BuildEngine } from '../models/build-engine';
import { Block } from '../models/block';
import MainHeadBlock from '../blocks/right-hand-totals/main-head-block';
import HeadBlock from '../blocks/right-hand-totals/head-block';
import ValueBlock from '../blocks/right-hand-totals/value-block';
import TotalBlock from '../blocks/right-hand-totals/total-block';
import { CalendarHelper } from '../helpers/calendar-helper';
import { StyleMapper } from '../models/styles';
import { TFlowchartDataLevelFlat } from '../helpers/level-helper';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { FormatKeyInfo } from '../models/format-key-info';
import { formatNumberFromDataType } from '../helpers/number-format-helper';
import { sum, filter } from 'lodash';
import { mergeAndSortMetrics, TMergedMetrics } from '../helpers/merge-sort-helper';

export interface IRightHandTotalsBuilderProps {
    clientId: string;
    ReferencedTotalsDefinition?: ReferencedTotalsDefinition;
    ReferencedMediaHierarchyDefinition?: ReferencedMediaHierarchyDefinition;
    FlatLevelData?: TFlowchartDataLevelFlat[];
    ReferencedCalendarDefinition?: ReferencedCalendarDefinition;
    CalendarHelper?: CalendarHelper;
    calendarOverlayHeight?: number;
    dataColumns?: TColumn[];
    currencyCode?: string;
    showSubtotalsAtBottom?: boolean;
    inflightOverlayMetrics?: string[];
    isDecimal?: boolean;
    ApplyToCurrencyMetric?: boolean;
    ApplyToNonCurrencyMetric?: boolean;
}


export class RightHandTotalsBuilder extends BuildEngine {
    clientId: string;
    ReferencedTotalsDefinition: ReferencedTotalsDefinition;
    ReferencedMediaHierarchyDefinition: ReferencedMediaHierarchyDefinition;
    FlatLevelData: TFlowchartDataLevelFlat[];
    ReferencedCalendarDefinition: ReferencedCalendarDefinition;
    CalendarHelper: CalendarHelper;
    calendarOverlayHeight: number;

    rightHandTotalsColumnLength: number = 0;
    rightHandTotalsRowLength: number = 0;
    inflightOverlayMetrics?: string[];
    isDecimal?: boolean;
    ApplyToCurrencyMetric?: boolean;
    ApplyToNonCurrencyMetric?: boolean;

    private dataColumns: TColumn[] = [];

    private calculateSubTotalSum(subTotal: FlowchartDataSubTotal): number {
        let sum = 0;
        if (subTotal?.Metrics) {
            subTotal.Metrics.forEach((metric) => {
                if (metric.ValueJson !== undefined) {
                    const value = JSON.parse(metric.ValueJson);
                    sum += value || 0;
                }
            });
        }
        return sum;
    }
    private getInflightOverlaySum = (inflightOverlays: string[][], inflightOverlayIndex: number): number => {
        return sum(inflightOverlays.map(c => c && c[inflightOverlayIndex] && JSON.parse(c[inflightOverlayIndex]) != null ? parseFloat(JSON.parse(c[inflightOverlayIndex])) : 0).flat());
    }

    private calculateFlightBarTotalSum(metrics: FlowchartDataMetric[]): number {
        let sum = 0;
        if (metrics?.length > 0) {
            metrics.forEach((metric) => {
                if (metric.ValueJson !== undefined) {
                    const value = JSON.parse(metric.ValueJson);
                    sum += value || 0;
                }
            });
        }
        return sum;
    }

    private formatKeyInfo: FormatKeyInfo[] = [];
    currencyCode?: string;
    showSubtotalsAtBottom?: boolean;
    constructor({
        clientId,
        ReferencedTotalsDefinition,
        ReferencedMediaHierarchyDefinition,
        FlatLevelData,
        ReferencedCalendarDefinition,
        CalendarHelper,
        calendarOverlayHeight,
        dataColumns,
        currencyCode,
        showSubtotalsAtBottom,
        inflightOverlayMetrics,
        isDecimal,
        ApplyToCurrencyMetric,
        ApplyToNonCurrencyMetric,
    }: IRightHandTotalsBuilderProps) {
        super();

        this.clientId = clientId;
        this.ReferencedTotalsDefinition = ReferencedTotalsDefinition;
        this.ReferencedMediaHierarchyDefinition =
            ReferencedMediaHierarchyDefinition;
        this.FlatLevelData = FlatLevelData;
        this.ReferencedCalendarDefinition = ReferencedCalendarDefinition;
        this.CalendarHelper = CalendarHelper;
        this.calendarOverlayHeight = calendarOverlayHeight ?? 0;
        this.dataColumns = dataColumns ?? [];
        this.currencyCode = currencyCode;
        this.showSubtotalsAtBottom = showSubtotalsAtBottom;
        this.inflightOverlayMetrics = inflightOverlayMetrics;
        this.isDecimal = isDecimal;
        this.ApplyToCurrencyMetric = ApplyToCurrencyMetric;
        this.ApplyToNonCurrencyMetric = ApplyToNonCurrencyMetric;
    }

    build = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];

        if (
            this.ReferencedTotalsDefinition?.Definition &&
            this.ReferencedMediaHierarchyDefinition?.Definition &&
            this.FlatLevelData &&
            this.ReferencedCalendarDefinition?.Definition &&
            this.CalendarHelper
        ) {
            rows = await this.buildRightHandTotals();
        }

        return rows;
    };
    private createValueBlock(value: string, totalIndex: number): ValueBlock {
        const block = new ValueBlock({ value });
        const styling = this.ReferencedTotalsDefinition.Definition.Columns[totalIndex].Styling;
        block.getDirectStyles = () => StyleMapper.mapStyleToExcelStyle(styling, false);
        return block;
    }

    private buildSubTotalRow(
        subTotal: FlowchartDataSubTotal,
        data: TFlowchartDataLevelFlat,
        totalColumns: any[],
        isValidCurrency: boolean
    ): Block[] {
        const row = new Array<Block>(totalColumns.length);
        const sum = this.calculateSubTotalSum(subTotal);
        const currency = isValidCurrency ? this.currencyCode : data.LocalCurrency;
        const totals = data.Totals || [];

        for (let i = 0; i < totalColumns.length; i++) {
            const currentTotal = totals[i];
            if (currentTotal?.ColumnName === subTotal.ColumnName &&
                currentTotal?.TableId === subTotal.TableId) {
                const sumFormatted = formatNumberFromDataType(
                    sum,
                    subTotal.TableId,
                    subTotal.ColumnName,
                    this.dataColumns,
                    currency,
                    this.isDecimal,
                    this.ApplyToCurrencyMetric,
                    this.ApplyToNonCurrencyMetric,
                );
                row[i] = this.createValueBlock(sumFormatted, i);
            } else {
                row[i] = this.createValueBlock('', i);
            }
        }

        return row;
    }
    private addRowToCollection(
        row: Block[],
        depth: number,
        rows: Block[][],
        stack: [[Block[]]]
    ): void {
        if (this.showSubtotalsAtBottom) {
            if (!stack[depth]) {
                stack[depth] = [[]];
            }
            stack[depth].push(row);
        } else {
            rows.push(row);
        }
    }

    private processSubTotalSummaries(
        data: TFlowchartDataLevelFlat,
        totalColumns: any[],
        rows: Block[][],
        subTotalSummaryStack: [[Block[]]],
        isValidCurrency: boolean
    ): void {
        if (!data.SubTotalSummary?.length) return;

        const settingsObj = data.LevelDefinition?.Settings?.find(s => s.Order === data.Order);
        const processedOrders = new Set<number>();

        for (const subTotal of data.SubTotalSummary) {
            // Skip already processed orders
            if (processedOrders.has(subTotal.Order)) continue;

            const displayNames = settingsObj?.SubTotalSummary?.[subTotal.Order]?.DisplayNames ?? {};
            const hasDisplayNames = Object.keys(displayNames).length > 0;

            if (hasDisplayNames) {
                // Get all subtotals with this order once
                const subTotals = filter(data.SubTotalSummary, { Order: subTotal.Order }) || [];
                processedOrders.add(subTotal.Order);

                for (const [displayKey, _] of Object.entries(displayNames)) {
                    // Find matching subtotal
                    let matchingSubTotal = null;
                    for (const st of subTotals) {
                        if (displayKey.includes(st.ColumnName) && displayKey.includes(st.DisplayName)) {
                            matchingSubTotal = st;
                            break;
                        }
                    }

                    if (matchingSubTotal) {
                        const subTotalRow = this.buildSubTotalRow(
                            matchingSubTotal, data, totalColumns, isValidCurrency
                        );
                        this.addRowToCollection(subTotalRow, data.Depth, rows, subTotalSummaryStack);
                    }
                }
            } else {
                // Process normal subtotal
                const subTotalRow = this.buildSubTotalRow(subTotal, data, totalColumns, isValidCurrency);
                this.addRowToCollection(subTotalRow, data.Depth, rows, subTotalSummaryStack);
            }
        }
    }
    buildRightHandTotals = async (): Promise<Block[][]> => {
        const rows: Block[][] = [];
        let isValidCurrency = this.currencyCode && this.currencyCode != 'LLL';

        // Get total column names
        let totalColumns = [];
        let totalsData = [];
        this.FlatLevelData.forEach((data) => {
            if (!totalColumns.length && data.Totals?.length) {
                totalColumns = data.Totals.map(({ Name }) => Name);
                totalsData = data.Totals;
            }
        });

        // Add main totals headline
        rows.push([
            new MainHeadBlock({
                value: 'Totals',
                rowStart: -2 - this.calendarOverlayHeight,
                columnLength: totalColumns.length,
            }),
        ]);

        // Add total column name headlines
        const totalsRow = [];
        totalColumns.forEach((totalColumn, index) => {
            const headerBlock = new HeadBlock({
                value: totalColumn,
            });

            headerBlock.getDirectStyles = () =>
                StyleMapper.mapStyleToExcelStyle(
                    this.ReferencedTotalsDefinition.Definition.Columns[index]
                        .HeaderStyling,
                    false
                );

            totalsRow.push(headerBlock);
        });
        rows.push(totalsRow);

        // Add blank rows if we have a calendar overlay
        if (this.calendarOverlayHeight > 0) {
            Array.from({ length: this.calendarOverlayHeight }).forEach(() => {
                const calendarOverlayRow = [];

                Array.from({ length: totalColumns.length }).forEach(
                    (x, totalIndex) => {
                        const valueBlock = new ValueBlock({
                            value: '',
                        });
                        valueBlock.getDirectStyles = () =>
                            StyleMapper.mapStyleToExcelStyle(
                                this.ReferencedTotalsDefinition.Definition
                                    .Columns[totalIndex].Styling,
                                false
                            );

                        calendarOverlayRow.push(valueBlock);
                    }
                );

                rows.push(calendarOverlayRow);
            });
        }

        // Add total column values and collect total sums
        let totalSums = [];
        let totalSumsRows = [];
        let localCurrency = this.FlatLevelData.find(c => c.LocalCurrency)?.LocalCurrency;
        const subTotalStack: [[Block[]]] = [[[]]];
        const subTotalSummaryStack: [[Block[]]] = [[[]]];
        let depthCursor = 0;
        this.FlatLevelData.forEach((data) => {

            if (this.showSubtotalsAtBottom) {
                if (subTotalStack.length && data.Depth <= depthCursor) {
                    // subTotalStack.forEach((subTotalBlock: Block) => {
                    //     rows.push([subTotalBlock]);
                    //  })
                    //subTotalStack.length = 0;
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
            // Add an empty value block, if inflight overlays are activated
            if (data?.Metrics?.[0]?.InflightOverlays?.length && data?.Metrics?.[0]?.InflightOverlays.filter((val, index) => data?.Metrics.some(c => c.InflightOverlays && c.InflightOverlays[index] && JSON.parse(c.InflightOverlays[index]) != null))?.length) {
                /*if (data?.Metrics?.[0]?.InflightOverlayValueJson) {*/
                data.Metrics[0]?.InflightOverlays.forEach((inflightOverlay, inflightOverlayIndex) => {
                    const inflightOverlayRow = [];
                    let isDataExists = data.Metrics.some(c => c.InflightOverlays && c.InflightOverlays.length && c.InflightOverlays[inflightOverlayIndex] && c.InflightOverlays[inflightOverlayIndex] != null && JSON.parse(c.InflightOverlays[inflightOverlayIndex]) != null);
                    if (isDataExists) {
                        Array.from({ length: totalColumns.length }).forEach(
                            (x, totalIndex) => {
                                const currentTotal = data.Totals?.[totalIndex] ?? totalsData[totalIndex];
                                const currentInflightOverlay = data.LevelDefinition?.Settings.find(c =>
                                    c.Order == data.Order)?.InflightOverlays?.[inflightOverlayIndex];
                                if (currentTotal?.ColumnName == currentInflightOverlay?.ColumnName && currentTotal?.TableId == currentInflightOverlay?.TableId) {
                                    const isInflightOverlayMetric = data.LevelDefinition?.Settings.find(c =>
                                        c.Order == data.Order)?.InflightOverlays?.[inflightOverlayIndex].ColumnName &&
                                        this.inflightOverlayMetrics.includes(
                                            data.LevelDefinition?.Settings.find(c =>
                                                c.Order == data.Order)?.InflightOverlays?.[inflightOverlayIndex].ColumnName.toLowerCase()
                                        );
                                    const levelInflightOverlays = data.LevelDefinition?.Settings.find(c =>
                                        c.Order == data.Order)?.InflightOverlays;
                                    const inflightOverlayIndexes = levelInflightOverlays && levelInflightOverlays.length ? levelInflightOverlays.map((c, index) => {
                                        return this.inflightOverlayMetrics.includes(c.ColumnName) ? index : -1;
                                    }) : [];
                                    const tempData = { ...data };
                                    const mergedMetrics = isInflightOverlayMetric ? mergeAndSortMetrics(tempData.Metrics, tempData.FlightRange, inflightOverlayIndexes) : {} as TMergedMetrics;
                                    const sum = isInflightOverlayMetric ? this.getInflightOverlaySum(mergedMetrics.map(i => i.InflightOverlays), inflightOverlayIndex) : 0;

                                    const inflightOverlaySelected = data.LevelDefinition?.Settings.find(c =>
                                        c.Order == data.Order)?.InflightOverlays?.[inflightOverlayIndex];
                                    const sumFormatted = isInflightOverlayMetric ? formatNumberFromDataType(
                                        sum,
                                        inflightOverlaySelected.TableId,
                                        inflightOverlaySelected.ColumnName,
                                        this.dataColumns,
                                        isValidCurrency ? this.currencyCode : data.LocalCurrency,
                                        this.isDecimal,
                                        this.ApplyToCurrencyMetric,
                                        this.ApplyToNonCurrencyMetric,
                                    ) : null;
                                    const valueBlock = new ValueBlock({
                                        value: sumFormatted,
                                    });
                                    valueBlock.getDirectStyles = () =>
                                        StyleMapper.mapStyleToExcelStyle(
                                            this.ReferencedTotalsDefinition.Definition
                                                .Columns[totalIndex].Styling,
                                            false
                                        );

                                    inflightOverlayRow.push(valueBlock);
                                } else {
                                    const valueBlock = new ValueBlock({
                                        value: '',
                                    });
                                    valueBlock.getDirectStyles = () =>
                                        StyleMapper.mapStyleToExcelStyle(
                                            this.ReferencedTotalsDefinition.Definition.Columns[totalIndex].Styling,
                                            false
                                        );
                                    inflightOverlayRow.push(valueBlock);
                                }
                            }
                        );
                    }
                    if (inflightOverlayRow.length) {
                        rows.push(inflightOverlayRow);
                    }
                });
            }
            // Add total column values
            const totalRow = [];
            Array.from({ length: totalColumns.length }).forEach(
                (x, totalIndex) => {
                    const total =
                        data?.Totals?.[totalIndex] ??
                        ({ ValuesJson: [] } as FlowchartDataTotal);
                    const { ValuesJson } = total;

                    let sum = 0;
                    if (data?.IsSplitRow) {
                        if (total?.ColumnName == data?.MetricColumnName && total?.TableId == data?.TableId) {
                            sum = this.calculateFlightBarTotalSum(data.Metrics);
                        } else if (data?.IsLastDepth) {
                            ValuesJson.forEach((ValueJson) => {
                                sum += JSON.parse(ValueJson);
                            });
                        }
                    } else {
                        ValuesJson.forEach((ValueJson) => {
                            sum += JSON.parse(ValueJson);
                        });
                    }

                    // Sum only top level data
                    if (data.Depth == 1 || totalSumsRows[totalIndex] == data.Depth) {
                        if (totalSums[totalIndex] === undefined) {
                            totalSums[totalIndex] = 0;
                        }
                        totalSums[totalIndex] += sum;
                    }
                    if (totalSumsRows[totalIndex] === undefined) {
                        totalSumsRows[totalIndex] = 0;
                    }
                    totalSumsRows[totalIndex] = totalSums[totalIndex] == 0 ? data.Depth + 1 : totalSumsRows[totalIndex] != 0 ? data.Depth : totalSumsRows[totalIndex];

                    this.formatKeyInfo[totalIndex] = {
                        Index: totalIndex,
                        TableId: data.TableId,
                        ColumnName: total.ColumnName,
                    };

                    const sumFormatted = formatNumberFromDataType(
                        sum,
                        data.TableId,
                        total.ColumnName,
                        this.dataColumns,
                        isValidCurrency ? this.currencyCode : data.LocalCurrency,
                        this.isDecimal,
                        this.ApplyToCurrencyMetric,
                        this.ApplyToNonCurrencyMetric,
                    );

                    const valueBlock = new ValueBlock({
                        value: sum > 0 ? sumFormatted : '',
                    });
                    valueBlock.getDirectStyles = () =>
                        StyleMapper.mapStyleToExcelStyle(
                            this.ReferencedTotalsDefinition.Definition.Columns[
                                totalIndex
                            ].Styling,
                            false
                        );

                    totalRow.push(valueBlock);
                }
            );
            rows.push(totalRow);

            if (data.SubTotals?.length) {
                data.SubTotals.forEach((subTotal) => {
                    const subTotalsRow = [];
                    Array.from({ length: totalColumns.length }).forEach((x, totalIndex) => {
                        const currentTotal = data.Totals?.[totalIndex];
                        const currentSubTotal = subTotal;
                        if (currentTotal?.ColumnName == currentSubTotal?.ColumnName && currentTotal?.TableId == currentSubTotal?.TableId) {
                            const sum = this.calculateSubTotalSum(subTotal);
                            const sumFormatted = formatNumberFromDataType(
                                sum,
                                subTotal.TableId,
                                subTotal.ColumnName,
                                this.dataColumns,
                                isValidCurrency ? this.currencyCode : data.LocalCurrency,
                                this.isDecimal,
                                this.ApplyToCurrencyMetric,
                                this.ApplyToNonCurrencyMetric,
                            );

                            const valueBlock = new ValueBlock({
                                value: sumFormatted,
                            });
                            valueBlock.getDirectStyles = () =>
                                StyleMapper.mapStyleToExcelStyle(
                                    this.ReferencedTotalsDefinition.Definition
                                        .Columns[totalIndex].Styling,
                                    false
                                );

                            subTotalsRow.push(valueBlock);
                        } else {
                            const valueBlock = new ValueBlock({
                                value: '',
                            });
                            valueBlock.getDirectStyles = () =>
                                StyleMapper.mapStyleToExcelStyle(
                                    this.ReferencedTotalsDefinition.Definition.Columns[totalIndex].Styling,
                                    false
                                );
                            subTotalsRow.push(valueBlock);
                        }
                    }
                    );
                    if (this.showSubtotalsAtBottom) {
                        if (!subTotalStack[data.Depth]) {
                            subTotalStack[data.Depth] = [[]];
                        }
                        subTotalStack[data.Depth].push(subTotalsRow);
                    }
                    else {
                        rows.push(subTotalsRow);
                    }

                });

            }

            if (data.SubTotalSummary?.length) {
                this.processSubTotalSummaries(
                    data, totalColumns, rows, subTotalSummaryStack, isValidCurrency
                );
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
        // Add total sums
        const totalSumsRow = [];
        totalSums.forEach((totalSum, index) => {
            const sumFormatted = formatNumberFromDataType(
                totalSum,
                this.formatKeyInfo[index].TableId,
                this.formatKeyInfo[index].ColumnName,
                this.dataColumns,
                isValidCurrency ? this.currencyCode : localCurrency,
                this.isDecimal,
                this.ApplyToCurrencyMetric,
                this.ApplyToNonCurrencyMetric,
            );

            const totalBlock = new TotalBlock({
                value: sumFormatted,
            });

            totalBlock.getDirectStyles = () =>
                StyleMapper.mapStyleToExcelStyle(
                    this.ReferencedTotalsDefinition.Definition.Columns[index]
                        .SumStyling,
                    false
                );

            totalSumsRow.push(totalBlock);
        });
        rows.push(totalSumsRow);

        this.rightHandTotalsColumnLength = this.getColumnLength(rows);
        this.rightHandTotalsRowLength = this.getRowLength(rows);

        return rows;
    };
}