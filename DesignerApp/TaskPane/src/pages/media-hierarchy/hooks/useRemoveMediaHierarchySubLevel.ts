import { useCallback } from 'react';
import { findIndex } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useRemoveMediaHierarchySubLevel = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            subLevelId: number | number[]
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

                            let removed = false;
                            if (
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubLevels.length
                            ) {
                                subLevelIds.forEach((Id) => {
                                    const subLevelIdIndex = findIndex(
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubLevels,
                                        { Id }
                                    );

                                    if (subLevelIdIndex >= 0) {
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubLevels.splice(subLevelIdIndex, 1);
                                        removed = true;
                                    }
                                });
                            }

                            if (removed) {
                                if (
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubLevels.length
                                ) {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubLevels.forEach((subLevel, order) => {
                                        subLevel.Order = order;
                                    });
                                } else {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubLevels = null;
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

export default useRemoveMediaHierarchySubLevel;
