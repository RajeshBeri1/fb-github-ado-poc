import * as API from '@omniflow/omni-webapi';
import { CalendarType } from '@omniflow/omni-webapi';
import moment from 'moment';
import { differenceInDays } from 'date-fns';
import { uniqBy } from 'lodash';
import differenceInCalendarDays from 'date-fns/esm/fp/differenceInCalendarDays/index';
export type Period = {
    startDate: Date;
    endDate: Date;
    actualStartDate?: Date;
    actualEndDate?: Date;
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

export const normalizePeriodToBroadcast = (
    periodUnit: PeriodUnit
): BcPeriodUnit => {
    let normalizedPeriod;

    switch (periodUnit) {
        case MomentPeriodUnit.WEEK:
            normalizedPeriod = PeriodUnit.BC_WEEK;
            break;
        case MomentPeriodUnit.MONTH:
            normalizedPeriod = PeriodUnit.BC_MONTH;
            break;
        case MomentPeriodUnit.QUARTER:
            normalizedPeriod = PeriodUnit.BC_QUARTER;
            break;
        case MomentPeriodUnit.YEAR:
            normalizedPeriod = PeriodUnit.BC_YEAR;
            break;
        default:
            normalizedPeriod = PeriodUnit.WEEK;
    }

    return normalizedPeriod;
};

export const normalizePeriodToStandard = (
    periodUnit: PeriodUnit
): MomentPeriodUnit => {
    let normalizedPeriod;

    switch (periodUnit) {
        case BcPeriodUnit.BC_WEEK:
            normalizedPeriod = PeriodUnit.WEEK;
            break;
        case BcPeriodUnit.BC_MONTH:
            normalizedPeriod = PeriodUnit.MONTH;
            break;
        case BcPeriodUnit.BC_QUARTER:
            normalizedPeriod = PeriodUnit.QUARTER;
            break;
        case BcPeriodUnit.BC_YEAR:
            normalizedPeriod = PeriodUnit.YEAR;
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
        finalPeriod = getBroadcastQuarter(date, index);
    } else if (periodUnit === PeriodUnit.BC_YEAR) {
        finalPeriod = getBroadcastYear(date);
    }

    return finalPeriod;
};

export const getPeriods = (
    startDate: Date = moment.utc().toDate(),
    endDate: Date = moment.utc().toDate(),
    periodUnit: PeriodUnit,
    calendarType: CalendarType,
    isNormalCalendarViewEnabledAndDifferentFlight?: boolean
): Period[] => {
    const periods = [] as Period[];
    const actualPeriods = [] as Period[];
    const isDifferentCalendarType = (Object.values(MomentPeriodUnit).includes(periodUnit as any) && calendarType == CalendarType.Broadcast) || (Object.values(BcPeriodUnit).includes(periodUnit as any) && (calendarType == CalendarType.Standard || calendarType == CalendarType.Client));
    // Get number of periods
    const momentStartDate = moment.utc(
        startDate > endDate ? endDate : startDate
    );
    const momentEndDate = moment.utc(endDate < startDate ? startDate : endDate);
    const numberOfPeriods = (`${periodUnit.replace(/^bc_/, '')}` as MomentPeriodUnit) == MomentPeriodUnit.YEAR ? (endDate.getFullYear() - startDate.getFullYear()) + 1 :
        momentEndDate.diff(
            momentStartDate,
            `${periodUnit.replace(/^bc_/, '')}s` as MomentPeriodUnit
        ) + 1;
  
    // Get periods
    let nextStartDate = startDate;
    Array.from({ length: numberOfPeriods }).forEach((item, index) => {
        periods.push(getPeriod(nextStartDate, periodUnit, index));
        nextStartDate = moment
            .utc(periods[periods.length - 1].endDate)
            .add('1', 'day')
            .toDate();
    });

    //Get calendartype dates to plot values
    if (isDifferentCalendarType) {
        let nextStartDate = moment.utc(startDate).toDate();
        let convertPeriodUnitType = (Object.values(MomentPeriodUnit).includes(periodUnit as any) && calendarType == CalendarType.Broadcast) ? normalizePeriodToBroadcast(periodUnit) : normalizePeriodToStandard(periodUnit);
        Array.from({ length: numberOfPeriods }).forEach((item, index) => {
            actualPeriods.push(getPeriod(nextStartDate, convertPeriodUnitType, index));
            nextStartDate = moment
                .utc(actualPeriods[actualPeriods.length - 1].endDate)
                .add('1', 'day')
                .toDate();
        });

        actualPeriods.forEach((period, index) => {
            const findIndex = periods.findIndex(differentPeriod => {
                return moment.utc(differentPeriod.endDate).add(-Math.ceil(differenceInCalendarDays(differentPeriod.endDate, differentPeriod.startDate) / 2), "days").isBetween(moment.utc(actualPeriods[index].startDate), moment.utc(actualPeriods[index].endDate));
            });

            period.actualEndDate = moment.utc(period.endDate).toDate();
            period.actualStartDate = moment.utc(period.startDate).toDate();

            if (findIndex != -1) {
                period.endDate = moment.utc(periods[findIndex].endDate).toDate();
                period.startDate = moment.utc(periods[findIndex].startDate).toDate();
            }

        });
        if (isNormalCalendarViewEnabledAndDifferentFlight && periods?.length) {
            actualPeriods[periods.length - 1].endDate = moment.utc(endDate).toDate();
        }
        return uniqBy(actualPeriods, "startDate");
    } else {

        if (isNormalCalendarViewEnabledAndDifferentFlight && periods?.length) {
            periods[periods.length - 1].endDate = moment.utc(endDate).toDate();
        }
        return uniqBy(periods, "startDate");
    }


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
    //console.debug(`the day is ${day} and date is ${momentDate}`);
    // Find the closest Monday <= the date
    const startDate = moment
        .utc(date)
        .subtract((day + 6) % 7, 'day')
        .startOf('day')
        .toDate();

    // Find the closest Sunday >= the date
    const endDate = moment(startDate)
        .add(6, 'day')
        .endOf('day')
        .toDate();
    //console.debug(`${date} translated to broadcast week ${startDate} - ${endDate}`);
    return { startDate, endDate };
};

const getBroadcastMonth = (date: Date = moment.utc().toDate(), index?: number): Period => {
    // Since the beginning of the Gregorian month is always included in the beginning
    // of the month, we can find it using the getBroadcastWeek method

    let { startDate } = getBroadcastWeek(
        moment.utc(date).startOf('month').toDate()
    );

    // The broadcast end date in a month is always the last Sunday of the month
    let momentEndDate = moment.utc(date).endOf('month');
    let endDate = momentEndDate.subtract(momentEndDate.day(), 'day').toDate();

    // If original date is greater than calculated broadcast month end date, then take next broadcast month
    if (moment.utc(date).toDate() > endDate) {
        ({ startDate, endDate } = getBroadcastWeek(
            moment.utc(endDate).add('1', 'day').toDate()
        ));

        momentEndDate = moment.utc(endDate).endOf('month');
        endDate = momentEndDate.subtract(momentEndDate.day(), 'day').toDate();
    }
    return { startDate, endDate };

};

const getBroadcastQuarter = (date: Date = moment.utc().toDate(), index?: number): Period => {
    // Since a transmitted first of the month always contains the Gregorian first of the
    // month, each first day in a broadcast quarter contains the first day in the Gregorian
    // quarter
    const { endDate: weekEndDate } = getBroadcastWeek(date);
    const { startDate } = getBroadcastWeek(
        moment.utc(weekEndDate).startOf('quarter').toDate()
    );
    const { endDate } = getBroadcastWeek(
        moment.utc(weekEndDate).endOf('quarter').toDate()
    );

    const finalEndDate = moment(endDate).isAfter(moment.utc(weekEndDate).endOf('quarter').toDate()) ? moment(endDate).add(-7, "days").toDate() : endDate; //getBroadcastMonth(moment(weekEndDate).add(2, "month").toDate(), index);// moment(endDate).add(-7, "days").toDate();
    return { startDate, endDate: finalEndDate };

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
