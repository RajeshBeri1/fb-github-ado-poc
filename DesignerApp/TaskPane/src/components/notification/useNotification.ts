import { useCallback } from 'react';
import { useSnackbar } from 'notistack';

const defaultOptions = {};

export enum NotificationType {
    'INFO' = 'info',
    'SUCCESS' = 'success',
    'WARNING' = 'warning',
    'DANGER' = 'error',
}

export type TPushNotificationReturn = (
    message: string,
    type?: NotificationType,
    duration?: number,
) => void;

const useNotification = (): TPushNotificationReturn => {
    const { enqueueSnackbar } = useSnackbar();

    const pushNotification = useCallback(
        (message: string, type: NotificationType = NotificationType.INFO, duration:number) => {
            const finalOptions = { ...defaultOptions, variant: type, autoHideDuration: duration||2000 };
            enqueueSnackbar(message, finalOptions);
        },
        []
    );

    return pushNotification;
};

export default useNotification;
