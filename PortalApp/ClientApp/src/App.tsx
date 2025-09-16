import './App.scss';
import React, { useCallback, useEffect, lazy, Suspense, useMemo } from 'react';
import { useState } from 'react';
import axios from 'axios';

import { LoginCallback } from '@okta/okta-react';
import { Route, Routes, useLocation } from 'react-router';
import { Layout } from './omni-ui-components/layout';
import NotificationProvider from './components/notification/NotificationProvider';
// import {ErrorBoundary} from './components/ErrorBoundary';
import TemplateRecentList from './components/template-recent-list/template-recent-list';
import { UrlAndQueryParamKey } from './enums/url-and-query-param-key.enum';
import { useAuth } from './components/omni-auth-provider/use-auth';
import { OMNI_AUTH_CONFIG, WEBAPI_CONFIGURATION } from './config/webapi.config';
import { Icon } from './omni-ui-components/icon';
import { NotificationType } from './components/notification/useNotification';
import { ClientApi } from '@omniflow/omni-webapi';
import RequestInterceptor from '../../ClientApp/src/business/request-interceptor';
import { enqueueSnackbar } from 'notistack';
import Spinner, { SpinnerType } from './omni-ui-components/spinner/Spinner';
import * as signalR from '@microsoft/signalr';
import { connect } from 'tls';
import { Tooltip } from './omni-ui-components/tooltip';
declare global {
    interface Window {
        dataLayer: any[];
    }
}

function App() {
    // Fallback client id for development.
    // const {authState, oktaAuth} = useOktaAuth();
    const RunConfig = lazy(() => import('./pages/runconfig/run-config'));
    const [clientId, setClientId] = useState<string | null | undefined>();
    const [drawerOpen] = useState<any>();
    const [endDrawerOpen] = useState<any>();
    const [currentUserId, setCurrentUserId] = useState<
        string | null | undefined
    >();
    const [currentVersion, setCurrentVersion] = useState<number>(2); // Default to version 2
    const [showDropdown, setShowDropdown] = useState<boolean>(false);
    const auth = useAuth();
    const search = useLocation().search;
    //const searchParams = new URLSearchParams(search);
    const locationSearch = useLocation().search;
    const searchParams = useMemo(() => {
        return new URLSearchParams(locationSearch);
    }, [locationSearch]);
    const [isVersionActionInProgress, setIsVersionActionInProgress] =
        useState<boolean>(false);
    const redirectToLoginPage = () => {
        const win: Window = window;
        win.location = OMNI_AUTH_CONFIG.issuer;
    };
    const [authenticated, setAuthenticated] = useState<boolean>(false);
    const [connection, setConnection] = useState<any>(null);
    const [connectionId, setConnectionId] = useState<any>(null);
    React.useEffect(() => {
        if (authenticated) {
            const newConnection = new signalR.HubConnectionBuilder()
                .withUrl(`${WEBAPI_CONFIGURATION.basePath}/notificationHub`)
                .withAutomaticReconnect()
                .build();
            setConnection(newConnection);
        }
    }, [authenticated]);

    React.useEffect(() => {
        if (connection) {
            connection
                .start()
                .then(() => {
                    connection.invoke('GetConnectionId').then((id: string) => {
                        console.log(`ConnectionId is : ${id}`);
                        setConnectionId(id);
                    });
                    connection.on(
                        'ReceiveNotification',
                        (
                            message: string,
                            isError: boolean,
                            identifier: string
                        ) => {
                            if (identifier == currentUserId) {
                                enqueueSnackbar(message, {
                                    variant: isError
                                        ? NotificationType.DANGER
                                        : NotificationType.SUCCESS,
                                });
                            }
                        }
                    );
                })
                .catch((err: any) => {
                    console.log('SignalR Connection Error: ', err);
                });
        }
    }, [connection]);

    React.useEffect(() => {
        (async () => {
            const queryString = window.location.search;
            const urlParams = new URLSearchParams(queryString);
            const ANsid: any = urlParams.get(UrlAndQueryParamKey.ANSID);
            if (ANsid || sessionStorage.getItem(UrlAndQueryParamKey.ANSID)) {
                // check for ANsid
                let res: any;
                try {
                    res = await axios.get(
                        `${
                            WEBAPI_CONFIGURATION.basePath
                        }/api/User/ValidateToken/${
                            ANsid ||
                            sessionStorage.getItem(UrlAndQueryParamKey.ANSID)
                        }`
                    ); // validate ANsid
                } catch (error) {
                    console.log('unable to validate the ANsid', error);
                    redirectToLoginPage();
                }
                if (res && res.data && res.data.IsAuthorized) {
                    sessionStorage.setItem(UrlAndQueryParamKey.ANSID, ANsid);
                    auth.isAuthenticated = true;
                    setAuthenticated(true);
                    setCurrentUserId(res.data.PersonId);
                    window.dataLayer.push({
                        event: 'loginIdTrackingPortalApp',
                        userId: res.data.personId,
                    });

                    const urlClientId = searchParams.get('clientid');
                    if (urlClientId) {
                        setClientId(urlClientId);
                        console.log('Client ID set from URL:', urlClientId);
                    } else {
                        console.error('No client ID found in URL parameters');
                    }
                } else {
                    console.log(
                        'user is not authorized, check validation api call'
                    );
                    redirectToLoginPage();
                }
            } else {
                console.log('no ANsid');
                console.table(window.location);
                redirectToLoginPage();
            }
        })();
        setClientId(searchParams.get('clientid'));
    }, [searchParams]);

    const clientApi = new ClientApi(
        WEBAPI_CONFIGURATION,
        '',
        RequestInterceptor
    );

    useEffect(() => {
        if (clientId) {
            fetchCurrentVersion(clientId);
            fetchCurrentUserid(
                sessionStorage.getItem(UrlAndQueryParamKey.ANSID) || ''
            );
        }
    }, [clientId]);

    const fetchCurrentUserid = async (ANsid: string) => {
        try {
            const userData = await axios.get(
                `${WEBAPI_CONFIGURATION.basePath}/api/User/ValidateToken/${
                    ANsid || sessionStorage.getItem(UrlAndQueryParamKey.ANSID)
                }`
            ); // validate ANsid
            if (userData && userData.data && userData.data.IsAuthorized) {
                setCurrentUserId(userData.data.PersonId);
                setAuthenticated(true);
            }
        } catch (error) {
            console.log('unable to validate the ANsid', error);
            redirectToLoginPage();
        }
    };

    const fetchCurrentVersion = async (clientId: string) => {
        try {
            const { data } = await clientApi.clientGet(clientId);

            // Access the version from the response DTO
            setCurrentVersion(data.Version || 1); // Default to 1 if Version is null
        } catch (e) {
            console.error('Error fetching current version:', e);
            setCurrentVersion(2);
        }
    };

    const toggleVersion = useCallback(async () => {
        if (!clientId) {
            console.error('Client ID is not available');
            return;
        }
        if (!currentUserId) {
            console.error('CurrentUserId is not available');
            return;
        }

        try {
            setIsVersionActionInProgress(true);
            await fetchCurrentVersion(clientId);
            await fetchCurrentUserid(
                sessionStorage.getItem(UrlAndQueryParamKey.ANSID) || ''
            );
            const newVersion = currentVersion === 2 ? 1 : 2;
            const endpoint =
                newVersion === 2
                    ? 'MigrateClientToNewVersion'
                    : 'MigrateClientToInitialVersion';

            const response = await axios.post(
                `${WEBAPI_CONFIGURATION.basePath}/api/Migration/${endpoint}`,
                null,
                {
                    params: {
                        omniClientId: clientId,
                        connectionId: currentUserId,
                    },
                }
            );
            setIsVersionActionInProgress(false);

            enqueueSnackbar(
                'Migration has been started, will notify once done',
                {
                    variant: NotificationType.SUCCESS,
                }
            );
        } catch (error) {
            setIsVersionActionInProgress(false);
            if (currentVersion === 1) {
                enqueueSnackbar(
                    'Update unsuccessful as your flowchart templates are not yet migrated to version 2',
                    { variant: NotificationType.DANGER }
                );
            } else {
                enqueueSnackbar('Error while changing the version', {
                    variant: NotificationType.DANGER,
                });
            }
        }

        setShowDropdown(false);
        await fetchCurrentVersion(clientId);
        fetchCurrentUserid(
            sessionStorage.getItem(UrlAndQueryParamKey.ANSID) || ''
        );
    }, [clientId, currentVersion, currentUserId]);

    return (
        <Routes>
            <Route path="/" element={
                <NotificationProvider>
                {/* <ErrorBoundary> */}
                    <div className="App">
                        <Layout
                            drawerOpen={drawerOpen}
                            endDrawerOpen={endDrawerOpen}
                        >
                            <header slot="header" className="fb-header is-flex is-justify-content-space-between is-align-items-center">
                            <h2>Flowchart Templates</h2>
                            {authenticated && clientId && currentUserId && (
                                <div
                                    id="versionActions"
                                    className={`dropdown dropdownVersionActions  mr-5 ${
                                        showDropdown ? 'is-active' : ''
                                    }`}>
                                    <div className="dropdown-trigger">
                                        <Tooltip>
                                            <button
                                                className="tertiary flex gap-1"
                                                aria-haspopup="true"
                                                aria-controls="dropdown-menu"
                                                onClick={() =>
                                                    setShowDropdown(
                                                        !showDropdown
                                                    )
                                                }>
                                                    <Icon icon-id="omni:informative:settings" />
                                            </button>
                                            <div slot="content"> Settings </div>
                                        </Tooltip>
                                    </div>
                                    <div
                                        slot="dropdown-menu"
                                        className={`dropdown-menu ${
                                            showDropdown
                                                ? 'is-block fliter-menu-position'
                                                : ''
                                        }`}
                                        id="dropdown-menu"
                                        role="menu">
                                        <div
                                            slot="dropdown-content"
                                            className="dropdown-content">
                                            <a
                                                className="dropdown-item is-flex is-align-items-center"
                                                onClick={toggleVersion}>
                                                    <div className="w-5">
                                                        <Icon
                                                        className="fill-color"
                                                        icon-id="omni:informative:check"
                                                    />
                                                </div>
                                                <span className="is-size-6 p-1">
                                                    Change to v
                                                    {currentVersion === 2
                                                        ? '1'
                                                        : '2'}
                                                </span>
                                            </a>
                                        </div>
                                    </div>
                                </div>
                            )}
                        </header>
                        <main>
                            <div className="mt-4  mx-4">
                                {authenticated && clientId && (
                                    <TemplateRecentList
                                        ClientId={clientId}
                                        NumberOfItemsToLoad={20}
                                        LoggedInUserId={currentUserId}
                                    />
                                )}
                            </div>
                            {isVersionActionInProgress && (
                                <Spinner type={SpinnerType.STANDARD} />
                            )}
                            </main>
                        </Layout>
                </div>
                {/* </ErrorBoundary> */}
            </NotificationProvider>
            } />
            <Route path="/run-config" element={
                <Suspense fallback={<Spinner type={SpinnerType.STANDARD} />}>
                    <RunConfig />
                </Suspense>
            } />
            <Route path="/login/callback" Component={LoginCallback} />
        </Routes>
    );
}

export default App;
