import { useCallback } from 'react';
import { find, findIndex } from 'lodash';
import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useOrderMediaHierarchyLevelInflightOverlays = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (param: any, levelId: number,settingId:number) => {
            if (!param.destination) {
                return;
            }

            setDraft(({ Definition: draft }) => {
                const srcIdx = param.source.index;
                const desIdx = param.destination.index;
                const levelIndex = findIndex(draft.Levels, { Id: levelId });
                const settingIndex = findIndex(draft.Levels[levelIndex].Settings, { Id: settingId });


                let updatedInflightOverlays = [...find(draft.Levels[levelIndex].Settings, { Id: settingId }).InflightOverlays];
                // Remove dragged item
                const [reorderedItem] = updatedInflightOverlays.splice(srcIdx, 1);
                // Re-Add dropped item
                updatedInflightOverlays.splice(desIdx, 0, reorderedItem);
                // Update order
                updatedInflightOverlays = updatedInflightOverlays.map((level, index) => ({
                    ...level,
                    Order: index,
                }));

                draft.Levels[levelIndex].Settings[settingIndex].InflightOverlays = updatedInflightOverlays;
            });
        },
        [setDraft]
    );
};

export default useOrderMediaHierarchyLevelInflightOverlays;