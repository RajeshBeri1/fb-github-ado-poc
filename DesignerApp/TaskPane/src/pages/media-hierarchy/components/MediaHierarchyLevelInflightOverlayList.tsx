import React, { JSX, memo, useContext } from 'react';
import * as API from '@omniflow/omni-webapi';
import Button from '../../../components/buttons/Button';
import useMediaHierarchyLevelInflightOverlay from '../hooks/useMediaHierarchyLevelInflightOverlays';
import useAddMediaHierarchyLevelInflightOverlay from '../hooks/useAddMediaHierarchyLevelInflightOverlay';
import MediaHierarchyLevelInflightOverlay from './MediaHierarchyLevelInflightOverlay';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';
import useOrderMediaHierarchyLevelInflightOverlays from '../hooks/useOrderMediaHierarchyLevelInflightOverlays';

export type TMediaHierarchyLevelInflightOverlayListProps = {
    levelId: number;
    levelSettingId: number;
    flightRange?: API.FlightRange;
    inflightOverlayColumns: TColumn[];
};

const MediaHierarchyLevelInflightOverlayList = ({
    levelId,
    levelSettingId,
    flightRange = API.FlightRange.FlightTotal,
    inflightOverlayColumns,
}: TMediaHierarchyLevelInflightOverlayListProps): JSX.Element => {
    const inflightOverlays = useMediaHierarchyLevelInflightOverlay(levelId, levelSettingId);
    const addInflightOverlay = useAddMediaHierarchyLevelInflightOverlay();
    const changeOrder = useOrderMediaHierarchyLevelInflightOverlays();
    const defaultInflightOverlay = inflightOverlayColumns && inflightOverlayColumns.length > 1 ? inflightOverlayColumns[1] : null;
    return (
        <div>
            <div slot="header" className="d-flex is-justify-content-space-between mb-4">
                Inflight Overlays

                <div slot="end">
                    <Button
                        className="secondary is-size-7 px-4"
                        onClick={() =>
                            addInflightOverlay(levelId, levelSettingId, 1, {
                                ColumnName: defaultInflightOverlay ? defaultInflightOverlay.Name : null,
                                TableId: defaultInflightOverlay ? defaultInflightOverlay.TableId : null,
                            }, false)
                        }>
                        Add inflight overlay
                    </Button>
                </div>
            </div>
            <table
                className="table is-fullwidth is-shadowless"
                style={{ padding: 0 }}>
                <thead>
                    <tr className="no-shadow">
                        <th style={{ padding: 0 }}></th>
                        <th className="is-size-6" style={{ padding: 0 }}>
                            Inflight Overlay
                        </th>
                        <th className="is-size-6 text-center" style={{ padding: 0 }}>
                            Action
                        </th>
                    </tr>
                </thead>
                <DragDropContext onDragEnd={(param) => changeOrder(param, levelId, levelSettingId)}>
                    <Droppable
                        style={{ transform: 'none' }}
                        droppableId="droppable-rt-rows-inflightoverlays">
                        {(provided) => (
                            <tbody ref={provided.innerRef}
                                {...provided.droppableProps}>
                                {inflightOverlays?.map((inflightOverlay, index) => (
                                    <Draggable
                                        draggableId={`${inflightOverlay.Id}`}
                                        key={inflightOverlay.Id}
                                        index={index}>
                                        {(provided, snapshot) => (
                                            <MediaHierarchyLevelInflightOverlay
                                                key={inflightOverlay.Id}
                                                levelId={levelId}
                                                levelSettingId={levelSettingId}
                                                inflightOverlay={inflightOverlay}
                                                inflightOverlayColumns={inflightOverlayColumns}
                                                isDragging={snapshot?.isDragging}
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
        </div>
    );
};

export default memo(MediaHierarchyLevelInflightOverlayList);
