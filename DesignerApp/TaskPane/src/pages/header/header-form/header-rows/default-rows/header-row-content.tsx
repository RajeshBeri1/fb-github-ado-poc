import React, { useState } from 'react';
import { RegisterOptions, UseFormRegisterReturn } from 'react-hook-form';
import {
    HeaderDateFormat,
    HeaderRow,
    HeaderRowType,
} from '@omniflow/omni-webapi';
import { findIndex } from 'lodash';
import moment from 'moment';

import { TRow } from '../shared/header-rows.type';
import {
    renderDateFormatSelection,
    renderDateInput,
    renderTextInput,
    renderTypeSelection,
    renderValueSelection,
    renderValueSeparatorSelection,
} from '../shared/row-render-functions';
import useApiColumns from '../../../../../hooks/useApiColumns';
import { Input } from '../../../../../components/form/input';
import { Select } from '../../../../../components/form/select';
import { TValueOf } from '../../../../../interfaces/valueOf.type';
import { Icon } from '../../../../../omni/icon';
import { ClickableIcon } from '../../../../../components/utils/clickable-icon';
import { StylingConfig } from '../../../../../components/styling/styling-config';
import Tools from '../../../../../business/tools';
import { Tooltip } from '../../../../../omni/tooltip';

import '../shared/header-rows.css';
import { filter } from "lodash";
import useEventLogger from '../../../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../../../enums/event.enum';
import { OmniDropDownInput } from '../../../../../omni/dropdown';
import '../../../../../pages/common-styles.css'; 
interface IHeaderRowContentProps {
    clientId: string;
    row: TRow;
    index: number;
    updateRows: (index: number, updatedRow: TRow) => void;
    removeRow: (index: number) => void;
    isDragging?: boolean;
    provided: any;
    register: (
        name: string,
        options?: RegisterOptions
    ) => UseFormRegisterReturn;
}

export const HeaderRowContent: React.FC<IHeaderRowContentProps> = ({
    clientId,
    row,
    index,
    isDragging = false,
    provided,
    register,
    updateRows,
    removeRow,
}) => {
    const { columns, isLoading } = useApiColumns();
    const { logEvent } = useEventLogger(Module.HEADER);
    const commonColumns = filter(columns, ({ IsCommon }) =>
        IsCommon === true
    );

    const otherColumns = filter(columns, ({ IsCommon }) =>
        IsCommon === false
    );

    const totalColumns = commonColumns.concat(otherColumns);

    const [showStyling, toggleStyling] = useState(false);

    const renderByType = (
        index: number,
        row: any,
        inputType: 'Select' | 'Other' = 'Other'
    ) => {
        switch (row.Type) {
            case HeaderRowType.Text:
                return renderTextInput({
                    index,
                    currentValue: row.Text,
                    handleRowChange,
                    isRequired: false,
                });

            case HeaderRowType.Date:
                if (inputType === 'Other') {
                    return renderDateInput({
                        register: register,
                        registerKey: `Definition.Configuration.Rows.${index}.Date`,
                    });
                }
                return renderDateFormatSelection({
                    index,
                    currentValue: row.DateFormat,
                    handleRowChange,
                });

            case HeaderRowType.Value:
                if (inputType === 'Other') {
                    return renderValueSeparatorSelection({
                        index,
                        currentValue: row.ValueSeparator,
                        handleRowChange,
                    });
                }

                const currentValue = findIndex(totalColumns, {
                    Name: row.ColumnName,
                    TableId: row.TableId,
                });
                return renderValueSelection({
                    index,
                    currentValue,
                    handleValueChange,
                    isLoading,
                    columns,
                });
            default:
                if (inputType === 'Other') {
                    return <Input label="Row Text" disabled breakLine />;
                }
                return (
                    <OmniDropDownInput
                        label="Row Value"
                        value={[]}
                        options={[
                            {
                                id: "",
                                value: "Select Value",
                                disabled: true,
                            }
                        ]}
                        placeholder="Select Value"
                        hidefooter
                        disabled
                        className="w-100 dropdown-lg"
                    />

                );
        }
    };

    const handleRowChange = (
        index: number,
        key: keyof HeaderRow,
        value: TValueOf<HeaderRow>
    ) => {
        const validValue = value ?? null;

        let overwrite = { [key]: validValue };
        if (key === 'Type') {
            // reset all other values in this case
            overwrite = {
                ...overwrite,
                DateFormat:
                    value === HeaderRowType.Date
                        ? HeaderDateFormat.MMDDYYYY
                        : null,
                Date: moment.utc().toDate(),
                Text: '',
                TableId: null,
                ColumnName: null,
                CampaignId: null,
                MediaBriefId: null,
            };
        }

        updateRows(index, { ...row, ...overwrite });
    };

    const handleValueChange = (index: number, newValue: string) => {
        const i = newValue ? parseInt(newValue, 10) : -1;
        const { Name: ColumnName, TableId } =
            i < 0 ? { Name: null, TableId: null } : totalColumns[i];

        updateRows(index, {
            ...row,
            ColumnName,
            TableId,
        });
    };

    const onChangeStyling = (...args) => {
        row = Tools.setProperty(row, args[0], args[1] === '' ? null : args[1]);
        updateRows(index, {
            ...row,
        });
    };

    return (
        <>
            <tr
                className={`is-shadowless ${isDragging} ? 'isDragging' : undefined`}
                {...provided.draggableProps}
                ref={provided.innerRef}>
                <td className="px-2" colSpan={1}>
                    <Icon
                        {...provided.dragHandleProps}
                        icon-id="omni:interactive:reorder" className="custom-width-height"></Icon>
                </td>
                <td className="smallPadding" colSpan={3}>
                    {renderTypeSelection({
                        index,
                        currentValue: row.Type,
                        handleRowChange,
                    })}
                </td>
                {/* when row type = text -> input field is two columns long */}
                <td
                    colSpan={row.Type === HeaderRowType.Text ? 8 : 5}
                    className="smallPadding">
                    {renderByType(index, row, 'Select')}
                </td>
                {row.Type !== HeaderRowType.Text && (
                    <td className="smallPadding" colSpan={3}>{renderByType(index, row)}</td>
                )}

                <td className="smallPadding actions" colSpan={3}>
                    <div className="d-flex">
                        <div className="mr-3">
                            <Tooltip>
                                <ClickableIcon
                                    className="custom-width-height"
                                    iconId="omni:informative:theme"
                                    onClick={() => { toggleStyling(!showStyling); logEvent({ action: showStyling ? Action.OPEN : Action.CLOSE, subModule: SubModule.HEADER, label: 'Style Format' }); }}
                                />
                                <div slot="content">Format style</div>
                            </Tooltip>
                        </div>
                        <div>
                            <Tooltip>
                                <ClickableIcon
                                    className="custom-width-height"
                                    iconId="omni:interactive:trash"
                                    onClick={() => removeRow(index)}
                                />
                                <div slot="content">Delete</div>
                            </Tooltip>
                        </div>
                    </div>
                </td>
            </tr>
            {showStyling && (
                <tr className="is-shadowless">
                    <td colSpan={14}>
                        <StylingConfig
                            styling={row.Styling}
                            formKey={'Styling'}
                            onChange={onChangeStyling}
                        />
                    </td>
                </tr>
            )}
        </>
    );
};
