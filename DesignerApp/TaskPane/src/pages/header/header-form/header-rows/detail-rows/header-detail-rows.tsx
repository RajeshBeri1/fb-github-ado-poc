import React from 'react';
import { Control } from 'react-hook-form';
import { HeaderTemplateDetailsDTO } from '@omniflow/omni-webapi';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';

import { Title } from '../../../../../components/form/title';
import useDetailRows from './useDetailRows';
import { changeOrder, removeRow } from '../../../../../lib/utils/row-helpers';
import { HeaderDetailRowContent } from './header-detail-row-content';
import useEventLogger from '../../../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../../../enums/event.enum'
interface IHeaderDetailRowsProps {
    clientId: string;
    title?: string;
    control: Control<HeaderTemplateDetailsDTO>;
}

export const HeaderDetailRows: React.FC<IHeaderDetailRowsProps> = ({
    clientId,
    control,
    title = 'Detail Rows',
}) => {
    const { rows, addRow, onChange } = useDetailRows(control, title);
    const { logEvent } = useEventLogger(Module.HEADER);
    const handleRemoveRow = (index) => {
        logEvent({ action: Action.DELETE, subModule: SubModule.DETAIL });
        removeRow(index, rows, onChange);
    };

    const handleUpdateRow = (index, updatedRow) => {
        logEvent({ action: Action.UPDATE, subModule: SubModule.DETAIL });
        onChange(
            rows.map((row, i) => {
                if (i === index) return updatedRow;
                return row;
            })
        );
    };

    return (
        <>
            <div className="d-flex my-3 justify-content-between">
                <Title text={title} />
                <button
                    className="secondary small"
                    onClick={(e) => { addRow(e); logEvent({ action: Action.ADDROW, subModule: SubModule.DETAIL }); }}>
                    Add row
                </button>
            </div>

            <table className="table p-0">
                <DragDropContext
                    onDragEnd={(param) => changeOrder(param, rows, onChange)}>
                    <Droppable
                        style={{ transform: 'none' }}
                        droppableId="droppable-header-rows">
                        {(provided) => (
                            <tbody
                                className="static-body"
                                ref={provided.innerRef}
                                {...provided.droppableProps}>
                                {rows.map((row, index) => (
                                    <Draggable
                                        draggableId={row.id}
                                        key={row.id}
                                        index={index}>
                                        {(provided, snapshot) => (
                                            <HeaderDetailRowContent
                                                clientId={clientId}
                                                row={row}
                                                index={index}
                                                updateRows={handleUpdateRow}
                                                removeRow={handleRemoveRow}
                                                isDragging={
                                                    snapshot?.isDragging
                                                }
                                                provided={provided}
                                            />
                                        )}
                                    </Draggable>
                                ))}

                                {provided.placeholder}
                            </tbody>
                        )}
                    </Droppable>
                </DragDropContext>
            </table>
        </>
    );
};
