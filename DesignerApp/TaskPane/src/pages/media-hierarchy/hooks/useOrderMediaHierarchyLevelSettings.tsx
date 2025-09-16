import { useCallback } from 'react';
import { find, findIndex } from 'lodash';
import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useOrderMediaHierarchyLevelSettings = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (param: any, levelId: number) => {
            if (!param.destination) {
                return;
            }

            setDraft(({ Definition: draft }) => {
                const srcIdx = param.source.index;
                const desIdx = param.destination.index;
                const levelIndex = findIndex(draft.Levels, { Id: levelId });
                let updatedLevelSettings = [...find(draft.Levels, { Id: levelId }).Settings];
                // Remove dragged item
                const [reorderedItem] = updatedLevelSettings.splice(srcIdx, 1);
                // Re-Add dropped item
                updatedLevelSettings.splice(desIdx, 0, reorderedItem);
                // Update order
                updatedLevelSettings = updatedLevelSettings.map((level, index) => ({
                    ...level,
                    Order: index,
                }));

                draft.Levels[levelIndex].Settings = updatedLevelSettings;
            });
        },
        [setDraft]
    );
};

export default useOrderMediaHierarchyLevelSettings;