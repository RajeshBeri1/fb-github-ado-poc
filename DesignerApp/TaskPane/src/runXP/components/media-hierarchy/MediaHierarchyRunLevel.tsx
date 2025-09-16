import React, { JSX, memo, useState } from 'react';
import { findIndex } from 'lodash';
import { MediaHierarchyLevel } from '@omniflow/omni-webapi';

import { Tile } from '../../../omni/tile';
import { Icon } from '../../../omni/icon';
import Button from '../../../components/buttons/Button';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import MediaHierarchyRunSettingList from './MediaHierarchyRunSettingList';

export type TMediaHierarchyLevelProps = {
    clientId: string;
    level: MediaHierarchyLevel;
    columns: TColumn[];
};

const MediaHierarchyRunLevel = ({
    level,
    columns,
}: TMediaHierarchyLevelProps): JSX.Element => {
    const [showSettings, setShowSettings] = useState<boolean>(false);

    const toggleSettings = () => {
        setShowSettings(!showSettings);
    };

    const columnValue = findIndex(columns, {
        Name: level.ColumnName,
        TableId: level.TableId,
    });

    return (
        <div>

            <div className="d-flex w-100 align-items-center mh-small-tile">
                <Button className="icon" disabled>
                    <Icon icon-id="omni:interactive:reorder"></Icon>
                </Button>
                <div className="w-25">
                    <span>Level {level.Order + 1}</span>
                </div>
                <div className="w-50">
                    <h2>{`${columns[columnValue].DisplayName} - (${columns[columnValue].TableName})`}</h2>
                </div>
                <div className="w-25 d-flex">
                    <div className="ms-auto">
                        <Button
                            className="icon"
                            disabled={!level.Settings.length}
                            onClick={toggleSettings}>
                            <Icon
                                icon-id={`omni:interactive:${showSettings ? 'left' : 'edit'
                                    }`}></Icon>
                        </Button>
                    </div>
                </div>
            </div>
            {showSettings && (
                <MediaHierarchyRunSettingList
                    levelId={level.Order}
                    settings={level.Settings}
                />
            )}
        </div>
    );
};

export default memo(MediaHierarchyRunLevel);