import { useCallback } from 'react';
import { findIndex } from 'lodash';
import * as API from '@omniflow/omni-webapi';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useUpdateMediaHierarchySubLevel = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            subLevelId: number | number[],
            updatedSubLevel:
                | API.MediaHierarchySubLevel
                | API.MediaHierarchySubLevel[]
        ) => {
            setDraft(({ Definition: draft }) => {
                if (levelId && levelSettingId) {
                    const levelIdIndex = findIndex(draft.Levels, {
                        Id: levelId,
                    });

                    if (levelIdIndex >= 0) {
                        const levelSettingIdIndex = findIndex(
                            draft.Levels[levelIdIndex].Settings,
                            { Id: levelSettingId }
                        );

                        if (levelSettingIdIndex >= 0) {
                            const subLevelIds = Array.isArray(subLevelId)
                                ? subLevelId
                                : [subLevelId];
                            const updatedSubLevels = Array.isArray(
                                updatedSubLevel
                            )
                                ? updatedSubLevel
                                : [updatedSubLevel];

                            let updated = false;
                            subLevelIds.forEach((Id, index) => {
                                if (
                                    updatedSubLevels[index] &&
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubLevels.length
                                ) {
                                    const subLevelIdIndex = findIndex(
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubLevels,
                                        { Id }
                                    );

                                    if (subLevelIdIndex >= 0) {
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubLevels[subLevelIdIndex] = {
                                            ...draft.Levels[levelIdIndex]
                                                .Settings[levelSettingIdIndex]
                                                .SubLevels[subLevelIdIndex],
                                            ...updatedSubLevels[index],
                                        };
                                        updated = true;
                                    }
                                }
                            });

                            if (updated) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubLevels.sort((x, y) =>
                                    x.Order > y.Order ? 1 : -1
                                );
                            }
                        }
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useUpdateMediaHierarchySubLevel;
