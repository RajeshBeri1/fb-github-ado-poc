import { useCallback } from 'react';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useOrderMediaHierarchyLevels = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (param: any) => {
            if (!param.destination) {
                return;
            }

            setDraft(({ Definition: draft }) => {
                const srcIdx = param.source.index;
                const desIdx = param.destination.index;
                let updatedLevels = [...draft.Levels];
                // Remove dragged item
                const [reorderedItem] = updatedLevels.splice(srcIdx, 1);
                // Re-Add dropped item
                updatedLevels.splice(desIdx, 0, reorderedItem);
                // Update order
                updatedLevels = updatedLevels.map((level, index) => ({
                    ...level,
                    Order: index,
                }));

                draft.Levels = updatedLevels;
            });
        },
        [setDraft]
    );
};

export default useOrderMediaHierarchyLevels;
