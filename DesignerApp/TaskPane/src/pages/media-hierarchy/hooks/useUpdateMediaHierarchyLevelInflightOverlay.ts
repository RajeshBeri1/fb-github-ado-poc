import { useCallback } from 'react';
import { findIndex } from 'lodash';
import * as API from '@omniflow/omni-webapi';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useUpdateMediaHierarchyLevelInflightOverlay = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            inflightOverlayId: number | number[],
            updatedInflightOverlay:
                | API.MediaHierarchyInflightOverlay
                | API.MediaHierarchyInflightOverlay[]
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
                            const inflightOverlayIds = Array.isArray(inflightOverlayId)
                                ? inflightOverlayId
                                : [inflightOverlayId];
                            const updatedInflightOverlays = Array.isArray(
                                updatedInflightOverlay
                            )
                                ? updatedInflightOverlay
                                : [updatedInflightOverlay];

                            let updated = false;
                            inflightOverlayIds.forEach((Id, index) => {
                                if (
                                    updatedInflightOverlays[index] &&
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].InflightOverlays.length
                                ) {
                                    const inflightOverlayIdIndex = findIndex(
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].InflightOverlays,
                                        { Id }
                                    );

                                    if (inflightOverlayIdIndex >= 0) {
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].InflightOverlays[inflightOverlayIdIndex] = {
                                            ...draft.Levels[levelIdIndex]
                                                .Settings[levelSettingIdIndex]
                                                .InflightOverlays[inflightOverlayIdIndex],
                                            ...updatedInflightOverlays[index],
                                        };
                                        updated = true;
                                    }
                                }
                            });

                            if (updated) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].InflightOverlays.sort((x, y) =>
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

export default useUpdateMediaHierarchyLevelInflightOverlay;
