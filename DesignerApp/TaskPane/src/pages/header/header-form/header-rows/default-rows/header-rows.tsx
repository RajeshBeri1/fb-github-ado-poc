import React from 'react';
import {
    Control,
    RegisterOptions,
    UseFormRegisterReturn,
} from 'react-hook-form';
import { HeaderTemplateDetailsDTO } from '@omniflow/omni-webapi';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';

import { Title } from '../../../../../components/form/title';
import { HeaderRowContent } from './header-row-content';
import useRows from './useRows';
import { changeOrder, removeRow } from '../../../../../lib/utils/row-helpers';
import { TRow } from '../shared/header-rows.type';
import useEventLogger from '../../../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../../../enums/event.enum'

interface IHeaderRowsProps {
    clientId: string;
    title?: string;
    control: Control<HeaderTemplateDetailsDTO>;
    register: (
        name: string,
        options?: RegisterOptions
    ) => UseFormRegisterReturn;
}

export const HeaderRows: React.FC<IHeaderRowsProps> = ({
    clientId,
    control,
    register,
    title = 'Header Rows',
}) => {
    const { rows, addRow, onChange } = useRows(control, title);
    const { logEvent } = useEventLogger(Module.HEADER);
    const handleRemoveRow = (index) => {
        logEvent({ action: Action.DELETE, subModule: SubModule.HEADER });
        removeRow(index, rows, onChange);
    };

    const handleUpdateRow = (index, updatedRow) => {
        logEvent({ action: Action.UPDATE, subModule: SubModule.HEADER });
        onChange(
            rows.map((row, i) => {
                if (i === index) return updatedRow;
                return row;
            })
        );
    };

    return (
        <>
            <div className="d-flex mb-3 justify-content-between">
                <Title text={title} />
                <button
                    className="secondary small"
                    onClick={(e) => { addRow(e); logEvent({ action: Action.ADDROW, subModule: SubModule.HEADER }); }}>
                    Add row
                </button>
            </div>

            <table className="table  table-layout-fixed p-0">
                <DragDropContext
                    onDragEnd={(param) => changeOrder(param, rows, onChange)}>
                    <Droppable
                        style={{ transform: 'none' }}
                        droppableId="droppable-header-rows">
                        {(provided) => (
                            <tbody
                                ref={provided.innerRef}
                                {...provided.droppableProps}>
                                {rows.map((row, index) => (
                                    <Draggable
                                        draggableId={row.id}
                                        key={row.id}
                                        index={index}>
                                        {(provided, snapshot) => (
                                            <HeaderRowContent
                                                clientId={clientId}
                                                row={row as TRow}
                                                index={index}
                                                updateRows={handleUpdateRow}
                                                removeRow={handleRemoveRow}
                                                isDragging={
                                                    snapshot?.isDragging
                                                }
                                                provided={provided}
                                                register={register}
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
