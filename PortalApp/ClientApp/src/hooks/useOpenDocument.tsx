import { WEBAPI_CONFIGURATION } from '../config/webapi.config';
import { UrlAndQueryParamKey } from '../enums/url-and-query-param-key.enum';
import useEventLogger from '../hooks/eventLogger';
import { Action, Module, SubModule } from '../enums/event.enum';

// eslint-disable-next-line @typescript-eslint/explicit-module-boundary-types
const useOpenDocument = () => {
    const openDocument = (id?: string | null, name?: string | null) => {
        if (!id || !name) return;
        const url = `${WEBAPI_CONFIGURATION.basePath}/api/Document/GenerateExcelDocumentTemplate/${id}/${name}.xlsx`;
        const token = sessionStorage.getItem(UrlAndQueryParamKey.ANSID);

        const headers = new Headers();
        token && headers.append('Authorization', 'ANsid ' + token);

        const link = document.createElement('a');
        fetch(url, { headers })
            .then((response) => response.blob())
            .then((blobby) => {
                const objectUrl = window.URL.createObjectURL(blobby);

                link.href = objectUrl;
                link.download = name + '.xlsx';
                link.click();

                window.URL.revokeObjectURL(objectUrl);
            });
        const { logEvent } = useEventLogger(Module.EDIT);
        logEvent(Action.OPENREPORTTOEDIT);
    };

    const openDocumentbyReportId = (
        id?: string | null,
        name?: string | null
    ) => {
        if (!id || !name) return;
        const url = `${WEBAPI_CONFIGURATION.basePath}/api/Document/GenerateExcelDocumentReportId/${id}/${name}.xlsx`;
        const token = sessionStorage.getItem(UrlAndQueryParamKey.ANSID);

        const headers = new Headers();
        token && headers.append('Authorization', 'ANsid ' + token);
        const userBasedLink = isMacUser() ? url : `ms-excel:ofe|u|${url}`;
        _createAndClickLink(userBasedLink);
    };

    const openDocumentbyFlowchartTemplateVersionHistoryId = (
        id?: string | null,
        name?: string | null
    ) => {
        if (!id || !name) return;
        const url = `${WEBAPI_CONFIGURATION.basePath}/api/Document/GenerateExcelDocumentByFlowchartTemplateVersionHistoryId/${id}/${name}.xlsx`;
        const token = sessionStorage.getItem(UrlAndQueryParamKey.ANSID);

        const headers = new Headers();
        token && headers.append('Authorization', 'ANsid ' + token);

        const link = document.createElement('a');
        fetch(url, { headers })
            .then((response) => response.blob())
            .then((blobby) => {
                const objectUrl = window.URL.createObjectURL(blobby);

                link.href = objectUrl;
                link.download = name + '.xlsx';
                link.click();

                window.URL.revokeObjectURL(objectUrl);
            });
        const { logEvent } = useEventLogger(Module.EDIT);
        logEvent(Action.OPENREPORTTOEDIT, SubModule.HISTORY);
    };

    const openFlowchartDocumentbyReportId = (
        id?: string | null,
        name?: string | null
    ) => {
        if (!id || !name) return;
        const url = `${WEBAPI_CONFIGURATION.basePath}/api/FlowchartTemplate/GetFlowchartByReportID/${id}.xlsx`;
        const token = sessionStorage.getItem(UrlAndQueryParamKey.ANSID);

        const headers = new Headers();
        token && headers.append('Authorization', 'ANsid ' + token);
        const userBasedLink = isMacUser() ? url : `ms-excel:ofe|u|${url}`;
        _createAndClickLink(userBasedLink);
    };

    // TODO #937 Complete implementation of runDocumentWithAuth to open this endpoint with bearer token in Excel when on Windows (ms-excel: protocol handler) then remove [AllowAnonymous] on DocumentController.GenerateExcelDocumentRun
    // TODO runDocumentWithAuth can then replace runDocument above
    const runDocument = (runConfigId?: string | null, name?: string | null) => {
        if (!runConfigId || !name) return;
        const url = `${WEBAPI_CONFIGURATION.basePath}/api/Document/GenerateExcelDocumentRun/${runConfigId}/${name}.xlsx`;
        const token = sessionStorage.getItem(UrlAndQueryParamKey.ANSID);

        const headers = new Headers();
        token && headers.append('Authorization', 'ANsid ' + token);

        //const userBasedLink = isMacUser() ? url : `ms-excel:ofe|u|${url}`;
        const link = document.createElement('a');
        fetch(url, { headers })
            .then((response) => response.blob())
            .then((blobby) => {
                const objectUrl = window.URL.createObjectURL(blobby);

                link.href = objectUrl;
                link.download = name + '.xlsx';
                link.click();

                window.URL.revokeObjectURL(objectUrl);
            });
        const { logEvent } = useEventLogger(Module.LAUNCH);
        logEvent(Action.LAUNCHREPORTTOVIEW, SubModule.HISTORY);

        //_createAndClickLink(userBasedLink);
    };

    const downloadDocument = (id?: string | null, name?: string | null) => {
        if (!id || !name) return;
        const url = `${WEBAPI_CONFIGURATION.basePath}/api/Report/Download/${id}`;
        const token = sessionStorage.getItem(UrlAndQueryParamKey.ANSID);

        const headers = new Headers();
        token && headers.append('Authorization', 'ANsid ' + token);

        const link = document.createElement('a');
        fetch(url, { headers })
            .then((response) => response.blob())
            .then((blobby) => {
                const objectUrl = window.URL.createObjectURL(blobby);

                link.href = objectUrl;
                link.download = name + '.xlsx';
                link.click();

                window.URL.revokeObjectURL(objectUrl);
            });
        const { logEvent } = useEventLogger(Module.DOWNLOAD);
        logEvent(Action.DOWNLOADREPORTTOVIEW, SubModule.HISTORY);
    };

    const downloadDocumentByFlowChartTemplateId = (
        id?: string | null,
        name?: string | null
    ) => {
        if (!id || !name) return;
        const url = `${WEBAPI_CONFIGURATION.basePath}/api/Report/DownloadByFlowChartTemplateId/${id}`;
        const token = sessionStorage.getItem(UrlAndQueryParamKey.ANSID);

        const headers = new Headers();
        token && headers.append('Authorization', 'ANsid ' + token);

        const link = document.createElement('a');
        fetch(url, { headers })
            .then((response) => response.blob())
            .then((blobby) => {
                const objectUrl = window.URL.createObjectURL(blobby);

                link.href = objectUrl;
                link.download = name + '.xlsx';
                link.click();

                window.URL.revokeObjectURL(objectUrl);
            });
        const { logEvent } = useEventLogger(Module.DOWNLOAD);
        logEvent(Action.DOWNLOADREPORTTOVIEW);
    };

    const isMacUser = () => {
        return navigator.platform.indexOf('Mac') === 0;
    };
    const _createAndClickLink = (url: string) => {
        const link = document.createElement('a');
        link.id = 'report'; //give it an ID!
        link.href = url;
        link.target = 'blank';
        link.click();
    };

    return {
        openDocument,
        openDocumentbyReportId,
        openDocumentbyFlowchartTemplateVersionHistoryId,
        openFlowchartDocumentbyReportId,
        downloadDocument,
        downloadDocumentByFlowChartTemplateId,
        runDocument,
    };
};

export default useOpenDocument;
