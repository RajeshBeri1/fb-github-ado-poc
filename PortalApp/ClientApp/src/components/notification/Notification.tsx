import React, { forwardRef } from 'react';
import { SnackbarContent, CustomContentProps } from 'notistack';
import { Icon} from '../../omni-ui-components/icon';

export type TNotificationProps = CustomContentProps & {};

const Notification = forwardRef<HTMLDivElement, TNotificationProps>(
    ({ message, variant }, ref) => {
        const notificationType =
            variant === 'default'
                ? 'info'
                : variant === 'error'
                ? 'danger'
                : variant;
        const iconType =
            variant === 'default'
                ? 'info'
                : variant === 'warning'
                ? 'alert'
                : variant;

        return (
            <SnackbarContent
                ref={ref}
                role="alert"
                className={`notification is-${notificationType}`}>
                <Icon icon-id={`omni:informative:${iconType}`}></Icon>
                {message}
            </SnackbarContent>
        );
    }
);

export default Notification;
