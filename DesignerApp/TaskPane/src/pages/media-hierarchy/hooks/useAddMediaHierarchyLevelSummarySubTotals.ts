import { useCallback } from 'react';
import { findIndex, omit, isEmpty } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';
import * as API from '@omniflow/omni-webapi';
import { DefaultMetrics } from '../../../business/engine/constant/metric';

const useAddMediaHierarchyLevelSubTotalSummary = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            quantity: number = 1,
            initialSummarySubTotal:
                | API.MediaHierarchySubTotalBase
                | API.MediaHierarchySubTotalBase[] = [],
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
                            ].SubTotalSummary, {Id: summaryId}
                            )
                            if (summaryIndex >= 0) {
                                if (isEmpty(draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[summaryIndex].SubTotals)) {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summaryIndex].SubTotals = [];
                                }
                            }
                            const initialSummarySubTotals = Array.isArray(
                                initialSummarySubTotal
                            )
                                ? initialSummarySubTotal
                                : [initialSummarySubTotal];

                            

                            let nextId =
                                Math.max(
                                    0,
                                    ...(draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ]?.SubTotalSummary?.[summaryIndex]?.SubTotals?.map?.((o) => o.Id) || [])
                                ) + 1;

                            const { MetricColumnName: defaultColumn, MetricTableId: defaultTableId } = tableId == defaultMetrics.Briefed.MetricTableId ? defaultMetrics.Briefed : defaultMetrics.NonBriefed;


                            Array.from({ length: quantity }).forEach(
                                (_t, index) => {
                                    const defaultSubTotals = {
                                        Id: nextId++,
                                        FlightRange:
                                            API.FlightRange.FlightTotal,
                                        ColumnName:
                                            defaultColumn,
                                        TableId: defaultTableId,
                                        Order: draft?.Levels?.[levelIdIndex]
                                            ?.Settings?.[levelSettingIdIndex]
                                            ?.SubTotalSummary?.[summaryIndex]?.SubTotals?.length || 0,
                                       
                                    };

                                    let finalInitialSummarySubTotals = {};
                                    if (initialSummarySubTotals[index]) {
                                        finalInitialSummarySubTotals = omit(
                                            initialSummarySubTotals[index],
                                            ['Id']
                                        );
                                    }

                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summaryIndex].SubTotals.push({
                                        ...defaultSubTotals,
                                        ...finalInitialSummarySubTotals,
                                    });
                                }
                            );

                            if (
                                !draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[summaryIndex].SubTotals.length
                            ) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary[summaryIndex].SubTotals = null;
                            }
                        }
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useAddMediaHierarchyLevelSubTotalSummary;
