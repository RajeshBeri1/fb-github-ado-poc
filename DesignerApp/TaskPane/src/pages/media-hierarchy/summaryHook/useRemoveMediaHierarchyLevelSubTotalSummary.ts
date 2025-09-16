import { useCallback } from 'react';
import { findIndex } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useRemoveMediaHierarchyLevelSubTotalSummary = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            summaryId: number | number[]
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
                            const summaryIds = Array.isArray(summaryId)
                                ? summaryId
                                : [summaryId];

                            let removed = false;
                            if (
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary.length
                            ) {
                                summaryIds.forEach((Id) => {
                                    const subTotalIdIndex = findIndex(
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary,
                                        { Id }
                                    );

                                    if (subTotalIdIndex >= 0) {
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary.splice(subTotalIdIndex, 1);
                                        removed = true;
                                    }
                                });
                            }

                            if (removed) {
                                if (
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary.length
                                ) {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary.forEach((subTotal, order) => {
                                        subTotal.Order = order;
                                    });
                                } else {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary = null;
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

export default useRemoveMediaHierarchyLevelSubTotalSummary;
