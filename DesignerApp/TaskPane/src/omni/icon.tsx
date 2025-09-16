import { createComponent } from '@lit-labs/react';
import React from 'react';
import { OmniIcon } from 'omni-ui/omni-icon.js';

export const Icon = React.memo(createComponent({
    tagName: 'omni-icon',
    elementClass: OmniIcon,
    react: React
})
);