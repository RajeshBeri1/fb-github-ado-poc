import { createComponent } from '@lit-labs/react';
import React from 'react';
import { OmniSearch } from 'omni-ui/omni-search.js';

export const SearchInput = React.memo(createComponent({
    tagName: 'omni-search',
    elementClass: OmniSearch,
    react: React,
    events: {
        onChange: 'change',
    },
})
);