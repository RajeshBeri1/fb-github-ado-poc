import React, { memo, ChangeEvent, useContext, JSX } from 'react';
import {
    MediaHierarchySubLevelSetting,
    MediaHierarchyTemplateDetailsDTO,
} from '@omniflow/omni-webapi';

import Button from '../../../components/buttons/Button';
import { mediaHierarchyTemplateApi } from '../../../lib/api';
import { Icon } from '../../../omni/icon';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import { OmniCheckBoxInput } from '../../../omni/checkbox';

export type TMediaHierarchySubSettingProps = {
    levelId: number;
    settingIndex: number;
    sublevelIndex: number;
    subsettingIndex: number;
    subsetting: MediaHierarchySubLevelSetting;
};

const MediaHierarchyRunSubSetting = ({
    levelId,
    settingIndex,
    sublevelIndex,
    subsettingIndex,
    subsetting,
}: TMediaHierarchySubSettingProps): JSX.Element => {
    const {
        currentMediaHierarchyDetails,
        setCurrentMediaHierarchyDetails,
        flowchartTemplateDefinition,
    } = useContext(AppContext);

    const updateEnabled = async (event) => {
        debugger;
        const value = event.target.checked;
        const flowchartHierarchyId =
            flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition
                ?.TemplateId;
        let current: MediaHierarchyTemplateDetailsDTO = JSON.parse(
            JSON.stringify(currentMediaHierarchyDetails)
        );

        if ((!currentMediaHierarchyDetails?.Id && flowchartHierarchyId) || !current?.Definition) {
            //load mh
            const { data } =
                await mediaHierarchyTemplateApi.mediaHierarchyTemplateGet(
                    flowchartHierarchyId
                );
            if (data) current = JSON.parse(JSON.stringify(data));
        }

        if (!current?.Definition?.Levels) return;
        current.Definition.Levels[levelId].Settings[settingIndex].SubLevels[
            sublevelIndex
        ].Settings[subsettingIndex].Enabled = value;
        setCurrentMediaHierarchyDetails(current);
    };

    return (
        <>
            <td className="d-flex is-align-items-center p-1">
                <Button className="icon" disabled>
                    <Icon icon-id="omni:interactive:reorder"></Icon>
                </Button>
            </td>
            <td className="d-flex is-align-items-center p-1">
                <OmniCheckBoxInput
                    checked={subsetting.Enabled}
                    onChange={(e: CustomEvent) => { updateEnabled(e) }}
                />
            </td>
            <td className="d-flex is-align-items-center p-1 is-size-6 w-100">
                {subsetting.Name}
            </td>
        </>
    );
};

export default memo(MediaHierarchyRunSubSetting);
