import axios from 'axios';
import { StorageKey } from '../enums/storage-key.enum';
  import { UrlAndQueryParamKey } from '../enums/url-and-query-param-key.enum';

// Add a request interceptor
axios.interceptors.request.use(
    (config) => {
        // console.log('axios.interceptors.request', config);
        // const BEARER = localStorage.getItem(StorageKey.BearerToken);
        // config.headers['Authorization'] = BEARER;
        // config.headers['Content-Type'] = 'application/json';

        if (!config?.url?.includes('User/ValidateToken')) {
            config.headers['Authorization'] =
                'ANsid ' + sessionStorage.getItem(UrlAndQueryParamKey.ANSID);
        }
        return config;
    },
    (error) => {
        Promise.reject(error);
    }
);
// const RequestInterceptor = axios;
export default axios;
