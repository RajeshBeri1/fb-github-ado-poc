import React from 'react';
import { Control } from 'react-hook-form';
import { FooterTemplateDetailsDTO } from '@omniflow/omni-webapi';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';

import useRows from './useRows';
import { changeOrder, removeRow } from '../../../../lib/utils/row-helpers';
import { Title } from '../../../../components/form/title';
import { FooterRowContent } from './footer-row-content';
import '../../../../pages/common-styles.css';
interface IFooterRowsProps {
    title?: string;
    control: Control<FooterTemplateDetailsDTO>;
}

export const FooterRows: React.FC<IFooterRowsProps> = ({
    control,
    title = 'Footer Rows',
}) => {
    const { rows, addRow, onChange } = useRows(control, title);

    const handleRemoveRow = (index) => {
        removeRow(index, rows, onChange);
    };

    const handleUpdateRow = (index, updatedRow) => {
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
                <p className="component-title" >{title}</p>
                <button
                    className="secondary small"
                    onClick={(e) => addRow(e)}>
                    Add row
                </button>
            </div>

            <table className="table">
                <thead>
                    <tr>
                        <th className="p-0"></th>
                        <th className="py-0 pl-9rem footer-thead text-sm">Row Text</th>
                        <th className="py-0 footer-thead text-center text-sm">Actions</th>
                    </tr>
                </thead>
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
                                            <FooterRowContent
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
