import * as API from '@omniflow/omni-webapi';
import { WEBAPI_CONFIGURATION } from '../config/webapi.config';
import RequestInterceptor from '../business/request-interceptor';

export const calendarOverlayTemplateApi = new API.CalendarOverlayTemplateApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const calendarTemplateApi = new API.CalendarTemplateApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const clientApi = new API.ClientApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const dataApi = new API.DataApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const dataDictionaryApi = new API.DataDictionaryApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const documentApi = new API.DocumentApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const flowchartTemplateApi = new API.FlowchartTemplateApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const footerTemplateApi = new API.FooterTemplateApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const grandTotalTemplateApi = new API.GrandTotalTemplateApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const headerTemplateApi = new API.HeaderTemplateApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const mediaHierarchyTemplateApi = new API.MediaHierarchyTemplateApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const reportApi = new API.ReportApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const runConfigurationApi = new API.RunConfigurationApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const summaryTemplateApi = new API.SummaryTemplateApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const themeTemplateApi = new API.ThemeTemplateApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const totalsTemplateApi = new API.TotalsTemplateApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);
export const userApi = new API.UserApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);

export const commonApi = new API.CommonApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);

export const fieldApi = new API.FieldInfoApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);

export const mediaopsToFlowChartApi = new API.MediaopsToFlowChartApi(
    WEBAPI_CONFIGURATION,
    null,
    RequestInterceptor
);

const api = {
    calendarOverlayTemplateApi,
    calendarTemplateApi,
    clientApi,
    dataApi,
    dataDictionaryApi,
    documentApi,
    flowchartTemplateApi,
    footerTemplateApi,
    grandTotalTemplateApi,
    headerTemplateApi,
    mediaHierarchyTemplateApi,
    reportApi,
    runConfigurationApi,
    summaryTemplateApi,
    themeTemplateApi,
    totalsTemplateApi,
    userApi,
    commonApi,
    fieldApi,
    mediaopsToFlowChartApi
};

export default api;
