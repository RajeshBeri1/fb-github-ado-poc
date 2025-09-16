import React, { JSX, useContext, useState } from 'react';
import { Routes, Route, Navigate, useParams, useResolvedPath } from 'react-router-dom';
import { LoginCallback, SecureRoute, useOktaAuth } from '@okta/okta-react';

import Progress from './Progress';
import { AppContext } from '../contexts/AppContext';
import Dashboard from './Dashboard';
import './App.css';
import { StorageKey } from '../../enums/storage-key.enum';
import { OKTA_AUTH_STATUS } from '../../config/webapi.config';
import OmniAuthCallback from '../../components/omni-auth-provider/omni-auth-callback';
import OmniAuthProvider from '../../components/omni-auth-provider/omni-auth-provider';
import { useAuth } from '../../components/omni-auth-provider/use-auth';
import { CustomPropertyKey } from '../../enums/custom-property-key.enum';
import { CustomPropertyService } from '../../business/custom-property-service';
import ModalWrapper from '../../components/modal/modal';
import { OMNI_AUTH_CONFIG } from '../../config/webapi.config';
import axios from 'axios';
import Spinner from '../../components/spinner/Spinner';
import { UrlAndQueryParamKey } from '../../enums/url-and-query-param-key.enum';

import Header from '../../pages/header';
import Calendar from '../../pages/calendar';
import Footer from '../../pages/footer';
import RunXPView from '../../runXP/components/runxp-view';
import CalendarOverlay from '../../pages/calendar-overlay';
import Theme from '../../pages/theme';
import MediaHierarchy from '../../pages/media-hierarchy';
import GrandTotals from '../../pages/grand-totals';
import RightHandTotals from '../../pages/right-hand-totals';
import { SecretTestMode } from './secret-test-mode';
import { PageTest } from '../../enums/page-test.enum';
import { Page } from '../../enums/page.enum';

const App = (): JSX.Element => {
    //const [mode, setMode] = useState<boolean | null>(null);
    let authState, oktaAuth;
    if (!OKTA_AUTH_STATUS.disabled) {
        ({ authState, oktaAuth } = useOktaAuth());
    }
    const { mode, title, isOfficeInitialized, flowchartTemplateId, user, isSessionExpired, setSessionExpired, isFlowchartLoading } =
        useContext(AppContext);

    const auth = useAuth();
    const clearSessionStorage = () => {
        sessionStorage.clear();
    };

    const redirectToLoginPage = () => {
        clearSessionStorage();
        const win: Window = window;
        win.location = OMNI_AUTH_CONFIG.issuer;
    }

    const routes = [
        {
            path: `${Page.Header}`,
            component: Header,
        },
        {
            path: `${Page.Calendar}`,
            component: Calendar,
        },
        {
            path: `${Page.CalendarOverlay.replace(' ', '')}`,
            component: CalendarOverlay,
        },
        {
            path: `${Page.MediaHierarchy}`,
            component: MediaHierarchy,
        },
        {
            path: `${Page.GrandTotals}`,
            component: GrandTotals,
        },
        {
            path: `${Page.RightHandTotals}`,
            component: RightHandTotals,
        },
        {
            path: `${Page.Theme}`,
            component: Theme,
        },
        {
            path: `${Page.Footer}`,
            component: Footer,
        },
        //{
        //    path: `/${Page.Dashboard}/:${UrlAndQueryParamKey.FLOWCHART_TEMPLATE_ID}/${PageTest.SecretTestMode}`,
        //    component: SecretTestMode,
        //},
        {
            path: `runxp`,
            component: RunXPView,
        },
    ];

    if (!isOfficeInitialized) {
        return (
            <Progress
                title={title}
                logo={require('./../../../assets/logo-filled.png')}
                message="Please side-load your add-in to see app body."
            />
        );
    }


    React.useEffect(() => {
        if (!OKTA_AUTH_STATUS.disabled) {
            if (authState && !authState.isAuthenticated) {
                oktaAuth.signInWithRedirect();
                localStorage.removeItem(StorageKey.BearerToken);
            } else if (authState && authState.isAuthenticated) {
                localStorage.setItem(
                    StorageKey.BearerToken,
                    `Bearer ${oktaAuth.getAccessToken() || ''}`
                );
            }
        }
    }, [authState]);

    axios.interceptors.response.use(
        (response) => {
            return response;
        },
        (error) => {

            if (error.response && error.response.status === 401) {
                setSessionExpired(true);
            }
            return Promise.reject(error);
        }
    );
    const params = useParams();
    let resolvedPath = useResolvedPath('');
    let path = resolvedPath.pathname;
    return (
        <>
            <Routes>
                {!auth.isAuthenticated && (
                    <Route path="/login/callback" element={<OmniAuthCallback /> } />
                        
                )}

                {!OKTA_AUTH_STATUS.disabled ? (
                    <SecureRoute path={`/dashboard/${flowchartTemplateId}/*`} element={ <div className="app-container">
                            <Dashboard
                                authUser={
                                    user?.DisplayName
                                }
                                mode={mode}
                            />
                    </div>}>

                        <Route
                            path={`${Page.Calendar}`}

                            element={ <div>Sweta</div>}
                            />
                    </SecureRoute>
                ) : (
                        <Route path={`/dashboard/:flowchartTemplateId/*`} element={<div className="app-container">
                            <Dashboard
                                authUser={
                                    user?.DisplayName
                                }
                                mode={mode}
                            />
                        </div>

                        }>
                            {routes.map((route, i) => (
                                <Route
                                    path={`${route.path}/*`}
                                    element={
                                        // Pass the sub-routes down to keep nesting
                                        <route.component />
                                    }
                                />

                            ))}
                    </Route>
                )}

                {auth && auth.isAuthenticated ? (
                    <Route path="*" element={flowchartTemplateId ? (
                        <Navigate to={`/dashboard/${flowchartTemplateId}`} replace />
                    ) : isFlowchartLoading ?
                        (<Spinner />) : (
                            <div>
                                <h3>The flowchart template could not be loaded!</h3>
                            </div>
                        )}>
                       
                    </Route>
                ) : (
                    <Route path="*" element={<OmniAuthProvider /> } />
                   
                )}
                {authState && authState.isAuthenticated ? (
                    <Route path="*" element={flowchartTemplateId ? (
                        <Navigate to={`/dashboard/${flowchartTemplateId}`} replace />
                    ) : isFlowchartLoading ?
                        (<Spinner />) : (
                            <div>
                                <h3>The flowchart template could not be loaded!</h3>
                            </div>
                        )}>
                        
                    </Route>
                ) : (
                        <Route path="*" element={
                            <div>
                                <h3>Authentication in process</h3>
                            </div>
                        }>
                        
                    </Route>
                )}
            </Routes>
            {isSessionExpired &&
                <div id="sessionModal">
                    <ModalWrapper
                        header={<div>Session Expired.</div>}
                        controls={false}
                        body={
                            <>
                                <h4>Your session has expired, click on login to continue.</h4>

                                <div className="d-flex flex-column col-5 mt-4 mx-auto">
                                    <button className="button is-primary" onClick={redirectToLoginPage}>
                                        Login
                                    </button>
                                </div>
                            </>

                        }
                    />
                </div>

            }
        </>

    );
};

export default App;
