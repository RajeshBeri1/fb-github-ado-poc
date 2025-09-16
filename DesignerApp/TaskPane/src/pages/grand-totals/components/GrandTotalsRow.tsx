import React, { ChangeEvent, JSX, memo, useState } from 'react';
import * as API from '@omniflow/omni-webapi';
import { findIndex } from 'lodash';

import { Icon } from '../../../omni/icon';
import useRemoveGrandTotalsRow from '../hooks/useRemoveGrandTotalsRow';
import useUpdateGrandTotalsRow from '../hooks/useUpdateGrandTotalsRow';
import { GrandTotalsSelectionWithId } from '../states/GrandTotalsState';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { ClickableIcon } from '../../../components/utils/clickable-icon';
import { StylingConfig } from '../../../components/styling/styling-config';
import Tools from '../../../business/tools';
import { filter } from "lodash";
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum';
import { OmniDropDownInput } from '../../../omni/dropdown';

const FlightRange = Object.values(API.FlightRange).filter(x => x !== API.FlightRange.None && x !== API.FlightRange.FlightTotal);
const { logEvent } = useEventLogger(Module.GRANDTOTALS);
export type TGrandTotalsRowProps = {
    row: GrandTotalsSelectionWithId;
    columns: TColumn[];
    isDragging?: boolean;
    provided: any;
};

const GrandTotalsRow = ({
    row,
    columns,
    isDragging,
    provided,
}: TGrandTotalsRowProps): JSX.Element => {
    const removeRow = useRemoveGrandTotalsRow();
    const updateRow = useUpdateGrandTotalsRow();

    const updateFlightRange = (event: CustomEvent) => {
        const selected = event.detail;
        if (!selected || selected.id === '-1') {
            updateRow(row.Id, { FlightRange: null });
            return;
        }
        updateRow(row.Id, { FlightRange: selected.id });
    };

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
            LeftMenuStyling: updatedRow.LeftMenuStyling,
        });
    };

    function getColumnOptions(columns: TColumn[]) {
        const commonColumns = filter(columns, ({ IsMetric, IsCommon }) =>
            IsMetric === true && IsCommon === true
        );
        const otherColumns = filter(columns, ({ IsMetric, IsCommon }) =>
            IsMetric === true && IsCommon === false
        );

        return [
            ...commonColumns.map(({ Name, DisplayName, TableName, TableId }, i) => ({
                id: Name,
                value: `${DisplayName}${TableName == null ? '' : ' (' + TableName + ')'}`,
                group: 'Common',
                index: i,
                TableId,
            })),
            ...(otherColumns.length > 0
                ? [{ id: '__separator__', value: '-----------------', disabled: true }]
                : []),
            ...otherColumns.map(({ Name, DisplayName, TableName, TableId }, i) => ({
                id: Name,
                value: `${DisplayName} (${TableName})`,
                group: 'Other',
                index: i + commonColumns.length,
                TableId,
            })),
        ];
    }

    const columnOptions = getColumnOptions(columns);

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
    const flightRangeOptions = [
        ...FlightRange.map(value => ({
            id: value,
            value: value
        }))
    ];
    const flightRangeValue = (() => {
        if (!row.FlightRange) return [flightRangeOptions[0]]; // Placeholder
        const opt = flightRangeOptions.find(option => option.id === row.FlightRange);
        return opt ? [opt] : [flightRangeOptions[0]];
    })();
    function shouldEnableDropdownSearch(options: any[], minCount: number = 5): boolean {
        return options.length > 0 && options.length >= minCount;
    }

    return (
        <>
            <tr
                className={isDragging ? 'isDragging' : undefined}
                {...provided.draggableProps}
                ref={provided.innerRef}>
                <td className="smallPadding">
                    <Icon
                        {...provided.dragHandleProps}
                        icon-id="omni:interactive:reorder" className="custom-width-height"></Icon>
                </td>
                <td className="smallPadding">
                    <OmniDropDownInput onValueChange={setColumn} value={columnValue} options={columnOptions} hidefooter
                        className="w-100" searchindropdown={shouldEnableDropdownSearch(columnOptions)}>
                    </OmniDropDownInput>
                </td>
                <td className="smallPadding">
                    <OmniDropDownInput onValueChange={updateFlightRange}
                        options={flightRangeOptions}
                        value={flightRangeValue}
                        hidefooter
                        className="w-100"
                        placeholder="Flight Range"
                        searchindropdown={shouldEnableDropdownSearch(flightRangeOptions)}
                    ></OmniDropDownInput>
                </td>
                <td className="smallPadding">
                    <div className="d-flex">
                        <div className="mr-3">
                            <ClickableIcon
                                className="custom-width-height"
                                iconId="omni:informative:theme"
                                onClick={() => {
                                    toggleStyling(!showStyling);
                                    logEvent({ action: showStyling ? Action.OPEN : Action.CLOSE, subModule: SubModule.GRANDTOTAL, label: 'Style Format' });
                                }}
                            />
                        </div>
                        <div>
                            <ClickableIcon
                                className="custom-width-height"
                                iconId="omni:interactive:trash"
                                onClick={() => removeRow(row.Id)}
                            />
                        </div>
                    </div>
                </td>
            </tr>
            {showStyling && (
                <tr>
                    <td colSpan={8}>
                        <table className="table w-100">
                            <thead>
                                <tr>
                                    <th>Left Menu</th>
                                </tr>
                            </thead>
                            <tr>
                                <td colSpan={8}>
                                    <StylingConfig
                                        styling={row.LeftMenuStyling}
                                        formKey={'LeftMenuStyling'}
                                        onChange={onChangeStyling}
                                    />
                                </td>
                            </tr>
                            <thead>
                                <tr>
                                    <th>Totals</th>
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
                        </table>
                    </td>
                </tr>
            )}
        </>
    );
};

export default memo(GrandTotalsRow);
