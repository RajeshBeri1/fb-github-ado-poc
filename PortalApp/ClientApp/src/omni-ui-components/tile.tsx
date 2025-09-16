import { createComponent } from '@lit/react';
import React from 'react';
import { OmniTile } from 'omni-ui/omni-tile.js';

export const Tile = createComponent({
    tagName: 'omni-tile',
    elementClass: OmniTile,
    react: React,
});
