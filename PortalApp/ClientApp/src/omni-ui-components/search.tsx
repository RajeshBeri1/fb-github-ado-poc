// example web component with React wrapper from @lit-labs/react
import { createComponent } from '@lit/react';
import React from 'react';
import { OmniSearch } from 'omni-ui/omni-search.js';


export const SearchInput = createComponent({
    tagName: 'omni-search',
    elementClass: OmniSearch,
    react: React,
    events: {
        onSearchUpdate: 'search-update',
    },
});