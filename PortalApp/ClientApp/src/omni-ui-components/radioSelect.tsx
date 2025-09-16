import * as React from 'react';
import { OmniRadio } from 'omni-ui/omni-radio.js';
// example web component with React wrapper from @lit-labs/react
import { createComponent } from '@lit/react';

export const Radio= createComponent({
    tagName: 'omni-radio',
    elementClass: OmniRadio,
    react: React,
    events: {
        onChange: 'change',
    },
});