import { createComponent } from '@lit/react';
import React from 'react';
import { OmniAccordion } from 'omni-ui/omni-accordion.js';

export const Accordion = createComponent({
    tagName: 'omni-accordion',
    elementClass: OmniAccordion,
    react: React,
});
