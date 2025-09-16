import React, {
    createContext,
    useCallback,
    useEffect,
    useState,
    PropsWithChildren,
    useRef,
    JSX,
} from 'react';

import * as API from '@omniflow/omni-webapi';
import { DuplicateNameCheckDto, TaskpaneMode, ThemeTemplateSearchDTO, MediaopsColumnSearchDTO } from '@omniflow/omni-webapi';
import { ColumnType } from '@omniflow/omni-webapi';
import { isEmpty } from 'lodash';
import { CustomPropertyService } from '../../business/custom-property-service';
import { CustomPropertyKey } from '../../enums/custom-property-key.enum';
import {
    clientApi,
    commonApi,
    dataDictionaryApi,
    fieldApi,
    flowchartTemplateApi,
    mediaHierarchyTemplateApi,
    themeTemplateApi,
    userApi,
    mediaopsToFlowChartApi
} from '../../lib/api';
import { CustomProperty } from '../../models/custom-property';
import getColumnsFromDataDictionary, {
    TColumn,
} from '../../lib/utils/getColumnsFromDataDictonary';
import useNotification, {
    NotificationType,
} from '../../components/notification/useNotification';

import { useAuth } from '../../components/omni-auth-provider/use-auth';

import {
    runConfigurationApi
} from '../../lib/api';
import { plainToClass } from 'class-transformer';

import metricFieldsFilteringListInfo from '../../../assets/datadictionary/metricFieldsFilteringListInfo.json'
import stringFieldsFilteringListInfo from '../../../assets/datadictionary/stringFieldsFilteringListInfo.json'
import { OMNI_AUTH_CONFIG, WEBAPI_CONFIGURATION } from '../../config/webapi.config';
import { UrlAndQueryParamKey } from '../../enums/url-and-query-param-key.enum';
import axios from '../../business/request-interceptor';
import { DefaultMetric, DefaultMetricMediaBrief, DefaultMetrics, UsdMetrics } from '../../business/engine/constant/metric';
import { FieldInfo } from '../../models/fieldinfo';
import getThemeTemplateDetails from '../../pages/theme/components/theme-form/theme-default';
import { set } from 'react-hook-form';
import { Definition } from '../../models/definition';
declare global {
    interface Window { dataLayer: any[]; }
}

export const inflightOverlayMetrics: string[] = [
    'impressions',
    'budgetedimpressions',
    'impressionsactual'
];


export type TAppContextValue = {
    title: string;
    isOfficeInitialized: boolean;
    isInSecretTestMode: boolean;
    enableSecretTestMode: () => void;
    disableSecretTestMode: () => void;
    user: API.UserDetailsDTO;
    runConfigurationId: string;
    setRunConfigurationId: React.Dispatch<React.SetStateAction<string>>;
    flowchartTemplateId: string;
    flowchartTemplateDefinition: API.FlowchartTemplateDetailsDTO;
    setFlowchartTemplateDefinition: React.Dispatch<
        React.SetStateAction<API.FlowchartTemplateDetailsDTO>
    >;
    flowchartRunConfiguration: API.RunConfigurationDetailsDTO;
    columns: TColumn[];
    columnsLoaded: boolean,
    mode: boolean,
    currentHeaderTemplateDetails: API.HeaderTemplateDetailsDTO;
    setCurrentHeaderTemplateDetails: React.Dispatch<
        React.SetStateAction<API.HeaderTemplateDetailsDTO>
    >;
    currentCalendarOverlayTemplateDetails: API.CalendarOverlayTemplateDetailsDTO;
    setCurrentCalendarOverlayTemplateDetails: React.Dispatch<
        React.SetStateAction<API.CalendarOverlayTemplateDetailsDTO>
    >;
    currentCalendarTemplateDetails: API.CalendarTemplateDetailsDTO;
    setCurrentCalendarTemplateDetails: React.Dispatch<
        React.SetStateAction<API.CalendarTemplateDetailsDTO>
    >;
    currentMediaHierarchyDetails: API.MediaHierarchyTemplateDetailsDTO;
    setCurrentMediaHierarchyDetails: (
        x: API.MediaHierarchyTemplateDetailsDTO
    ) => void;
    getCurrentMediaHierarchyDetails: () => API.MediaHierarchyTemplateDetailsDTO;
    currentTotalsDetails: API.TotalsTemplateDetailsDTO;
    setCurrentTotalsDetails: (x: API.TotalsTemplateDetailsDTO) => void;
    currentGrandTotalsDetails: API.GrandTotalTemplateDetailsDTO;
    setCurrentGrandTotalsDetails: (x: API.GrandTotalTemplateDetailsDTO) => void;
    currentThemeTemplateDetails: API.ThemeTemplateDetailsDTO;
    setCurrentThemeTemplateDetails: (x: API.ThemeTemplateDetailsDTO) => void;
    currentFooterTemplateDetails: any;
    setCurrentFooterTemplateDetails: (x: any) => void;
    initialRenderDone: boolean;
    setInitialRenderDone: (x: boolean) => void;
    // guard to indicate an initial render has been started (in-progress)
    initialRenderStarted: boolean;
    setInitialRenderStarted: (x: boolean) => void;
    isUserAuthenticated: boolean;
    setIsUserAuthenticated: (x: boolean) => void;
    isTemplateChanged: boolean;
    setTemplateChanged: (x: boolean) => void;
    isSessionExpired: boolean;
    setSessionExpired: (x: boolean) => void;
    clientName: string;
    unsavedChanges: any;
    setUnsavedChanges: (x: any) => void;
    invalidCharacters: RegExp;
    mediaBriefValidColumns: string[];
    onRun: boolean;
    setOnRun: (x: boolean) => void;
    mediaBriefValidMetricColumns: string[];
    setCurrentExcelContext: (x: Excel.RequestContext) => void;
    currentExcelContext: Excel.RequestContext;
    clientFields: API.FieldInfo[];
    clientFieldsLoaded: boolean,
    mediaHierarchyTemplate: API.MediaHierarchyTemplateDetailsDTO;
    setMediaHierarchyTemplate: (
        x: API.MediaHierarchyTemplateDetailsDTO
    ) => void;
    runConfigLoaded: boolean;
    setRunConfigLoaded: (x: boolean) => void;
    levelLoading: boolean;
    setLevelLoading: (x: boolean) => void;
    isTrackFormatting: boolean;
    setIsTrackFormatting: (x: boolean) => void;
    defaultMetrics: DefaultMetrics;
    setDefaultMetrics: (x: DefaultMetrics) => void;
    clientVersion: number;
    setClientVersion: (x: number) => void;
    manualFormattingStyles: Array<{ address: string, format: Excel.RangeFormat }>;
    setManualFormattingStyles: (x: Array<{ address: string, format: Excel.RangeFormat }>) => void;
    isTrackFormattingRunConfig: boolean,
    setIsTrackFormattingRunConfig: (x: boolean) => void,
    manualFormattingStylesRunConfig: Array<{ address: string, format: Excel.RangeFormat }>;
    setManualFormattingStylesRunConfig: (x: Array<{ address: string, format: Excel.RangeFormat }>) => void;
    isFlowchartLoading: boolean;
    isRenderClicked: boolean;
    setIsRenderClicked: (x: boolean) => void;
    inflightOverlayMetrics: string[];
    setIsFlowchartSaving: (x: boolean) => void,
    isFlowchartSaving: boolean,
    currentRenderedTemplateDetails: Definition;
    setCurrentRenderedTemplateDetails: React.Dispatch<
        React.SetStateAction<Definition>>,
    previousData: any;
    setPreviousData: React.Dispatch<
        React.SetStateAction<any>>,
    staticFilter: boolean;
    setStaticFilter: (x: boolean) => void;
};

export const AppContext = createContext<TAppContextValue>({
    title: '',
    isOfficeInitialized: false,
    isInSecretTestMode: false,
    enableSecretTestMode: (): void => undefined,
    disableSecretTestMode: (): void => undefined,
    user: null,
    runConfigurationId: null,
    setRunConfigurationId: (value: string): void => undefined,
    flowchartTemplateId: null,
    flowchartTemplateDefinition: {},
    setFlowchartTemplateDefinition: (
        value: API.FlowchartTemplateDetailsDTO
    ): void => undefined,
    flowchartRunConfiguration: {},
    columns: [],
    columnsLoaded: false,
    mode: false,
    currentCalendarOverlayTemplateDetails: null,
    setCurrentCalendarOverlayTemplateDetails: (
        value: API.CalendarOverlayTemplateDetailsDTO
    ): void => undefined,
    currentHeaderTemplateDetails: null,
    setCurrentHeaderTemplateDetails: (
        value: API.HeaderTemplateDetailsDTO
    ): void => undefined,
    currentCalendarTemplateDetails: null,
    setCurrentCalendarTemplateDetails: (
        value: API.CalendarTemplateDetailsDTO
    ): void => undefined,
    currentMediaHierarchyDetails: null,
    setCurrentMediaHierarchyDetails: (
        x: API.MediaHierarchyTemplateDetailsDTO
    ) => undefined,
    getCurrentMediaHierarchyDetails: () => ({}),
    currentTotalsDetails: null,
    setCurrentTotalsDetails: (x: API.TotalsTemplateDetailsDTO) => undefined,
    currentGrandTotalsDetails: null,
    setCurrentGrandTotalsDetails: (x: API.GrandTotalTemplateDetailsDTO) =>
        undefined,
    currentThemeTemplateDetails: null,
    setCurrentThemeTemplateDetails: (x: API.ThemeTemplateDetailsDTO) =>
        undefined,
    currentFooterTemplateDetails: null,
    setCurrentFooterTemplateDetails: (x: any) => undefined,
    initialRenderDone: false,
    setInitialRenderDone: (x: boolean) => undefined,
    initialRenderStarted: false,
    setInitialRenderStarted: (x: boolean) => undefined,
    isUserAuthenticated: false,
    setIsUserAuthenticated: (x: boolean) => undefined,
    isTemplateChanged: false,
    setTemplateChanged: (x: boolean) => undefined,
    isSessionExpired: false,
    setSessionExpired: (x: boolean) => undefined,
    clientName: '',
    unsavedChanges: {},
    setUnsavedChanges: (x: any) => undefined,
    invalidCharacters: /[#$+%!`&'={}@<>:"/\\|?*\u0000-\u001F^()\-_,\[\]]/,
    mediaBriefValidColumns: [],
    onRun: false,
    setOnRun: (x: boolean) => undefined,
    mediaBriefValidMetricColumns: [],
    setCurrentExcelContext: (x: Excel.RequestContext) => undefined,
    currentExcelContext: null,
    clientFields: [],
    clientFieldsLoaded: false,
    mediaHierarchyTemplate: null,
    setMediaHierarchyTemplate: (
        x: API.MediaHierarchyTemplateDetailsDTO
    ) => undefined,
    runConfigLoaded: false,
    setRunConfigLoaded: (x: boolean) => undefined,
    levelLoading: false,
    setLevelLoading: (x: boolean) => undefined,
    isTrackFormatting: false,
    setIsTrackFormatting: (x: boolean) => undefined,
    defaultMetrics: undefined,
    setDefaultMetrics: (x: DefaultMetrics) => undefined,
    clientVersion: 0,
    setClientVersion: (x: number) => undefined,
    manualFormattingStyles: [],
    setManualFormattingStyles: (x: Array<{ address: string, format: Excel.RangeFormat }>) => undefined,
    isTrackFormattingRunConfig: false,
    setIsTrackFormattingRunConfig: (x: boolean) => undefined,
    manualFormattingStylesRunConfig: [],
    setManualFormattingStylesRunConfig: (x: Array<{ address: string, format: Excel.RangeFormat }>) => undefined,
    isFlowchartLoading: true,
    isRenderClicked: false,
    setIsRenderClicked: (x: boolean) => undefined,
    inflightOverlayMetrics: inflightOverlayMetrics,
    setIsFlowchartSaving: (x: boolean) => undefined,
    isFlowchartSaving: false,
    currentRenderedTemplateDetails: null,
    setCurrentRenderedTemplateDetails: (value: Definition) => undefined,
    previousData: null,
    setPreviousData: (value: any) => undefined,
    staticFilter: false,
    setStaticFilter: (x: boolean) => undefined
});

export type TAppContextProviderProps = Partial<
    PropsWithChildren<{
        title: string;
        isOfficeInitialized: boolean;
    }>
>;

function AppContextProvider({
    title,
    isOfficeInitialized,
    children,
}: TAppContextProviderProps): JSX.Element {
    const invalidCharacters: RegExp = new RegExp(/[#$+%!`&'={}@<>:"/\\|?*\u0000-\u001F^(),\[\]]/);
    const maxRefreshCount = useRef<number>(0);
    const maxRefreshTime = useRef<number>(2);
    const [authenticated, setAuthenticated] = useState<boolean>(false);
    const auth = useAuth();
    const mediaBriefValidColumns = ['channel',
        'subchannel',
        'buymethod',
        'briefdetail',
        'mygrid_flightstartdate',
        'mygrid_flightenddate',
        'briefedctc',
        'channeltype',
        'channelsubtype',
        'mediatype1',
        'brand4',
        'brand5',
        'mediaauthstring',
        'system_campaignid',
        'effective_date'];

    const mediaBriefValidMetricColumns = ['channeltype',
        'channelsubtype',
        'mediatype1',
        'brand4',
        'brand5', 'channel', 'subchannel'];
    const clearSessionStorage = () => {
        sessionStorage.clear();
    };

    const redirectToLoginPage = () => {
        clearSessionStorage();
        const win: Window = window;
        win.location = OMNI_AUTH_CONFIG.issuer;
    }
    const [mediaOpsColumns, setMediaOpsColumns] = useState<TColumn[]>([]);
    const isSubscribed = useRef(false);
    const pushNotification = useNotification();
    const [isInSecretTestMode, setIsInSecretTestMode] =
        useState<boolean>(false);
    const [user, setUser] = useState<API.UserDetailsDTO>({});
    const [runConfigurationId, setRunConfigurationId] = useState<string>(null);
    const [flowchartTemplateId, setFlowchartTemplateId] =
        useState<string>(null);
    const [flowchartTemplateVersion, setFlowchartTemplateVersion] =
        useState<string>(null);
    const [flowchartTemplateDefinition, setFlowchartTemplateDefinition] =
        useState<API.FlowchartTemplateDetailsDTO>({});
    const [columns, setColumns] = useState<TColumn[]>([]);

    const [
        currentCalendarOverlayTemplateDetails,
        setCurrentCalendarOverlayTemplateDetails,
    ] = useState<API.CalendarOverlayTemplateDetailsDTO>({});
    const [currentHeaderTemplateDetails, setCurrentHeaderTemplateDetails] =
        useState<API.HeaderTemplateDetailsDTO>({});
    const [currentCalendarTemplateDetails, setCurrentCalendarTemplateDetails] =
        useState<API.CalendarTemplateDetailsDTO>({});
    const [currentMediaHierarchyDetails, setCurrentMediaHierarchyDetails] =
        useState<API.MediaHierarchyTemplateDetailsDTO>({});
    const [currentTotalsDetails, setCurrentTotalsDetails] =
        useState<API.TotalsTemplateDetailsDTO>({});
    const [currentGrandTotalsDetails, setCurrentGrandTotalsDetails] =
        useState<API.GrandTotalTemplateDetailsDTO>({});
    const [currentThemeTemplateDetails, setCurrentThemeTemplateDetails] =
        useState<API.ThemeTemplateDetailsDTO>({});
    const [currentFooterTemplateDetails, setCurrentFooterTemplateDetails] =
        useState<API.FooterTemplateDetailsDTO>({});
    const [initialRenderDone, setInitialRenderDone] = useState(false);
    const [initialRenderStarted, setInitialRenderStarted] = useState(false);

    const [isUserAuthenticated, setIsUserAuthenticated] = useState(false);

    const [clientFieldsLoaded, setClientFieldsLoaded] = useState<boolean>(false);
    const [clientFields, setClientFields] = useState<API.FieldInfo[]>([]);
    const [columnsLoaded, setColumnsLoaded] = useState<boolean>(false);

    const [mode, setMode] = useState<boolean | null>(null);
    const [flowchartRunConfiguration, setFlowchartRunConfiguration] = useState<API.RunConfigurationDetailsDTO>({});

    const [isTemplateChanged, setTemplateChanged] = useState(false);
    const [isSessionExpired, setSessionExpired] = useState(false);
    const [clientName, setClientName] = useState('');
    const [clientVersion, setClientVersion] = useState(0);
    const [unsavedChanges, setUnsavedChanges] = useState<any>({});
    const unsavedEnabled = useRef(false);
    const notificationEnabled = useRef(false);
    const loadCurrentUser = useCallback(async () => {
        const { data } = await userApi.userCurrent();
        if (data && isSubscribed.current) setUser(data);
    }, []);
    const [onRun, setOnRun] = useState<boolean>(false);
    const [currentExcelContext, setCurrentExcelContext] = useState<Excel.RequestContext>();
    const [mediaHierarchyTemplate, setMediaHierarchyTemplate] = useState<API.MediaHierarchyTemplateDetailsDTO>({});
    const [runConfigLoaded, setRunConfigLoaded] = useState<boolean>(false);
    const [levelLoading, setLevelLoading] = useState<boolean>(false);
    const [isTrackFormatting, setIsTrackFormatting] = useState<boolean>(false);
    const [manualFormattingStyles, setManualFormattingStyles] = useState<Array<{ address: string, format: Excel.RangeFormat }>>([]);
    const [isTrackFormattingRunConfig, setIsTrackFormattingRunConfig] = useState(false);

    const [manualFormattingStylesRunConfig, setManualFormattingStylesRunConfig] = useState<Array<{ address: string, format: Excel.RangeFormat }>>([]);
    const [isFlowchartLoading, setIsFlowchartLoading] = useState<boolean>(true);
    const [isFlowchartSaving, setIsFlowchartSaving] = useState<boolean>(false);
    const [isRenderClicked, setIsRenderClicked] = useState<boolean>(false);
    const currentMediaHierarchyDetailsRef = useRef<API.MediaHierarchyTemplateDetailsDTO>({});
    const [currentRenderedTemplateDetails, setCurrentRenderedTemplateDetails] = useState<Definition>(null);
    const [previousData, setPreviousData] = useState<any>(null);
    const [staticFilter, setStaticFilter] = useState<boolean>(false);
    const [defaultMetrics, setDefaultMetrics] = useState<DefaultMetrics>({
        Briefed: {
            MetricColumnName: 'netmedia',
            MetricTableId: '3e185d41-9e21-44dc-9532-cd4bda1b5a0d',
        },
        NonBriefed: {
            MetricColumnName: 'netmedia',
            MetricTableId: 'f0867476-5d91-4fbf-8ebf-c5308309f4ea',
        },
        UsdMetrics: [
            {
                MetricColumnName: 'totalnet',
                MetricTableId: 'f0867476-5d91-4fbf-8ebf-c5308309f4ea',
            },
            {
                MetricColumnName: 'grossmedia',
                MetricTableId: 'f0867476-5d91-4fbf-8ebf-c5308309f4ea',
            },
        ]
    });
    useEffect(() => {
        (async () => {

            const cps = CustomPropertyService.getInstance();
            try {
                await cps.load(
                    CustomPropertyKey.TaskpaneMode,
                    ({ value }) => {
                        if (value === "Edit") {
                            setMode(false);
                        } else if (value === "Run") {
                            setMode(true);
                        }
                    }
                );
            } catch (e) {
                console.error(e);
            }

            //let modeUrl = window.location.href.split('?')?.[1];
            //let mode = modeUrl?.includes('mode=run&et=') ? true : false;
            //setMode(mode);
        })();
    }, [window.location]);
    useEffect(() => {
        if (Object.values(unsavedChanges).some(x => x)) {
            if (!unsavedEnabled.current) {
                Office.addin.beforeDocumentCloseNotification.enable();
                if (!notificationEnabled.current) {
                    Office.addin.beforeDocumentCloseNotification.onCloseActionCancelled(async function () {
                        pushNotification(
                            'There are unsaved changes. Please Click on Save Flowchart button before leave this page.',
                            NotificationType.INFO, 5000
                        );
                    });
                    notificationEnabled.current = true;
                }

                unsavedEnabled.current = true;
            }


        }
        else {
            Office.addin.beforeDocumentCloseNotification.disable();
            unsavedEnabled.current = false;
        }
    }, [unsavedChanges])
    useEffect(() => {
        if (flowchartTemplateDefinition?.Definition?.GrandTotalDefinition?.TemplateId && isEmpty(currentGrandTotalsDetails)) {
            setCurrentGrandTotalsDetails({ Id: flowchartTemplateDefinition?.Definition?.GrandTotalDefinition?.TemplateId, ...(flowchartTemplateDefinition?.Definition?.GrandTotalDefinition?.Definition?.Selections?.[0] || {}) });
        }
        if (flowchartTemplateDefinition?.Definition?.TotalsDefinition?.TemplateId && isEmpty(currentTotalsDetails)) {
            setCurrentTotalsDetails({ Id: flowchartTemplateDefinition?.Definition?.TotalsDefinition?.TemplateId, ...(flowchartTemplateDefinition?.Definition?.TotalsDefinition?.Definition?.Columns?.[0] || {}) });
        }
        if (flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition?.TemplateId && isEmpty(currentMediaHierarchyDetails)) {
            setCurrentMediaHierarchyDetails({ Id: flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition?.TemplateId, ...(flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition?.Definition?.Levels?.[0] || {}) });
        }
        if (flowchartTemplateDefinition?.Definition?.CalendarDefinition?.TemplateId && isEmpty(currentCalendarTemplateDetails)) {
            setCurrentCalendarTemplateDetails({ Id: flowchartTemplateDefinition?.Definition?.CalendarDefinition?.TemplateId, ...(flowchartTemplateDefinition?.Definition?.CalendarDefinition?.Definition?.Configuration?.Rows?.[0] || {}) });
        }
        if (flowchartTemplateDefinition?.Definition?.CalendarOverlayDefinition?.TemplateId && isEmpty(currentCalendarOverlayTemplateDetails)) {
            setCurrentCalendarOverlayTemplateDetails({ Id: flowchartTemplateDefinition?.Definition?.CalendarOverlayDefinition?.TemplateId, ...(flowchartTemplateDefinition?.Definition?.CalendarOverlayDefinition?.Definition?.OverlaySections?.[0] || {}) });
        }
        if (flowchartTemplateDefinition?.Definition?.HeaderDefinition?.TemplateId && isEmpty(currentHeaderTemplateDetails)) {
            setCurrentHeaderTemplateDetails({ Id: flowchartTemplateDefinition?.Definition?.HeaderDefinition?.TemplateId, ...(flowchartTemplateDefinition?.Definition?.HeaderDefinition?.Definition?.Configuration?.Rows || {}) });
        }
        if (flowchartTemplateDefinition?.Definition?.FooterDefinition?.TemplateId && isEmpty(currentFooterTemplateDetails)) {
            setCurrentFooterTemplateDetails({ Id: flowchartTemplateDefinition?.Definition?.FooterDefinition?.TemplateId, ...(flowchartTemplateDefinition?.Definition?.FooterDefinition?.Definition?.Configuration?.Rows?.[0] || {}) });
        }
        if (flowchartTemplateDefinition?.Definition?.ThemeDefinition?.TemplateId && isEmpty(currentThemeTemplateDetails)) {
            setCurrentThemeTemplateDetails({ Id: flowchartTemplateDefinition?.Definition?.ThemeDefinition?.TemplateId, ...(flowchartTemplateDefinition?.Definition?.ThemeDefinition || {}) });
        }
    }, [flowchartTemplateDefinition])

    const updateCurrentMediaHierarchy = async (flowchartTemplateData) => {

        const flowchartHierarchyId =
            flowchartTemplateData?.Definition?.MediaHierarchyDefinition
                ?.TemplateId;
        let current = JSON.parse(JSON.stringify(currentMediaHierarchyDetails));

        if (!currentMediaHierarchyDetails?.Id && flowchartHierarchyId) {
            try {
                //load mh
                const data =
                    await mediaHierarchyTemplateApi.mediaHierarchyTemplateGet(
                        flowchartHierarchyId
                    );
                if (data) current = JSON.parse(JSON.stringify(data));
            } catch (e) {
                console.error(e);
            }

        }

        setMediaHierarchyTemplate(current.data);

    };
    useEffect(() => {
        if (isUserAuthenticated == true) {
            (async () => {
                const cps = CustomPropertyService.getInstance();
                cps.load(
                    CustomPropertyKey.RunConfigurationId,
                    async ({ value }: CustomProperty) => {
                        const { data } =
                            await runConfigurationApi.runConfigurationGet(
                                value
                            );

                        if (data) {
                            setFlowchartRunConfiguration(data);
                            //setFlowchartTemplateDefinition(data);
                            //setRunResctrictions(data?.RunResctrictions || []);
                        }

                    }
                );
            })();

            const loginTime = new Date().getTime();
            let alertTime = loginTime + (60 * 60 * 1000) - (10 * 60 * 1000);
            let notificationTriggered = false;
            const checkAlert = () => {
                const currentTime = new Date().getTime();

                if (currentTime >= alertTime && maxRefreshCount.current >= maxRefreshTime.current) {

                    if (!notificationTriggered) {
                        pushNotification(
                            'Your session will time out in 10 minutes.Please save your work.',
                            NotificationType.INFO, 5000
                        );
                        notificationTriggered = true;
                    }
                }
            };


            let userLoginTime = new Date().getTime();
            let refreshTokenTime = userLoginTime + (60 * 60 * 1000) - (10 * 60 * 1000);

            const refreshSession = async (isFirstTimeSessionStart: boolean = false) => {
                const queryString = window.location.search;
                const urlParams = new URLSearchParams(queryString);
                let ANsid = urlParams.get(UrlAndQueryParamKey.ANSID);
                if (ANsid || sessionStorage.getItem(UrlAndQueryParamKey.ANSID)) {
                    const token = sessionStorage.getItem(UrlAndQueryParamKey.ANSID);
                    let result = await axios.post(`${WEBAPI_CONFIGURATION.basePath}/api/User/ExtendSession/${ANsid || token}`);
                    if (isFirstTimeSessionStart && result.data != null) {
                        window.dataLayer.push({
                            event: 'loginIdTrackingDesignerApp',
                            userId: result.data.PersonId //this number must be replaced with an actual User ID
                        });

                    }
                }
            }

            refreshSession(true).then().catch();

            const refreshToken = async () => {

                if (maxRefreshCount.current < maxRefreshTime.current) {
                    let dateTimeNow = new Date().getTime();
                    if (dateTimeNow >= refreshTokenTime) {

                        try {
                            await refreshSession();

                            if (maxRefreshCount.current == maxRefreshTime.current - 1) {
                                alertTime = new Date().getTime() + (60 * 60 * 1000) - (10 * 60 * 1000);
                            }
                            maxRefreshCount.current = maxRefreshCount.current + 1;

                            refreshTokenTime = new Date().getTime() + (60 * 60 * 1000) - (10 * 60 * 1000);
                        } catch {
                            console.log('clear interval called.');
                            clearInterval(refreshTokenInterval);
                        }
                    }
                }
                else {
                    console.log('Clear Interval called');
                    clearInterval(refreshTokenInterval);
                }

            };

            const interval = setInterval(checkAlert, 1000);
            const refreshTokenInterval = setInterval(refreshToken, 10000);
            return () => {
                clearInterval(refreshTokenInterval);
                clearInterval(interval);

            }
        }
        return () => { };

    }, [isUserAuthenticated]);
    const loadFlowchartTemplate = useCallback(
        async (cb?: (clientId: string) => void) => {
            const cps = CustomPropertyService.getInstance();
            let isVersionLoaded = false;
            let version: Number = 0;
            // Load Flowchart Template ID
            try {
                setIsFlowchartLoading(true);
                await cps.load(
                    CustomPropertyKey.FlowchartTemplateId,
                    ({ value }: CustomProperty) => {
                        if (value && isSubscribed.current) {
                            setFlowchartTemplateId(value);
                            console.debug(`Flowchart template ID is ${value}`);
                            setIsFlowchartLoading(false);
                        }
                    }
                );
            } catch (e) {
                setIsFlowchartLoading(false);
                pushNotification(
                    `Flowchart Template ID could not be loaded!`,
                    NotificationType.DANGER
                );
                console.error(e);
            }

            // Load Flowchart Template Version
            try {
                await cps.load(
                    CustomPropertyKey.Version,
                    ({ value }: CustomProperty) => {
                        if (value && isSubscribed.current)
                            setFlowchartTemplateVersion(value);
                        console.debug(`Flowchart template version is ${value}`);
                        isVersionLoaded = true;
                        version = parseInt(value);
                    }
                );
            } catch (e) {
                console.error(e);
            }

            // Load Flowchart Template
            try {
                cps.get(
                    CustomPropertyKey.FlowchartTemplateId,
                    async ({ value }: CustomProperty) => {
                        let flowchartTemplateData = null;
                        if (isVersionLoaded && Number(version) > 0) {
                            const { data } = await flowchartTemplateApi.flowchartTemplateSearch(plainToClass(API.FlowchartTemplateVersionSearchDTO, {
                                FlowchartTemplateId: value,
                                Version: version
                            }));
                            flowchartTemplateData = data;

                        } else {
                            const { data } = await flowchartTemplateApi.flowchartTemplateGet(
                                value
                            );

                            flowchartTemplateData = data;
                        }
                        console.debug('Template data:');
                        console.debug(flowchartTemplateData);
                        console.debug(
                            `Subscribe status: ${isSubscribed.current}`
                        );


                        if (flowchartTemplateData && isSubscribed.current) {

                            console.debug(
                                `Initializing app context from flowchart template ${value}`
                            );
                            try {
                                const { data: defaultExists } = await commonApi.commonCheckDuplicateComponentName(plainToClass(DuplicateNameCheckDto, { OmniClientId: flowchartTemplateData?.OmniClientId, Name: "Default", ComponentName: "Theme" }));
                                if (!defaultExists) {
                                    const templateDetails = getThemeTemplateDetails(plainToClass(API.ThemeTemplateCreateDTO, { Name: "Default" }));
                                    const createDTO = plainToClass(API.ThemeTemplateCreateDTO, templateDetails);
                                    createDTO.OmniClientId = flowchartTemplateData?.OmniClientId;
                                    const { data } = await themeTemplateApi.themeTemplateCreate(createDTO);

                                    if (!flowchartTemplateData.Definition) {
                                        flowchartTemplateData.Definition = {};
                                    }
                                    if (!flowchartTemplateData.Definition.ThemeDefinition) {
                                        setCurrentThemeTemplateDetails(data);
                                        flowchartTemplateData.Definition.ThemeDefinition = data.Definition;
                                    }

                                }
                                else if (defaultExists && !flowchartTemplateData?.Definition?.ThemeDefinition) {
                                    const { data } = await themeTemplateApi
                                        .themeTemplateList(
                                            plainToClass(ThemeTemplateSearchDTO, {
                                                OmniClientId: flowchartTemplateData?.OmniClientId,
                                                SearchText: 'Default',
                                                Start: 0,
                                                Count: 0,
                                                OrderAscending: true,
                                                OrderBy: null,
                                                Removed: false,
                                            })
                                        )
                                    if (data?.Items?.[0]?.Id) {
                                        const { data: themeData } = await themeTemplateApi.themeTemplateGet(data?.Items?.[0]?.Id);
                                        setCurrentThemeTemplateDetails(themeData);
                                        if (!flowchartTemplateData.Definition) {
                                            flowchartTemplateData.Definition = {};
                                        }
                                        if (!flowchartTemplateData.Definition.ThemeDefinition) {
                                            flowchartTemplateData.Definition.ThemeDefinition = { Definition: themeData.Definition, TemplateId: themeData.Id, TemplateVersion: themeData.Version };
                                        }
                                    }

                                }


                            }
                            catch (e) {
                                console.log(e);
                            }
                            await updateCurrentMediaHierarchy(flowchartTemplateData);
                            setFlowchartTemplateDefinition(flowchartTemplateData);
                            if (cb) cb(flowchartTemplateData.OmniClientId);
                        }
                    }
                );
            } catch (e) {
                pushNotification(
                    `Flowchart Template could not be loaded!`,
                    NotificationType.DANGER
                );
                console.error(e);
            }
        },
        []
    );

    const loadColumns = useCallback(async (clientId?: string, fields?: API.FieldInfo[]) => {
        try {
            const displaySource = currentMediaHierarchyDetails?.DisplaySource as API.DisplaySource.PlanitClientAlias;
            const { data } = await dataDictionaryApi.dataDictionaryGet(
                clientId, displaySource
            );

            if (data && isSubscribed.current) {
                const metricFields = fields?.filter(f => f.IsMetric).map(x => x.Name.toLowerCase());
                const stringFields = fields?.filter(f => !f.IsMetric).map(x => x.Name.toLowerCase());
                const columns = getColumnsFromDataDictionary(data).filter(column => column.TableId == DefaultMetric.MetricTableId ? column.Name.toLowerCase() != "briefedctc" : (column.TableName != 'media_briefs' || mediaBriefValidColumns.includes(column.Name.toLowerCase())))
                    .filter(val => (val.IsMetric && metricFields.includes(val.Name.toLowerCase())) || (!val.IsMetric && stringFields.includes(val.Name.toLowerCase())))
                    // Sort columns by DisplayName
                    .sort((a, b) => a.DisplayName.localeCompare(b.DisplayName))
                    // Sort columns by IsCommon
                    .sort((a, b) =>
                        a.IsCommon === b.IsCommon
                            ? 0
                            : a.IsCommon === true
                                ? -1
                                : 1
                    );

                setColumns(columns);
                setColumnsLoaded(true);
            }
        } catch (e) {
            pushNotification(
                `Columns could not be loaded!`,
                NotificationType.DANGER
            );
            console.error(e);
        }

    }, [currentMediaHierarchyDetails?.DisplaySource]);

    useEffect(() => {
        if (currentMediaHierarchyDetails?.DisplaySource) {
            if (clientVersion <= 1) {
                loadClientFields(flowchartTemplateDefinition?.OmniClientId).then(fields => {
                    loadColumns(flowchartTemplateDefinition?.OmniClientId, fields).then();
                });
            } else {
                loadFlowChartColumnsFromMediaOps(flowchartTemplateDefinition?.OmniClientId).then();
            }
        }
    }, [currentMediaHierarchyDetails?.DisplaySource, clientVersion, flowchartTemplateDefinition?.OmniClientId, loadColumns]);

    useEffect(() => {
        currentMediaHierarchyDetailsRef.current = currentMediaHierarchyDetails;
    }, [currentMediaHierarchyDetails]);

    const getCurrentMediaHierarchyDetails = useCallback(() => {
        return currentMediaHierarchyDetailsRef.current;
    }, []);

    const loadFlowChartColumnsFromMediaOps = useCallback(async (clientId: string) => {
        try {
            //const mediaopsToFlowChartApi = new MediaopsToFlowChartApi(WEBAPI_CONFIGURATION);
            const { data } = await mediaopsToFlowChartApi.mediaopsToFlowChartGet(
                plainToClass(API.MediaopsColumnSearchDTO, {
                    CreatedAt: null,
                    CreatedBy: null,
                    EndDate: null,
                    Id: null,
                    Name: null,
                    StartDate: null,
                    Status: "Active",
                    UpdatedAt: null,
                    UpdatedBy: null,
                    ClientId: clientId,
                })
            );

            if (data && isSubscribed.current) {

                const columns: TColumn[] = data.map(column => ({
                    DisplayName: column.DisplayName ?? column.ColumnName,
                    IsMetric: column.IsMetric,
                    IsCurrency: column.IsCurrency,
                    IsCommon: column.IsCommon,
                    TableName: column.TableName,
                    TableId: column.TableId,
                    Name: column.ColumnName,
                    Type: column.ColumnType as ColumnType
                })).sort((a, b) => a.DisplayName.localeCompare(b.DisplayName))
                    // Sort columns by IsCommon
                    .sort((a, b) =>
                        a.IsCommon === b.IsCommon
                            ? 0
                            : a.IsCommon === true
                                ? -1
                                : 1
                    );;

                setColumns(columns);
                setColumnsLoaded(true);
            }
        } catch (e) {
            pushNotification(
                `Columns could not be loaded!`,
                NotificationType.DANGER
            );
            console.error(e);
        }
    }, []);

    const loadClientFields = async (clientId?: string): Promise<API.FieldInfo[]> => {
        try {
            const { data } = await fieldApi.fieldInfoList(
                false
            );

            if (data && isSubscribed.current) {
                setClientFields(data);
                setClientFieldsLoaded(true);
            }
            return data;
        } catch (e) {
            pushNotification(
                `Client Fields could not be loaded!`,
                NotificationType.DANGER
            );
            console.error(e);
        }
        return [];
    };

    const loadRunConfig = useCallback(
        async (cb?: (clientId: string) => void) => {
            const cps = CustomPropertyService.getInstance();

            // Load Run Mode
            try {
                await cps.load(
                    CustomPropertyKey.TaskpaneMode,
                    async ({ value: mode }: CustomProperty) => {
                        console.log(`Taskpane Mode is ${mode}`);
                        if (
                            mode &&
                            mode === TaskpaneMode.Run &&
                            isSubscribed.current
                        ) {
                            await cps.load(
                                CustomPropertyKey.RunConfigurationId,
                                ({ value: id }: CustomProperty) => {
                                    console.log(`run config id is ${mode}`);
                                    if (id) {
                                        setRunConfigurationId(id);
                                    }
                                }
                            );
                        }
                    }
                );
            } catch (e) {
                pushNotification(
                    `Flowchart Template ID could not be loaded!`,
                    NotificationType.DANGER
                );
                console.error(e);
            }
        },
        []
    );

    useEffect(() => {
        if (isUserAuthenticated) {
            isSubscribed.current = true;

            loadCurrentUser()
                .then(() => {
                    loadRunConfig().then();
                    loadFlowchartTemplate((clientId) => {

                        (async (omniClientId) => {
                            const { data } = await clientApi.clientGet(
                                omniClientId
                            ).then();
                            if (data.Version > 1) { // If version > 1 load from MediaOps else load columns  
                                setDefaultMetrics({
                                    Briefed: DefaultMetricMediaBrief,
                                    NonBriefed: DefaultMetric,
                                    UsdMetrics: UsdMetrics
                                });
                                loadFlowChartColumnsFromMediaOps(clientId).then();
                            }
                            else {
                                loadClientFields(clientId).then((res) => {
                                    loadColumns(clientId, res).then();
                                });
                            }
                            setClientName(data?.Client?.Name || '');
                            setClientVersion(data?.Version || 0);
                            document.title = `${document.title}-${data?.Client?.Name}`
                        })(clientId);

                    }).then();
                })
                .catch((e) => {
                    // pushNotification(
                    //     `It seems that you are not logged in. Please log in!`,
                    //     NotificationType.DANGER
                    // );
                    console.error(e);
                });

            return () => {
                isSubscribed.current = false;
            };
        }
        else {
            return () => {
                isSubscribed.current = true;
            };
        }

    }, [loadCurrentUser, loadFlowchartTemplate, loadFlowChartColumnsFromMediaOps, isUserAuthenticated]);

    const enableSecretTestMode = useCallback(() => {
        console.log('Enable secret test mode.');
        setIsInSecretTestMode(true);
    }, []);

    const disableSecretTestMode = useCallback(() => {
        console.log('Disable secret test mode.');
        setIsInSecretTestMode(false);
    }, []);

    return (
        <AppContext.Provider
            value={{
                title,
                isOfficeInitialized,
                isInSecretTestMode,
                enableSecretTestMode,
                disableSecretTestMode,
                user,
                runConfigurationId,
                setRunConfigurationId,
                flowchartTemplateId,
                flowchartTemplateDefinition,
                flowchartRunConfiguration,
                columns,
                columnsLoaded,
                mode,
                setCurrentCalendarOverlayTemplateDetails,
                currentCalendarOverlayTemplateDetails,
                setFlowchartTemplateDefinition,
                currentHeaderTemplateDetails,
                setCurrentHeaderTemplateDetails,
                currentCalendarTemplateDetails,
                setCurrentCalendarTemplateDetails,
                currentMediaHierarchyDetails,
                setCurrentMediaHierarchyDetails,
                getCurrentMediaHierarchyDetails,
                currentTotalsDetails,
                setCurrentTotalsDetails,
                currentGrandTotalsDetails,
                setCurrentGrandTotalsDetails,
                currentThemeTemplateDetails,
                setCurrentThemeTemplateDetails,
                currentFooterTemplateDetails,
                setCurrentFooterTemplateDetails,
                initialRenderDone,
                initialRenderStarted,
                setInitialRenderStarted,
                setInitialRenderDone,
                isUserAuthenticated,
                setIsUserAuthenticated,
                isTemplateChanged,
                setTemplateChanged,
                isSessionExpired,
                setSessionExpired,
                clientName,
                unsavedChanges,
                setUnsavedChanges,
                invalidCharacters,
                mediaBriefValidColumns,
                setOnRun,
                onRun,
                mediaBriefValidMetricColumns,
                setCurrentExcelContext,
                currentExcelContext,
                clientFields,
                clientFieldsLoaded,
                mediaHierarchyTemplate,
                setMediaHierarchyTemplate,
                runConfigLoaded,
                setRunConfigLoaded,
                levelLoading,
                setLevelLoading,
                isTrackFormatting,
                setIsTrackFormatting,
                defaultMetrics,
                setDefaultMetrics,
                clientVersion,
                setClientVersion,
                manualFormattingStyles,
                setManualFormattingStyles,
                isTrackFormattingRunConfig,
                setIsTrackFormattingRunConfig,
                manualFormattingStylesRunConfig,
                setManualFormattingStylesRunConfig,
                isFlowchartLoading,
                isRenderClicked,
                setIsRenderClicked,
                inflightOverlayMetrics,
                isFlowchartSaving,
                setIsFlowchartSaving,
                currentRenderedTemplateDetails,
                setCurrentRenderedTemplateDetails,
                previousData,
                setPreviousData,
                staticFilter,
                setStaticFilter
            }}>
            {children}
        </AppContext.Provider>
    );
}

if (process.env.NODE_ENV !== 'production') {
    AppContext.displayName = 'AppContext';
}

export default AppContextProvider;
