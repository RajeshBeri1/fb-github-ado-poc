import { useCallback } from 'react';
import { findIndex } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useRemoveMediaHierarchyLevelSubTotalSummarySubtotal = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            summarySettingsId: number | number[],
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
                            const summarySettingsIds = Array.isArray(summarySettingsId)
                                ? summarySettingsId
                                : [summarySettingsId];

                            let removed = false;
                            if (
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[summarySubTotalIdIndex].Settings.length
                            ) {
                                summarySettingsIds.forEach((Id) => {
                                    const settingsIdIndex = findIndex(
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary[summarySubTotalIdIndex].Settings,
                                        { Id }
                                    );

                                    if (settingsIdIndex >= 0) {
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary[summarySubTotalIdIndex].Settings.splice(settingsIdIndex, 1);
                                        removed = true;
                                    }
                                });
                            }

                            if (removed) {
                                if (
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summarySubTotalIdIndex].Settings.length
                                ) {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summarySubTotalIdIndex].Settings.forEach((subTotal, order) => {
                                        subTotal.Order = order;
                                    });
                                } else {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summarySubTotalIdIndex].Settings = null;
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
