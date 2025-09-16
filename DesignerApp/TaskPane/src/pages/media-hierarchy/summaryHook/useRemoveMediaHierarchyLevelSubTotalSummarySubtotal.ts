import { useCallback } from 'react';
import { findIndex } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useRemoveMediaHierarchyLevelSubTotalSummarySubtotal = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            summarySubTotalId: number | number[],
            summaryId: number | number[],
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
                            const summarySubTotalIdIndex = findIndex(
                                draft.Levels[levelIdIndex].Settings[levelSettingIdIndex].SubTotalSummary,
                                { Id: summaryId }
                            );
                            const summarySubTotalIds = Array.isArray(summarySubTotalId)
                                ? summarySubTotalId
                                : [summarySubTotalId];

                            let removed = false;
                            if (
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[summarySubTotalIdIndex].SubTotals.length
                            ) {
                                summarySubTotalIds.forEach((Id) => {
                                    const subTotalIdIndex = findIndex(
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary[summarySubTotalIdIndex].SubTotals,
                                        { Id }
                                    );

                                    if (subTotalIdIndex >= 0) {
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary[summarySubTotalIdIndex].SubTotals.splice(subTotalIdIndex, 1);
                                        removed = true;
                                    }
                                });
                            }

                            if (removed) {
                                if (
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summarySubTotalIdIndex].SubTotals.length
                                ) {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summarySubTotalIdIndex].SubTotals.forEach((subTotal, order) => {
                                        subTotal.Order = order;
                                    });
                                } else {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summarySubTotalIdIndex].SubTotals = null;
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

export default useRemoveMediaHierarchyLevelSubTotalSummarySubtotal;
