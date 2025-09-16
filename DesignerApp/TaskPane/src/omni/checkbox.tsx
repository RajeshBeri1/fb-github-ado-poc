import { createComponent } from '@lit-labs/react';
import React from 'react';
import { OmniCheckbox } from 'omni-ui/omni-checkbox.js';

export const OmniCheckBoxInput = React.memo(createComponent({
    tagName: 'omni-checkbox',
    elementClass: OmniCheckbox,
    react: React,
    events: {
        onChange: 'change',
    },
})
);
