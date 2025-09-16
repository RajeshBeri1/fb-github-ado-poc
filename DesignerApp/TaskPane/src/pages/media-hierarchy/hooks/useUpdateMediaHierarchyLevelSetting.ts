import { useCallback } from 'react';
import { findIndex } from 'lodash';
import * as API from '@omniflow/omni-webapi';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useUpdateMediaHierarchyLevelSetting = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number | number[],
            updatedSetting:
                | API.MediaHierarchySetting
                | API.MediaHierarchySetting[]
        ) => {
            setDraft(({ Definition: draft }) => {
                if (levelId) {
                    const levelIdIndex = findIndex(draft.Levels, {
                        Id: levelId,
                    });

                    if (levelIdIndex >= 0) {
                        const levelSettingIds = Array.isArray(levelSettingId)
                            ? levelSettingId
                            : [levelSettingId];
                        const updatedSettings = Array.isArray(updatedSetting)
                            ? updatedSetting
                            : [updatedSetting];

                        let updated = false;
                        levelSettingIds.forEach((Id, index) => {
                            if (updatedSettings[index]) {
                                const levelSettingIdIndex = findIndex(
                                    draft.Levels[levelIdIndex].Settings,
                                    { Id }
                                );

                                if (levelSettingIdIndex >= 0) {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ] = {
                                        ...draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ],
                                        ...updatedSettings[index],
                                    };
                                    updated = true;
                                }
                            }
                        });

                        if (updated) {
                            draft.Levels[levelIdIndex].Settings.sort((x, y) =>
                                x.Order > y.Order ? 1 : -1
                            );
                        }
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useUpdateMediaHierarchyLevelSetting;
