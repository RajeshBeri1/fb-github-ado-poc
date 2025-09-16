import * as API from '@omniflow/omni-webapi';
import moment from 'moment';

export type Period = {
    startDate: Date;
    endDate: Date;
};

export enum MomentPeriodUnit {
    'WEEK' = 'week',
    'MONTH' = 'month',
    'QUARTER' = 'quarter',
    'YEAR' = 'year',
}

export enum BcPeriodUnit {
    'BC_WEEK' = 'bc_week',
    'BC_MONTH' = 'bc_month',
    'BC_QUARTER' = 'bc_quarter',
    'BC_YEAR' = 'bc_year',
}

export const PeriodUnit = {
    ...MomentPeriodUnit,
    ...BcPeriodUnit,
};
export type PeriodUnit = MomentPeriodUnit | BcPeriodUnit;

export const normalizeFlightRangePeriod = (
    flightRangePeriod: API.FlightRange
): PeriodUnit => {
    let normalizedPeriod;

    switch (flightRangePeriod) {
        case API.FlightRange.WeeklyBroadcast:
            normalizedPeriod = PeriodUnit.BC_WEEK;
            break;
        case API.FlightRange.Monthly:
            normalizedPeriod = PeriodUnit.MONTH;
            break;
        case API.FlightRange.MonthlyBroadcast:
            normalizedPeriod = PeriodUnit.BC_MONTH;
            break;
        case API.FlightRange.Quarterly:
            normalizedPeriod = PeriodUnit.QUARTER;
            break;
        case API.FlightRange.QuarterlyBroadcast:
            normalizedPeriod = PeriodUnit.BC_QUARTER;
            break;
        default:
            normalizedPeriod = PeriodUnit.WEEK;
    }

    return normalizedPeriod;
};

export const getPeriod = (
    date: Date = moment.utc().toDate(),
    periodUnit: PeriodUnit,
    index?: number
): Period | null => {
    let finalPeriod = null;
    if (Object.values(MomentPeriodUnit).includes(periodUnit as any)) {
        const finalPeriodUnit = periodUnit as any;
        const startDate = moment.utc(date).startOf(finalPeriodUnit).toDate();
        const endDate = moment.utc(date).endOf(finalPeriodUnit).toDate();
        finalPeriod = { startDate, endDate };
    } else if (periodUnit === PeriodUnit.BC_WEEK) {
        finalPeriod = getBroadcastWeek(date);
    } else if (periodUnit === PeriodUnit.BC_MONTH) {
        finalPeriod = getBroadcastMonth(date, index);
    } else if (periodUnit === PeriodUnit.BC_QUARTER) {
        finalPeriod = getBroadcastQuarter(date);
    } else if (periodUnit === PeriodUnit.BC_YEAR) {
        finalPeriod = getBroadcastYear(date);
    }

    return finalPeriod;
};

export const getPeriods = (
    startDate: Date = moment.utc().toDate(),
    endDate: Date = moment.utc().toDate(),
    periodUnit: PeriodUnit
): Period[] => {
    const periods = [];

    // Get number of periods
    const momentStartDate = moment.utc(
        startDate > endDate ? endDate : startDate
    );
    const momentEndDate = moment.utc(endDate < startDate ? startDate : endDate);
    const numberOfPeriods =
        momentEndDate.diff(
            momentStartDate,
            `${periodUnit.replace(/^bc_/, '')}s` as MomentPeriodUnit
        ) + 1;

    // Get periods
    let nextStartDate = startDate;
    Array.from({ length: numberOfPeriods }).forEach(() => {
        periods.push(getPeriod(nextStartDate, periodUnit));
        nextStartDate = moment
            .utc(periods[periods.length - 1].endDate)
            .add('1', 'day')
            .toDate();
    });

    return periods;
};

export const hasPeriodOverlap = (period1: Period, period2: Period): boolean => {
    const { startDate: startDate1, endDate: endDate1 } = period1;
    const { startDate: startDate2, endDate: endDate2 } = period2;

    if (startDate1 <= startDate2 && startDate2 <= endDate1) return true;
    if (startDate1 <= endDate2 && endDate2 <= endDate1) return true;

    return startDate2 < startDate1 && endDate1 < endDate2;
};

const getBroadcastWeek = (date: Date = moment.utc().toDate()): Period => {
    const momentDate = moment.utc(date);
    const day = momentDate.day();

    // Find the closest Monday <= the date
    const startDate = moment
        .utc(date)
        .subtract((day + 6) % 7, 'day')
        .startOf('day')
        .toDate();

    // Find the closest Sunday >= the date
    const endDate = moment
        .utc(date)
        .add((7 - day) % 7, 'day')
        .endOf('day')
        .toDate();
    return { startDate, endDate };
};

const getBroadcastMonth = (date: Date = moment.utc().toDate(), index?: number): Period => {
    
    let { startDate } = getBroadcastWeek(
        moment.utc(date).startOf('month').toDate()
    );

    // The broadcast end date in a month is always the last Sunday of the month
    let momentEndDate = moment.utc(date).endOf('month');
    let endDate = momentEndDate.subtract(momentEndDate.day(), 'day').toDate();

    // If original date is greater than calculated broadcast month end date, then take next broadcast month
    if (date > endDate) {
        ({ startDate, endDate } = getBroadcastWeek(
            moment.utc(endDate).add('1', 'day').toDate()
        ));

        momentEndDate = moment.utc(endDate).endOf('month');
        endDate = momentEndDate.subtract(momentEndDate.day(), 'day').toDate();
    }
    return { startDate, endDate };
};

const getBroadcastQuarter = (date: Date = moment.utc().toDate()): Period => {
    // Since a transmitted first of the month always contains the Gregorian first of the
    // month, each first day in a broadcast quarter contains the first day in the Gregorian
    // quarter
    const { endDate: weekEndDate } = getBroadcastWeek(date);
    const { startDate } = getBroadcastWeek(
        moment.utc(weekEndDate).startOf('quarter').toDate()
    );
    const { endDate } = getBroadcastMonth(
        moment.utc(weekEndDate).add(2, 'months').toDate()
    );

    return { startDate, endDate };
};

const getBroadcastYear = (date: Date = moment.utc().toDate()): Period => {
    let { startDate } = getBroadcastWeek(
        moment.utc(date).startOf('year').toDate()
    );
    let { endDate } = getBroadcastMonth(
        moment.utc(date).month(11).startOf('month').toDate()
    );

    // If original date is greater than calculated broadcast year-end date, then take next broadcast year
    if (date > endDate) {
        const { startDate: newStartDate, endDate: newEndDate } =
            getBroadcastWeek(moment.utc(endDate).add('1', 'day').toDate());
        startDate = newStartDate;

        ({ endDate } = getBroadcastMonth(
            moment.utc(newEndDate).month(11).startOf('month').toDate()
        ));
    }

    return { startDate, endDate };
};
