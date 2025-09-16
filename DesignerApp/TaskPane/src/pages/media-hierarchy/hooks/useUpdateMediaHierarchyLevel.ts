import { useCallback } from 'react';
import { findIndex } from 'lodash';
import * as API from '@omniflow/omni-webapi';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useUpdateMediaHierarchyLevel = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number | number[],
            updatedLevel: API.MediaHierarchyLevel | API.MediaHierarchyLevel[]
        ) => {
            setDraft(({ Definition: draft }) => {
                const levelIds = Array.isArray(levelId) ? levelId : [levelId];
                const updatedLevels = Array.isArray(updatedLevel)
                    ? updatedLevel
                    : [updatedLevel];

                let updated = false;
                levelIds.forEach((Id, index) => {
                    if (updatedLevels[index]) {
                        const levelIdIndex = findIndex(draft.Levels, { Id });

                        if (levelIdIndex >= 0) {
                            draft.Levels[levelIdIndex] = {
                                ...draft.Levels[levelIdIndex],
                                ...updatedLevels[index],
                            };
                            updated = true;
                        }
                    }
                });

                if (updated) {
                    draft.Levels.sort((x, y) => (x.Order > y.Order ? 1 : -1));
                }
            });
        },
        [setDraft]
    );
};

export default useUpdateMediaHierarchyLevel;
