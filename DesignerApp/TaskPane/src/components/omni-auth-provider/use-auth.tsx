import React, { useContext, createContext, useState } from 'react';

const authContext = createContext({
    user: {},
    isAuthenticated: false,
});

export const useAuth = () => {
    return useContext(authContext);
};
