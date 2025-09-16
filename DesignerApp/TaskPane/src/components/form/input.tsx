import React, { HTMLInputTypeAttribute, KeyboardEvent } from 'react';
import { Label } from './label';

interface IInputProps {
    label?: string;
    type?: HTMLInputTypeAttribute;
    register?: any;
    registerKey?: string;
    registerOptions?: unknown;
    value?: string;
    labelStyle?: string;
    inputStyle?: string;
    onChange?: (input: string) => void;
    onKeyUp?: (
        event:
            | KeyboardEvent<HTMLInputElement>
            | KeyboardEvent<HTMLTextAreaElement>
    ) => void;
    onKeyDown?: (
        event:
            | KeyboardEvent<HTMLInputElement>
            | KeyboardEvent<HTMLTextAreaElement>
    ) => void;
    breakLine?: boolean;
    disabled?: boolean;
    autoFocus?: boolean;
    required?: boolean;
    placeholder?: string;
    isTextArea?: boolean;
    min?: string;
    max?: string;
    errorClass?: string;
    inputLabel?: string;
    rows?: number;
}

export const Input: React.FC<IInputProps> = ({
    label,
    type = 'text',
    register,
    registerKey,
    registerOptions,
    value,
    labelStyle,
    placeholder,
    min,
    max,
    inputStyle = 'input',
    autoFocus = false,
    breakLine = false,
    disabled = false,
    required = false,
    isTextArea = false,
    onChange,
    onKeyUp,
    onKeyDown,
    errorClass = '',
    inputLabel,
    rows = 2,
}) => {
    const handleKeyDown = (e: KeyboardEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            (e.target as HTMLElement).blur();
        }

        if (onKeyDown) {
            const eventType = isTextArea
                ? e as KeyboardEvent<HTMLTextAreaElement>
                : e as KeyboardEvent<HTMLInputElement>;
            onKeyDown(eventType);
        }
    };
    const registerInput = () => {
        if (isTextArea) {
            return (
                <textarea
                    placeholder={placeholder}
                    disabled={disabled}
                    autoFocus={autoFocus}
                    required={required}
                    className={`${inputStyle} ${type === 'text' ? 'text' : ''}`}
                    onKeyUp={(e) => onKeyUp && onKeyUp(e)}
                    onKeyDown={handleKeyDown}
                    {...register(registerKey, registerOptions)}
                    rows={rows}
                />
            );
        }
        return (
            <input
                placeholder={placeholder}
                disabled={disabled}
                autoFocus={autoFocus}
                required={required}
                type={type}
                min={min}
                max={max}
                className={`${inputStyle} ${type === 'text' ? 'text' : ''} ${errorClass}`}
                onKeyUp={(e) => onKeyUp && onKeyUp(e)}
                onKeyDown={handleKeyDown}
                {...register(registerKey, registerOptions)}
            />
        );
    };

    const defaultInput = () => {
        if (isTextArea) {
            return (
                <textarea
                    placeholder={placeholder}
                    disabled={disabled}
                    autoFocus={autoFocus}
                    required={required}
                    value={value ?? ''}
                    className={`${inputStyle} ${type === 'text' ? 'text' : ''}`}
                    onChange={(e) => onChange(e.target.value)}
                    onKeyUp={(e) => onKeyUp && onKeyUp(e)}
                    onKeyDown={handleKeyDown}
                    rows={rows}
                />
            );
        }
        return (
            <input
                placeholder={placeholder}
                disabled={disabled}
                autoFocus={autoFocus}
                required={required}
                type={type}
                min={min}
                max={max}
                value={value ?? ''}
                className={`${inputStyle} ${type === 'text' ? 'text' : ''}`}
                onChange={(e) => onChange(e.target.value)}
                onKeyUp={(e) => onKeyUp && onKeyUp(e)}
                onKeyDown={handleKeyDown}
            />
        );
    };

    const renderInput = () => {
        if (register) {
            return registerInput();
        }
        return defaultInput();
    };

    return (
        <>
            {label ? (
                <Label
                    label={label}
                    labelStyle={labelStyle}
                    breakLine={breakLine}
                    inputLabel={inputLabel}>
                    {renderInput()}
                </Label>
            ) : (
                renderInput()
            )}
        </>
    );
};
