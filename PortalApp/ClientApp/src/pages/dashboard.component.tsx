/*import * as React from 'react';
import {
    OmniAppLayoutElement,
    OmniStyleElement,
    OmniElement,
    css,
    html,
} from 'omni-ui';*/

export type IDashboardProps = Record<string, never>;

export type IDashboardState = Record<string, never>;

//export default class Dashboard extends OmniElement {
//    constructor(props: IDashboardProps) {
//        super(props);

//        this.state = {};
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
