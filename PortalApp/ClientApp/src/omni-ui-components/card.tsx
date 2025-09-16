import { createComponent } from '@lit/react';
import React from 'react';
import { OmniCard } from 'omni-ui/omni-card.js';


export const Card = createComponent({
    tagName: 'omni-card',
    elementClass: OmniCard,
    react: React,
});