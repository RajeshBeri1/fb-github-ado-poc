import {
    FlowchartDataMetric
} from '@omniflow/omni-webapi';
import { groupBy } from 'lodash';
import { TFlowchartDataLevelFlat } from './level-helper';
import * as DateFns from 'date-fns';
import moment from 'moment';

export type Period = {
    startDate: Date;
    endDate: Date;
    actualStartDate?: Date;
    actualEndDate?: Date;
};

export const hasPeriodOverlap = (period1: Period, period2: Period): boolean => {
    const { startDate: startDate1, endDate: endDate1 } = period1;
    const { startDate: startDate2, endDate: endDate2 } = period2;

    if (startDate1 <= startDate2 && startDate2 <= endDate1) return true;
    if (startDate1 <= endDate2 && endDate2 <= endDate1) return true;

    return startDate2 < startDate1 && endDate1 < endDate2;
};
export const mergeAndRemoveConflicts = (arrays: FlowchartDataMetric[][]): FlowchartDataMetric[][] => {
    arrays.forEach((parentArray, parentIndex) => {
        arrays.forEach((arr, index) => {
            if (index > parentIndex) {
                arrays[index] = arr.filter(row => {
                    const conflict = parentArray.some(mergedRow =>
                        //!(DateFns.areIntervalsOverlapping(
                        //    {
                        //        start: moment.utc(mergedRow.FlightStart).toDate(),
                        //        end: moment.utc(mergedRow.FlightEnd).toDate(),
                        //    },
                        //    {
                        //        start: moment.utc(row.FlightStart).toDate(),
                        //        end: moment.utc(row.FlightEnd).toDate(),
                        //    }
                        //) &&

                        hasPeriodOverlap({ startDate: new Date(mergedRow.FlightStart), endDate: new Date(mergedRow.FlightEnd) }, { startDate: new Date(row.FlightStart), endDate: new Date(row.FlightEnd) })
                        &&
                        //(new Date(mergedRow.FlightStart) <= new Date(row.FlightEnd)) &&
                        //(new Date(mergedRow.FlightEnd) >= new Date(row.FlightStart)) &&
                        ((mergedRow.Legend == undefined || row.Legend == undefined) || (JSON.parse(mergedRow.Legend) != JSON.parse(row.Legend)))
                    );
                    if (!conflict) {
                        arrays[parentIndex].push(row);
                    }
                    return conflict;

                })
            }
        });

    });
    return arrays.filter(c => c.length > 0).map(c => c.sort((a, b) => new Date(a.FlightStart) < new Date(b.FlightStart) ? -1 : new Date(a.FlightStart) == new Date(b.FlightStart) ? 0 : 1));
}

const isTotalsExists = (totals: { ColumnName: string, TableId: string }[] | null, data: TFlowchartDataLevelFlat): boolean => {

    if (totals?.length) {
        return totals.findIndex(c => c.ColumnName == data.MetricColumnName && c.TableId == data.MetricTableId) > -1;
    } else {
        return false;
    }
}

export const splitFlightBars = (flatLevelData: TFlowchartDataLevelFlat[]): TFlowchartDataLevelFlat[] => {
    let flatLevelNonConflictData: TFlowchartDataLevelFlat[] = [];
    const maxDepth = Math.max(...flatLevelData.map(c => c.Depth));
    const totals = flatLevelData
        .flatMap(c => c.Totals || [])  // Use empty array if Totals is null/undefined
        ?.map(c => { return { ColumnName: c.ColumnName, TableId: c.TableId }; });
    flatLevelData.forEach(row => {
        if (row.Depth >= maxDepth) {
            //let sortBy = row.Metrics?.sort((a, b) => a.FlightStart.valueOf() < b.FlightStart.valueOf() ? 0 : 1);
            const groupedMetrics = groupBy(row.Metrics, "Legend");
            const mergedMetrics = mergeAndRemoveConflicts(Object.values(groupedMetrics));
            const maxLength = mergedMetrics.length;
            mergedMetrics.forEach((metrics, index) => {
                const isSplitRow = isTotalsExists(totals, row);
                if (maxLength == index + 1) {
                    flatLevelNonConflictData.push({ ...row, Metrics: metrics, IsSplitRow: isSplitRow, IsLastDepth:true });
                } else {
                    flatLevelNonConflictData.push({ ...row, Metrics: metrics, SubTotals: [], SubTotalSummary: [], IsSplitRow: isSplitRow, Totals: isSplitRow ? [...row.Totals] : [] });
                }
            });
        } else {
            flatLevelNonConflictData.push(row);
        }
    });
    return flatLevelNonConflictData;
}
