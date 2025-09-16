import React, { JSX, memo, useContext } from 'react';
import { ColumnType, MediaHierarchyLevel } from '@omniflow/omni-webapi';
import { filter } from 'lodash';

import MediaHierarchyRunLevel from './MediaHierarchyRunLevel';
import { Tile } from '../../../omni/tile';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import Button from '../../../components/buttons/Button';

const MediaHierarchyRunLevelList = (): JSX.Element => {
    const {
        columns,
        flowchartTemplateDefinition,
        flowchartRunConfiguration,
        currentMediaHierarchyDetails,
        mode,
    } = useContext(AppContext);

    const filteredColumns = filter(
        columns,
        ({ Type }) => Type === ColumnType.String
    );

    const mediaHierarchyDetails = mode ? flowchartRunConfiguration?.MediaHierarchyLevels : flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition
        ?.Definition?.Levels;
    const getRunLevel = (levels: MediaHierarchyLevel[]) => {
        return levels.map((level) => (
            <MediaHierarchyRunLevel
                key={level.Order}
                clientId={flowchartTemplateDefinition.OmniClientId}
                level={level}
                columns={filteredColumns}
            />
        ));
    };

    const renderRunLevel = () => {
        if (currentMediaHierarchyDetails?.Definition?.Levels) {
            return getRunLevel(currentMediaHierarchyDetails.Definition.Levels);
        }

        //const flowchartLevels =
        //    flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition
        //        ?.Definition?.Levels;
        if (mediaHierarchyDetails) {
            return getRunLevel(mediaHierarchyDetails);
        }
        return [];
    };

    return (
        <>
           
            <div className="d-flex w-100 mt-12px">
                <div >
                    <Button className="tertiary" disabled>
                    </Button>
                </div>
                <div className="w-25 f-12">Level</div>
                <div className="w-50 f-12">Column Name</div>
                <div className="w-25 d-flex">
                    <span className="ms-auto f-12">Actions</span>
                    </div>
                </div>
            
            {renderRunLevel()}
        </>
    );
};

export default memo(MediaHierarchyRunLevelList);
