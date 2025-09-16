import React, { JSX, memo, useEffect, useState, useCallback } from 'react';
import { MediaHierarchySubTotalSummarySettingWithId, MediaHierarchySubTotalSummaryWithId, MediaHierarchySummarySubTotalWithId } from '../states/MediaHierarchyState';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import '../../../pages/common-styles.css';
import MediaHierarchySubTotalSummarySubTotal from './MediaHierarchySubTotalSummarySubTotal';
import useMediaHierarchyLevelSubTotalsSummarySubTotal from '../hooks/useMediaHierarchyLevelSubTotalSummarySubTotal';
import useMediaHierarchyLevelSubTotalsSummaryDisplayNames from '../hooks/useMediaHierarchyLevelSubTotalSummaryDisplayNames';
import useMediaHierarchyLevelSubTotalsSummarySetting from '../hooks/useMediaHierarchyLevelSubTotalSummarySetting';
import { Tooltip } from '@fluentui/react';
import useUpdateMediaHierarchyLevelSubTotalSummaryDisplayNames from '../summaryHook/useUpdateMediaHierarchyLevelSubTotalSummaryDisplayNames';
import useRemoveMediaHierarchyLevelSubTotalSummaryDisplayNames from '../summaryHook/useRemoveMediaHierarchyLevelSubTotalSummaryDisplayNames';
import { Icon } from '../../../omni/icon';
import { find } from 'lodash';
import Button from '../../../components/buttons/Button';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';
import useOrderMediaHierarchyLevelSummaryDisplayNames  from '../summaryHook/useOrderMediaHierarchyLevelSummaryDisplayNames'

export type TMediaHierarchySubTotalSummaryDisplayNameListProps = {
    levelId: number;
    levelSettingId: number;
    summary: MediaHierarchySubTotalSummaryWithId,
    clientId: string,
    level: any,
    levelSettingName: string,
    columns: TColumn[],
};

const MediaHierarchySubTotalSummaryDisplayNameList = ({
    levelId,
    levelSettingId,
    summary,
    clientId,
    level,
    levelSettingName,
    columns,
}: TMediaHierarchySubTotalSummaryDisplayNameListProps): JSX.Element => {
    const [inputValues, setInputValues] = useState<{ [key: string]: string }>({});
    const updateSummaryDisplayNames = useUpdateMediaHierarchyLevelSubTotalSummaryDisplayNames();
    const remove = useRemoveMediaHierarchyLevelSubTotalSummaryDisplayNames();
    const subTotalSummaryDisplayNames = useMediaHierarchyLevelSubTotalsSummaryDisplayNames(levelId, levelSettingId, summary.Id);
    const changeOrder = useOrderMediaHierarchyLevelSummaryDisplayNames();

    useEffect(() => {
        if (subTotalSummaryDisplayNames && !Object.keys(inputValues).length) {
            setInputValues(subTotalSummaryDisplayNames);
        }
    }, [subTotalSummaryDisplayNames]);

    const getColumnDisplayName = (tableId: string, columnName: string): string => {
        const columnData = find(columns, {
            Name: columnName,
            TableId: tableId,
        });
        return columnData ? columnData.DisplayName : columnName;
    }

    const generateCombinations = useCallback((
        additionalString: string,
        data: MediaHierarchySubTotalSummarySettingWithId[] | [],
        additionalArray: MediaHierarchySummarySubTotalWithId[]
    ): string[] => {
        if (!data || data.length === 0) return [];

        // Split the values into arrays
        const tempData = [...data];
        const splitValues = tempData.sort((a, b) => b.Order - a.Order).map(item => item.Values?.split(';') || []);

        // Helper function to generate combinations
        function combine(arrays: string[][]): string[] {
            if (arrays.length === 0) return [];
            if (arrays.length === 1) return arrays[0];

            const result: string[] = [];
            const [first, ...rest] = arrays;
            const combinations = combine(rest);

            for (const value of first) {
                for (const combination of combinations) {
                    result.push(`${value} ${combination}`);
                }
            }

            return result;
        }

        // Generate initial combinations
        const initialCombinations = combine(splitValues);

        // Combine with additional string and array
        const finalCombinations: string[] = [];
        for (const combination of initialCombinations) {
            for (const additionalItem of additionalArray) {
                const key = `${additionalString} ${combination} ${additionalItem.FlightRange} ${additionalItem.ColumnName}`;
                const displayName = getColumnDisplayName(additionalItem.TableId, additionalItem.ColumnName);
                const initialValue = `${additionalString} ${combination} ${additionalItem.FlightRange} ${displayName??additionalItem.ColumnName}`;

                finalCombinations.push(key);
                setInputValues(prevState => ({
                    ...prevState,
                    [key]: initialValue
                }));
                updateSummaryDisplayNames(levelId, levelSettingId, summary.Id, { key: key, value: subTotalSummaryDisplayNames ? subTotalSummaryDisplayNames[key] ?? initialValue : initialValue });
            }
        }

        remove(levelId, levelSettingId, summary.Id, finalCombinations);
        return finalCombinations;
    }, [levelId, levelSettingId, summary.Id, subTotalSummaryDisplayNames, updateSummaryDisplayNames]);

    const handleInputChange = (val: string, event: React.ChangeEvent<HTMLInputElement>) => {
        const { value } = event.target;
        setInputValues(prevState => ({
            ...prevState,
            [val]: value
        }));
        updateSummaryDisplayNames(levelId, levelSettingId, summary.Id, { key: val, value });
    };
    const handleResetClick = (key: string) => {
        const value = inputValues[key];
        
        updateSummaryDisplayNames(levelId, levelSettingId, summary.Id, { key, value });
    };
    const subTotalSummarySettings = useMediaHierarchyLevelSubTotalsSummarySetting(levelId, levelSettingId, summary.Id);
    const subTotalSummarySubTotals = useMediaHierarchyLevelSubTotalsSummarySubTotal(levelId, levelSettingId, summary.Id);

    useEffect(() => {
        generateCombinations(levelSettingName, subTotalSummarySettings, subTotalSummarySubTotals);
    }, [levelSettingName, subTotalSummarySettings, subTotalSummarySubTotals, generateCombinations]);

    return (
        <>
            <DragDropContext onDragEnd={(param) => changeOrder(param, levelId, levelSettingId, summary.Id)}>
                <Droppable
                    style={{ transform: 'none' }}
                    droppableId="droppable-rt-rows-displayNames">
                    {(provided) => (
                    <div ref={provided.innerRef}
                            {...provided.droppableProps}>
                    {subTotalSummaryDisplayNames && Object.keys(subTotalSummaryDisplayNames).map((val, index) => (
                    <Draggable
                        draggableId={val}
                        key={val}
                        index={index}>
                        {(provided, snapshot) => (
                            <div className={`is-flex is-align-items-center alias-names-list mb-2 ${snapshot?.isDragging ? 'isDragging' : undefined}`} key={val} ref={provided.innerRef}  {...provided.draggableProps}>
                                <Icon
                                    {...provided.dragHandleProps}
                                    icon-id="omni:interactive:reorder" className="custom-width-height mr-1"></Icon>

                            <p className="is-size-4 mr-2">L{index + 1}</p>
                            <div className="w-100">
                                <input className="w-100 alias-names"
                                    placeholder="Enter alias name"
                                    title={val}
                                    type="text"
                                    maxLength={50}
                                    value={(subTotalSummaryDisplayNames[val] ?? val) || ''}
                                    onChange={(e) => handleInputChange(val, e)}
                                ></input>
                                
                                    </div>
                                    <div className="reset">
                                    <Button
                                        className="icon ml-2 custom-width-height"
                                        tooltip="Redo"
                                            onClick={() => handleResetClick(val)}>
                                            <Icon icon-id="omni:editor:redo" className="mr-0"></Icon>
                                        </Button>
                                    </div>
                            </div>
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

export default memo(MediaHierarchySubTotalSummaryDisplayNameList);
