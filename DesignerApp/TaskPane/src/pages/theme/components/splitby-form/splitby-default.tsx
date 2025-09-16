import React, { useState, useContext, useCallback, useEffect } from 'react';
import { SplitByColumn } from '@omniflow/omni-webapi';
import { AppContext } from '../../../../taskpane/contexts/AppContext';
import { OmniDropDownInput } from '../../../../omni/dropdown';
import { filter } from 'lodash';
import { dataApi } from '../../../../lib/api';
import Button from '../../../../components/buttons/Button';
import { Icon } from '../../../../omni/icon';
import { DefaultStyling } from '../../../../business/engine/models/styles';
import { plainToClass } from 'class-transformer';
import { useForm } from 'react-hook-form';
import { OmniDropdown } from 'omni-ui'


interface IInputProps {
    formKey?: string;
    onChange?: (attr, e) => void;
    splitByColumn: SplitByColumn;
}

export const SplitByDefault: React.FC<IInputProps> = React.memo(({
    formKey,
    onChange,
    splitByColumn,
}) => {
    const [splitByState, setSplitByState] = useState(splitByColumn);
    const [showSettings, setShowSettings] = useState<boolean>(false);
    const [isDataLoading, setDataLoading] = useState<boolean>(false);
    const splitByRef = React.useRef<OmniDropdown>(null);
    const toggleSettings = () => {
        setShowSettings(!showSettings);
    };
    const {
        register,
        setValue,
        formState: { isDirty }, getValues } =
        useForm<Record<string, any>>({
            mode: 'onChange',
            reValidateMode: 'onChange',
            defaultValues: {
                SplitByColumn: splitByState,
            },
        });
    const { columns, flowchartTemplateDefinition, currentCalendarTemplateDetails } = useContext(AppContext);
    const capitalizeFirstWord = (phrase) => {
        return phrase.replace(/^\w/, c => c.toUpperCase());
    }
    // Filter columns for dropdown (example: only string/date, not metrics)
    const filteredColumns = columns
        ? columns.filter(
            (col) => (col.Type === 'String' || col.Type === 'Date') && col.IsMetric !== true
        ).map((col) => ({
            value: `${col.DisplayName} (${col.TableName})`,
            id: `${col.TableId}_${col.Name}`,
            column: col
        }))
        : [];





    const handleColumnChange = (event) => {
        event.preventDefault();
        const value = event.detail?.id ? event.detail.id : null;
        const tableId = value?.split('_')[0];
        const columnName = value?.split('_')[1];
        onChange(formKey + '.TableId', tableId);
        onChange(formKey + '.ColumnName', columnName);
        setValue('SplitByColumn.TableId', tableId);
        setValue('SplitByColumn.ColumnName', columnName);

    }

    const columnSelected = getValues('SplitByColumn.TableId') && getValues('SplitByColumn.ColumnName') ? columns.find(c => c.Name == getValues('SplitByColumn.ColumnName') && c.TableId == getValues('SplitByColumn.TableId')) : null;
    const splitByValue = getValues('SplitByColumn.TableId') && getValues('SplitByColumn.ColumnName') ? [filteredColumns.find(fc => fc.value == `${columnSelected.DisplayName} (${columnSelected.TableName})`)] : [];
    const deselectColumn = () => {
        splitByRef.current?.reset();
    }
    return (
        <>
            <div className="d-flex w-100 align-items-center">
                <OmniDropDownInput
                    ref={splitByRef}
                    label="Column Name"
                    className="w-95"
                    disabled={isDataLoading}
                    searchindropdown
                    typeahead
                    options={filteredColumns}
                    value={splitByValue}
                    onValueChange={handleColumnChange}
                />
                <Button
                    className="icon"
                    tooltip={'Delete'}
                    disabled={!getValues('SplitByColumn.TableId')}
                    onClick={deselectColumn}>
                    <Icon
                        icon-id={`omni:interactive:delete`}></Icon>
                </Button>
            </div>

        </>
    );
});