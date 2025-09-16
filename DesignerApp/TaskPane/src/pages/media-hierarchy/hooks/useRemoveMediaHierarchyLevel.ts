import { useCallback } from 'react';
import { findIndex } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useRemoveMediaHierarchyLevel = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (levelId: number | number[]) => {
            setDraft(({ Definition: draft }) => {
                const levelIds = Array.isArray(levelId) ? levelId : [levelId];
 
                let removed = false;
                levelIds.forEach((Id) => {
                    const levelIdIndex = findIndex(draft.Levels, { Id });

                    if (levelIdIndex >= 0) {
                        draft.Levels.splice(levelIdIndex, 1);
                        removed = true;
                    }
                });

                if (removed) {
                    draft.Levels.forEach((level, order) => {
                        level.Order = order;
                    });
                }
            });
        },
        [setDraft]
    );
};

export default useRemoveMediaHierarchyLevel;
