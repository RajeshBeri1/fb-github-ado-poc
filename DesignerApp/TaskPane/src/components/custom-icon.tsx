import React, { JSX } from 'react';

import IcomoonReact from 'icomoon-react';
import iconSet from '../../assets/fonts/selection.json';

export type TCustomIconProps = {
    icon: string;
    color?: string;
    size?: string | number;
    className?: string;
};

const CustomIcon = ({
    icon,
    color = '#000',
    size = 14,
    className,
}: TCustomIconProps): JSX.Element => {
    return (
        <IcomoonReact
            className={className}
            iconSet={iconSet}
            color={color}
            size={size}
            icon={icon}
        />
    );
};

export default CustomIcon;
