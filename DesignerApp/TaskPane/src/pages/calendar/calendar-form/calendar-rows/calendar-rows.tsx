import {
    CalendarRow,
    DataDictionaryColumnDetailsDTO,
    StandardCalenderRowType,
} from '@omniflow/omni-webapi';
import React, {FC, useMemo, useState } from 'react';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';

import Tools from '../../../../business/tools';
import { StylingConfig } from '../../../../components/styling/styling-config';
import { ClickableIcon } from '../../../../components/utils/clickable-icon';
import { changeOrder, removeRow } from '../../../../lib/utils/row-helpers';
import { Icon } from '../../../../omni/icon';
import { TRows } from './calendar-rows.type';
import { OmniDropDownInput } from '../../../../omni/dropdown';
import { Tooltip } from '../../../../omni/tooltip';
interface ICalendarRowsProps {
    rows: (CalendarRow & { id: string })[];
    types: DataDictionaryColumnDetailsDTO[];
    usingStandardType: boolean;
    changeRows: (changendRows: TRows) => void;
}

export const CalendarRows: React.FC<ICalendarRowsProps> = ({
    rows,
    types,
    usingStandardType,
    changeRows,
}) => {
    const availableTypes = useMemo(() => {
        const usedTypes = rows.map((row) =>
            usingStandardType ? row.StandardCalenderRowType : row.ColumnName
        );
        return types.filter((type) => !usedTypes.includes(type.Name));
    }, [rows, types]);

    const [showStyling, toggleStyling] = useState(-1);

    const handleTypeChange = (index: number, type?: string) => {
        const validType = type ?? undefined;
        changeRows(
            rows.map((row, i) => {
                if (i === index)
                    return {
                        ...row,
                        ColumnName: usingStandardType
                            ? row.ColumnName
                            : validType,
                        StandardCalenderRowType: usingStandardType
                            ? (validType as StandardCalenderRowType)
                            : row.StandardCalenderRowType,
                    };
                return row;
            })
        );
    };
    const getTypeOptions = (
        currentType: DataDictionaryColumnDetailsDTO[],
        index: number
    ) => [...availableTypes, ...currentType].map((type) => {
        return type && { id: type.Name, value: type.DisplayName }

    }).filter(x => x);



    const getCurrentType = (row: CalendarRow) => {
        let currentType = [];
        if (types.length) {
            if (usingStandardType && row.StandardCalenderRowType) {
                currentType = [
                    types.find(
                        (type) => type.Name === row.StandardCalenderRowType
                    ),
                ];
            } else if (!usingStandardType && row.ColumnName) {
                currentType = [
                    types.find((type) => type.Name === row.ColumnName),
                ];
            }
        }
        return currentType;
    };
    const onChangeStyling = (index: number, ...args) => {
        changeRows(
            rows.map((row, i) => {
                if (i === index) {
                    row = Tools.setProperty(
                        row,
                        args[0],
                        args[1] === '' ? null : args[1]
                    );
                    return {
                        ...row,
                    };
                }
                return row;
            })
        );
    };

    const renderRows = () => {
        return rows.map((row, index) => {
            //needed to ensure that the current type stays available to the current row
            let currentType = getCurrentType(row);
            let value = usingStandardType
                ?[{ id: row.StandardCalenderRowType, value: row.StandardCalenderRowType }] 
                : [{ id: row.ColumnName, value: row.ColumnName }];
            const options = getTypeOptions(currentType, index);
            let currentValue = options.find(x => x.id === value[0].id)? [options.find(x => x.id === value[0].id)]:[];
            return (
                <Draggable draggableId={row.id} key={row.id} index={index}>
                    {(provided, snapshot) => {
                        return (
                            <>
                                <tr
                                    className={
                                        snapshot.isDragging
                                            ? 'isDragging'
                                            : undefined
                                    }
                                    {...provided.draggableProps}
                                    ref={provided.innerRef}>
                                    <td>
                                        <Icon
                                            {...provided.dragHandleProps}
                                            icon-id="omni:interactive:reorder" className="custom-width-height"></Icon>
                                    </td>
                                    <td className="dropdown-container">
                                        <OmniDropDownInput
                                            placeholder="Select row type"
                                            required
                                            value={
                                                currentValue
                                            }
                                            onValueChange={(e: CustomEvent) =>
                                                handleTypeChange(
                                                    index,
                                                    e.detail.id)
                                            }
                                            options={getTypeOptions(currentType, index)}
                                            hidefooter
                                        >

                                        </OmniDropDownInput>
                                    </td>
                                    <td>
                                        <div className="d-flex">
                                            <div className=" mr-3">
                                                <Tooltip>
                                                <ClickableIcon
                                                    iconId="omni:informative:theme"
                                                    className="custom-width-height"
                                                    onClick={() =>
                                                        toggleStyling(
                                                            showStyling ===
                                                                index
                                                                ? -1
                                                                : index
                                                        )
                                                    }
                                                    />
                                                    <div slot="content">Format style</div>
                                                </Tooltip>
                                            </div>

                                            <div>
                                                <Tooltip>
                                                    <ClickableIcon
                                                        iconId="omni:interactive:trash"
                                                        className="custom-width-height"
                                                        onClick={() =>
                                                            removeRow(
                                                                index,
                                                                rows,
                                                                changeRows
                                                            )
                                                        }
                                                    />
                                                    <div slot="content">Delete</div>
                                                </Tooltip>
                                               
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                                {showStyling === index && (
                                    <tr>
                                        <td colSpan={8}>
                                            <StylingConfig
                                                styling={row.Styling}
                                                formKey={'Styling'}
                                                onChange={(x, y) =>
                                                    onChangeStyling(index, x, y)
                                                }
                                            />
                                        </td>
                                    </tr>
                                )}
                            </>
                        );
                    }}
                </Draggable>
            );
        });
    };

    return (
        <DragDropContext
            onDragEnd={(param) => changeOrder(param, rows, changeRows)}>
            <Droppable
                style={{ transform: 'none' }}
                droppableId="droppable-calendar-settings">
                {(provided) => (
                    <tbody ref={provided.innerRef} {...provided.droppableProps}>
                        {renderRows()}

                        {provided.placeholder}
                    </tbody>
                )}
            </Droppable>
        </DragDropContext>
    );
};
