import { createComponent } from '@lit-labs/react';
import React from 'react';
import { OmniTooltip } from 'omni-ui/omni-tooltip.js';

export const Tooltip = React.memo(createComponent({
    tagName: 'omni-tooltip',
    elementClass: OmniTooltip,
    react: React
})
);