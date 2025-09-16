import { createComponent } from '@lit-labs/react';
import React from 'react';
import { OmniSwitch } from 'omni-ui/omni-switch.js';


export const Switch = React.memo(createComponent({
    tagName: 'omni-switch',
    elementClass: OmniSwitch,
    react: React,
    events: {
        onChange
            : 'change',
    }
})
);