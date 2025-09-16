import React, { JSX, memo } from 'react';
import { MediaHierarchySubLevelSetting } from '@omniflow/omni-webapi';

import MediaHierarchyRunSubsetting from './MediaHierarchyRunSubSetting';

export type TMediaHierarchySubSettingListProps = {
    levelId: number;
    settingIndex: number;
    sublevelIndex: number;
    subsettings: MediaHierarchySubLevelSetting[];
};

const MediaHierarchyRunSubSettingList = ({
    levelId,
    settingIndex,
    sublevelIndex,
    subsettings,
}: TMediaHierarchySubSettingListProps): JSX.Element => {
    return (
        <div className="m-3">
            {subsettings.map((subsetting, index) => (
                <tr
                    key={JSON.stringify(subsetting) + index}
                    style={{ borderColor: 'transparent', padding: 0 }}
                    className="d-flex flex-row w-100">
                    <MediaHierarchyRunSubsetting
                        key={subsetting.Order}
                        levelId={levelId}
                        settingIndex={settingIndex}
                        sublevelIndex={sublevelIndex}
                        subsettingIndex={index}
                        subsetting={subsetting}
                    />
                </tr>
            ))}
        </div>
    );
};

export default memo(MediaHierarchyRunSubSettingList);
