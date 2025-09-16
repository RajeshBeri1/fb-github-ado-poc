import { createComponent } from '@lit-labs/react';
import React from 'react';
import { OmniImageInput } from 'omni-ui/omni-img-input.js';

export const ImageUpload = React.memo(createComponent({
    tagName: 'omni-img-input',
    elementClass: OmniImageInput,
    react: React,
    events: {
        onImageChange: 'change',
    },
})
);