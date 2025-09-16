import { createComponent } from '@lit-labs/react';
import React from 'react';
import { OmniDialog } from 'omni-ui/omni-dialog.js';

export const OmniModalElement = React.memo(createComponent({
    tagName: 'omni-modal',
    elementClass: OmniDialog,
    react: React
})
);