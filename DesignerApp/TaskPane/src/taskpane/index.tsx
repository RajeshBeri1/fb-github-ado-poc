import 'construct-style-sheets-polyfill';
import React, { JSX } from 'react';
import ReactDOM  from 'react-dom/client';
import { initializeIcons } from '@fluentui/font-icons-mdl2';
import { ThemeProvider } from '@fluentui/react';
import { Security } from '@okta/okta-react';
import { OktaAuth, toRelativeUrl } from '@okta/okta-auth-js';
import { BrowserRouter } from 'react-router-dom';
import moment from 'moment';
import App from './components/App';
import NotificationProvider from '../components/notification/NotificationProvider';
import AppContextProvider from './contexts/AppContext';
import { ErrorBoundary } from '../components/ErrorBoundary';
import { OKTA_AUTH_CONFIG, OKTA_AUTH_STATUS } from '../config/webapi.config';

// shim is required for class-transformer
import 'reflect-metadata';
import DataContextProvider from './contexts/DataContext';

// Set moment to english locale
moment.locale('en');

/* global document, Office, module, require */
declare global {
    interface Array<T> {
        group(
            cb: (element: T, index: number, array: T[]) => void
        ): Array<Array<T>>;
    }
}

initializeIcons();

let isOfficeInitialized = false;

const title = 'Omnicom Flowchart Builder';

const oktaAuth = new OktaAuth(OKTA_AUTH_CONFIG);


function restoreOriginalUri(oktaAuth: OktaAuth, originalUri: string) {
    window.location.replace(
        toRelativeUrl(originalUri || '/', window.location.origin)
    );
}

export const AppInner = ({
    Component,
}: {
    Component: React.ElementType;
}): JSX.Element => (
    <BrowserRouter>
        <ThemeProvider>
            <NotificationProvider>
                <ErrorBoundary>
                    <AppContextProvider
                        title={title}
                        isOfficeInitialized={isOfficeInitialized}>
                        <DataContextProvider>
                            <Component />
                        </DataContextProvider>
                    </AppContextProvider>
                </ErrorBoundary>
            </NotificationProvider>
        </ThemeProvider>
    </BrowserRouter>
);

const render = (Component: React.ComponentType<any>) => {
    const container = document.getElementById('container');
    if (!container) {
        throw new Error('Container element not found');
    }
    const root = ReactDOM.createRoot(container);
    root.render(
        <>
            {!OKTA_AUTH_STATUS.disabled ? (
                <Security
                    oktaAuth={oktaAuth}
                    restoreOriginalUri={restoreOriginalUri}>
                    <AppInner Component={Component} />
                </Security>
            ) : (
                <>
                    <AppInner Component={Component} />
                </>
            )}
        </>
    );
};

/* Render application after Office initializes */

Office.onReady(() => {
    isOfficeInitialized = true;

    window.confirm = (_m: string): boolean => false; // assures confirm works

    render(App);
});

if ((module as any).hot) {
    (module as any).hot.accept('./components/App', () => {
        const NextApp = require('./components/App').default;
        render(NextApp);
    });
}