import { createComponent } from '@lit-labs/react';
import React from 'react';
import { OmniToolbar } from 'omni-ui/omni-toolbar.js';


export const Toolbar = React.memo(createComponent({
    tagName: 'omni-toolbar',
    elementClass: OmniToolbar,
    react: React
})
);