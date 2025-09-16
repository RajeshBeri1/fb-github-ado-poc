import React from 'react';
import { RegisterOptions, UseFormRegisterReturn } from 'react-hook-form';
import {
    HeaderDateFormat,
    HeaderRow,
    HeaderRowType,
    ValueSeparator,
} from '@omniflow/omni-webapi';

import { TColumn } from '../../../../../lib/utils/getColumnsFromDataDictonary';
import { TValueOf } from '../../../../../interfaces/valueOf.type';
import { Input } from '../../../../../components/form/input';
import { Select } from '../../../../../components/form/select';
import { StylingConfig } from '../../../../../components/styling/styling-config';
import { filter } from "lodash";
import { OmniDropDownInput } from "../../../../../omni/dropdown";
import '../../../../../pages/common-styles.css';
type THandleRowChange = (
    index: number,
    key: keyof HeaderRow,
    value: TValueOf<HeaderRow>
) => void;

type THandleValueChange = (index: number, newValue: string) => void;

interface IHeaderRowRenderProps {
    index: number;
    currentValue: any;
    handleRowChange?: THandleRowChange;
    handleValueChange?: THandleValueChange;
    isLoading?: boolean;
    columns?: TColumn[];
    isRequired?: boolean;
}

interface IHeaderRowRenderRegisterProps {
    register: (
        name: string,
        options?: RegisterOptions
    ) => UseFormRegisterReturn;
    registerKey: string;
}
function shouldEnableDropdownSearch(options: any[], minCount: number = 5): boolean {
    return options.length > 0 && options.length >= minCount;
}
function shouldEnableDropDown(options: any[]): boolean {
    return options.length < 0;
}
export const renderStyling = ({}) => {
    return (
        <StylingConfig styling={null} formKey={'Styling'} onChange={() => {}} />
    );
};

// ------------------ Inputs
export const renderTextInput = ({
    index,
    currentValue,
    handleRowChange,
    isRequired = true,
}: IHeaderRowRenderProps) => {
    return (
        <Input
            required={isRequired}
            label="Row Text"
            labelStyle="w-100"
            onChange={(input) => handleRowChange(index, 'Text', input)}
            value={currentValue}
            breakLine
        />
    );
};

export const renderDateInput = ({
    register,
    registerKey,
}: IHeaderRowRenderRegisterProps) => (
    <Input
        labelStyle="w-100"
        label="Header Row Date"
        type="date"
        register={register}
        registerKey={registerKey}
        registerOptions={{
            valueAsDate: true,
            required: true,
        }}
        breakLine
    />
);

// ------------------ Selections
export const renderValueSeparatorSelection = ({
    index,
    currentValue,
    handleRowChange,
}: IHeaderRowRenderProps) => {
    return (
        <OmniDropDownInput
            label="Separator"
            value={
                // Find the selected option as an array (OmniDropDownInput expects an array)
                (() => {
                    const options = Object.keys(ValueSeparator).map((key) => ({
                        id: key,
                        value: key,
                    }));
                    const selected = options.find(opt => opt.value === currentValue);
                    return selected ? [selected] : [];
                })()
            }
            onValueChange={(e: CustomEvent) => {
                // e.detail is the selected option object or null
                const selected = e.detail ? e.detail.value : null;
                handleRowChange(
                    index,
                    'ValueSeparator',
                    selected as ValueSeparator
                );
            }}
            options={Object.keys(ValueSeparator).map((key) => ({
                id: key,
                value: key,
            }))}
            placeholder="Select Separator"
            className="w-100 dropdown-lg"
            hidefooter
            searchindropdown={shouldEnableDropdownSearch(Object.keys(ValueSeparator))}
        />
    );
};

export const renderValueSelection = ({
    index,
    currentValue,
    handleValueChange,
    isLoading,
    columns,
}: IHeaderRowRenderProps) => {

    const commonColumns = filter(columns, ({ IsCommon }) =>
        IsCommon === true
    );

    const otherColumns = filter(columns, ({ IsCommon }) =>
        IsCommon === false
    );
    const allOptions = [
        ...commonColumns.map(({ Name, DisplayName, TableName }, i) => ({
            id: i.toString(),
            value: `${DisplayName}${TableName == null ? '' : ' (' + TableName + ')'}`,
            isCommon: true,
        })),
        ...(otherColumns.length > 0
            ? [{
                id: 'separator',
                value: '-----------------',
                disabled: true,
            }]
            : []),
        ...otherColumns.map(({ Name, DisplayName, TableName }, i) => ({
            id: (i + commonColumns.length).toString(),
            value: `${DisplayName} (${TableName})`,
            isCommon: false,
        })),
    ];
    return (
        <OmniDropDownInput
            className="w-100 dropdown-lg"
            required
            label="Row Value"
            value={
                (() => {
                    const selected = allOptions.find(opt => opt.id === String(currentValue));
                    return selected ? [selected] : [];
                })()
            }
            onValueChange={(e: CustomEvent) => {
                // e.detail is the selected option object or null
                const selected = e.detail ? e.detail.id : null;
                handleValueChange(index, selected);
            }}
            options={allOptions}
            placeholder={isLoading ? 'Loading...' : 'Select Value'}
            hidefooter
            searchindropdown={shouldEnableDropdownSearch(allOptions)}
            disabled={shouldEnableDropDown(allOptions) }

        />
  
    );
};

export const renderDateFormatSelection = ({
    index,
    currentValue,
    handleRowChange,
}: IHeaderRowRenderProps) => {
    // Build options as { id, value } objects
    const formatOptions = Object.values(HeaderDateFormat).map((val) => ({
        id: val,
        value: val,
    }));

    // Find the selected option as an array (for OmniDropDownInput)
    const selectedOption = formatOptions.find(opt => opt.value === currentValue);
    const selectedValues = selectedOption ? [selectedOption] : [];

    return (
        <OmniDropDownInput
            className="w-100 dropdown-lg"
            label="Date Format"
            value={selectedValues}
            onValueChange={(e: CustomEvent) => {
                // e.detail is expected to be a single object or null
                const selected = e.detail ? e.detail.value : null;
                handleRowChange(index, 'DateFormat', selected as HeaderDateFormat);
            }}
            options={formatOptions}
            placeholder="Select Format"
            hidefooter
        />
    );

};
export const renderTypeSelection = ({
    index,
    currentValue,
    handleRowChange,
}: IHeaderRowRenderProps) => {
    const typeOptions = Object.keys(HeaderRowType).map((key) => ({
        id: key,
        value: HeaderRowType[key as keyof typeof HeaderRowType],
    }));
    const selectedOption = typeOptions.find(opt => opt.value === currentValue);
    const selectedValues = selectedOption ? [selectedOption] : [];
    return (
        <OmniDropDownInput
            className="w-100 dropdown-lg"
            label="Row Type"
            value={selectedValues}
            onValueChange={(e: CustomEvent) => {
                const selected = e.detail ? e.detail.value : null;
                handleRowChange(index, 'Type', selected as HeaderRowType);
            }}
            options={typeOptions}
            placeholder="Select Type"
            hidefooter
            searchindropdown={shouldEnableDropdownSearch(typeOptions) }
        />
    );
};
