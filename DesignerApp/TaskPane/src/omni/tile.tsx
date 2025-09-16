import { createComponent } from '@lit-labs/react';
import React from 'react';
import { OmniTile } from 'omni-ui/omni-tile.js';

export const Tile = React.memo(createComponent({
    tagName: 'omni-tile',
    elementClass: OmniTile,
    react: React
})
);