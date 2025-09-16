import { Expose } from 'class-transformer';

export class CalendarConfig {
    @Expose()
    calendarLocationColumn: string;

    @Expose()
    calendarLocationRow: string;

    @Expose()
    calendarType: string;

    @Expose()
    endOfYear: Date;

    @Expose()
    rowTypeMonth: boolean;

    @Expose()
    rowTypeMonthStyle: string;

    @Expose()
    rowTypePeriodMonth: boolean;

    @Expose()
    rowTypePeriodMonthStyle: string;

    @Expose()
    rowTypePeriodWeek: boolean;

    @Expose()
    rowTypePeriodWeekStyle: string;

    @Expose()
    rowTypeQuarter: boolean;

    @Expose()
    rowTypeQuarterStyle: string;

    @Expose()
    rowTypeWeek: boolean;

    @Expose()
    rowTypeWeekStyle: string;

    @Expose()
    rowTypeYear: boolean;

    @Expose()
    rowTypeYearStyle: string;

    @Expose()
    startOfWeek: Date;
}
