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
    type?: NotificationType
) => void;

const useNotification = (): TPushNotificationReturn => {
    const { enqueueSnackbar } = useSnackbar();

    const pushNotification = useCallback(
        (message: string, type: NotificationType = NotificationType.INFO) => {
            const finalOptions = { ...defaultOptions, variant: type };
            enqueueSnackbar(message, finalOptions);
        },
        []
    );

    return pushNotification;
};

export default useNotification;
