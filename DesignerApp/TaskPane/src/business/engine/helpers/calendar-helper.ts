import {
    CalendarConfiguration,
    CalendarReportingTimeFrame,
    CalendarType,
    StandardCalenderRowType,
} from '@omniflow/omni-webapi';
import { find } from 'lodash';
import moment from 'moment';

import { dataApi } from '../../../lib/api';
import { getPeriod, getPeriods, Period, PeriodUnit } from './period-helper';

declare global {
    interface Window {
        CalendarHelper: any;
    }
}

export class CalendarHelper {
    // Private vars
    private clientId: string;
    private isReportingTimeFrame: boolean;
    private reportingTimeFrame: CalendarReportingTimeFrame;

    // Public vars
    calendarFrom: Date;
    calendarTo: Date;
    data: any[];
    dataFrom: Date;
    dataTo: Date;
    calendarType: CalendarType;

    constructor(clientId: string, configuration: CalendarConfiguration) {
        this.clientId = clientId;
        this.isReportingTimeFrame = configuration.IsReportingTimeFrame ?? false;
        this.reportingTimeFrame = configuration.ReportingTimeFrame;
        this.calendarType = configuration.Type;
        const { startDate, endDate } =
            this.calculateCalendarDates(configuration);

        this.calendarFrom = startDate;
        this.calendarTo = endDate;

        window['CalendarHelper'] = this;
    }

    async getData() {
        const currentYear = moment.utc().year();

        const year =
            this.reportingTimeFrame === CalendarReportingTimeFrame.CurrentYear
                ? currentYear
                : currentYear - 1;

        switch (this.calendarType) {
            case CalendarType.Broadcast:
                this.data = await this.fetchBroadCastData(year);
                break;
            case CalendarType.Client:
                this.data = await this.fetchClientCalendarData(year);
                break;
            default:
                this.data = this.getCalendarData();
                break;
        }

        const { startDate, endDate } = this.calculateDataDates(this.data);
        this.dataFrom = startDate;
        this.dataTo = endDate;

        return this.data;
    }

    private async fetchBroadCastData(year) {
        let result;
        if (this.isReportingTimeFrame) {
            result = await dataApi.dataGetBroadcastCalendar(year);
        } else {
            result = await dataApi.dataGetBroadcastCalendar_1(
                moment.utc(this.calendarFrom).format('YYYY-MM-DD'),
                moment.utc(this.calendarTo).format('YYYY-MM-DD')
            );
        }
        return this.sortByDate(result?.data, 'firstday');
    }

    private async fetchClientCalendarData(year) {
        let result;
        if (this.isReportingTimeFrame) {
            result = await dataApi.dataGetClientCalendar(year, this.clientId);
        } else {
            result = await dataApi.dataGetClientCalendar_2(
                moment.utc(this.calendarFrom).format('YYYY-MM-DD'),
                moment.utc(this.calendarTo).format('YYYY-MM-DD'),
                this.clientId
            );
        }
        const sortedData = this.sortByDate(result?.data, 'weekdaydate');

        // Extract only one entry per week
        return sortedData.filter((_, index) => {
            return index === 0 || index % 7 === 0;
        });
    }

    formatValue(jsonValue: string, key: string) {
        const formatListKeys = ['week_text'];
        if (formatListKeys.includes(key)) {
            if (key === 'week_text') {
                const value = moment.utc(JSON.parse(jsonValue)).date();

                return value;
            }
        }
        try {
            return JSON.parse(jsonValue);
        } catch {
            return jsonValue;
        }
    }

    private sortByDate(data, key) {
        let sortedData = [];

        if (data) {
            sortedData = data.sort((weekA, weekB) => {
                const validDate = JSON.parse(
                    weekA.find((obj) => obj.ColumnName === key).JsonValue
                );
                const validComparisonDate = JSON.parse(
                    weekB.find((obj) => obj.ColumnName === key).JsonValue
                );

                if (validDate && validComparisonDate) {
                    return (
                        new Date(validDate).getTime() -
                        new Date(validComparisonDate).getTime()
                    );
                }
                return false;
            });
        }

        return sortedData;
    }

    private calculateCalendarDates(configuration: CalendarConfiguration) {
        let { startDate, endDate } = getPeriod(
            moment.utc().toDate(),
            PeriodUnit.YEAR
        );

        if (this.isReportingTimeFrame) {
            if (
                this.reportingTimeFrame === CalendarReportingTimeFrame.LastYear
            ) {
                startDate.setFullYear(startDate.getFullYear() - 1);
                endDate.setFullYear(endDate.getFullYear() - 1);
            }

            if (this.calendarType === CalendarType.Broadcast) {
                ({ startDate, endDate } = getPeriod(
                    startDate,
                    PeriodUnit.BC_YEAR
                ));
            }
        } else {
            // Custom dates set?
            startDate = configuration.CustomStartDate
                ? moment.utc(configuration.CustomStartDate).toDate()
                : startDate;
            endDate = configuration.CustomEndDate
                ? moment.utc(configuration.CustomEndDate).toDate()
                : endDate;
        }

        return { startDate, endDate };
    }

    private calculateDataDates(data: any[] = []) {
        let startDate,
            endDate = null;

        if (data && data.length) {
            // Get date of first element
            let firstDate = find(data[0], { ColumnName: 'weekdaydate' });
            if (!firstDate)
                firstDate = find(data[0], { ColumnName: 'firstday' });
            if (!firstDate)
                firstDate = find(data[0], { ColumnName: 'lastday' });

            // Get date of last element
            const lastIndex = data.length - 1;
            let lastDate = find(data[lastIndex], {
                ColumnName: 'weekdaydate',
            });
            if (!lastDate)
                lastDate = find(data[lastIndex], { ColumnName: 'firstday' });
            if (!lastDate)
                lastDate = find(data[lastIndex], { ColumnName: 'lastday' });

            // Get start and end date of api data
            if (firstDate?.JsonValue && lastDate?.JsonValue) {
                const periodUnit =
                    this.calendarType === CalendarType.Broadcast
                        ? PeriodUnit.BC_WEEK
                        : PeriodUnit.WEEK;
                ({ startDate } = getPeriod(
                    moment.utc(JSON.parse(firstDate.JsonValue)).toDate(),
                    periodUnit
                ));

                ({ endDate } = getPeriod(
                    moment.utc(JSON.parse(lastDate.JsonValue)).toDate(),
                    periodUnit
                ));
            }
        }

        return {
            startDate: startDate ?? this.calendarFrom,
            endDate: endDate ?? this.calendarTo,
        };
    }

    private getDates() {
        let start = this.calendarFrom;
        let end = this.calendarTo;
        if (this.isReportingTimeFrame) {
            const year =
                this.reportingTimeFrame ===
                    CalendarReportingTimeFrame.CurrentYear
                    ? moment.utc().year()
                    : moment.utc().year() - 1;
            start = moment.utc(`${year}`).toDate();
            end = moment.utc(start).endOf('year').toDate();
        }

        return getPeriods(start, end, PeriodUnit.WEEK, this.calendarType);
    }

    private getCalendarData() {
        const dates = this.getDates();
        const weeks = this.formatDates(dates, 'D');
        const years = this.formatDates(dates, 'YYYY');
        const months = this.formatDates(dates, 'MMMM');
        const quarters = this.formatDates(dates, 'Q');

        return weeks.map((week, index) => [
            {
                ColumnName: StandardCalenderRowType.Week,
                JsonValue: week.toString(),
            },
            {
                ColumnName: StandardCalenderRowType.Month,
                JsonValue: months[index].toString(),
            },
            {
                ColumnName: StandardCalenderRowType.Year,
                JsonValue: years[index].toString(),
            },
            {
                ColumnName: StandardCalenderRowType.Quarter,
                JsonValue: quarters[index].toString(),
            },
        ]);
    }

    private formatDates(dates: Period[], format: string) {
        return dates.map((date) => {
            const result = moment.utc(date.startDate).format(format);

            return `${format === 'Q' ? 'Q' : ''}${result}`;
        });
    }
}

/**
* Normalizes date range for broadcast calendars to align with week boundaries
* Adjusts start date to Monday (1) and end date to Sunday (0)
* 
* @param startDate The initial start date to normalize
* @param endDate The initial end date to normalize
* @param isEnabled Whether normalization should be applied
* @returns Object containing normalized start and end dates
*/
export function normalizeCalendarDateRange(
    startDate: Date,
    endDate: Date,
    isEnabled: boolean,
    startDay?: number
): { normalizedStartDate: Date; normalizedEndDate: Date } {
    // Skip normalization if not enabled
    if (!isEnabled) {
        return { normalizedStartDate: startDate, normalizedEndDate: endDate };
    }

    // Create moment objects to work with
    const momentStartDate = moment.utc(startDate);
    const momentEndDate = moment.utc(endDate);

    // Start date should be a Monday (day 1)
    const startDayOfWeek = startDay??1;
    const currentStartDay = momentStartDate.day() % 7;

    // Adjust start date if needed
    if (currentStartDay !== startDayOfWeek) {
        momentStartDate.subtract(currentStartDay - startDayOfWeek, 'day');
    }

    // End date should be a Sunday (day 0)
    const endDayOfWeek = 0;
    const currentEndDay = momentEndDate.day() % 7;

    // Adjust end date if needed
    if (currentEndDay !== endDayOfWeek) {
        momentEndDate.subtract(currentEndDay - endDayOfWeek + 7, 'day');
    }

    return {
        normalizedStartDate: momentStartDate.toDate(),
        normalizedEndDate: momentEndDate.toDate()
    };
}
