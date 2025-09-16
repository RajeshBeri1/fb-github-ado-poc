import React, { useState, useContext, useEffect, Suspense } from 'react';
import {
    useResolvedPath,
    useNavigate,
    useParams,
    Outlet
} from 'react-router-dom';

import TemplateConfigNavigation from '../../components/template-config-navigation/template-config-navigation';
import { Page } from '../../enums/page.enum';
import { PageTest } from '../../enums/page-test.enum';
import { Tile } from '../../omni/tile';
import { Toolbar } from '../../omni/toolbar';
import CalendarOverlay from '../../pages/calendar-overlay';
import Theme from '../../pages/theme';
import MediaHierarchy from '../../pages/media-hierarchy';
import GrandTotals from '../../pages/grand-totals';
import RightHandTotals from '../../pages/right-hand-totals';
import { RouteOutlet } from '../../business/router-outlet';
import { SecretTestMode } from './secret-test-mode';
import { UrlAndQueryParamKey } from '../../enums/url-and-query-param-key.enum';
import { Icon } from '../../omni/icon';
import Link from '../../components/Link';
import Header from '../../pages/header';
import Calendar from '../../pages/calendar';
import Footer from '../../pages/footer';
import RunXPView from '../../runXP/components/runxp-view';
import { AppContext } from '../contexts/AppContext';
import { TESTMODE_ENABLED } from '../../config/webapi.config';
import { clientApi } from '../../lib/api';
import Loader from '../../components/loader';
import Spinner from '../../components/spinner/Spinner';


const Dashboard = ({ authUser, mode }) => {
    const { flowchartTemplateDefinition, clientName, setOnRun, onRun } = useContext(AppContext);
    let resolvedPath = useResolvedPath('');
    let path  = resolvedPath.pathname;
    let navigate = useNavigate();

    useEffect(() => {
        // TODO #936 troubleshoot why navigation to run experience is not working reliably from "launch" action in portal page
        setOnRun(mode);
        navigate(!mode ? `${path}/${Page.Calendar}` : `${path}/runxp`);
    }, [mode]);

    const modeHandler = () => {
        setOnRun(!onRun);
    };
    const params = useParams();
  
    return (
        <Tile>
            {!onRun ? (
                <>
                    <Loader />
                    <Toolbar slot="header" className="toolbar-header">
                        <div slot="start" className="is-flex is-align-items-center">
                            < h3 className={clientName != '' ? 'title is-4 mb-0' : 'table skeleton'} >
                                {clientName != '' ? clientName : 'Loading'}:
                            </h3>
                            <h3 className="title is-4 ml-1">
                                Component configuration
                            </h3>
                        </div>
                       
                        <div className="toolbar-divider"></div>
                        <TemplateConfigNavigation />
                        <div className="toolbar-divider"></div>
                        {/*<button>*/}
                        {/*    <Icon icon-id="omni:interactive:close"></Icon>*/}
                        {/*</button>*/}

                        {/*hidding publish button as we need the access to run configuration only from launch FB-210 & FB-225*/}

                        {/* {mode && flowchartTemplateDefinition.Version > 0 && <Link to={`${path}/runxp`} onClick={modeHandler}>
                        PUBLISH
                    </Link>}*/}
                        {<Link to={`${path}/runxp`} onClick={modeHandler} className={flowchartTemplateDefinition.Version > 0 ? "" : "disabled"} >
                            Run Configuration
                        </Link>}
                        {/*TESTMODE_ENABLED && (
                        <Link to={`${path}/${PageTest.SecretTestMode}`}>
                            TEST
                        </Link>
                    )*/}
                    </Toolbar>
                </>
            ) : (
                <>
                    <Loader />
                    <Toolbar slot="header" className="toolbar-header">
                        < h3 slot="start" className="title is-2">
                            {clientName}:
                        </h3>
                        <h3 slot="start" className="title is-4">
                            Run Configuration
                        </h3>
                        <div className="toolbar-divider"></div>
                        {/*removing edit button FB-225*/}
                        {/* {authUser ===
                        flowchartTemplateDefinition?.CreatedByUser
                            ?.DisplayName && (*/
                            <Link
                                to={`${path}/${Page.Calendar}`}
                                onClick={modeHandler}>
                                Edit Template
                            </Link>
                   /* )}*/}
                    </Toolbar>
                </>
            )}

            <Outlet/>
        </Tile>
    );
};

export default Dashboard;
