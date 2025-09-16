import { FooterRowMerge, FooterTemplateUpdateDTO } from '@omniflow/omni-webapi';

import { DefaultStyling } from '../../../business/engine/models/styles';

export const defaultFooterDetails = (
    config: FooterTemplateUpdateDTO | undefined
) => ({
    Id: config?.Id,
    Name: config?.Name,
    Definition: {
        Configuration: {
            ColumnMargin: config?.Definition?.Configuration?.ColumnMargin,
            EnableLegend:
                config?.Definition?.Configuration?.EnableLegend ?? false,
            LegendColumnCount:
                config?.Definition?.Configuration?.LegendColumnCount,
            RowMargin: config?.Definition?.Configuration?.RowMargin,
            RowMerge:
                config?.Definition?.Configuration?.RowMerge ??
                FooterRowMerge.MergeAndCenter,
            Rows: config?.Definition?.Configuration?.Rows ?? [],
        },
        Styling: DefaultStyling(),
    },
});
