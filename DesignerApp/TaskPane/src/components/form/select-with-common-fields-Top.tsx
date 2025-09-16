import React, {ChangeEvent} from "react";
import {filter} from "lodash";
import { TColumn } from "../../lib/utils/getColumnsFromDataDictonary";
import { OmniDropDownInput } from "../../omni/dropdown"

interface ISelectProps {
    isLoading: boolean;
    placeHolder?: string;
    onChange: (event: ChangeEvent<HTMLSelectElement>) => void;
    columnValue: number;
    columns: TColumn[];
    style?: object;
    isMetricSelection: boolean;
}

export const SelectWithCommonFieldsTop: React.FC<ISelectProps> = ({
    isLoading = false,
    placeHolder = "Select a value",
    onChange,
    style = {},
    columnValue,
    columns,
    isMetricSelection = false,
}) => {
    const commonColumns = filter(
        columns,
        ({ IsMetric, IsCommon }) => IsMetric === isMetricSelection && IsCommon === true
    );

    const otherColumns = filter(
        columns,
        ({ IsMetric, IsCommon }) => IsMetric === isMetricSelection && IsCommon === false
    );

    const totalColumnCount = commonColumns.length + otherColumns.length;

    // Build options array
    const options: Option[] = [
        {
            id: "-1",
            value: isLoading ? "Loading..." : placeHolder,
            disabled: true,
        },
        ...commonColumns.map(({ DisplayName, TableName }, i) => ({
            id: i.toString(),
            value: `${DisplayName} (${TableName})`,
        })),
        ...(otherColumns.length > 0
            ? [
                  {
                      id: "separator",
                      value: "-----------------",
                      disabled: true,
                  },
              ]
            : []),
        ...otherColumns.map(({ DisplayName, TableName }, i) => ({
            id: (i + commonColumns.length).toString(),
            value: `${DisplayName} (${TableName})`,
        })),
    ];

    // Find the selected option from options (skip disabled/separator)
    const selectableOptions = options.filter(
        (opt) => !opt.disabled && opt.id !== "separator"
    );
    const selectedOption = selectableOptions.find(
        (opt) => opt.id === columnValue.toString()
    );

    // Ensure value is always Option[]
    const value: Option[] = selectedOption ? [selectedOption] : [];

    return (
        <OmniDropDownInput
            className="text-capitalize w-100 dropdown-lg"
            value={value}
            options={options}
            onValueChange={(e: CustomEvent) => {
                const selectedId = e.detail ? e.detail.id : "-1";
                onChange({
                    target: { value: selectedId },
                } as unknown as ChangeEvent<HTMLSelectElement>);
            }}
            placeholder={isLoading ? "Loading..." : placeHolder}
            disabled={isLoading || !totalColumnCount}
            hidefooter
            searchindropdown
        />
    );
};