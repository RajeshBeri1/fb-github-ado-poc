import { Configuration } from '@omniflow/omni-webapi';

export const WEBAPI_CONFIGURATION = new Configuration({
    basePath: 'https://localhost:44363',
    // basePath: 'https://portal-endpoint-azcgffh8apb2dja8.z01.azurefd.net',
});

export const OKTA_AUTH_STATUS: { disabled: boolean; bearer: string | null } = {
    // You can disable Okta authentication locally
    disabled: true,
    // You can overwrite the bearer token with a manual bearer token (or null if you don't want to)
    // Here is a valid dev token:
    // Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJhcGk6Ly9kZWZhdWx0IiwidWlkIjoiMDB1N2F5cWpwZUZSc3lQb0k1ZDciLCJzY3AiOiJbcHJvZmlsZSwgZW1haWwsIG9wZW5pZF0iLCJzdWIiOiJtci5oYWNrZXJAb21uaWNvbW1lZGlhZ3JvdXAuY29tIiwidmVyIjoiMSIsImF1dGhfdGltZSI6IjE2NzAzNTg2MTgiLCJpc3MiOiJodHRwczovL2Rldi04NDExMDk2NS5va3RhLmNvbS9vYXV0aDIvZGVmYXVsdCIsImV4cCI6IjE5MjI4MjI4MDEiLCJpYXQiOiIxNjcwMzU4NjIxIiwianRpIjoiQVQuM0ZSb0VRMWZsMlN4VGhUVGNYOWNkX1J5dFBUWFJzWVhLRk9QTUpKWnVidyIsImNpZCI6IjBvYTcwdG56NW1FU3NNYmxYNWQ3In0.RnEt2vn896HfbXAlUnzFuAUm3m69ZqmlEp8Gtv1mvOQ
    bearer: null,
};

// export const BEARER = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkJvYiBKb25lcyIsIm5hbWVpZCI6ImU4MjY4OWZmLTZhODgtNDRhOC1iMzkzLTIxYTkyZTdkYjU5MyIsImVtYWlsIjoiYm9iLmpvbmVzQG9tbmljb21tZWRpYWdyb3VwLmNvbSIsIm5iZiI6MTY2Mzc4MzA5NSwiZXhwIjoxNjk1MzE5MDk1LCJpYXQiOjE2NjM3ODMwOTV9.4WCV-lHmlu4XmWrJnHFO2nfDgsK09wDQuW9qQNQL8a4';
export const OKTA_AUTH_CONFIG = {
    issuer: 'https://dev-152662.okta.com/oauth2/default',
    clientId: '0oab04bwmyzSGboGw4x7',
    redirectUri: 'https://localhost:3000' + '/login/callback',
    devMode: true,
    scopes: ['openid', 'email', 'profile'],
};

export const TESTMODE_ENABLED = true;

export const OMNI_AUTH_CONFIG = {
    issuer: 'https://devomni.annalect.com/extsso?resourcekey=omg_flowchartportal&redirecturl=https://localhost:3000/login/callback?appLogin=true'
}