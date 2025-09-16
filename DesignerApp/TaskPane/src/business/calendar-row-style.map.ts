import * as API from '@omniflow/omni-webapi';

/**
 * Get readable string from component type enum.
 * @param type Page
 * @returns string
 */

export const CalendarRowStyleMap = {
    Styles: (type): API.CalendarRowStyle[] => {
        return Object.keys(API.CalendarRowStyle).filter((style) =>
            style.startsWith(type)
        ) as API.CalendarRowStyle[];
    },
    Quarter: (type: API.CalendarRowStyle): string => {
        switch (type) {
            case API.CalendarRowStyle.QuarterQ1:
                return 'Q1';
            default:
                return '???';
        }
    },
    PeriodMonth: (type: API.CalendarRowStyle): string => {
        switch (type) {
            case API.CalendarRowStyle.PeriodMonthP1:
                return 'P1';
            default:
                return '???';
        }
    },
    PeriodWeek: (type: API.CalendarRowStyle): string => {
        switch (type) {
            case API.CalendarRowStyle.PeriodWeekOneXOne:
                return '1 x 1';
            default:
                return '???';
        }
    },
    Year: (type: API.CalendarRowStyle): string => {
        switch (type) {
            case API.CalendarRowStyle.YearYY:
                return 'YY';
            case API.CalendarRowStyle.YearYYYY:
                return 'YYYY';
            default:
                return '???';
        }
    },
    Month: (type: API.CalendarRowStyle): string => {
        switch (type) {
            case API.CalendarRowStyle.MonthMM:
                return 'MM';
            case API.CalendarRowStyle.MonthMMM:
                return 'MMMM';
            default:
                return '???';
        }
    },
    Week: (type: API.CalendarRowStyle): string => {
        switch (type) {
            case API.CalendarRowStyle.WeekDD:
                return 'DD';
            default:
                return '???';
        }
    },
};
