import React from 'react';
import { Route, Navigate } from 'react-router-dom';
import { useAuth } from '../auth-guard/auth-guard';

export const AuthRoute = ({ children, ...rest }) => {
    let auth = useAuth();
    const [isAuthenticated, setIsAuthenticated] = React.useState(false);
    const [loading, setLoading] = React.useState(true);

    React.useEffect(() => {
        auth.isAuthenticated((authenticated) => {
            setIsAuthenticated(authenticated);
            setLoading(false);
        });
    }, [auth]);

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <Route
            {...rest}
            element={
                isAuthenticated ? (
                    children
                ) : (
                    <Navigate
                        to="/login"
                        state={{ from: rest.location }}
                    />
                )
            }
        />
    );
};
