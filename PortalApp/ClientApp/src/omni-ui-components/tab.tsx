import { createComponent } from '@lit/react';
import React from 'react';
import { OmniTab } from 'omni-ui/omni-tab.js';

export const Tab = createComponent({
    tagName: 'omni-tab',
    elementClass: OmniTab,
    react: React,
    events: {
        onTabChange : 'tab-change',
    },
});
