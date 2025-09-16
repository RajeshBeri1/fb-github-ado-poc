import React, { JSX, memo, useContext } from 'react';
import { ColumnType } from '@omniflow/omni-webapi';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';
import { filter } from 'lodash';

import GrandTotalsRow from './GrandTotalsRow';
import useGrandTotalsRows from '../hooks/useGrandTotalsRows';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import useOrderGrandTotalsRows from '../hooks/useOrderGrandTotalsRows';

export type TGrandTotalsRowListProps = {};

const GrandTotalsRowList = ({}: TGrandTotalsRowListProps): JSX.Element => {
    const { columns } = useContext(AppContext);
    const rows = useGrandTotalsRows();
    const changeOrder = useOrderGrandTotalsRows();

    const filteredColumns = filter(
        columns,
        ({ Type }) => Type === ColumnType.Decimal
    );

    return (
        <>
            <table className="table gt-table">
                <thead>
                    <tr>
                        <td className="smallPadding"></td>
                        <td className="smallPadding">Column Name</td>
                        <td className="smallPadding">Flight Range</td>
                        <td className="smallPadding">Actions</td>
                    </tr>
                </thead>
                <DragDropContext onDragEnd={(param) => changeOrder(param)}>
                    <Droppable
                        style={{ transform: 'none' }}
                        droppableId="droppable-gt-rows">
                        {(provided) => (
                            <tbody
                                ref={provided.innerRef}
                                {...provided.droppableProps}>
                                {rows.map((row, index) => (
                                    <Draggable
                                        draggableId={`${row.Id}`}
                                        key={row.Id}
                                        index={index}>
                                        {(provided, snapshot) => (
                                            <GrandTotalsRow
                                                row={row}
                                                columns={filteredColumns}
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

export default memo(GrandTotalsRowList);
