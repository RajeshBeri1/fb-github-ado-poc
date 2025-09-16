import React, { useState } from 'react';
import { AuthService } from '../auth-service/auth-service';
import { LoginDTO, UserApi } from '@omniflow/omni-webapi';
import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';
import { StorageKey } from '../../enums/storage-key.enum';

export const AuthProvider = () => {
    const [user, setUser] = useState(null);
    const [token, setToken] = useState(null);

    const userService = new UserApi(WEBAPI_CONFIGURATION);

    const signin = (data, cb) => {
        const login: LoginDTO = {
            UserName: data.UserName,
            Password: data.Password,
        };
        return userService.userLogin(login).then((response) => {
            setToken(response.data);
            cb();
        });
    };

    const signout = (cb) => {
        return AuthService.signout(() => {
            setUser(null);
            cb();
        });
    };

    const isAuthenticated = (cb) =>
        cb(!!localStorage.getItem(StorageKey.BearerToken));

    return {
        user,
        isAuthenticated,
        signin,
        signout,
    };
};
