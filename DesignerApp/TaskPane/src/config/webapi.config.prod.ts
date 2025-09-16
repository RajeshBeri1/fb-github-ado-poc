import { Configuration } from '@omniflow/omni-webapi';
import { StorageKey } from '../enums/storage-key.enum';

export const WEBAPI_CONFIGURATION = new Configuration({
    basePath:
    'https://devflowchartbuilder-api.annalect.com',
});

export const OKTA_AUTH_STATUS: { disabled: boolean; bearer: string | null } = {
    // You can disable Okta authentication locally
    disabled: true,
    // You can overwrite the bearer token with a manual bearer token (or null if you don't want to)
    bearer: null,
};

// export const BEARER = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkJvYiBKb25lcyIsIm5hbWVpZCI6ImU4MjY4OWZmLTZhODgtNDRhOC1iMzkzLTIxYTkyZTdkYjU5MyIsImVtYWlsIjoiYm9iLmpvbmVzQG9tbmljb21tZWRpYWdyb3VwLmNvbSIsIm5iZiI6MTY2Mzc4MzA5NSwiZXhwIjoxNjk1MzE5MDk1LCJpYXQiOjE2NjM3ODMwOTV9.4WCV-lHmlu4XmWrJnHFO2nfDgsK09wDQuW9qQNQL8a4';
export const BEARER = localStorage.getItem(StorageKey.BearerToken);
export const OKTA_AUTH_CONFIG = {
    issuer: 'https://onewp.okta.com/oauth2/default',
    clientId: '0oan33qjj9hghOJd3357',
    redirectUri:
        'https://devflowchartbuilder-exceladdin.annalect.com' +
        '/login/callback',
    scopes: ['openid', 'email', 'profile'],
};

export const TESTMODE_ENABLED = false;

export const OMNI_AUTH_CONFIG = {
    issuer: 'https://devomni.annalect.com/extsso?resourcekey=omg_flowchartportal&redirecturl=https://devflowchartbuilder-exceladdin.annalect.com/login/callback?appLogin=true'
}