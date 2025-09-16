import { orderBy, sumBy } from 'lodash';
import { differenceInCalendarDays, differenceInDays, differenceInMonths } from 'date-fns';
import { Block } from './block';
import * as API from '@omniflow/omni-webapi';
import moment from 'moment';
import { getExtraWeekForSpace, getColumnLengthWithRules, getCustomColumnCount, getPreviousWeekStart, getPreviousSunday } from '../helpers/week-count-helper';

export type Period = {
    startDate: Date;
    endDate: Date;
};

export class BuildEngine {
    constructor() { }

    build = async (): Promise<Block[][]> => {
        throw new Error(
            `There is no build method defined for the ${this.constructor.name} class!`
        );
    };

    buildPeriodicBlocks = ({
        startDate,
        endDate,
        columnStart = 0,
        columnLength,
        periodicData,
        calendarType,
        calendarStartDate,
        isFilterRequired = false,
        isFlightRangeSelected,
        isLastLevel = false,
        isNormalCalendarEnabled = false,
        flightRange = API.FlightRange.None,
        extraWeeks = 0,
        normalizedStartDate = null,
        referencedCalendarDefinition,
        blockCb,
    }: {
        startDate: Date;
        endDate: Date;
        columnStart?: number;
        columnLength: number;
        periodicData: {
            startDate: Date;
            endDate: Date;
            data: any;
        }[];
        calendarType: API.CalendarType;
        calendarStartDate: Date;
        isFilterRequired: boolean;
        isFlightRangeSelected: boolean;
        isLastLevel: boolean;
        isNormalCalendarEnabled: boolean;
        flightRange: API.FlightRange;
        extraWeeks: number;
        normalizedStartDate: Date;
        referencedCalendarDefinition: API.ReferencedCalendarDefinition,
        blockCb: ({
            startDate,
            endDate,
            columnStart,
            columnLength,
            data,
        }: {
            startDate: Date;
            endDate: Date;
            columnStart: number;
            columnLength: number;
            data: any;
        }) => Block | null;
    }): Block[] => {
        if (isFlightRangeSelected) {

            // utilities to calculate the column length and start date
            const isBroadcastCalendar = referencedCalendarDefinition?.Definition?.Configuration?.Type == API.CalendarType.Broadcast;
            const flightRangeDifferentFromCalendar = isNormalCalendarEnabled && isBroadcastCalendar
                && (flightRange == API.FlightRange.Weekly
                    || flightRange == API.FlightRange.Monthly
                    || flightRange == API.FlightRange.Quarterly
                    || flightRange == API.FlightRange.Annually);
            const isStandardAndNormalized = referencedCalendarDefinition?.Definition?.Configuration?.Type == API.CalendarType.Standard && referencedCalendarDefinition?.Definition?.Configuration?.IsNormalizedEnabled;
            const isStandardAndUseCustomStartDay = referencedCalendarDefinition?.Definition?.Configuration?.Type == API.CalendarType.Standard && referencedCalendarDefinition?.Definition?.Configuration?.UseCustomStartDay;

            // flight range without broadcast
            const isMonthly = flightRange == API.FlightRange.Monthly || flightRange == API.FlightRange.MonthlyBroadcast;
            const isQuarterly = flightRange == API.FlightRange.Quarterly || flightRange == API.FlightRange.QuarterlyBroadcast;
            const isWeekly = flightRange == API.FlightRange.Weekly || flightRange == API.FlightRange.WeeklyBroadcast;
            const isAnnually = flightRange == API.FlightRange.Annually || flightRange == API.FlightRange.AnnuallyBroadcast;
            const isBroadcast = flightRange == API.FlightRange.MonthlyBroadcast || flightRange == API.FlightRange.WeeklyBroadcast || flightRange == API.FlightRange.QuarterlyBroadcast || flightRange == API.FlightRange.AnnuallyBroadcast;

            const columns: Block[] = [];
            const addedColumns: Period[] = [];
            const calendarSelectedStartDateDate = calendarType == API.CalendarType.Client ? moment.utc(calendarStartDate).toDate() : moment.utc(startDate).isBefore(moment.utc(calendarStartDate).toDate()) ? startDate : moment.utc(calendarStartDate).toDate();
            const startDayOfWeek = (calendarType == API.CalendarType.Standard || flightRangeDifferentFromCalendar) && !isStandardAndUseCustomStartDay ? 0 : calendarType == API.CalendarType.Broadcast || isStandardAndUseCustomStartDay ? 1 : moment.utc(calendarSelectedStartDateDate).day();
            const extraWeekToAdd = !isStandardAndUseCustomStartDay && (calendarType == API.CalendarType.Standard || flightRangeDifferentFromCalendar) ? moment.utc(calendarSelectedStartDateDate).day() == startDayOfWeek || moment.utc(calendarSelectedStartDateDate).date() >= 7 ? 0 : 1 : 0;

            // Sort periodic data (by start date ascending and end date descending)
            const sortedPeriodicData = orderBy(
                periodicData,
                ['startDate', 'endDate'],
                ['asc', 'desc']
            ).filter((val, index) => calendarType != API.CalendarType.Client || !isFilterRequired || (index != 0 || Math.abs(differenceInMonths(calendarSelectedStartDateDate, val.startDate)) == 0));

            // Build period blocks
            sortedPeriodicData.forEach(
                ({ startDate: periodStartDate, endDate: periodEndDate, data }, index) => {
                    let columnStartsDays = 0;
                    let periodColumnLengthFinal = 1;
                    // Process only periodic data that fall within the overall period
                    if (periodStartDate >= startDate || periodEndDate <= endDate) {

                        // Determine the absolute columnStart of the period
                        const finalPeriodStartDate = periodStartDate < startDate
                            ? startDate
                            : periodStartDate;

                        let lastPeriodIndex = 0;
                        if (addedColumns.length) {
                            lastPeriodIndex = sortedPeriodicData.findIndex((val, periodIndex) => val.startDate.getTime() == addedColumns[addedColumns.length - 1].startDate.getTime());
                        } else {
                            lastPeriodIndex = 0;
                        }
                        // Determine columnLength of the period
                        const finalPeriodEndDate = periodEndDate > endDate ? endDate : periodEndDate;
                        if (lastPeriodIndex > -1 && (!addedColumns.length || lastPeriodIndex != index - 1)) {
                            const periodsToCalculate =
                                sortedPeriodicData.filter((val, dataindex) =>
                                    (!addedColumns.length || (!addedColumns.length ? dataindex >= lastPeriodIndex : dataindex > lastPeriodIndex)) && dataindex < index);// && (dataindex != 0 || Math.abs(differenceInDays(val.startDate, calendarSelectedStartDateDate))<=6)
                            if (periodsToCalculate.length) {

                                const first = periodsToCalculate[0];
                                const last = sortedPeriodicData[index];

                                let acualStartDate = moment.utc(first.startDate).isBefore(calendarSelectedStartDateDate)
                                    ? calendarSelectedStartDateDate
                                    : first.startDate;
                                const startDay = moment.utc(acualStartDate).day() % 7;
                                const isStartDaySameAsWeek = startDay === startDayOfWeek;
                                let calculatedStartDate = moment.utc(acualStartDate).toDate();
                                startDay === startDayOfWeek
                                    ? moment.utc(acualStartDate).toDate()
                                    : moment.utc(acualStartDate).date() >= 7
                                        ? moment.utc(acualStartDate).subtract(startDay - startDayOfWeek, "day").toDate()
                                        : moment.utc(acualStartDate).add(((7 - startDay) + startDayOfWeek) % 7, "day").toDate();
                                const daysDiff = differenceInDays(moment.utc(last.startDate).toDate(), moment.utc(calculatedStartDate).toDate());
                                const finalDaysDiff = !isStartDaySameAsWeek ? daysDiff + Math.abs(startDay - startDayOfWeek) : daysDiff;
                                const weeks = Math.max(0, Math.ceil(finalDaysDiff / 7));
                                columnStartsDays += weeks;

                                // Calculate column length for normalized view with spaces
                                if (isNormalCalendarEnabled) {
                                    if (!addedColumns.length) {
                                        const extraWeeksToAdd = this.calculateNormalizedCalendarColumns({
                                            addedColumns,
                                            normalizedStartDate,
                                            periodsToCalculate,
                                            calendarSelectedStartDateDate,
                                            startDayOfWeek,
                                            periodEndDate,
                                            periodStartDate,
                                            index,
                                            isBroadcast,
                                            isWeekly,
                                            isMonthly,
                                            isQuarterly,
                                            flightRange,
                                            flightRangeDifferentFromCalendar,
                                            extraWeeks,
                                            isNormalCalendarEnabled,
                                            sortedPeriodicData,
                                            isStandardAndNormalized
                                        });
                                        columnStartsDays += extraWeeksToAdd;
                                    } else {
                                        // unhandled
                                    }

                                }

                            }
                        }


                        // Calculate column length
                        let actualCalendarStartDate = moment(calendarSelectedStartDateDate).isAfter(moment(finalPeriodStartDate)) ? calendarSelectedStartDateDate : finalPeriodStartDate; //index == 0 ? moment.utc(periodStartDate).isBefore(calendarSelectedStartDateDate) ? periodStartDate : calendarSelectedStartDateDate : moment.utc(calendarSelectedStartDateDate).isAfter(moment.utc(finalPeriodStartDate)) ? calendarSelectedStartDateDate : finalPeriodStartDate;
                        const startDay = moment.utc(actualCalendarStartDate).day() % 7;
                        //let actualStartDate = startDay == startDayOfWeek ? moment.utc(actualCalendarStartDate).toDate() : moment.utc(actualCalendarStartDate).date() >= 7 ? moment.utc(actualCalendarStartDate).subtract(startDay - startDayOfWeek, "day").toDate() : moment.utc(actualCalendarStartDate).add(((7 - startDay) + startDayOfWeek) % 7, "day").toDate();
                        let actualStartDate = startDay == startDayOfWeek
                            ? moment.utc(actualCalendarStartDate).toDate() : index == 0 ? moment.utc(actualCalendarStartDate).subtract(startDay - startDayOfWeek, "day").toDate() : differenceInCalendarDays(periodEndDate, periodStartDate) > 7 ? moment.utc(actualCalendarStartDate).add(((7 - startDay) + startDayOfWeek) % 7, "day").toDate() : moment.utc(actualCalendarStartDate).subtract(startDay - startDayOfWeek, "day").toDate();

                        if (differenceInDays(finalPeriodEndDate, actualStartDate) > 7) {
                            periodColumnLengthFinal = 0;
                            const endDateForNormalView = moment.utc(sortedPeriodicData[sortedPeriodicData.length - 1].endDate).toDate();
                            let previousStartDate = actualStartDate;
                            const staticStartDate = moment.utc(actualStartDate).toDate();
                            let extraWeekAdded = 0;
                            while (moment.utc(actualStartDate).isBefore(moment.utc(finalPeriodEndDate)) && (!isNormalCalendarEnabled || !flightRangeDifferentFromCalendar || moment.utc(actualStartDate).isBefore(moment.utc(endDateForNormalView).add(-1, 'days')))) {
                                periodColumnLengthFinal++;

                                if (isNormalCalendarEnabled && moment.utc(previousStartDate).daysInMonth() != moment.utc(previousStartDate).date() && moment.utc(actualStartDate).date() < moment.utc(previousStartDate).date()) {
                                    periodColumnLengthFinal++;
                                    extraWeekAdded += 1;
                                }
                                previousStartDate = actualStartDate;
                                actualStartDate = moment.utc(actualStartDate).add(7, "day").toDate();
                            }

                            if (isNormalCalendarEnabled && index != sortedPeriodicData.length - 1) {
                                const extraWeeks = getExtraWeekForSpace(staticStartDate, finalPeriodEndDate);

                                periodColumnLengthFinal = this.calculateFinalPeriodColumnLength({
                                    isNormalCalendarEnabled,
                                    index,
                                    sortedPeriodicData,
                                    staticStartDate,
                                    periodColumnLengthFinal,
                                    flightRangeDifferentFromCalendar,
                                    finalPeriodEndDate,
                                    extraWeeks
                                });
                            }

                        } else if (isNormalCalendarEnabled && index != sortedPeriodicData.length - 1) {
                            const checkDate = moment.utc(actualStartDate).add(7, 'days');
                            if (moment.utc(actualStartDate).daysInMonth() != moment.utc(actualStartDate).date() && moment.utc(actualStartDate).date() > moment.utc(checkDate).date()) {
                                periodColumnLengthFinal++;
                            }
                        }

                        let isNextItemHasData = sortedPeriodicData.length > index + 1 && (isLastLevel || sortedPeriodicData[index + 1].data?.flightBarSum > 0);

                        // Get period column
                        const periodColumn = blockCb({
                            startDate: finalPeriodStartDate,
                            endDate: finalPeriodEndDate,
                            columnStart:
                                (!columns.length ? columnStart : 0) +
                                columnStartsDays,
                            columnLength: periodColumnLengthFinal,
                            data: { ...data, flightStartDate: columns.length > 0 ? finalPeriodStartDate : data.flightStartDate, flightEndDate: isNextItemHasData ? finalPeriodEndDate : data.flightEndDate },
                        });
                        // Push period column
                        if (periodColumn) {
                            columns.push(periodColumn);
                            addedColumns.push({ startDate: periodStartDate, endDate: periodEndDate });
                        }
                    }
                }
            );

            return columns;
        }
        else if (flightRange == API.FlightRange.FlightTotal && isNormalCalendarEnabled) {
            const columns: Block[] = [];
            const isBroadcastCalendar = referencedCalendarDefinition?.Definition?.Configuration?.Type == API.CalendarType.Broadcast;
            const isStandardAndUseCustomStartDay = referencedCalendarDefinition?.Definition?.Configuration?.Type == API.CalendarType.Standard && referencedCalendarDefinition?.Definition?.Configuration?.UseCustomStartDay;
            const flightRangeDifferentFromCalendar = isNormalCalendarEnabled && isBroadcastCalendar
                && (flightRange == API.FlightRange.FlightTotal);
            const calendarSelectedStartDateDate = calendarType == API.CalendarType.Client ? moment.utc(calendarStartDate).toDate() : moment.utc(startDate).isBefore(moment.utc(calendarStartDate).toDate()) ? startDate : moment.utc(calendarStartDate).toDate();
            const startDayOfWeek = (calendarType == API.CalendarType.Standard || flightRangeDifferentFromCalendar) && !isStandardAndUseCustomStartDay ? 0 : calendarType == API.CalendarType.Broadcast || isStandardAndUseCustomStartDay ? 1 : moment.utc(calendarSelectedStartDateDate).day();
            // Determine the number of days of the total period
            const periodDays = differenceInDays(endDate, startDate) + 1;

            // Determine the days per column
            const columnDays = (periodDays) / (columnLength);

            // Sort periodic data (by start date ascending and end date descending)
            const sortedPeriodicData = orderBy(
                periodicData,
                ['startDate', 'endDate'],
                ['asc', 'desc']
            );
            // Build period blocks
            let currentColumn = 0;
            let foundRows = false;
            const addedColumns: Period[] = [];
            sortedPeriodicData.forEach(
                ({ startDate: periodStartDate, endDate: periodEndDate, data }, index) => {
                    // Process only periodic data that fall within the overall period
                    if (periodStartDate >= startDate || periodEndDate <= endDate) {
                        // Determine the absolute columnStart of the period
                        const finalPeriodStartDate = periodStartDate < startDate
                            ? startDate
                            : periodStartDate;

                        const absolutePeriodColumnStart = getCustomColumnCount(startDate, finalPeriodStartDate, startDayOfWeek);

                        // leave empty columns if no flightbar data and start from actual start date
                        if ((data?.flightBarSum > 0 || isLastLevel) && !foundRows && index > 0 && !columns.length) {
                            currentColumn = 0;
                            foundRows = true;
                        }

                        let periodColumnStart =
                            (absolutePeriodColumnStart)
                            - currentColumn;

                        // Determine columnLength of the period
                        const refactoredEndDate = getPreviousWeekStart(endDate, startDayOfWeek);
                        const finalPeriodEndDate = periodEndDate > refactoredEndDate ? refactoredEndDate : periodEndDate;
                        const isCalendarEndDate = finalPeriodEndDate === refactoredEndDate;
                        let periodColumnLength = getColumnLengthWithRules(finalPeriodStartDate, finalPeriodEndDate, isCalendarEndDate, startDayOfWeek);

                        currentColumn += periodColumnLength;

                        if (periodColumnStart < 0 && currentColumn != 0) {
                            periodColumnLength = periodColumnLength + periodColumnStart;
                        }
                        currentColumn += periodColumnStart;
                        periodColumnStart =
                            periodColumnStart < 0 ? 0 : periodColumnStart;

                        currentColumn = currentColumn < 0 ? 0 : currentColumn

                        const periodColumn = blockCb({
                            startDate: finalPeriodStartDate,
                            endDate: finalPeriodEndDate,
                            columnStart:
                                (!columns.length ? columnStart : 0) +
                                periodColumnStart,
                            columnLength: periodColumnLength,
                            data,
                        });
                        // Push period column
                        if (periodColumn) {
                            columns.push(periodColumn);
                            addedColumns.push({ startDate: finalPeriodStartDate, endDate: finalPeriodEndDate });
                        }
                        else {
                            currentColumn -=
                                (!columns.length ? columnStart : 0) +
                                periodColumnStart +
                                periodColumnLength;
                        }
                    }
                }
            );
            return columns;

        }

        else {
            const columns: Block[] = [];
            // Determine the number of days of the total period
            const periodDays = differenceInDays(endDate, startDate) + 1;

            // Determine the days per column
            // Defensive: ensure columnLength > 0 and columnDays > 0
            const safeColumnLength = columnLength > 0 ? columnLength : 1;
            const columnDays = periodDays / safeColumnLength;
            const divisor = Math.max(columnDays, Number.EPSILON);

            // Sort periodic data (by start date ascending and end date descending)
            const sortedPeriodicData = orderBy(
                periodicData,
                ['startDate', 'endDate'],
                ['asc', 'desc']
            );

            // Build period blocks
            let currentColumn = 0;
            let foundRows = false;
            const addedColumns: Period[] = [];
            sortedPeriodicData.forEach(
                ({ startDate: periodStartDate, endDate: periodEndDate, data }, index) => {
                    // Process only periodic data that fall within the overall period (inclusive overlap)
                    const overlaps =
                        (periodStartDate >= startDate && periodStartDate <= endDate) ||
                        (periodEndDate >= startDate && periodEndDate <= endDate) ||
                        (periodStartDate <= startDate && periodEndDate >= endDate);

                    if (overlaps) {
                        // Determine the absolute columnStart of the period (use 0-based column index mapping)
                        const finalPeriodStartDate = periodStartDate < startDate ? startDate : periodStartDate;
                        const finalPeriodEndDate = periodEndDate > endDate ? endDate : periodEndDate;

                        // days offset from overall start (0-based)
                        const startOffset = Math.max(0, differenceInDays(finalPeriodStartDate, startDate));
                        const endOffset = Math.max(0, differenceInDays(finalPeriodEndDate, startDate));

                        // Map day offsets to column indexes using proportional mapping:
                        // startIndex = floor(startOffset / columnDays)
                        // endIndex = floor(endOffset / columnDays)
                        const startIndex = Math.floor(startOffset / divisor);
                        const endIndex = Math.floor(endOffset / divisor);

                        // ensure indexes are within [0, columnLength-1]
                        const boundedStartIndex = Math.min(Math.max(startIndex, 0), safeColumnLength - 1);
                        const boundedEndIndex = Math.min(Math.max(endIndex, 0), safeColumnLength - 1);

                        const absolutePeriodColumnStart = boundedStartIndex;

                        // leave empty columns if no flightbar data and start from actual start date
                        if ((data?.flightBarSum > 0 || isLastLevel) && !foundRows && index > 0 && !columns.length) {
                            currentColumn = 0;
                            foundRows = true;
                        }

                        let periodColumnStart =
                            (absolutePeriodColumnStart) - currentColumn;

                        // Determine columnLength of the period as number of columns spanned
                        let periodColumnLength =
                            Math.max(1, boundedEndIndex - boundedStartIndex + 1);

                        // Adjust if periodColumnStart is negative (we overlap previous columns)
                        if (periodColumnStart < 0) {
                            periodColumnLength = Math.max(1, periodColumnLength + periodColumnStart);
                            periodColumnStart = 0;
                        }

                        currentColumn += periodColumnLength;
                        periodColumnStart =
                            periodColumnStart < 0 ? 0 : periodColumnStart;
                        currentColumn += periodColumnStart;

                        const periodColumn = blockCb({
                            startDate: finalPeriodStartDate,
                            endDate: finalPeriodEndDate,
                            columnStart:
                                (!columns.length ? columnStart : 0) +
                                periodColumnStart,
                            columnLength: periodColumnLength,
                            data,
                        });
                        // Push period column
                        if (periodColumn) {
                            columns.push(periodColumn);
                            addedColumns.push({ startDate: finalPeriodStartDate, endDate: finalPeriodEndDate });
                        }
                        else {
                            currentColumn -=
                                (!columns.length ? columnStart : 0) +
                                periodColumnStart +
                                periodColumnLength;
                        }
                    }
                }
            );

            return columns;
        }

    };

    merge = (blockGroups: Block[][][], leftMenuColumnLength: number): Block[][] => {
        let blocks: Block[][] = [];

        // Get max number of group rows
        const maxGroupRows = Math.max(...blockGroups.map((g) => g.length));

        // Get max row height off all groups for each row
        const maxRowHeight = {};
        Array.from({ length: maxGroupRows }).forEach((_r, rowIndex) => {
            maxRowHeight[rowIndex] = Math.max(
                ...blockGroups.map((g) =>
                    g[rowIndex] ? this.getRowLength([g[rowIndex]]) : 0
                )
            );
        });

        // Get max number of group columns
        const maxGroupColumns = {};
        blockGroups.forEach((blockGroup, groupIndex) => {
            maxGroupColumns[groupIndex] = this.getColumnLength(blockGroup) + (leftMenuColumnLength || 0);
        });

        blocks = Array.from({ length: maxGroupRows }).map((_r, rowIndex) => {
            const block: Block[] = [];

            let needColumnBefore = 0;
            Array.from({ length: blockGroups.length }).forEach(
                (_g, groupIndex) => {
                    if (
                        Array.isArray(blockGroups[groupIndex][rowIndex]) &&
                        blockGroups[groupIndex][rowIndex].length
                    ) {
                        const needColumnAfter =
                            maxGroupColumns[groupIndex] -
                            this.getRowColumnEnd(
                                blockGroups[groupIndex][rowIndex]
                            );

                        if (needColumnAfter) {
                            blockGroups[groupIndex][rowIndex][
                                blockGroups[groupIndex][rowIndex].length - 1
                            ].columnAfter += needColumnAfter;
                        }

                        if (needColumnBefore) {
                            blockGroups[groupIndex][rowIndex][0].columnStart +=
                                needColumnBefore;
                        }

                        block.push(...blockGroups[groupIndex][rowIndex]);
                    } else {
                        needColumnBefore += maxGroupColumns[groupIndex];
                    }
                }
            );

            return block;
        });

        return blocks;
    };

    move = (
        blocks: Block[][],
        {
            top = 0,
            left = 0,
        }: {
            top?: number;
            left?: number;
        } = {}
    ): Block[][] => {
        const movedBlocks: Block[][] = blocks;

        if (movedBlocks.length) {
            if (top) {
                movedBlocks[0].forEach((column, columnIndex) => {
                    movedBlocks[0][columnIndex].rowStart += top;
                });
            }

            if (left) {
                movedBlocks.forEach((row, rowIndex) => {
                    row[0].columnStart += left;
                });
            }
        }

        return movedBlocks;
    };

    getRowColumnEnd = (blocks: Block[]): number => {
        let columnEnd = 0;
        blocks.forEach((block) => {
            columnEnd +=
                block.columnStart + block.columnLength + block.columnAfter;
        });

        return columnEnd;
    };

    getFullColumnLength = (rows: Block[][]): number => {
        let columnLength = 0;
        let newcolumnLength = 0;
        rows.forEach((row) => {
            const rowColumnStart = sumBy(row, 'columnStart') ?? 0;
            const rowColumnAfter = sumBy(row, 'columnAfter') ?? 0;
            const rowColumnLength =
                (Math.abs(sumBy(row, 'columnLength') ?? 0)) +
                rowColumnStart +
                rowColumnAfter;
            columnLength = columnLength > Math.abs(sumBy(row, 'columnStart') ?? 0) ? columnLength : Math.abs(sumBy(row, 'columnStart') ?? 0);
            newcolumnLength = columnLength > rowColumnLength ? columnLength : rowColumnLength;
        });
        return newcolumnLength;


    };

    getColumnLength = (rows: Block[][]): number => {
        let max = 0;
        rows.forEach((row) => {
            row.forEach(r => {
                max = Math.max(Math.abs(r.columnLength + r.columnAfter), max)
            })

        });
        return max;

    };
    getRowLength = (rows: Block[][]): number => {
        let rowLength = 0;
        rows.forEach((row) => {
            const rowRowLength = Math.max(
                1,
                ...row.map(
                    (o) =>
                        (o.rowStart ?? 0) +
                        (o?.rowLength ?? 1) +
                        (o?.rowAfter ?? 0)
                )
            );

            rowLength += rowRowLength;
        });

        return rowLength;
    };

    getDatesDifferencesByCalendarType = (endDate: Date, startDate: Date, calendarType: API.CalendarType): number => {
        return calendarType == API.CalendarType.Standard ? differenceInDays(endDate, startDate) : differenceInDays(endDate, startDate);
    };

    /**
 * Calculates column start days for normalized calendar view based on various flight range configurations
 * 
 * @param {Object} params - Parameters for the calculation
 * @returns {number} Number of column start days to add
 */
    private calculateNormalizedCalendarColumns = ({
        addedColumns,
        normalizedStartDate,
        periodsToCalculate,
        calendarSelectedStartDateDate,
        startDayOfWeek,
        periodEndDate,
        periodStartDate,
        index,
        isBroadcast,
        isWeekly,
        isMonthly,
        isQuarterly,
        flightRange,
        flightRangeDifferentFromCalendar,
        extraWeeks,
        isNormalCalendarEnabled,
        sortedPeriodicData,
        isStandardAndNormalized,
    }: {
        addedColumns: Period[];
        normalizedStartDate: Date;
        periodsToCalculate: { startDate: Date; endDate: Date; data: any }[];
        calendarSelectedStartDateDate: Date;
        startDayOfWeek: number;
        periodEndDate: Date;
        periodStartDate: Date;
        index: number;
        isBroadcast: boolean;
        isWeekly: boolean;
        isMonthly: boolean;
        isQuarterly: boolean;
        flightRange: API.FlightRange;
        flightRangeDifferentFromCalendar: boolean;
        extraWeeks: number;
        isNormalCalendarEnabled: boolean;
        sortedPeriodicData: { startDate: Date; endDate: Date; data: any }[];
        isStandardAndNormalized: boolean;
    }): number => {
        if (!isNormalCalendarEnabled || addedColumns.length) {
            return 0;
        }

        let columnStartsDays = 0;
        const previousPeriodStartDate = normalizedStartDate;
        const previousPeriodEndDate = periodsToCalculate[periodsToCalculate.length - 1].endDate;

        // Calculate actual calendar start date
        const actualCalendarStartDate = moment(calendarSelectedStartDateDate).isAfter(moment(previousPeriodStartDate))
            ? calendarSelectedStartDateDate
            : previousPeriodStartDate;

        const startDay = moment.utc(actualCalendarStartDate).day() % 7;

        // Determine actual start date based on multiple conditions
        const isStartDaySameAsWeek = startDay == startDayOfWeek;
        const isStartDayBeforeWeekStart = (startDay - startDayOfWeek) < 0;

        //const actualStartDate = isStartDaySameAsWeek ? moment.utc(actualCalendarStartDate).toDate() : moment.utc(actualCalendarStartDate).subtract(Math.abs(startDay - (7 - startDayOfWeek) - 1), "day").toDate();
        const actualStartDate = isStartDaySameAsWeek ? moment.utc(actualCalendarStartDate).toDate()
            : isStartDayBeforeWeekStart ? moment.utc(actualCalendarStartDate).subtract((7 + (startDay - startDayOfWeek)), "day").toDate() :
                moment.utc(actualCalendarStartDate).subtract(startDay - startDayOfWeek, "day").toDate();

        // Calculate extra spaces based on date ranges
        const extraWeekSpaces = getExtraWeekForSpace(actualStartDate, previousPeriodEndDate);
        columnStartsDays = extraWeekSpaces;

        return columnStartsDays;
    }


    /**
 * Adjusts the final period column length based on calendar and flight range settings
 * 
 * @param {Object} params - Parameters for the period column length calculation
 * @returns {number} The adjusted period column length
 */
    private calculateFinalPeriodColumnLength = ({
        isNormalCalendarEnabled,
        index,
        sortedPeriodicData,
        staticStartDate,
        periodColumnLengthFinal,
        flightRangeDifferentFromCalendar,
        finalPeriodEndDate,
        extraWeeks,
    }: {
        isNormalCalendarEnabled: boolean;
        index: number;
        sortedPeriodicData: { startDate: Date; endDate: Date; data: any }[];
        staticStartDate: Date;
        periodColumnLengthFinal: number;
        flightRangeDifferentFromCalendar: boolean;
        finalPeriodEndDate: Date;
        extraWeeks: number;
    }): number => {
        // If not applicable, return original value
        if (!isNormalCalendarEnabled || index === sortedPeriodicData.length - 1) {
            return periodColumnLengthFinal;
        }

        // Calculate check date by adding 7 days periodColumnLengthFinal-1 times
        let checkDate = moment.utc(staticStartDate);
        if (periodColumnLengthFinal - extraWeeks - 1 > 0) {
            for (let i = 0; i < periodColumnLengthFinal - extraWeeks - 1; i++) {
                checkDate = moment.utc(checkDate).add(7, 'days');
            }
        }

        // Adjust column length based on flight range settings
        if (flightRangeDifferentFromCalendar) {
            if (moment.utc(finalPeriodEndDate).daysInMonth() !== checkDate.date()) {
                return periodColumnLengthFinal + 1;
            }
        } else {
            // More complex case for non-different flight range
            if (moment.utc(finalPeriodEndDate).daysInMonth() !== checkDate.date() &&
                checkDate.month() !== moment.utc(finalPeriodEndDate).date() &&
                moment.utc(finalPeriodEndDate).add(1, 'seconds').month() !== moment.utc(finalPeriodEndDate).month()) {
                return periodColumnLengthFinal + 1;
            }
        }

        return periodColumnLengthFinal;
    }

    /**
 * Calculates extra spacing metrics for normalized calendar layout
 * Computes both extra week spaces and extra column length when enabled
 * 
 * @param startDate The reference start date
 * @param finalPeriodStartDate The period start date
 * @param finalPeriodEndDate The period end date
 * @param addedColumns Previously added columns to consider for spacing
 * @param isNormalCalendarEnabled Whether to apply normalization
 * @returns Object with extraWeekSpaces and extraColumnLength values
 */
    private calculateNormalizedSpacing(
        startDate: Date,
        finalPeriodStartDate: Date,
        finalPeriodEndDate: Date,
        addedColumns: Period[],
        isNormalCalendarEnabled: boolean
    ): { extraWeekSpaces: number; extraColumnLength: number } {
        // Default values when normalization is disabled
        if (!isNormalCalendarEnabled) {
            return { extraWeekSpaces: 0, extraColumnLength: 0 };
        }

        let extraWeekSpaces = 0;
        let extraColumnLength = 0;

        // Calculate extraWeekSpaces based on previous column or start date
        if (addedColumns.length) {
            // Use the end date of the last added column
            const lastItem = addedColumns[addedColumns.length - 1];
            extraWeekSpaces = getExtraWeekForSpace(lastItem.endDate, finalPeriodStartDate);
        } else {
            // Use the overall start date
            extraWeekSpaces = getExtraWeekForSpace(startDate, finalPeriodStartDate);
        }

        // Calculate extraColumnLength
        extraColumnLength = getExtraWeekForSpace(finalPeriodStartDate, finalPeriodEndDate);

        return { extraWeekSpaces, extraColumnLength };
    }


}


