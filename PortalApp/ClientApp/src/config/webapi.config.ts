import { Configuration } from '@omniflow/omni-webapi';

export const WEBAPI_CONFIGURATION = new Configuration({
    basePath: 'https://localhost:44363',
});

// export const BEARER = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkJvYiBKb25lcyIsIm5hbWVpZCI6ImU4MjY4OWZmLTZhODgtNDRhOC1iMzkzLTIxYTkyZTdkYjU5MyIsImVtYWlsIjoiYm9iLmpvbmVzQG9tbmljb21tZWRpYWdyb3VwLmNvbSIsIm5iZiI6MTY2Mzc4MzA5NSwiZXhwIjoxNjk1MzE5MDk1LCJpYXQiOjE2NjM3ODMwOTV9.4WCV-lHmlu4XmWrJnHFO2nfDgsK09wDQuW9qQNQL8a4';
// export const BEARER = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkJvYiBKb25lcyIsIm5hbWVpZCI6ImU4MjY4OWZmLTZhODgtNDRhOC1iMzkzLTIxYTkyZTdkYjU5MyIsImVtYWlsIjoiYm9iLmpvbmVzQG9tbmljb21tZWRpYWdyb3VwLmNvbSIsIm5iZiI6MTY2NDI2ODM5MSwiZXhwIjoxNjk1ODA0MzkxLCJpYXQiOjE2NjQyNjgzOTF9.pcq9SQtExus0VwF75CdlHpyyJsqCld8X85-WJXfS-sg'

export const OKTA_AUTH_CONFIG = {
    issuer: 'https://dev-152662.okta.com/oauth2/default',
    clientId: '0oab04bwmyzSGboGw4x7',
    redirectUri: 'https://localhost:44421' + '/login/callback',
    devMode: true,
    scopes: ['openid', 'email', 'profile'],
};

export const OMNI_AUTH_CONFIG = {
    issuer: 'https://devomni.annalect.com/extsso?resourcekey=omg_flowchartportal&redirecturl=https://localhost:44421/',
    validator: 'https://devaccess2.annalect.com/am/amapi/user/session/'
}