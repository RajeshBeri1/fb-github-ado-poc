import React, { JSX, ChangeEvent, memo, useCallback } from 'react';
import { sumBy } from 'lodash';

import MediaHierarchySubLevelSetting from './MediaHierarchySubLevelSetting';
import useMediaHierarchySubLevelSettings from '../hooks/useMediaHierarchySubLevelSettings';
import useUpdateMediaHierarchySubLevelSetting from '../hooks/useUpdateMediaHierarchySubLevelSetting';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { OmniCheckBoxInput } from '../../../omni/checkbox';

export type TMediaHierarchySubLevelSettingListProps = {
    levelId: number;
    levelSettingId: number;
    subLevelId: number;
    inflightOverlayColumns: TColumn[];
    metricColumns: TColumn[];
};

const MediaHierarchySubLevelSettingList = ({
    levelId,
    levelSettingId,
    subLevelId,
    inflightOverlayColumns,
    metricColumns,
}: TMediaHierarchySubLevelSettingListProps): JSX.Element => {
    const settings = useMediaHierarchySubLevelSettings(
        levelId,
        levelSettingId,
        subLevelId
    );
    const updateSetting = useUpdateMediaHierarchySubLevelSetting();
    const countEnabledSettings = sumBy(settings, (s) => (s.Enabled ? 1 : 0));

    const updateEnabled = useCallback(
        (event: ChangeEvent<HTMLInputElement>) => {
            const value = event.target.checked;

            settings.forEach((setting) => {
                updateSetting(levelId, levelSettingId, subLevelId, setting.Id, {
                    Enabled: value,
                });
            });
        },
        []
    );

    return (
        <>
           
                <table
                className="table table-layout-fixed is-fullwidth is-shadowless"
                    style={{ padding: 0 }}>
                    <thead>
                    <tr className="is-shadowless" >
                        <th style={{ padding: '0 10px' }} colSpan={2}>
                                <OmniCheckBoxInput
                                    checked={
                                        countEnabledSettings === settings.length
                                    }
                                    onChange={(e)=>updateEnabled(e)}
                                />
                            </th>
                        <th className="is-size-6" style={{ padding: 0 }} colSpan={4}>
                                {countEnabledSettings === settings.length ? 'Unselect All' : 'Select All'}
                            </th>
                        <th className="is-size-6" style={{ padding: 0 }} colSpan={6}>
                                Inflight Overlay
                            </th>
                        <th className="is-size-6" style={{ padding: 0 }} colSpan={4}>
                                Metric
                            </th>
                        <th className="is-size-6" style={{ padding: 0 }} colSpan={4}>
                                Flight Range
                            </th>
                        <th className="is-size-6" style={{ padding: 0 }} colSpan={2}>
                                Actions
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        {settings.map((setting) => (
                            <MediaHierarchySubLevelSetting
                                key={setting.Id}
                                levelId={levelId}
                                levelSettingId={levelSettingId}
                                subLevelId={subLevelId}
                                setting={setting}
                                inflightOverlayColumns={inflightOverlayColumns}
                                metricColumns={metricColumns}
                            />
                        ))}
                    </tbody>
              
            </table>
        </>
    );
};

export default memo(MediaHierarchySubLevelSettingList);
