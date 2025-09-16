import { useCallback } from 'react';
import { findIndex, omit } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';
import * as API from '@omniflow/omni-webapi';
import { DefaultMetrics } from '../../../business/engine/constant/metric';
import { DefaultStyling } from '../../../business/engine/models/styles';

const defaultColor = {
    Alpha: 1,
    Blue: 1,
    Green: 1,
    Red: 1,
};

const useAddMediaHierarchyLevelSubTotal = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            quantity: number = 1,
            initialSubTotal:
                | API.MediaHierarchySubTotal
                | API.MediaHierarchySubTotal[] = [],
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
                            const initialSubTotals = Array.isArray(
                                initialSubTotal
                            )
                                ? initialSubTotal
                                : [initialSubTotal];

                            if (
                                reset ||
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotals === null
                            ) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotals = [];
                            }

                            let nextId =
                                Math.max(
                                    0,
                                    ...draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ]?.SubTotals?.map((o) => o.Id)
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
                                        Order: draft.Levels[levelIdIndex]
                                            .Settings[levelSettingIdIndex]
                                            .SubTotals.length,
                                        Styling: DefaultStyling(),
                                        TitleStyling: DefaultStyling(),
                                    };

                                    let finalInitialSubTotals = {};
                                    if (initialSubTotals[index]) {
                                        finalInitialSubTotals = omit(
                                            initialSubTotals[index],
                                            ['Id']
                                        );
                                    }

                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotals.push({
                                        ...defaultSubTotals,
                                        ...finalInitialSubTotals,
                                    });
                                }
                            );

                            if (
                                !draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotals.length
                            ) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubTotals = null;
                            }
                        }
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useAddMediaHierarchyLevelSubTotal;
