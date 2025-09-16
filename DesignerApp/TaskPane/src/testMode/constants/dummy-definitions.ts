import {
    CalendarReportingTimeFrame,
    CalendarType,
    CalendarRowStyle,
    FontWeight,
    HeaderDateFormat,
    HeaderRowMerge,
    HeaderRowType,
    BorderLineStyle,
    HeaderDefinition,
    TotalsDefinition,
    GrandTotalDefinition,
    FlightRange,
    CalendarConfiguration,
    CalendarDefinition,
} from '@omniflow/omni-webapi';
import { DefaultStyling } from '../../business/engine/models/styles';

export const DUMMY_CALENDAR_DEFINITION: CalendarDefinition = {
    Configuration: {
        //CustomEndDate: '2022-12-31T00:00:00Z',
        CustomEndDate: new Date('2023-12-31T00:00:00Z'),
        //CustomStartDate: '2022-01-01T00:00:00Z',
        CustomStartDate: new Date('2023-01-01T00:00:00Z'),
        IsReportingTimeFrame: false,
        ReportingTimeFrame: CalendarReportingTimeFrame.LastYear,
        Type: CalendarType.Broadcast,
        Rows: [
            {
                ColumnName: 'year_text',
                //ColumnSource: null,
                Order: 0,
                Style: CalendarRowStyle.YearYYYY,
                Styling: DefaultStyling(),
            },
            {
                ColumnName: 'month_text',
                //ColumnSource: null,
                Order: 1,
                Style: CalendarRowStyle.MonthMMM,
                Styling: DefaultStyling(),
                //id: 'calendar_rows3',
            },
        ],
    },
    Styling: {
        Alignment: {},
        Border: {
            Top: {
                Color: {
                    Alpha: 1,
                    Blue: 1,
                    Green: 1,
                    Red: 1,
                },
                Style: BorderLineStyle.Continuous,
            },
            Left: {
                Color: {
                    Alpha: 1,
                    Blue: 1,
                    Green: 1,
                    Red: 1,
                },
                Style: BorderLineStyle.Continuous,
            },
            Right: {
                Color: {
                    Alpha: 1,
                    Blue: 1,
                    Green: 1,
                    Red: 1,
                },
                Style: BorderLineStyle.Continuous,
            },
            Bottom: {
                Color: {
                    Alpha: 1,
                    Blue: 1,
                    Green: 1,
                    Red: 1,
                },
                Style: BorderLineStyle.Continuous,
            },
        },
        Fill: {},
        Font: {
            // BackgroundColor: {
            //     Alpha: 0,
            //     Blue: 0,
            //     Green: 0,
            //     Red: 0,
            // },
            Background: {
                Alpha: 1,
                Blue: 255,
                Green: 255,
                Red: 255,
            },
            ColorAlternate: {
                Alpha: 1,
                Blue: 255,
                Green: 255,
                Red: 255,
            },
            BackgroundAlternate: {
                Alpha: 1,
                Blue: 0,
                Green: 0,
                Red: 0,
            },
            Color: {
                Alpha: 1,
                Blue: 0,
                Green: 0,
                Red: 0,
            },
            Family: 'Calibri',
            Size: 10,
            Weight: FontWeight.Regular,
        },
    },
};

export const DUMMY_HEADER_DEFINITION: HeaderDefinition = {
    Configuration: {
        ColumnMargin: 0,
        RowMargin: 0,
        RowMerge: HeaderRowMerge.MergeAndCenter,
        Rows: [
            {
                Date: new Date('2022-11-29T12:03:50.088Z'),
                DateFormat: HeaderDateFormat.MMDDYYYY,
                Order: 0,
                Type: HeaderRowType.Date,
                Styling: {
                    Alignment: {},
                    Border: {
                        Top: {
                            Color: { Alpha: 1, Blue: 1, Green: 1, Red: 1 },
                            Style: BorderLineStyle.Continuous,
                        },
                        Left: {
                            Color: { Alpha: 1, Blue: 1, Green: 1, Red: 1 },
                            Style: BorderLineStyle.Continuous,
                        },
                        Right: {
                            Color: { Alpha: 1, Blue: 1, Green: 1, Red: 1 },
                            Style: BorderLineStyle.Continuous,
                        },
                        Bottom: {
                            Color: { Alpha: 1, Blue: 1, Green: 1, Red: 1 },
                            Style: BorderLineStyle.Continuous,
                        },
                    },
                    Fill: {},
                    Font: {
                        Color: {
                            Alpha: 0,
                            Blue: 0,
                            Green: 0,
                            Red: 0,
                        },
                        Background: {
                            Alpha: 1,
                            Blue: 255,
                            Green: 255,
                            Red: 255,
                        },
                        ColorAlternate: {
                            Alpha: 1,
                            Blue: 255,
                            Green: 255,
                            Red: 255,
                        },
                        BackgroundAlternate: {
                            Alpha: 0,
                            Blue: 0,
                            Green: 0,
                            Red: 0,
                        },
                        Family: 'Calibri',
                        Size: 10,
                        Weight: FontWeight.Regular,
                    },
                },
            },
        ],
    },
    Styling: {
        Alignment: {},
        Border: {
            Top: {
                Color: { Alpha: 1, Blue: 1, Green: 1, Red: 1 },
                Style: BorderLineStyle.Continuous,
            },
            Left: {
                Color: { Alpha: 1, Blue: 1, Green: 1, Red: 1 },
                Style: BorderLineStyle.Continuous,
            },
            Right: {
                Color: { Alpha: 1, Blue: 1, Green: 1, Red: 1 },
                Style: BorderLineStyle.Continuous,
            },
            Bottom: {
                Color: { Alpha: 1, Blue: 1, Green: 1, Red: 1 },
                Style: BorderLineStyle.Continuous,
            },
        },
        Fill: {},
        Font: {
            Color: {
                Alpha: 0,
                Blue: 0,
                Green: 0,
                Red: 0,
            },
            Background: {
                Alpha: 1,
                Blue: 255,
                Green: 255,
                Red: 255,
            },
            ColorAlternate: {
                Alpha: 1,
                Blue: 255,
                Green: 255,
                Red: 255,
            },
            BackgroundAlternate: {
                Alpha: 0,
                Blue: 0,
                Green: 0,
                Red: 0,
            },
            Family: 'Calibri',
            Size: 10,
            Weight: FontWeight.Regular,
        },
    },
};
