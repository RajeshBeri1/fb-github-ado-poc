import { useContext, createContext } from 'react';

const authContext = createContext({
    user: {},
    isAuthenticated: false,
});

export const useAuth = () => {
    return useContext(authContext);
};
