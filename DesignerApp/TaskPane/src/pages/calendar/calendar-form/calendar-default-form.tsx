import {
    BorderLineStyle,
    CalendarReportingTimeFrame,
    CalendarTemplateDetailsDTO,
} from '@omniflow/omni-webapi';
import moment from 'moment';

import { CalendarStyling, DefaultStyling } from '../../../business/engine/models/styles';

export const calendarTemplateDetails = (
    config: CalendarTemplateDetailsDTO
) => ({
    Id: config?.Id,
    Name: config?.Name,
    Definition: {
        Configuration: {
            CustomStartDate: moment
                .utc(
                    config?.Definition?.Configuration?.CustomStartDate ??
                    moment.utc().startOf('year')
                )
                .format('YYYY-MM-DD'),
            CustomEndDate: moment
                .utc(
                    config?.Definition?.Configuration?.CustomEndDate ??
                    moment.utc().endOf('year')
                )
                .format('YYYY-MM-DD'),
            Type: config?.Definition?.Configuration?.Type,
            Rows: config?.Definition?.Configuration?.Rows ?? [],
            IsReportingTimeFrame:
                config?.Definition?.Configuration?.IsReportingTimeFrame ?? true,
            ReportingTimeFrame:
                config?.Definition?.Configuration?.ReportingTimeFrame ??
                CalendarReportingTimeFrame.CurrentYear,
            IsNormalizedEnabled: config?.Definition?.Configuration?.IsNormalizedEnabled ?? false,
            UseCustomStartDay: config?.Definition?.Configuration?.UseCustomStartDay ?? false,
        },
        Styling: {
            ...DefaultStyling(), ...CalendarStyling()
        }
    },
    Version: config?.Version,
});
