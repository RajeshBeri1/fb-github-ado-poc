import { createComponent } from '@lit-labs/react';
import React from 'react';
import { OmniTextarea } from 'omni-ui/omni-textarea.js';

export const OmniTextareaInput = React.memo(createComponent({
    tagName: 'omni-textarea',
    elementClass: OmniTextarea,
    react: React,
    events: {
        onChange: 'change',
    },
})
);