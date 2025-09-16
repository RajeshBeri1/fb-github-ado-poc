import { useCallback } from 'react';
import { findIndex } from 'lodash';
import * as API from '@omniflow/omni-webapi';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useUpdateMediaHierarchySubLevelSetting = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            subLevelId: number,
            subLevelSettingId: number | number[],
            updatedSetting:
                | API.MediaHierarchySetting
                | API.MediaHierarchySetting[]
        ) => {
            setDraft(({ Definition: draft }) => {
                if (levelId && levelSettingId && subLevelId) {
                    const levelIdIndex = findIndex(draft.Levels, {
                        Id: levelId,
                    });

                    if (levelIdIndex >= 0) {
                        const levelSettingIdIndex = findIndex(
                            draft.Levels[levelIdIndex].Settings,
                            { Id: levelSettingId }
                        );

                        if (levelSettingIdIndex >= 0) {
                            const subLevelIdIndex = findIndex(
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubLevels,
                                { Id: subLevelId }
                            );

                            if (subLevelIdIndex >= 0) {
                                const subLevelSettingIds = Array.isArray(
                                    subLevelSettingId
                                )
                                    ? subLevelSettingId
                                    : [subLevelSettingId];
                                const updatedSettings = Array.isArray(
                                    updatedSetting
                                )
                                    ? updatedSetting
                                    : [updatedSetting];

                                let updated = false;
                                subLevelSettingIds.forEach((Id, index) => {
                                    if (updatedSettings[index]) {
                                        const subLevelSettingIdIndex =
                                            findIndex(
                                                draft.Levels[levelIdIndex]
                                                    .Settings[
                                                    levelSettingIdIndex
                                                ].SubLevels[subLevelIdIndex]
                                                    .Settings,
                                                { Id }
                                            );

                                        if (subLevelSettingIdIndex >= 0) {
                                            draft.Levels[levelIdIndex].Settings[
                                                levelSettingIdIndex
                                            ].SubLevels[
                                                subLevelIdIndex
                                            ].Settings[subLevelSettingIdIndex] =
                                                {
                                                    ...draft.Levels[
                                                        levelIdIndex
                                                    ].Settings[
                                                        levelSettingIdIndex
                                                    ].SubLevels[subLevelIdIndex]
                                                        .Settings[
                                                        subLevelSettingIdIndex
                                                    ],
                                                    ...updatedSettings[index],
                                                };
                                            updated = true;
                                        }
                                    }
                                });

                                if (updated) {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubLevels[subLevelIdIndex].Settings.sort(
                                        (x, y) => (x.Order > y.Order ? 1 : -1)
                                    );
                                }
                            }
                        }
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useUpdateMediaHierarchySubLevelSetting;
