import { useCallback } from 'react';
import { findIndex, omit } from 'lodash';
import * as API from '@omniflow/omni-webapi';
import { DefaultStyling } from '../../../business/engine/models/styles';


import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';
import { DefaultMetrics } from '../../../business/engine/constant/metric';

const defaultColor = {
    Alpha: 1,
    Blue: 1,
    Green: 1,
    Red: 1,
};

const useAddMediaHierarchySubLevelSetting = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            subLevelId: number,
            channelName: string | null | (string | null)[] = null,
            initialSetting:
                | API.MediaHierarchySubLevelSetting
                | API.MediaHierarchySubLevelSetting[] = [],
            reset: boolean = false,
            tableId: string | null | undefined = null,
            defaultMetrics: DefaultMetrics
        ) => {
            setDraft(({ Definition: draft }) => {
                if (levelId) {
                    const levelIdIndex = findIndex(draft.Levels, {
                        Id: levelId,
                    });

                    if (levelIdIndex >= 0) {
                        const levelSettingIdIndex = findIndex(
                            draft.Levels[levelIdIndex].Settings,
                            { Id: levelSettingId }
                        );

                        if (levelSettingIdIndex >= 0) {
                            const subLevelIdIndex = findIndex(
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubLevels,
                                { Id: subLevelId }
                            );

                            if (subLevelIdIndex >= 0) {
                                const channelNames = Array.isArray(channelName)
                                    ? channelName
                                    : [channelName];
                                const initialSettings = Array.isArray(
                                    initialSetting
                                )
                                    ? initialSetting
                                    : [initialSetting];

                                if (reset)
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubLevels[subLevelIdIndex].Settings = [];

                                let nextId =
                                    Math.max(
                                        0,
                                        ...draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].SubLevels[
                                            subLevelIdIndex
                                        ]?.Settings?.map((o) => o.Id)
                                    ) + 1;

                                const { MetricColumnName: defaultColumn, MetricTableId: defaultTableId } = tableId == defaultMetrics.Briefed.MetricTableId ? defaultMetrics.Briefed : defaultMetrics.NonBriefed;


                                channelNames.forEach((Name, index) => {
                                    const defaultSettings = {
                                        Id: nextId++,
                                        Enabled: true,
                                        FlightRange:
                                            API.FlightRange.FlightTotal,
                                        InflightOverlayColumnName: null,
                                        InflightOverlayTableId: null,
                                        MetricColumnName:
                                            defaultColumn,
                                        MetricTableId:
                                            defaultTableId,
                                        Name,
                                        Order: draft.Levels[levelIdIndex]
                                            .Settings[levelSettingIdIndex]
                                            .SubLevels[subLevelIdIndex].Settings
                                            .length,
                                        Styling: DefaultStyling(),
                                        InflightOverlayStyling: DefaultStyling(),
                                        SubTotals: null,
                                    };

                                    let finalInitialSettings = {};
                                    if (initialSettings[index]) {
                                        finalInitialSettings = omit(
                                            initialSettings[index],
                                            ['Id']
                                        );
                                    }

                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubLevels[subLevelIdIndex].Settings.push({
                                        ...defaultSettings,
                                        ...finalInitialSettings,
                                    });
                                });
                            }
                        }
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useAddMediaHierarchySubLevelSetting;
