import { Page } from '../../enums/page.enum';
import { UrlAndQueryParamKey } from '../../enums/url-and-query-param-key.enum';

const routes = {
    list: `/${Page.Dashboard}/:${UrlAndQueryParamKey.FLOWCHART_TEMPLATE_ID}/${Page.GrandTotals}/${Page.List}`,
    create: `/${Page.Dashboard}/:${UrlAndQueryParamKey.FLOWCHART_TEMPLATE_ID}/${Page.GrandTotals}/${Page.Create}`,
    update: `/${Page.Dashboard}/:${UrlAndQueryParamKey.FLOWCHART_TEMPLATE_ID}/${Page.GrandTotals}/${Page.Update}/:${UrlAndQueryParamKey.GRAND_TOTALS_TEMPLATE_ID}`,
};
const SUB_PATHS = {
    LIST: `${Page.List}`,
    CREATE: `${Page.Create}`,
    UPDATE: `${Page.Update}/:${UrlAndQueryParamKey.GRAND_TOTALS_TEMPLATE_ID}`,
};

export { SUB_PATHS };
export default routes;
