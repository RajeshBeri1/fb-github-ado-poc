import { useCallback } from 'react';
import { findIndex } from 'lodash';
import * as API from '@omniflow/omni-webapi';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useUpdateMediaHierarchyLevelSubTotal = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            subTotalId: number | number[],
            updatedSubTotal:
                | API.MediaHierarchySubTotal
                | API.MediaHierarchySubTotal[]
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
                            const subTotalIds = Array.isArray(subTotalId)
                                ? subTotalId
                                : [subTotalId];
                            const updatedSubTotals = Array.isArray(
                                updatedSubTotal
                            )
                                ? updatedSubTotal
                                : [updatedSubTotal];

                            let updated = false;
                            subTotalIds.forEach((Id, index) => {
                                if (
                                    updatedSubTotals[index] &&
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotals.length
                                ) {
                                    const subTotalIdIndex = findIndex(
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotals,
                                        { Id }
                                    );

                                    if (subTotalIdIndex >= 0) {
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotals[subTotalIdIndex] = {
                                            ...draft.Levels[levelIdIndex]
                                                .Settings[levelSettingIdIndex]
                                                .SubTotals[subTotalIdIndex],
                                            ...updatedSubTotals[index],
                                        };
                                        updated = true;
                                    }
                                }
                            });

                            if (updated) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotals.sort((x, y) =>
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

export default useUpdateMediaHierarchyLevelSubTotal;
