import { createComponent } from '@lit-labs/react';
import React from 'react';
import { OmniDropdown } from 'omni-ui/omni-dropdown.js';

export const OmniDropDownInput = React.memo(createComponent({
    tagName: 'omni-dropdown',
    elementClass: OmniDropdown,
    react: React,
    events: {
        onValueChange: 'change',
    },
})
);
