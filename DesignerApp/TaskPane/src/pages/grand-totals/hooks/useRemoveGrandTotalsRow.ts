import { useCallback } from 'react';
import { findIndex } from 'lodash';

import { useSetGrandTotalsTemplateDraft } from '../states/GrandTotalsState';

const useRemoveGrandTotalsRow = (): any => {
    const setDraft = useSetGrandTotalsTemplateDraft();

    return useCallback(
        (selectionId: number | number[]) => {
            setDraft(({ Definition: draft }) => {
                const selectionIds = Array.isArray(selectionId)
                    ? selectionId
                    : [selectionId];

                let removed = false;
                selectionIds.forEach((Id) => {
                    const selectionIdIndex = findIndex(draft.Selections, {
                        Id,
                    });

                    if (selectionIdIndex >= 0) {
                        draft.Selections.splice(selectionIdIndex, 1);
                        removed = true;
                    }
                });

                if (removed) {
                    draft.Selections.forEach((selection, order) => {
                        selection.Order = order;
                    });
                }
            });
        },
        [setDraft]
    );
};

export default useRemoveGrandTotalsRow;
