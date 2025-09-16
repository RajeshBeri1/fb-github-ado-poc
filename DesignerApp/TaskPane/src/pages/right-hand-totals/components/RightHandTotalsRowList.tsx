import React, { JSX, memo, useContext } from 'react';
import { ColumnType } from '@omniflow/omni-webapi';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';
import { filter } from 'lodash';

import RightHandTotalsRow from './RightHandTotalsRow';
import { Tile } from '../../../omni/tile';
import useRightHandTotalsRows from '../hooks/useRightHandTotalsRows';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import useOrderRightHandTotalsRows from '../hooks/useOrderRightHandTotalsRows';

export type TRightHandTotalsRowListProps = {};

const RightHandTotalsRowList =
    ({}: TRightHandTotalsRowListProps): JSX.Element => {
        const { columns } = useContext(AppContext);
        const rows = useRightHandTotalsRows();
        const changeOrder = useOrderRightHandTotalsRows();

        const filteredColumns = filter(
            columns,
            ({ Type }) => Type === ColumnType.Decimal
        );

        return (
            <>
                <Tile>
                    <div className="d-flex w-100">
                        <div className="w-25">Row</div>
                        <div className="w-50">Column</div>
                        <div className="w-25 d-flex is-justify-content-center">
                            Actions
                        </div>
                    </div>
                </Tile>
                <DragDropContext onDragEnd={(param) => changeOrder(param)}>
                    <Droppable
                        style={{ transform: 'none' }}
                        droppableId="droppable-rt-rows">
                        {(provided) => (
                            <div
                                ref={provided.innerRef}
                                {...provided.droppableProps}>
                                {rows.map((row, index) => (
                                    <Draggable
                                        draggableId={`${row.Id}`}
                                        key={row.Id}
                                        index={index}>
                                        {(provided, snapshot) => (
                                            <RightHandTotalsRow
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
                            </div>
                        )}
                    </Droppable>
                </DragDropContext>
            </>
        );
    };

export default memo(RightHandTotalsRowList);
