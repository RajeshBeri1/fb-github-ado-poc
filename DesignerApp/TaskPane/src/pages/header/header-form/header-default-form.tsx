import {
    BorderLineStyle,
    HeaderTemplateDetailsDTO,
} from '@omniflow/omni-webapi';
import moment from 'moment';

import { DefaultStyling } from '../../../business/engine/models/styles';

export const headerTemplateDetails = (config: HeaderTemplateDetailsDTO) => ({
    Id: config?.Id,
    Name: config?.Name,
    Definition: {
        Configuration: {
            ColumnMargin: config?.Definition?.Configuration?.ColumnMargin,
            RowMargin: config?.Definition?.Configuration?.RowMargin,
            RowMerge: config?.Definition?.Configuration?.RowMerge,
            Rows:
                config?.Definition?.Configuration?.Rows.map((row) => ({
                    ...row,
                    Date: moment
                        .utc(row?.Date ?? moment.utc())
                        .format('YYYY-MM-DD'),
                })) ?? [],
            Logos: config?.Definition?.Configuration?.Logos ?? [],
            Details: {
                Rows: config?.Definition?.Configuration?.Details?.Rows ?? [],
            },
        },
        Styling: DefaultStyling(),
    },
    Version: config?.Version,
});
