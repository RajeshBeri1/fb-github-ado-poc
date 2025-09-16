import React, { ChangeEvent, JSX, memo, useState } from 'react';
import { findIndex } from 'lodash';

import { Tile } from '../../../omni/tile';
import { Icon } from '../../../omni/icon';
import Button from '../../../components/buttons/Button';
import useRemoveRightHandTotalsRow from '../hooks/useRemoveRightHandTotalsRow';
import useUpdateRightHandTotalsRow from '../hooks/useUpdateRightHandTotalsRow';
import { RightHandTotalsColumnWithId } from '../states/RightHandTotalsState';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { StylingConfig } from '../../../components/styling/styling-config';
import Tools from '../../../business/tools';
import { filter } from "lodash";

import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum';
import { OmniDropDownInput } from '../../../omni/dropdown'

export type TRightHandTotalsRowProps = {
    row: RightHandTotalsColumnWithId;
    columns: TColumn[];
    isDragging?: boolean;
    provided: any;
};
const { logEvent } = useEventLogger(Module.RIGHTHANDTOTALS);
const RightHandTotalsRow = ({
    row,
    columns,
    isDragging,
    provided,
}: TRightHandTotalsRowProps): JSX.Element => {
    const removeRow = useRemoveRightHandTotalsRow();
    const updateRow = useUpdateRightHandTotalsRow();
    const [showStyling, toggleStyling] = useState(false);

    const onChangeStyling = (...args) => {
        const rowCopy = Tools.deepCopy(row);
        const updatedRow = Tools.setProperty(
            rowCopy,
            args[0],
            args[1] === '' ? null : args[1]
        );

        row = Tools.setProperty(row, args[0], args[1] === '' ? null : args[1]);
        updateRow(row.Id, {
            Styling: updatedRow.Styling,
            HeaderStyling: updatedRow.HeaderStyling,
            SumStyling: updatedRow.SumStyling,
        });
    };

    const commonColumns = filter(columns, ({ IsMetric, IsCommon }) =>
        IsMetric === true && IsCommon === true
    );
    
    const otherColumns = filter(columns, ({ IsMetric, IsCommon }) =>
        IsMetric === true && IsCommon === false
    );

    const columnOptions = [
        ...commonColumns.map(({ Name, DisplayName, TableName, TableId }, i) => ({
            id: Name,
            value: `${DisplayName}${TableName == null ? '' : ' (' + TableName + ')'}`,
            group: 'Common',
            index: i,
            TableId, // Add TableId for matching
        })),
        ...(otherColumns.length > 0
            ? [{ id: '__separator__', value: '-----------------', disabled: true }]
            : []),
        ...otherColumns.map(({ Name, DisplayName, TableName, TableId }, i) => ({
            id: Name,
            value: `${DisplayName} (${TableName})`,
            group: 'Other',
            index: i + commonColumns.length,
            TableId, // Add TableId for matching
        })),
    ];

    const setColumn = (event: CustomEvent) => {
        const selected = event.detail;
        if (!selected || selected.id === '-1' || selected.id === '__separator__' || typeof selected.index !== 'number') {
            updateRow(row.Id, { ColumnName: null, Name: null, TableId: null });
            return;
        }

        const col = columns[selected.index];
        if (col) {
            updateRow(row.Id, { ColumnName: col.Name, Name: col.DisplayName, TableId: col.TableId });
        }
    };
    
    const columnValue = (() => {
        if (!row.ColumnName || !row.TableId) return [];
        const opt = columnOptions.find(
            option => option.id === row.ColumnName && option.TableId === row.TableId
        );
        return opt ? [opt] : [];
    })();

    function shouldEnableDropdownSearch(options: any[], minCount: number = 5): boolean {
        return options.length > 0 && options.length >= minCount;
    }

    return (
        <Tile
            className={isDragging ? 'isDragging' : undefined}
            ref={provided.innerRef}
            {...provided.draggableProps}>
            <div slot="subheader">&nbsp;</div>
            <div className="d-flex w-100 align-items-center">
                <Icon
                    {...provided.dragHandleProps}
                    icon-id="omni:interactive:reorder" className="custom-width-height"></Icon>
                <div className="w-25">
                    <span>Row {row.Order + 1}</span>
                </div>
                <div className="w-50">
                    <OmniDropDownInput className="text-capitalize"
                        onValueChange={(x: CustomEvent) => setColumn(x)}
                        options={columnOptions} value={columnValue} hidefooter
                        searchindropdown={shouldEnableDropdownSearch(columnOptions)}>
                </OmniDropDownInput>
                   
                </div>
                <div className="w-25 d-flex is-justify-content-center">
                    <Button
                        className="icon"
                        tooltip="Format style"
                        onClick={() => {
                            toggleStyling(!showStyling)
                            logEvent({ action: showStyling ? Action.OPEN : Action.CLOSE, subModule: SubModule.RIGHTHANDTOTAL, label: 'Style Format' });
                        }}>
                        <Icon icon-id="omni:informative:theme"></Icon>
                    </Button>
                            
                    <Button
                        className="icon"
                        tooltip="Delete"
                        onClick={() => {
                            removeRow(row.Id)
                            logEvent({ action: Action.DELETE, subModule: SubModule.RIGHTHANDTOTAL })
                        }}>
                        <Icon icon-id="omni:interactive:remove"></Icon>
                    </Button>
                </div>
            </div>
            {showStyling && (
                <table className="table">
                    <thead>
                        <tr>
                            <td colSpan={8}>Header</td>
                        </tr>
                    </thead>
                    <tr>
                        <td colSpan={8}>
                            <StylingConfig
                                styling={row.HeaderStyling}
                                formKey={'HeaderStyling'}
                                onChange={onChangeStyling}
                            />
                        </td>
                    </tr>
                    <thead>
                        <tr>
                            <td colSpan={8}>Totals</td>
                        </tr>
                    </thead>
                    <tr>
                        <td colSpan={8}>
                            <StylingConfig
                                styling={row.Styling}
                                formKey={'Styling'}
                                onChange={onChangeStyling}
                            />
                        </td>
                    </tr>
                    <thead>
                        <tr>
                            <td colSpan={8}>Right Hand Totals sum</td>
                        </tr>
                    </thead>
                    <tr>
                        <td colSpan={8}>
                            <StylingConfig
                                styling={row.SumStyling}
                                formKey={'SumStyling'}
                                onChange={onChangeStyling}
                            />
                        </td>
                    </tr>
                </table>
            )}
        </Tile>
    );
};

export default memo(RightHandTotalsRow);
