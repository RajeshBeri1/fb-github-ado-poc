import React, { JSX,memo, ChangeEvent, useContext, useState } from 'react';
import { MediaHierarchySetting } from '@omniflow/omni-webapi';

import Button from '../../../components/buttons/Button';
import { mediaHierarchyTemplateApi } from '../../../lib/api';
import { Icon } from '../../../omni/icon';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import MediaHierarchyRunSubSettingList from './MediaHierarchyRunSubSettingList';
import { OmniCheckBoxInput } from '../../../omni/checkbox';

export type TMediaHierarchySettingProps = {
    levelId: number;
    settingIndex: number;
    setting: MediaHierarchySetting;
};

const MediaHierarchyRunSetting = ({
    levelId,
    settingIndex,
    setting,
}: TMediaHierarchySettingProps): JSX.Element => {
    const {
        currentMediaHierarchyDetails,
        setCurrentMediaHierarchyDetails,
        flowchartTemplateDefinition,
    } = useContext(AppContext);
    const [showSettings, setShowSettings] = useState(false);

    const updateEnabled = async (checked:boolean) => {
        const value = checked;
        const flowchartHierarchyId =
            flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition
                ?.TemplateId;
        let current = JSON.parse(JSON.stringify(currentMediaHierarchyDetails));

        if ((!currentMediaHierarchyDetails?.Id && flowchartHierarchyId )|| !current?.Definition) {
            //load mh
            const { data } =
                await mediaHierarchyTemplateApi.mediaHierarchyTemplateGet(
                    flowchartHierarchyId
                );
            if (data) current = JSON.parse(JSON.stringify(data));
        }

        if (!current?.Definition?.Levels) return;
        let index = current.Definition.Levels[levelId].Settings.findIndex(
            (item) => item.Name === setting.Name
        );
        current.Definition.Levels[levelId].Settings[index].Enabled = value;
        setCurrentMediaHierarchyDetails(current);
    };

    return (
        <>
            <tr className="d-flex flex-row w-100 mh-small-tile-inisde">
                <td className="d-flex is-align-items-center p-1">
                </td>
                <td className="d-flex is-align-items-center p-1">
                    <OmniCheckBoxInput
                        checked={setting.Enabled}
                        onClick={(e) => updateEnabled(Boolean(e?.currentTarget?.checked))}
                    />
                </td>
                <td className="d-flex is-align-items-center p-1 is-size-6 w-100">
                    {setting.Name}
                </td>
                <td className="w-25 d-flex">
                    <div className="ms-auto">
                    </div>
                </td>
            </tr>
            {showSettings &&
                setting.SubLevels.map((sublevel, index) => {
                    return (
                        <MediaHierarchyRunSubSettingList
                            key={levelId + JSON.stringify(sublevel)}
                            levelId={levelId}
                            settingIndex={settingIndex}
                            sublevelIndex={index}
                            subsettings={sublevel.Settings}
                        />
                    );
                })}
        </>
    );
};

export default memo(MediaHierarchyRunSetting);
