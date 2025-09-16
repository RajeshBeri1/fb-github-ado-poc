import React from 'react';
import { Route, useRouteMatch } from 'react-router-dom';
import { AuthRoute } from '../components/auth-route/auth-route';

export const RouteOutlet = (route) => {
    return (
        <Route
            path={route.path}
            render={(props) => (
                // pass the sub-routes down to keep nesting
                <route.component {...props} routes={route.routes} />
            )}
        />
    );
};
