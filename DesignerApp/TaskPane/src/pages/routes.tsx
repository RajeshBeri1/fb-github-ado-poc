import { Page } from '../enums/page.enum';
import { UrlAndQueryParamKey } from '../enums/url-and-query-param-key.enum';

export const routes = (key: string, params: any) => {
    key = key.replace(' ', '');
    const templateId = params[UrlAndQueryParamKey.FLOWCHART_TEMPLATE_ID];

    const basePath = `/${Page.Dashboard}/${templateId}/${Page[key]}`;

    return {
        list: `${basePath}/${Page.List}`,
        create: `${basePath}/${Page.Create}`,
        update: `${basePath}/${Page.Update}`,
    };
};

export const SUB_PATHS = {
    LIST: `${Page.List}`,
    CREATE: `${Page.Create}`,
    UPDATE: `${Page.Update}`
} 