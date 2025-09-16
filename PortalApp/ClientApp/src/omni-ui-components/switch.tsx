import * as React from 'react';
import { OmniSwitch } from 'omni-ui/omni-switch.js';

// example web component with React wrapper from @lit-labs/react
import { createComponent } from '@lit/react';

export const Switch = createComponent({
    tagName: 'omni-switch',
    elementClass: OmniSwitch,
    react: React,
    events: {
        onChange: 'change'
    },
});