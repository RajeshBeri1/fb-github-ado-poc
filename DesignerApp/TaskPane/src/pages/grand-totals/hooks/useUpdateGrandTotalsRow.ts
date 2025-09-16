import { useCallback } from 'react';
import { findIndex } from 'lodash';
import * as API from '@omniflow/omni-webapi';

import { useSetGrandTotalsTemplateDraft } from '../states/GrandTotalsState';

const useUpdateGrandTotalsRow = (): any => {
    const setDraft = useSetGrandTotalsTemplateDraft();

    return useCallback(
        (
            selectionId: number | number[],
            updatedSelection:
                | API.GrandTotalSelection
                | API.GrandTotalSelection[]
        ) => {
            setDraft(({ Definition: draft }) => {
                const selectionIds = Array.isArray(selectionId)
                    ? selectionId
                    : [selectionId];
                const updatedSelections = Array.isArray(updatedSelection)
                    ? updatedSelection
                    : [updatedSelection];

                let updated = false;
                selectionIds.forEach((Id, index) => {
                    if (updatedSelections[index]) {
                        const selectionIdIndex = findIndex(draft.Selections, {
                            Id,
                        });

                        if (selectionIdIndex >= 0) {
                            draft.Selections[selectionIdIndex] = {
                                ...draft.Selections[selectionIdIndex],
                                ...updatedSelections[index],
                            };
                            updated = true;
                        }
                    }
                });

                if (updated) {
                    draft.Selections.sort((x, y) =>
                        x.Order > y.Order ? 1 : -1
                    );
                }
            });
        },
        [setDraft]
    );
};

export default useUpdateGrandTotalsRow;
