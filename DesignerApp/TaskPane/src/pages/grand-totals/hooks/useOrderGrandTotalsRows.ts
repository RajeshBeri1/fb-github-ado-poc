import { useCallback } from 'react';

import { useSetGrandTotalsTemplateDraft } from '../states/GrandTotalsState';

const useOrderGrandTotalsRows = (): any => {
    const setDraft = useSetGrandTotalsTemplateDraft();

    return useCallback(
        (param: any) => {
            if (!param.destination) {
                return;
            }

            setDraft(({ Definition: draft }) => {
                const srcIdx = param.source.index;
                const desIdx = param.destination.index;
                let updatedSelections = [...draft.Selections];
                // Remove dragged item
                const [reorderedItem] = updatedSelections.splice(srcIdx, 1);
                // Re-Add dropped item
                updatedSelections.splice(desIdx, 0, reorderedItem);
                // Update order
                updatedSelections = updatedSelections.map((row, index) => ({
                    ...row,
                    Order: index,
                }));

                draft.Selections = updatedSelections;
            });
        },
        [setDraft]
    );
};

export default useOrderGrandTotalsRows;
