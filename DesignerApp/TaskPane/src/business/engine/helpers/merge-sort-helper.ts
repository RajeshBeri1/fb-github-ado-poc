import * as API from '@omniflow/omni-webapi';
import { findIndex, sortBy, sum } from 'lodash';
import moment from 'moment';
import { hasPeriodOverlap } from './period-helper';

export type TMergedMetrics = {
    EffectiveDate?: Date | null;
    FlightStart?: Date | null;
    FlightEnd?: Date | null;
    InflightOverlayValueJson?: string | null;
    ValueJson?: number | null;
    InflightOverlays?: string[] | [] | null;
    Legend?: string | null;
}[];

export const mergeAndSortMetrics = (
    metrics: API.FlowchartDataMetric[] = [],
    metricFlightRange: API.FlightRange,
    metricInflighOverlayIndexes: number[]
): TMergedMetrics => {
    const needEffective =
        metricFlightRange !== API.FlightRange.FlightTotal &&
        metricFlightRange !== API.FlightRange.None;

    const mergedMetrics1: TMergedMetrics = [];
    metrics?.forEach((metric) => {
        const { ValueJson, InflightOverlayValueJson, InflightOverlays, Legend } = metric;

        const EffectiveDate = moment.utc(metric?.EffectiveDate).toDate();
        const FlightStart = moment.utc(metric?.FlightStart).toDate();
        const FlightEnd = moment.utc(metric?.FlightEnd).toDate();

        const metricFilter = needEffective
            ? { EffectiveDate }
            : {
                FlightStart,
                FlightEnd,
            };

        const metricIndex1 = findIndex(mergedMetrics1, metricFilter);

        if (metricIndex1 === -1) {
            mergedMetrics1.push({
                EffectiveDate: needEffective ? EffectiveDate : null,
                FlightStart: !needEffective ? FlightStart : null,
                FlightEnd: !needEffective ? FlightEnd : null,
                ValueJson: ValueJson !== undefined ? JSON.parse(ValueJson) : 0,
                InflightOverlayValueJson: InflightOverlayValueJson !== undefined
                    ? JSON.parse(InflightOverlayValueJson)
                    : null,
                InflightOverlays: InflightOverlays !== undefined
                    ? [...InflightOverlays]
                    : [],
                Legend: Legend !== undefined
                    ? JSON.parse(Legend)
                    : null,
            });
        } else {
            const updatedMetric = { ...mergedMetrics1[metricIndex1] };
            updatedMetric.ValueJson += ValueJson !== undefined ? JSON.parse(ValueJson) : 0;
            if (InflightOverlays && InflightOverlays.length > 0) {
                InflightOverlays.forEach((c, index) => {
                    if (metricInflighOverlayIndexes.includes(index)) {
                        if (c && JSON.parse(c) != null) {
                            updatedMetric.InflightOverlays[index] = (parseFloat(JSON.parse(c)) + parseFloat(updatedMetric.InflightOverlays[index] && updatedMetric.InflightOverlays[index] != null && JSON.parse(updatedMetric.InflightOverlays[index]) != null ? JSON.parse(updatedMetric.InflightOverlays[index]) : '0')).toString();
                        }
                    }
                });
            }
            mergedMetrics1[metricIndex1] = updatedMetric;
        }
    });

    let mergedMetrics2: TMergedMetrics = [];
    if (!needEffective) {
        mergedMetrics1?.forEach((metric) => {
            const { FlightStart, FlightEnd, ValueJson, InflightOverlays } = metric;

            const metricIndex2 = findIndex(mergedMetrics2, (m) =>
                hasPeriodOverlap(
                    {
                        startDate: m.FlightStart,
                        endDate: m.FlightEnd,
                    },
                    { startDate: FlightStart, endDate: FlightEnd }
                )
            );

            if (metricIndex2 === -1) {
                mergedMetrics2.push({ ...metric });
            } else {
                const updatedMetric = { ...mergedMetrics2[metricIndex2] };
                if (updatedMetric.FlightStart > FlightStart) {
                    updatedMetric.FlightStart = FlightStart;
                }
                if (updatedMetric.FlightEnd < FlightEnd) {
                    updatedMetric.FlightEnd = FlightEnd;
                }

                updatedMetric.ValueJson += ValueJson;
                if (InflightOverlays && InflightOverlays.length > 0) {
                    InflightOverlays.forEach((c, index) => {
                        if (metricInflighOverlayIndexes.includes(index)) {
                            if (c && JSON.parse(c) != null) {
                                updatedMetric.InflightOverlays[index] = (parseFloat(JSON.parse(c)) + parseFloat(updatedMetric.InflightOverlays[index] && updatedMetric.InflightOverlays[index] != null && JSON.parse(updatedMetric.InflightOverlays[index]) != null ? JSON.parse(updatedMetric.InflightOverlays[index]) : '0')).toString();
                            }
                        }
                    });
                }
                mergedMetrics2[metricIndex2] = updatedMetric;
            }
        });
    } else {
        mergedMetrics2 = mergedMetrics1;
    }

    const mergedSortedMetrics = sortBy(
        mergedMetrics2,
        needEffective ? ['EffectiveDate'] : ['FlightStart']
    );

    return mergedSortedMetrics;
};