import { CalendarOverlayTemplateDetailsDTO } from '@omniflow/omni-webapi';
import moment from 'moment';

const calendarOverlayTemplateDetails = (
    config: CalendarOverlayTemplateDetailsDTO
) => {
    return {
        Id: config?.Id,
        Name: config?.Name,
        Definition: {
            OverlaySections:
                config?.Definition?.OverlaySections.map((section) => ({
                    ...section,
                    Rows:
                        section?.Rows.map((row) => {
                            return {
                                ...row,
                                StartDate: moment
                                    .utc(row?.StartDate ?? moment.utc())
                                    .format('YYYY-MM-DD'),
                                EndDate: moment
                                    .utc(row?.EndDate ?? moment.utc())
                                    .format('YYYY-MM-DD'),
                            };
                        }) ?? [],
                })) ?? [],
        },
    };
};

export default calendarOverlayTemplateDetails;
