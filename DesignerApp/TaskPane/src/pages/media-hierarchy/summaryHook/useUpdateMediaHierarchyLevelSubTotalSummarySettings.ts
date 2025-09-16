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
            settingId: number | number[],
            updatedSummarySetting:
                | API.MediaHierarchySubTotalSummarySettingBase
                | API.MediaHierarchySubTotalSummarySettingBase[]
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
                            const summarySettingIds = Array.isArray(settingId)
                                ? settingId
                                : [settingId];

                            const updatedSummarySettings = Array.isArray(
                                updatedSummarySetting
                            )
                                ? updatedSummarySetting
                                : [updatedSummarySetting];


                            let updated = false;
                            summarySettingIds.forEach((Id, index) => {
                                if (
                                    updatedSummarySettings[index] &&
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summarySettingsIdIndex].Settings.length

                                ) {
                                    const settingsIdIndex = findIndex(
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary[summarySettingsIdIndex].Settings,
                                        { Id }
                                    );

                                    if (settingsIdIndex >= 0) {
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubTotalSummary[summarySettingsIdIndex].Settings[settingsIdIndex] = {
                                            ...draft.Levels[levelIdIndex]
                                                .Settings[levelSettingIdIndex]
                                                .SubTotalSummary[summarySettingsIdIndex].Settings[settingsIdIndex],
                                            ...updatedSummarySettings[index],
                                        };
                                        updated = true;
                                    }
                                }
                            });

                            if (updated) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[summarySettingsIdIndex].Settings.sort((x, y) =>
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
