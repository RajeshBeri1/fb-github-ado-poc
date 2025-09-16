import { createComponent } from '@lit/react';
import React from 'react';
import { OmniToolbar } from 'omni-ui/omni-toolbar.js';

export const Toolbar = createComponent({
    tagName: 'omni-toolbar',
    elementClass: OmniToolbar,
    react: React,
});
