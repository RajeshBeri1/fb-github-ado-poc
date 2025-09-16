import React, { useState } from 'react';
import { HeaderDetailsRow } from '@omniflow/omni-webapi';
import { findIndex } from 'lodash';

import { TDetailRow } from '../shared/header-rows.type';
import { Icon } from '../../../../../omni/icon';
import { ClickableIcon } from '../../../../../components/utils/clickable-icon';
import { TValueOf } from '../../../../../interfaces/valueOf.type';
import useApiColumns from '../../../../../hooks/useApiColumns';
import {
    renderValueSelection,
    renderValueSeparatorSelection,
} from '../shared/row-render-functions';
import { Input } from '../../../../../components/form/input';
import { StylingConfig } from '../../../../../components/styling/styling-config';
import Tools from '../../../../../business/tools';
import { Tooltip } from '../../../../../omni/tooltip';

import '../shared/header-rows.css';
import { filter } from "lodash";
import useEventLogger from '../../../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../../../enums/event.enum'
interface IHeaderRowContentProps {
    clientId: string;
    row: TDetailRow;
    index: number;
    updateRows: (index: number, updatedRow: TDetailRow) => void;
    removeRow: (index: number) => void;
    isDragging?: boolean;
    provided: any;
}

export const HeaderDetailRowContent: React.FC<IHeaderRowContentProps> = ({
    clientId,
    row,
    index,
    updateRows,
    removeRow,
    isDragging = false,
    provided,
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

    const handleRowChange = (
        index: number,
        key: keyof HeaderDetailsRow,
        value: TValueOf<HeaderDetailsRow>
    ) => {
        const validValue = value ?? null;
        updateRows(index, { ...row, ...{ [key]: validValue } });
    };

    const handleValueChange = (index: number, newValue: string) => {
        const i = newValue ? parseInt(newValue, 10) : -1;
        const { Name: ColumnName, TableId } =
            i < 0 ? { Name: null, TableId: null } : totalColumns[i];

        // handle value label update if needed
        let isCustomText = false;
        if (row.Text) {
            isCustomText = !totalColumns
                .map((column) => column.DisplayName)
                .includes(row.Text);
        }
        const selectedColumn =
            totalColumns[
            findIndex(totalColumns, {
                Name: ColumnName,
                TableId: TableId,
            })
            ];

        if (!isCustomText && selectedColumn) {
            //update label too
            updateRows(index, {
                ...row,
                ColumnName,
                TableId,
                Text: selectedColumn.DisplayName,
            });
        } else {
            updateRows(index, {
                ...row,
                ColumnName,
                TableId,
            });
        }
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
                className={`is-shadowless position-static ${isDragging ? 'isDragging' : undefined}` }
                {...provided.draggableProps}
                ref={provided.innerRef}>
                <td className="px-2">
                    <Icon
                        {...provided.dragHandleProps}
                        icon-id="omni:interactive:reorder" className="custom-width-height"></Icon>
                </td>
                <td className="smallPadding" colSpan={3}>
                    {renderValueSelection({
                        index,
                        currentValue: findIndex(totalColumns, {
                            Name: row.ColumnName,
                            TableId: row.TableId,
                        }),
                        handleValueChange,
                        isLoading,
                        columns,
                    })}
                </td>

                <td className="smallPadding" width={150} colSpan={3}>
                    <Input
                        label="Label"
                        labelStyle="w-100"
                        onChange={(input) =>
                            handleRowChange(index, 'Text', input)
                        }
                        value={row.Text}
                        placeholder="Enter Label"
                        breakLine
                    />
                </td>

                <td className="smallPadding" colSpan={3}>
                    {renderValueSeparatorSelection({
                        index,
                        currentValue: row.ValueSeparator,
                        handleRowChange,
                    })}
                </td>
                <td className="smallPadding actions">
                    <div className="d-flex">
                        <div className="mr-3">
                            <Tooltip>
                                <ClickableIcon
                                    className="custom-width-height"
                                    iconId="omni:informative:theme"
                                    onClick={() => { toggleStyling(!showStyling); logEvent({ action: showStyling ? Action.OPEN : Action.CLOSE, subModule: SubModule.DETAIL, label: 'Style Format' }); }}
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
                <tr>
                    <td colSpan={11}>
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
