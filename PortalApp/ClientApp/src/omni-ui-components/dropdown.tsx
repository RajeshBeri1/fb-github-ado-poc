import { createComponent } from '@lit/react';
import React from 'react';
import { OmniDropdown } from 'omni-ui/omni-dropdown.js';


export const DropDownInput = React.memo(createComponent({
    tagName: 'omni-dropdown',
    elementClass: OmniDropdown,
    react: React,
    events: {
        onValueChange: 'change',
    },
})
);