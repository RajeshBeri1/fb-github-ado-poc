import { createComponent } from '@lit/react';
import React from 'react';
import { OmniIcon } from 'omni-ui/omni-icon.js';


export const Icon = createComponent({
    tagName: 'omni-icon',
    elementClass: OmniIcon,
    react: React,
});
