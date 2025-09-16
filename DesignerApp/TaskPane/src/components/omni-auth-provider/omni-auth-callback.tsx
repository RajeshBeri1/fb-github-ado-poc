import React, { useContext, useEffect } from 'react';
import axios from 'axios';
import { OMNI_AUTH_CONFIG, WEBAPI_CONFIGURATION } from '../../config/webapi.config';
import { UrlAndQueryParamKey } from '../../enums/url-and-query-param-key.enum';
import App from '../../taskpane/components/App';
import { useAuth } from './use-auth';
import { AppContext } from '../../taskpane/contexts/AppContext';

export const OmniAuthCallback = ({ ...rest }) => {
    let auth = useAuth();

    const [intialState, updateState] = React.useState('');

    const redirectToLoginPage = () => {
        const win: Window = window;
        win.location = OMNI_AUTH_CONFIG.issuer;
    }
    const appContext = useContext(AppContext);


    useEffect(() => {

        (async () => {
            const queryString = window.location.search;
            const urlParams = new URLSearchParams(queryString);
            const ANsid = urlParams.get(UrlAndQueryParamKey.ANSID);

            // check for ANsid
            if (ANsid || sessionStorage.getItem(UrlAndQueryParamKey.ANSID)) {
                // validate ANsid

                const res: any = await axios.get(`${WEBAPI_CONFIGURATION.basePath}/api/User/ValidateToken/${ANsid || sessionStorage.getItem(UrlAndQueryParamKey.ANSID)}`);

                if (res && res.data && res.data.IsAuthorized) {
                    sessionStorage.setItem(UrlAndQueryParamKey.ANSID, ANsid);
                    auth.isAuthenticated = true;
                    updateState('test');
                    appContext.setIsUserAuthenticated(true);
                } else {
                    redirectToLoginPage();
                    console.log('user is not authorized, check validation api call');
                }

            } else { // redirect to login page if ANsid is not available
                console.log('no ANsid');
                console.table(window.location);
                redirectToLoginPage();
            }
        })();

    }, [])

    return (
        <>
            {auth.isAuthenticated && <App />}
        </>
    );
};

export default OmniAuthCallback
