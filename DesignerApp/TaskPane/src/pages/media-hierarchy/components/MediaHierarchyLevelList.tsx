import React, { JSX, memo, useCallback, useContext, useState } from 'react';
import { ColumnType } from '@omniflow/omni-webapi';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';
import { filter } from 'lodash';

import MediaHierarchyLevel from './MediaHierarchyLevel';
import { Tile } from '../../../omni/tile';
import useMediaHierarchyLevels from '../hooks/useMediaHierarchyLevels';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import { MediaHierarchyDefinitionWithId } from '../states/MediaHierarchyState';
import useOrderMediaHierarchyLevels from '../hooks/useOrderMediaHierarchyLevels';
import Button from '../../../components/buttons/Button';
import {
    useMediaHierarchyTemplateState,
} from '../states/MediaHierarchyState';
import useAddMediaHierarchyLevel from '../hooks/useAddMediaHierarchyLevel';
import '../../../pages/common-styles.css';
import { OmniCheckBoxInput } from '../../../omni/checkbox';

export type TMediaHierarchyLevelListProps = {
    onLevelsLoaded?: (Definition: MediaHierarchyDefinitionWithId) => void;
};

const MediaHierarchyLevelList = ({
    onLevelsLoaded = (): void => undefined,
}: TMediaHierarchyLevelListProps): JSX.Element => {
    const levels = useMediaHierarchyLevels();
    const changeOrder = useOrderMediaHierarchyLevels();
    const [countLevelSettingsLoaded, setCountLevelSettingsLoaded] =
        useState<number>(0);
    const { columns, flowchartTemplateDefinition, currentMediaHierarchyDetails, setCurrentMediaHierarchyDetails } = useContext(AppContext);
    const { OmniClientId } = flowchartTemplateDefinition;
    const { inflightOverlayMetrics } = useContext(AppContext);

    const filteredColumns = filter(
        columns, 
        ({ Type }) => Type === ColumnType.String || Type === ColumnType.Date 
    );

    const filteredInflightOverlayColumns = filter(
        columns,
        ({ Type, Name, IsMetric }) =>
            Type === ColumnType.String ||
            Type === ColumnType.Date ||
            (IsMetric && inflightOverlayMetrics.includes(Name.toLowerCase()))
    );

    const filteredMetricColumns = filter(
        columns,
        ({ IsMetric }) => IsMetric === true
    );

    const handleSettingsLoaded = useCallback(
        (Definition) => {
            const newCountLevelSettingsLoaded = countLevelSettingsLoaded + 1;
            setCountLevelSettingsLoaded(newCountLevelSettingsLoaded);

            if (levels.length === newCountLevelSettingsLoaded) {
                onLevelsLoaded(Definition);
            }
        },
        [levels.length, countLevelSettingsLoaded]
    );
    const { Name } = useMediaHierarchyTemplateState();
    const addLevel = useAddMediaHierarchyLevel();
    
    return (
        <>
            <div className='d-flex is-justify-content-flex-end mb-3'>
                <Button
                    className="secondary small"
                    disabled={!Name}
                    onClick={() => addLevel()}>
                    Add level
                </Button>
            </div>
            <div className="d-flex w-100">
                <div className="w-5"> </div>
            </div>
            <div className="d-flex w-100">
                <div className="w-5"> </div>
                    <div className="w-12 text-sm">Level</div>
                <div className="w-60 text-sm">Column Name</div>
                <div className="w-25 text-center text-sm">
                        Actions
                    </div>
                </div>
           
            <DragDropContext onDragEnd={(param) => changeOrder(param)}>
                <Droppable
                    style={{ transform: 'none' }}
                    droppableId="droppable-rt-rows">
                    {(provided) => (
                        <div
                            ref={provided.innerRef}
                            {...provided.droppableProps}>
                            {levels.map((level, index) => (
                                <Draggable
                                    draggableId={`${level.Id}`}
                                    key={level.Id}
                                    index={index}>
                                    {(provided, snapshot) => (
                                        <MediaHierarchyLevel
                                            clientId={OmniClientId}
                                            level={level}
                                            columns={filteredColumns}
                                            inflightOverlayColumns={
                                                filteredInflightOverlayColumns
                                            }
                                            metricColumns={
                                                filteredMetricColumns
                                            }
                                            isDragging={snapshot?.isDragging}
                                            provided={provided}
                                            onSettingsLoaded={
                                                handleSettingsLoaded
                                            }
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

export default memo(MediaHierarchyLevelList);
