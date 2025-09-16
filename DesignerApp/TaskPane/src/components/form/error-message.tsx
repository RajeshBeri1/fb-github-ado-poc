import React from 'react';
import { OmniIcon } from './icon';

interface IMessageProps {
    message: string;
}

export const ErrorMessage: React.FC<IMessageProps> = ({
    message,
}) => {
    return (<div className="error-msg"><span className="mr-2 error-icon"><OmniIcon icon-id="omni:informative:error" slot='invoker'></OmniIcon></span><span>{message}</span> </div>
    );
};
