import React, { JSX, ChangeEvent, memo, useState, useEffect, useContext } from 'react';
import * as API from '@omniflow/omni-webapi';
import { filter, findIndex } from 'lodash';

import Button from '../../../components/buttons/Button';
import { Icon } from '../../../omni/icon';
import { MediaHierarchyInflightOverlayWithId } from '../states/MediaHierarchyState';
import useRemoveMediaHierarchyLevelInflightOverlay from '../hooks/useRemoveMediaHierarchyLevelInflightOverlay';
import useUpdateMediaHierarchyLevelInflightOverlay from '../hooks/useUpdateMediaHierarchyLevelInflightOverlay';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { StylingConfig } from '../../../components/styling/styling-config';
import Tools from '../../../business/tools';
import { SelectWithCommonFields } from '../../../components/form/select-with-common-fields';
import '../../../pages/common-styles.css';
import { AppContext } from '../../../../src/taskpane/contexts/AppContext';
const FlightRange = Object.values(API.FlightRange).filter(x => x !== 'None' && x !== API.FlightRange.Annually && x !== API.FlightRange.AnnuallyBroadcast);

export type TMediaHierarchyLevelInflightOverlayProps = {
    levelId: number;
    levelSettingId: number;
    inflightOverlay: MediaHierarchyInflightOverlayWithId;
    inflightOverlayColumns: TColumn[];
    isDragging?: boolean;
    provided: any;
};

const MediaHierarchyLevelInflightOverlay = ({
    levelId,
    levelSettingId,
    inflightOverlay,
    inflightOverlayColumns,
    isDragging,
    provided,
}: TMediaHierarchyLevelInflightOverlayProps): JSX.Element => {
    const removeInflightOverlay = useRemoveMediaHierarchyLevelInflightOverlay();
    const updateInflightOverlay = useUpdateMediaHierarchyLevelInflightOverlay();
    const [showStyling, setShowStyling] = useState(false);
    const { inflightOverlayMetrics } = useContext(AppContext);

    const updateInflightOverlayValue = (event: { target: { value: string } }) => {
        const i = parseInt(event.target.value, 10);
        if (i < 0) {
            updateInflightOverlay(levelId, levelSettingId, inflightOverlay.Id, {
                ColumnName: null,
                TableId: null,
            });
            return;
        }
        const { Name: ColumnName, TableId } = inflightOverlayColumns[i];
        updateInflightOverlay(levelId, levelSettingId, inflightOverlay.Id, {
            ColumnName,
            TableId,
        });
    };

    const toggleStyling = () => {
        setShowStyling(!showStyling);
    };

    const inflightOverlayColumnValue = findIndex(inflightOverlayColumns, {
        Name: inflightOverlay.ColumnName,
        TableId: inflightOverlay.TableId,
    });

    const onChangeStyling = (...args) => {
        const InflightOverlayCopy = Tools.deepCopy(inflightOverlay);
        const updatedInflightOverlay = Tools.setProperty(
            InflightOverlayCopy,
            args[0],
            args[1] === '' ? null : args[1]
        );
        updateInflightOverlay(levelId, levelSettingId, inflightOverlay.Id, {
            Styling: updatedInflightOverlay.Styling,
        });
    };

    return (
        <>
            <tr className={`no-shadow ${isDragging ? 'isDragging' : undefined}`}
                ref={provided.innerRef}
                {...provided.draggableProps}>
                <td style={{ padding: '0 5px 0 0' }}>
                    <Icon
                        {...provided.dragHandleProps}
                        icon-id="omni:interactive:reorder" className="custom-width-height"></Icon>



                </td>
                <td className="is-size-6" style={{ padding: '0 4px' }}>
                    <SelectWithCommonFields
                        isLoading={false}
                        placeHolder={'InflightOverlay'}
                        columnValue={inflightOverlayColumnValue}
                        columns={inflightOverlayColumns}
                        onChange={updateInflightOverlayValue}
                        isMetricSelection={false}
                        includedMetrics={inflightOverlayMetrics}
                    />
                </td>
                <td style={{ padding: 0 }} className="text-center">
                    <Button
                        className="icon"
                        tooltip="Format style"
                        onClick={toggleStyling}>
                        <Icon className="custom-width-height" icon-id={`omni:informative:theme`}></Icon>
                    </Button>
                    <Button
                        className="icon"
                        tooltip="Delete"
                        onClick={() =>
                            removeInflightOverlay(levelId, levelSettingId, inflightOverlay.Id)
                        }>
                        <Icon className="custom-width-height" icon-id="omni:interactive:remove"></Icon>
                    </Button>
                </td>
            </tr>
            {showStyling && (
                <>
                    <thead>
                        <tr>
                            <th colSpan={4} className="p-0">Inflight Overlay</th>
                        </tr>
                    </thead>
                    <tr>
                        <td colSpan={4} className="p-0">
                            <StylingConfig
                                styling={inflightOverlay.Styling}
                                formKey={'Styling'}
                                onChange={onChangeStyling}
                            />
                        </td>
                    </tr>
                </>
            )}
        </>
    );
};

export default memo(MediaHierarchyLevelInflightOverlay);
