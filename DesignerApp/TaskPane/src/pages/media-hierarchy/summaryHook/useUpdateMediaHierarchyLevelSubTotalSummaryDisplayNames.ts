import { useCallback } from 'react';
import { findIndex } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useUpdateMediaHierarchyLevelSubTotalSummaryDisplayNames = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            summaryId: number | number[],
            updatedSummaryDisplayName: { key: string, value: string }
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
                            const summarySettingsIdIndex = findIndex(
                                draft.Levels[levelIdIndex].Settings[levelSettingIdIndex].SubTotalSummary,
                                { Id: summaryId }
                            );

                            if (summarySettingsIdIndex >= 0) {
                                const displayNames = draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[summarySettingsIdIndex].DisplayNames;

                                if (!displayNames) {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summarySettingsIdIndex].DisplayNames = {};
                                }

                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[summarySettingsIdIndex].DisplayNames[updatedSummaryDisplayName.key] = updatedSummaryDisplayName?.value ?? '';
                            }
                        };
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useUpdateMediaHierarchyLevelSubTotalSummaryDisplayNames;
