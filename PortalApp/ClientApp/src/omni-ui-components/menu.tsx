import { createComponent } from '@lit/react';
import React from 'react';
import { OmniMenu } from 'omni-ui/omni-menu.js';

export const Menu = createComponent({
    tagName: 'omni-menu',
    elementClass: OmniMenu,
    react: React,
});
