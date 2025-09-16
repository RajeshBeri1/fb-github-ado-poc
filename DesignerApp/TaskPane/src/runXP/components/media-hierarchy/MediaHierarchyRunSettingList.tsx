import React, { JSX, memo } from 'react';
import { MediaHierarchySetting } from '@omniflow/omni-webapi';
import { sumBy } from 'lodash';

import MediaHierarchyRunSetting from './MediaHierarchyRunSetting';

export type TMediaHierarchySettingListProps = {
    levelId: number;
    settings: MediaHierarchySetting[];
};

const MediaHierarchyRunSettingList = ({
    levelId,
    settings,
}: TMediaHierarchySettingListProps): JSX.Element => {
    return (
        <>
            <div className="overflow-auto">
                <table className="table is-shadowless" style={{ padding: 0 }}>
                    <tbody className="d-flex flex-column">
                        {settings.map((setting, index) => (
                            <MediaHierarchyRunSetting
                                key={setting.Order}
                                levelId={levelId}
                                settingIndex={index}
                                setting={setting}
                            />
                        ))}
                    </tbody>
                </table>
            </div>
        </>
    );
};

export default memo(MediaHierarchyRunSettingList);
