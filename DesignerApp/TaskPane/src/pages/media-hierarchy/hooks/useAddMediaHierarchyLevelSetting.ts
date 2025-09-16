import { useCallback } from 'react';
import { cloneDeep, findIndex, omit } from 'lodash';
import * as API from '@omniflow/omni-webapi';

import {
    MediaHierarchyDefinitionWithId,
    useSetMediaHierarchyTemplateDraft,
} from '../states/MediaHierarchyState';
import { DefaultMetrics } from '../../../business/engine/constant/metric';
import { DefaultStyling } from '../../../business/engine/models/styles';
import { MediaHierarchyInflightOverlay } from '@omniflow/omni-webapi';

const useAddMediaHierarchyLevelSetting = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            channelName: string | null | (string | null)[] = null,
            initialSetting:
                | API.MediaHierarchySetting
                | API.MediaHierarchySetting[] = [],
            reset: boolean = false,
            tableId: string | null | undefined = null,
            defaultMetrics: DefaultMetrics,
            previousInflightOverlays: Array<{ name: string, inflightOverlays: MediaHierarchyInflightOverlay[] }> = [],
            previousLevelData: any[] = [],
            levels: any[] = [], 
            levelIndex: number = 0 
        ): MediaHierarchyDefinitionWithId => {
            let newDefinition;

            setDraft(({ Definition: draft }) => {
                if (levelId) {
                    const levelIdIndex = findIndex(draft.Levels, {
                        Id: levelId,
                    });

                    if (levelIdIndex >= 0) {
                        const channelNames = Array.isArray(channelName)
                            ? channelName
                            : [channelName];
                        const initialSettings = Array.isArray(initialSetting)
                            ? initialSetting
                            : [initialSetting];

                        if (reset) draft.Levels[levelIdIndex].Settings = [];

                        let nextId =
                            Math.max(
                                0,
                                ...draft.Levels[levelIdIndex]?.Settings?.map(
                                    (o) => o.Id
                                )
                            ) + 1;
                        const { MetricColumnName: defaultColumn, MetricTableId: defaultTableId } = tableId == defaultMetrics.Briefed.MetricTableId ? defaultMetrics.Briefed : defaultMetrics.NonBriefed;
                        const getDefaultInflightOverlays = (name: string) => {
                            return initialSettings.filter(c => c != null && c?.Name != null).length > 0 || previousInflightOverlays.length < 1 || previousInflightOverlays.findIndex(c => c.name == name) < 0 ? null : [...previousInflightOverlays.find(c => c.name == name)?.inflightOverlays?.map(c => { return { ...c, Styling: DefaultStyling() }; })];
                        }
                        // fix for new added values after the save
                        let orderLength = initialSettings.length && channelNames.length && initialSettings.filter((setting) => channelNames.includes(setting.Name)).length ? initialSettings.filter((setting) => channelNames.includes(setting.Name)).map(c => c.Order).sort((a, b) => b - a).find(a => a > -1) + 1 : 0;
                        const initialOrder = orderLength;
                        channelNames.forEach((Name, index) => {
                            // --- METRIC INHERITANCE LOGIC START ---
                            let inheritedMetric: { MetricColumnName?: string; MetricTableId?: string } | null = null;
                            if (levelIndex > 0 && previousLevelData && previousLevelData.length > 0 && levels && levels.length > 0) {
                                // Find parent value for this child (Name)
                               
                                const parentEntry = previousLevelData.find((d) => d.Column1 === Name);
                                if (parentEntry) {
                                    // Find parent level
                                    const parentLevel = levels[levelIndex - 1];
                                    if (parentLevel && parentLevel.Settings) {
                                        // Find parent setting by name (Column2)
                                        const parentSetting = parentLevel.Settings.find((s) => s.Name === parentEntry.Column2);
                                        if (parentSetting && parentSetting.MetricColumnName && parentSetting.MetricTableId) {
                                            inheritedMetric = {
                                                MetricColumnName: parentSetting.MetricColumnName,
                                                MetricTableId: parentSetting.MetricTableId
                                            };
                                        }
                                    }
                                }
                            }
                            // --- METRIC INHERITANCE LOGIC END ---

                            const defaultSettings = {
                                Id: nextId++,
                                Enabled: initialOrder > 0 ? false : true,// unselected when new values are added
                                FlightRange: API.FlightRange.FlightTotal,
                                InflightOverlayColumnName: null,
                                InflightOverlayTableId: null,
                                MetricColumnName:
                                    inheritedMetric?.MetricColumnName || defaultColumn,
                                MetricTableId: inheritedMetric?.MetricTableId || defaultTableId,
                                Name,
                                Order: draft.Levels[levelIdIndex].Settings
                                    .length,
                                Styling: DefaultStyling(),
                                InflightOverlayStyling: DefaultStyling(),
                                SubLevels: null,
                                SubTotals: null,
                                InflightOverlays: getDefaultInflightOverlays(Name),
                                SubTotalSummary:null,
                            };
                            let finalInitialSettings = {};
                            let channelIndex = initialSettings.findIndex(c => c.Name == Name);
                            if (channelIndex > -1) {
                                finalInitialSettings = omit(
                                    initialSettings[channelIndex],
                                    ['Id']
                                );
                            } else {
                                // For reorder
                                finalInitialSettings = { ...finalInitialSettings, Order: orderLength++ };
                            }

                            draft.Levels[levelIdIndex].Settings.push({
                                ...defaultSettings,
                                ...finalInitialSettings,
                            });
                        });
                        // For order
                        draft.Levels[levelIdIndex].Settings = draft.Levels[levelIdIndex].Settings.sort((a, b) => a.Order - b.Order);
                    }
                }

                newDefinition = cloneDeep(draft ?? null);
            });

            return newDefinition;
        },
        [setDraft]
    );
};

export default useAddMediaHierarchyLevelSetting;
