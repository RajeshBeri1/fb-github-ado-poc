import { useCallback } from 'react';
import { find, findIndex } from 'lodash';
import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useSortMediaHierarchyLevelSettings = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (levelId: number, sortType: string) => {
            if (!sortType) {
                return;
            }

            setDraft(({ Definition: draft }) => {
                const levelIndex = findIndex(draft.Levels, { Id: levelId });
                let updatedLevelSettings = [...find(draft.Levels, { Id: levelId }).Settings];
                // Sort items based on sort type (e.g. ascending, descending)
                updatedLevelSettings.sort((a, b) => {
                    return sortType === 'asc' ? a.Name.localeCompare(b.Name) : b.Name.localeCompare(a.Name);
                });

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

export default useSortMediaHierarchyLevelSettings;