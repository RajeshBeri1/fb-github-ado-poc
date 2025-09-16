import { createComponent } from '@lit/react';
import React from 'react';
import { OmniTable } from 'omni-ui/omni-table.js';


export const Table = createComponent({
    tagName: 'omni-table',
    elementClass: OmniTable,
    react: React,
});
