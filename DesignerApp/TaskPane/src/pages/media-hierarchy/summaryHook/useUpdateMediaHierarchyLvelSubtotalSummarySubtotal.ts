import { useCallback } from 'react';
import { findIndex } from 'lodash';
import * as API from '@omniflow/omni-webapi';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useUpdateMediaHierarchyLevelSubTotalSummarySubtotal = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            summaryId: number | number[],
            subtotalId: number | number[],
            updatedSubTotal:
                | API.MediaHierarchySubTotalBase
                | API.MediaHierarchySubTotalBase[]
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
                            const subTotalSummaryIdIndex = findIndex(
                                draft.Levels[levelIdIndex].Settings[levelSettingIdIndex].SubTotalSummary,
                                { Id: summaryId }
                            );
                            const subtotalIds = Array.isArray(subtotalId)
                                ? subtotalId
                                : [subtotalId];

                            const updatedSubTotals = Array.isArray(
                                updatedSubTotal
                            )
                                ? updatedSubTotal
                                : [updatedSubTotal];
                             

                            let updated = false;
                            subtotalIds.forEach((Id, index) => {
                                if (
                                    updatedSubTotals[index] &&
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[subTotalSummaryIdIndex].SubTotals.length

                                ) {
                                    const subTotalIdIndex = findIndex(
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary[subTotalSummaryIdIndex].SubTotals,
                                        { Id }
                                    );

                                    if (subTotalIdIndex >= 0) {
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary[subTotalSummaryIdIndex].SubTotals[subTotalIdIndex] = {
                                            ...draft.Levels[levelIdIndex]
                                                .Settings[levelSettingIdIndex]
                                                .SubTotalSummary[subTotalSummaryIdIndex].SubTotals[subTotalIdIndex],
                                            ...updatedSubTotals[index],
                                        };
                                        updated = true;
                                    }
                                }
                            });

                            if (updated) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[subTotalSummaryIdIndex].SubTotals.sort((x, y) =>
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

export default useUpdateMediaHierarchyLevelSubTotalSummarySubtotal;
