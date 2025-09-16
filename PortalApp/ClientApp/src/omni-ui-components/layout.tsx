import { createComponent } from '@lit/react';
import React from 'react';
import { OmniAppLayout } from 'omni-ui/omni-app-layout.js';
// example web component with React wrapper from @lit-labs/react

export type ILayoutProps = Record<string, never>;

export type ILayoutState = Record<string, never>;

//export class LayoutOmni extends OmniElement {
//    /**
//     *
//     */
//    constructor() {
//        super();
//    }

//    static get styles() {
//        return [
//            super.styles,
//            css`
//                :host {
//                    /* Customizable CSS Variables and their defaults */
//                    --omni-app-layout-header-height: 50px;
//                    --omni-app-layout-drawer-width: 300px;
//                    --omni-app-layout-drawer-closed-width: 75px;
//                    --omni-app-layout-end-drawer-width: 300px;
//                    --omni-app-layout-bg: #f1f5fa;
//                    --omni-app-layout-header-bg: #fff;
//                    --omni-app-layout-drawer-bg: var(--gradient-black);
//                    --omni-app-layout-end-drawer-bg: #fff;

//                    /* Variables useful for nesting layouts */
//                    --omni-app-layout-height: 100vh;
//                    --omni-app-layout-top: 0px;
//                    --omni-app-layout-left: 0px;
//                    --omni-app-layout-drawer-z-index: 32;
//                    --omni-app-layout-end-drawer-z-index: 34;
//                    --omni-app-layout-header-z-index: 36;
//                }
//            `,
//        ];
//    }
//    static get properties() {
//        return {
//            isAlertOpen: { type: Boolean },
//        };
//    }

//    updated(changedProps: any) {
//        if (changedProps.has('isAlertOpen')) {
//            // toggle document scrolling based on alert open status
//            if (this.isAlertOpen) {
//                this._clipDocumentElement();
//            } else {
//                this._unclipDocumentElement();
//            }
//        }
//    }
//    drawerOpen = true;
//    endDrawerOpen = 'endDrawerOpen';

//    test = () => (this.drawerOpen = true);
//    isAlertOpen = true;

//    render() {
//        return html`
//            <omni-style>
//                <omni-app-layout .drawerOpen=${this.drawerOpen} .endDrawerOpen=${this.endDrawerOpen}>
//                    <header slot="header">Header</header>
//                    <main>Main</main>
//                    <nav slot="drawer">Nav</nav>
//                    <aside slot="end-drawer">Aside</aside>
//                </omni-app-layout>
//            </omni-style>
//        `;
//    }
//}

//OmniElement.register('example-component', LayoutOmni);

export const Layout = createComponent({
    tagName: 'omni-app-layout',
    elementClass: OmniAppLayout,
    react: React,
    events: 
        {
        endDrawerOpen: 'end-drawer-open',
        drawerOpen: 'drawerOpen',
    }
    
});
