import { useCallback } from 'react';
import { findIndex } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useRemoveMediaHierarchyLevelInflightOverlay = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            inflightOverlayId: number | number[]
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
                            const infloghtOverlayIds = Array.isArray(inflightOverlayId)
                                ? inflightOverlayId
                                : [inflightOverlayId];

                            let removed = false;
                            if (
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].InflightOverlays.length
                            ) {
                                infloghtOverlayIds.forEach((Id) => {
                                    const inflightOverlayIdIndex = findIndex(
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].InflightOverlays,
                                        { Id }
                                    );

                                    if (inflightOverlayIdIndex >= 0) {
                                        draft.Levels[levelIdIndex].Settings[
                                            levelSettingIdIndex
                                        ].InflightOverlays.splice(inflightOverlayIdIndex, 1);
                                        removed = true;
                                    }
                                });
                            }

                            if (removed) {
                                if (
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].InflightOverlays.length
                                ) {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].InflightOverlays.forEach((inflightOverlay, order) => {
                                        inflightOverlay.Order = order;
                                    });
                                } else {
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].InflightOverlays = null;
                                }
                            }
                        }
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useRemoveMediaHierarchyLevelInflightOverlay;
