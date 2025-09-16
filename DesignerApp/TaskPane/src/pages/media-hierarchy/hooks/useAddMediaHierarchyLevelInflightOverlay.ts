import { useCallback } from 'react';
import { findIndex, omit } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';
import * as API from '@omniflow/omni-webapi';
import { DefaultMetric, DefaultMetricMediaBrief } from '../../../business/engine/constant/metric';
import { DefaultStyling } from '../../../business/engine/models/styles';

const defaultColor = {
    Alpha: 1,
    Blue: 1,
    Green: 1,
    Red: 1,
};

const useAddMediaHierarchyLevelInflightOverlay = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            quantity: number = 1,
            initialInflightOverlay:
                | API.MediaHierarchyInflightOverlay
                | API.MediaHierarchyInflightOverlay[] = [],
            reset: boolean = false,
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
                            const initialInflightOverlays = Array.isArray(
                                initialInflightOverlay
                            )
                                ? initialInflightOverlay
                                : [initialInflightOverlay];

                            if (
                                reset ||
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].InflightOverlays === null
                            ) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].InflightOverlays = [];
                            }

                            let nextId =
                                Math.max(
                                    0,
                                    ...draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ]?.InflightOverlays?.map((o) => o.Id)
                                ) + 1;

                            Array.from({ length: quantity }).forEach(
                                (_t, index) => {
                                    const defaultOnflightOverlays = {
                                        Id: nextId++,
                                        ColumnName:
                                            undefined,
                                        TableId: undefined,
                                        Order: draft.Levels[levelIdIndex]
                                            .Settings[levelSettingIdIndex]
                                            .InflightOverlays.length,
                                        Styling: DefaultStyling(),
                                    };

                                    let finalInitialInflightOverlays = {};
                                    if (initialInflightOverlays[index]) {
                                        finalInitialInflightOverlays = omit(
                                            initialInflightOverlays[index],
                                            ['Id']
                                        );
                                    }

                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].InflightOverlays.push({
                                        ...defaultOnflightOverlays,
                                        ...finalInitialInflightOverlays,
                                    });
                                }
                            );

                            if (
                                !draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].InflightOverlays.length
                            ) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].InflightOverlays = null;
                            }
                        }
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useAddMediaHierarchyLevelInflightOverlay;
