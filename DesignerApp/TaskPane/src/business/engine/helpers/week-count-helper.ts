import moment from "moment";

function getFirstWeekStartOfMonth(year: number, month: number, startOfWeek: number): number {
    const firstDay = moment.utc({ year, month, day: 1 });
    const dayOfWeek = firstDay.day(); // 0=Sunday, 1=Monday, ...
    return dayOfWeek === startOfWeek ? 1 : ((7 - dayOfWeek + startOfWeek) % 7) + 1;
}
export function getPreviousWeekStart(date: Date, startOfWeek: number): Date {
    const m = moment.utc(date).startOf('day');
    const diff = (m.day() - startOfWeek + 7) % 7;
    return m.subtract(diff === 0 ? 7 : diff, 'days').toDate();
}

export function getLastWeekdayOfMonth(year: number, month: number, dayOfWeek: number): Date {
    const lastDay = moment.utc({ year, month }).endOf('month');
    const diff = (lastDay.day() - dayOfWeek + 7) % 7;
    return lastDay.subtract(diff, 'days').toDate();
}

export function getColumnLengthWithRules(
    startDate: Date,
    endDate: Date, 
    isCalendarEndDate: boolean,
    startOfWeek: number // 0 = Sunday, 1 = Monday
): number {
    let counter = 0;
    const start = moment.utc(startDate).startOf('day');
    const end = moment.utc(endDate).startOf('day');

    // --- Start date logic ---
    if (start.day() !== startOfWeek) {
        const firstWeekStartDate = getFirstWeekStartOfMonth(start.year(), start.month(), startOfWeek);
        const prevMonthEnd = start.clone().subtract(1, 'month').endOf('month');
        const prevMonthEndIsWeekStart = prevMonthEnd.day() === startOfWeek;

        if (start.date() < firstWeekStartDate) {
            if (!prevMonthEndIsWeekStart) {
                counter += 2;
            } else {
                counter += 1;
            }
        } else {
            counter += 1;
        }
    }

    // 1. Count week starts (Sunday or Monday) between startDate and endDate (inclusive)
    let current = start.clone();
    while (current.isBefore(end)) {
        if (current.day() === startOfWeek) {
            counter++;
        }
        current.add(1, 'day');
    }

    // 2. For every month in the range, if the last day is not a week start, increase counter by 1
    let monthCursor = start.clone().startOf('month');
    const lastMonth = end.clone().startOf('month');
    while (monthCursor.isBefore(lastMonth)) {
        const monthEnd = monthCursor.clone().endOf('month');
        if (monthEnd.day() !== startOfWeek) {
            counter++;
        }
        monthCursor.add(1, 'month');
    }

    // 3. If endDate is a week start, increase counter by 1
    if (end.day() === startOfWeek) {
        counter++;

        // Also, check if between next two week starts is the month end date, then increase counter again by 1
        const nextWeekStart = end.clone().add((7 - (end.day() - startOfWeek + 7) % 7) % 7, 'days');
        const prevWeekStart = end.clone().subtract((end.day() - startOfWeek + 7) % 7, 'days');
        const monthEnd = end.clone().endOf('month');
        if (monthEnd.isAfter(prevWeekStart) && monthEnd.isSameOrBefore(nextWeekStart) && !isCalendarEndDate) {
            counter++;
        }
    }

    // 4. If endDate is the last day of its month and is not a week start, increase counter by 1
    const monthEnd = end.clone().endOf('month');
    if (end.isSame(monthEnd, 'day') && end.day() !== startOfWeek && !isCalendarEndDate) {
        counter++;
    }
    const lastWeekEnd = getLastWeekdayOfMonth(end.year(), end.month(), startOfWeek);

    if (monthEnd.day() !== startOfWeek && end.isBefore(monthEnd, 'day') && end.date() > moment.utc(lastWeekEnd).date()) {
        counter++;
    }

    return counter;
}

/**
 * Custom column counter based on specific Sunday and month-end rules.
 * For every month in the range, if the last day of the month is not a Sunday, increase counter by 1.
 */
export const getCustomColumnCount = (
    startDate: Date,
    endDate: Date,
    startOfWeek: number = 0 // 0=Sunday, 1=Monday, etc.
): number => {
    let counter = 0;
    const start = moment.utc(startDate).startOf('day');
    const end = moment.utc(endDate).startOf('day');

    // --- Handle start date logic ---
    const startMonthFirstWeekStartDate = getFirstWeekStartOfMonth(start.year(), start.month(), startOfWeek);
    const prevMonthEnd = start.clone().subtract(1, 'month').endOf('month');
    const prevMonthEndIsWeekStart = prevMonthEnd.day() === startOfWeek;

    if (start.day() !== startOfWeek && start.date() < startMonthFirstWeekStartDate) {
        if (!prevMonthEndIsWeekStart) {
            counter += 2;
        } else {
            counter += 1;
        }
    } else if (start.day() !== startOfWeek && start.date() > startMonthFirstWeekStartDate) {
        counter++;
    }

    // --- Count week starts between startDate and endDate (inclusive) ---
    let current = start.clone();
    while (current.isSameOrBefore(end)) {
        if (current.day() === startOfWeek) {
            counter++;
        }
        current.add(1, 'day');
    }

    // --- For every month in the range, if the last day is not a week start, increase counter by 1 ---
    let monthCursor = start.clone().startOf('month');
    const lastMonth = end.clone().startOf('month');
    while (monthCursor.isBefore(lastMonth)) {
        const monthEnd = monthCursor.clone().endOf('month');
        if (monthEnd.day() !== startOfWeek) {
            counter++;
        }
        monthCursor.add(1, 'month');
    }

    // --- Handle endDate less than first week start of its month ---
    const endMonthFirstWeekStart = getFirstWeekStartOfMonth(end.year(), end.month(), startOfWeek);
    const prevEndMonthEnd = end.clone().subtract(1, 'month').endOf('month');
    const prevEndMonthEndIsWeekStart = prevEndMonthEnd.day() === startOfWeek;

    if (end.date() < endMonthFirstWeekStart) {
        if (!prevEndMonthEndIsWeekStart) {
            counter -= 2;
        } else {
            counter -= 1;
        }
    } else{
        counter -= 1;
    }
    return counter;
};




export const getSundaysAndEndDateCounter = (year: number, month: number): number  => {
    let counter: number = 0 
    const sundays: number[] = [];
    const start = moment.utc({ year, month, day: 1 });
    const end = start.clone().endOf('month');
    let current = start.clone().day(0); // 0 = Sunday

    // If the first Sunday is before the start of the month, move to the next Sunday
    if (current.isBefore(start)) {
        current.add(7, 'days');
    }

    while (current.isSameOrBefore(end)) {
        sundays.push(current.date());
        current.add(7, 'days');
    }
    if (sundays[sundays.length - 1] !== end.date()) {
        counter++;
    }
    return counter;
};

const getColumnsLength = (startDate: Date, endDate: Date): { extraColumns: number, endDate: Date, isNextDateConsidered: boolean } => {
    let extraColumnLength = 0;
    if (moment.utc(startDate).date() == 1 && moment.utc(startDate).month() == 0) {
        extraColumnLength = 4;
    }
    let previousStartDate = moment.utc(startDate).toDate();
    let actualStartDate = moment.utc(startDate).toDate();
    let isNextDateConsidered = false;
    let currentYear = moment.utc(previousStartDate).year();
    let currentMonth = moment.utc(previousStartDate).month();
    let prevMonth = moment.utc(previousStartDate).month();
    while (moment.utc(actualStartDate).isBefore(moment.utc(endDate))) {

        if (moment.utc(previousStartDate).daysInMonth() != moment.utc(previousStartDate).date() && moment.utc(actualStartDate).date() < moment.utc(previousStartDate).date()) {
            extraColumnLength++;
        }
        previousStartDate = actualStartDate;
        actualStartDate = moment.utc(actualStartDate).add(7, "day").toDate();
        prevMonth = currentMonth;
        currentMonth = moment.utc(actualStartDate).month();
    }

    if (moment.utc(previousStartDate).daysInMonth() != moment.utc(previousStartDate).date() && moment.utc(actualStartDate).date() < moment.utc(previousStartDate).date()) {
        extraColumnLength++;
        isNextDateConsidered = true;
    }
    return { extraColumns: extraColumnLength, endDate: actualStartDate, isNextDateConsidered: isNextDateConsidered };
}

export const getExtraWeekForSpace = (startDate: Date, endDate: Date): number => {
    return getColumnsLength(startDate, endDate).extraColumns;
}
export const getExtraWeekNextDateConsidered = (startDate: Date, endDate: Date): { isNextDateConsidered: boolean, endDate: Date } => {
    const data = getColumnsLength(startDate, endDate);
    return { isNextDateConsidered: data.isNextDateConsidered, endDate: data.endDate };
}

