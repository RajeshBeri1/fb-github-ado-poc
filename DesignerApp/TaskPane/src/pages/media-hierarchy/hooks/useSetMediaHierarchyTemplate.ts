import { useCallback } from 'react';
import { cloneDeep } from 'lodash';

import {
    MediaHierarchyDefinitionWithId,
    MediaHierarchyTemplate,
    useSetMediaHierarchyTemplateDraft,
} from '../states/MediaHierarchyState';

const useAddMediaHierarchyTemplate = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (template: MediaHierarchyTemplate): MediaHierarchyDefinitionWithId => {
            const levelsWithIds = template?.Definition?.Levels?.map(
                (level, levelIndex) => {
                    const settingsWithIds = level?.Settings?.map(
                        (setting, settingIndex) => {
                            const subLevelsWithIds = setting?.SubLevels?.map(
                                (subLevel, subLevelIndex) => {
                                    const subLevelSettingsWithIds =
                                        subLevel?.Settings?.map(
                                            (
                                                subLevelSetting,
                                                subLevelSettingIndex
                                            ) => ({
                                                ...subLevelSetting,
                                                Id: subLevelSettingIndex + 1,
                                                Order: subLevelSettingIndex,
                                            })
                                        );

                                    return {
                                        ...subLevel,
                                        Settings: subLevelSettingsWithIds,
                                        Id: subLevelIndex + 1,
                                        Order: subLevelIndex,
                                    };
                                }
                            );

                            const subTotalsWithIds = setting?.SubTotals?.map(
                                (subTotal, subTotalIndex) => ({
                                    ...subTotal,
                                    Id: subTotalIndex + 1,
                                    Order: subTotalIndex,
                                })
                            );

                            const inflightOverlaysWithIds = setting?.InflightOverlays?.map(
                                (inflightOverlay, inflightOverlayIndex) => ({
                                    ...inflightOverlay,
                                    Id: inflightOverlayIndex + 1,
                                    Order: inflightOverlayIndex,
                                })
                            );

                            const subTotalSummaryIds = setting?.SubTotalSummary?.map(
                                (subTotalSummary, subTotalSummaryIndex) => {

                                    const subTotalsSummarySettingWithIds = subTotalSummary?.Settings?.map(
                                        (subTotalSetting, subTotalSettingIndex) => ({
                                            ...subTotalSetting,
                                            Id: subTotalSettingIndex + 1,
                                            Order: subTotalSettingIndex,
                                        })
                                    );

                                    const subTotalsSummarySubTotalWithIds = subTotalSummary?.SubTotals?.map(
                                        (subTotal, subTotalIndex) => ({
                                            ...subTotal,
                                            Id: subTotalIndex + 1,
                                            Order: subTotalIndex,
                                        })
                                    );

                                    return {
                                        ...subTotalSummary,
                                        Id: subTotalSummaryIndex + 1,
                                        Order: subTotalSummaryIndex,
                                        Settings: subTotalsSummarySettingWithIds,
                                        SubTotals: subTotalsSummarySubTotalWithIds,
                                    }
                                }
                            );

                            return {
                                ...setting,
                                SubLevels: subLevelsWithIds,
                                SubTotals: subTotalsWithIds,
                                Id: settingIndex + 1,
                                Order: settingIndex,
                                InflightOverlays: inflightOverlaysWithIds,
                                SubTotalSummary: subTotalSummaryIds,
                            };
                        }
                    );

                    return {
                        ...level,
                        Settings: settingsWithIds,
                        Id: levelIndex + 1,
                        Order: levelIndex,
                    };
                }
            );

            const Definition = {
                ...template?.Definition,
                Levels: levelsWithIds,
            };
            setDraft(() => ({
                ...template,
                Definition,
            }));

            return cloneDeep(Definition ?? null);
        },
        [setDraft]
    );
};

export default useAddMediaHierarchyTemplate;