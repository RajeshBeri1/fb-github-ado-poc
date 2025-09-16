import { useCallback } from 'react';
import { findIndex, omit, isEmpty } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';
import * as API from '@omniflow/omni-webapi';
import { DefaultMetrics } from '../../../business/engine/constant/metric';
import { DefaultStyling } from '../../../business/engine/models/styles';


const useAddMediaHierarchyLevelSubTotalSummary = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            quantity: number = 1,
            initialSubTotalSummary:
                | API.MediaHierarchySubTotalSummary
                | API.MediaHierarchySubTotalSummary[] = [],
            reset: boolean = false,
            tableId: string | null | undefined = null,
            defaultMetrics: DefaultMetrics
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
                            const initialSubTotalSummaries = Array.isArray(
                                initialSubTotalSummary
                            )
                                ? initialSubTotalSummary
                                : [initialSubTotalSummary];

                            if (
                                reset ||
                                isEmpty(draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary)
                            ) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary = [];
                            }

                            let nextId =
                                Math.max(
                                    0,
                                    ...(draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ]?.SubTotalSummary?.map?.((o) => o.Id) ||[])
                                ) + 1;

                            const { MetricColumnName: defaultColumn, MetricTableId: defaultTableId } = tableId == defaultMetrics.Briefed.MetricTableId ? defaultMetrics.Briefed : defaultMetrics.NonBriefed;


                            Array.from({ length: quantity }).forEach(
                                (_t, index) => {
                                    const defaultSubTotals = {
                                        Id: nextId++,
                                        Styling: DefaultStyling(),
                                        TitleStyling: DefaultStyling(),
                                        Order: draft?.Levels?.[levelIdIndex]
                                            ?.Settings?.[levelSettingIdIndex]
                                            ?.SubTotalSummary?.length || 0,
                                        SubTotals: [{
                                            Id: 1,
                                            FlightRange:
                                                API.FlightRange.FlightTotal,
                                            ColumnName:
                                                defaultColumn,
                                            TableId: defaultTableId,
                                            Order: 0,

                                        }],
                                        Settings: [{
                                            Id: 1,
                                            ColumnName: null,
                                            Values: null,
                                            TableId: null,
                                            Order: 0,

                                        }],
                                        DisplayNames: {} as {[key:string]:string}
                                    };

                                    let finalInitialSubTotalSummaries = {};
                                    if (initialSubTotalSummaries[index]) {
                                        finalInitialSubTotalSummaries = omit(
                                            initialSubTotalSummaries[index],
                                            ['Id']
                                        );
                                    }

                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary.push({
                                        ...defaultSubTotals,
                                        ...finalInitialSubTotalSummaries,
                                    });
                                }
                            );

                            if (
                                !draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary.length
                            ) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotalSummary = null;
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
