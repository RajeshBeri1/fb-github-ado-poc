import { createComponent } from '@lit/react';
import React from 'react';
import { OmniTooltip } from 'omni-ui/omni-tooltip.js';

export const Tooltip = createComponent({
    tagName: 'omni-tooltip',
    elementClass: OmniTooltip,
    react: React,
});
