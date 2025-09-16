import { useCallback } from 'react';
import { findIndex, omit, isEmpty } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';
import * as API from '@omniflow/omni-webapi';
import { DefaultMetrics } from '../../../business/engine/constant/metric';

const useAddMediaHierarchyLevelSummarySettings = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            quantity: number = 1,
            initialSummarySettings:
                | API.MediaHierarchySubTotalSummarySettingBase
                | API.MediaHierarchySubTotalSummarySettingBase[] = [],
            reset: boolean = false,
            tableId: string | null | undefined = null,
            defaultMetrics: DefaultMetrics,
            summaryId: number,
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
                            const summaryIndex = findIndex(draft.Levels[levelIdIndex].Settings[
                                levelSettingIdIndex
                            ].SubTotalSummary, { Id: summaryId }
                            )
                            if (summaryIndex >= 0) {
                                if (isEmpty(draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[summaryIndex].Settings)) {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summaryIndex].Settings = [];
                                }
                            }
                            const initialSummarySubTotals = Array.isArray(
                                initialSummarySettings
                            )
                                ? initialSummarySettings
                                : [initialSummarySettings];



                            let nextId =
                                Math.max(
                                    0,
                                    ...(draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ]?.SubTotalSummary?.[summaryIndex]?.Settings?.map?.((o) => o.Id) || [])
                                ) + 1;

                            const { MetricColumnName: defaultColumn, MetricTableId: defaultTableId } = tableId == defaultMetrics.Briefed.MetricTableId ? defaultMetrics.Briefed : defaultMetrics.NonBriefed;


                            Array.from({ length: quantity }).forEach(
                                (_t, index) => {
                                    const defaultSummarySettings = {
                                        Id: nextId++,
                                        ColumnName:null,
                                        Values:null ,
                                        TableId: null,
                                        Order: draft?.Levels?.[levelIdIndex]
                                            ?.Settings?.[levelSettingIdIndex]
                                            ?.SubTotalSummary?.[summaryIndex]?.Settings?.length || 0, 

                                    };

                                    let finalInitialSummarySettings = {};
                                    if (initialSummarySubTotals[index]) {
                                        finalInitialSummarySettings = omit(
                                            initialSummarySubTotals[index],
                                            ['Id']
                                        );
                                    }

                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summaryIndex].Settings.push({
                                        ...defaultSummarySettings,
                                        ...finalInitialSummarySettings,
                                    });
                                }
                            );

                            if (
                                !draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[summaryIndex].Settings.length
                            ) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[summaryIndex].Settings = null;
                            }
                        }
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useAddMediaHierarchyLevelSummarySettings;
