import React, { useContext, createContext, useState } from 'react';
import { AuthProvider } from '../auth-provider/auth-provider';

const authContext = createContext({
    user: {},
    isAuthenticated: (cb) => {},
    signin: (data, cb) => {},
    signout: (cb) => {},
});

export const AuthGuard = ({ children }) => {
    const auth = AuthProvider();

    return <authContext.Provider value={auth}>{children}</authContext.Provider>;
};

export const useAuth = () => {
    return useContext(authContext);
};
