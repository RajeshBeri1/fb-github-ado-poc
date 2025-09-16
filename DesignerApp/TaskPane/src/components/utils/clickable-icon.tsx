import React from 'react';
import {Icon} from '../../omni/icon';

interface ClickableIconProps {
    iconId: string;
    onClick: () => void;
    slot?: string;
    className?: string;
}

export const ClickableIcon: React.FC<ClickableIconProps> = ({ iconId, onClick, slot = "invoker", className }) => {
    return <Icon style={{ cursor: 'pointer' }} icon-id={iconId} onClick={onClick} slot={slot} className={className } />;
};
