import React from 'react';

interface ILabelProps {
    label: string;
    labelStyle?: string;
    breakLine?: boolean;
    inputLabel?: string;
    children?: any;
}

export const Label: React.FC<ILabelProps> = ({
    label,
    labelStyle,
    breakLine = false,
    inputLabel,
    children,
}) => {
    return (
        <label className={labelStyle}>
            <span>{label}</span>
            {inputLabel && <p className="input-label-top">* {inputLabel}</p>}
            {breakLine && <br />}
            {children}
        </label>
    );
};
