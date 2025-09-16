import { useCallback } from 'react';
import { findIndex } from 'lodash';
import * as API from '@omniflow/omni-webapi';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useUpdateMediaHierarchyLevelSubTotalSummary = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            summaryId: number | number[],
            updatedSummary:
                | API.MediaHierarchySubTotalSummary
                | API.MediaHierarchySubTotalSummary[]
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
                            const updatedSummaries = Array.isArray(
                                updatedSummary
                            )
                                ? updatedSummary
                                : [updatedSummary];

                            let updated = false;
                            summaryIds.forEach((Id, index) => {
                                if (
                                    updatedSummaries[index] &&
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary.length
                                ) {
                                    const summaryIdIndex = findIndex(
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary,
                                        { Id }
                                    );

                                    if (summaryIdIndex >= 0) {
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary[summaryIdIndex] = {
                                            ...draft.Levels[levelIdIndex]
                                                .Settings[levelSettingIdIndex]
                                                .SubTotalSummary[summaryIdIndex],
                                            ...updatedSummaries[index],
                                        };
                                        updated = true;
                                    }
                                }
                            });

                            if (updated) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary.sort((x, y) =>
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

export default useUpdateMediaHierarchyLevelSubTotalSummary;
