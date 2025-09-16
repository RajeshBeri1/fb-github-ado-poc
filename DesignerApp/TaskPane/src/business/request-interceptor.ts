import axios from 'axios';
import { StorageKey } from '../enums/storage-key.enum';
import { OKTA_AUTH_STATUS } from '../config/webapi.config';
import { UrlAndQueryParamKey } from '../enums/url-and-query-param-key.enum';

// Add a request interceptor
axios.interceptors.request.use(
    (config) => {
        //console.log('axios.interceptors.request', config);
        //const token = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkJvYiBKb25lcyIsIm5hbWVpZCI6ImU4MjY4OWZmLTZhODgtNDRhOC1iMzkzLTIxYTkyZTdkYjU5MyIsIm5iZiI6MTY2MzE5NDI4NiwiZXhwIjoxNjk0NzMwMjg2LCJpYXQiOjE2NjMxOTQyODZ9.AebcZWjWu8jC0xeSuaC7MPcNTBJVgm2yiRZ09ofos5I';

        // const BEARER = OKTA_AUTH_STATUS.bearer ? OKTA_AUTH_STATUS.bearer : localStorage.getItem(StorageKey.BearerToken);
        // config.headers['Authorization'] = BEARER;
        // console.log(config.url, sessionStorage.getItem(UrlAndQueryParamKey.ANSID));

        // if (!sessionStorage.getItem(UrlAndQueryParamKey.ANSID)) {

        // }
        
        if (!config.url.includes('User/ValidateToken')) {
            config.headers['Authorization'] = 'ANsid ' + sessionStorage.getItem(UrlAndQueryParamKey.ANSID);
        }

        // config.headers['Content-Type'] = 'application/json';
        return config;
    },
    (error) => {
        Promise.reject(error);
    }
);
// const RequestInterceptor = axios;
export default axios;

