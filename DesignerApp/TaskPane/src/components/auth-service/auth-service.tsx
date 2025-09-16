import { LoginDTO, UserApi } from '@omniflow/omni-webapi';
import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';

export const AuthService = {
    userService: new UserApi(WEBAPI_CONFIGURATION),

    isAuthenticated: false,

    signin(cb) {
        AuthService.isAuthenticated = true;
        setTimeout(cb, 100); // fake async
    },
    signout(cb) {
        AuthService.isAuthenticated = false;
        setTimeout(cb, 100);
    },
};
