import React, { HTMLInputTypeAttribute } from 'react';
import { Label } from './label';

interface ISelectProps {
    label?: string;
    optionPlaceholder?: string;
    value?: any;
    options?: any[];
    labelStyle?: string;
    onChange?: (input: any) => void;
    breakLine?: boolean;
    disabled?: boolean;
    required?: boolean;
    children?: any;
}

export const Select: React.FC<ISelectProps> = ({
    label,
    optionPlaceholder,
    options,
    value,
    labelStyle,
    breakLine = false,
    disabled = false,
    onChange,
    children,
    required = false,
}) => {
    const renderSelect = () => {
        return (
            <select
                required={required}
                className="select input"
                value={value ?? ''}
                onChange={onChange}
                disabled={disabled}>
                {optionPlaceholder && (
                    <option value="">{optionPlaceholder}</option>
                )}
                {options
                    ? options.map((option) => (
                          <option key={option} value={option}>
                              {option}
                          </option>
                      ))
                    : children}
            </select>
        );
    };
    return (
        <>
            {label ? (
                <Label
                    label={label}
                    labelStyle={labelStyle}
                    breakLine={breakLine}>
                    {renderSelect()}
                </Label>
            ) : (
                renderSelect()
            )}
        </>
    );
};
