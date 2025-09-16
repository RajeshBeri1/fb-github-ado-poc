import React, { JSX, PropsWithChildren } from 'react';
import { SnackbarProvider } from 'notistack';

import Notification from './Notification';
import { SnackbarStyles } from './styles';

export type TNotificationProviderProps = Partial<PropsWithChildren<{}>>;

const NotificationProvider = ({
    children,
}: TNotificationProviderProps): JSX.Element => {
    return (
        <SnackbarStyles>
            <SnackbarProvider
                Components={{
                    default: Notification,
                    error: Notification,
                    success: Notification,
                    warning: Notification,
                    info: Notification,
                }}
                anchorOrigin={{ horizontal: 'center', vertical: 'top' }}
                maxSnack={2}
                autoHideDuration={2000}>
                {children}
            </SnackbarProvider>
        </SnackbarStyles>
    );
};

export default NotificationProvider;
