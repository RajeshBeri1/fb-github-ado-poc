import { useCallback } from 'react';
import { findIndex } from 'lodash';

import { useSetRightHandTotalsTemplateDraft } from '../states/RightHandTotalsState';
 
const useRemoveRightHandTotalsRow = (): any => {
    const setDraft = useSetRightHandTotalsTemplateDraft();

    return useCallback(
        (columnId: number | number[]) => {
            setDraft(({ Definition: draft }) => {
                const columnIds = Array.isArray(columnId)
                    ? columnId
                    : [columnId];

                let removed = false;
                columnIds.forEach((Id) => {
                    const columnIdIndex = findIndex(draft.Columns, {
                        Id,
                    });

                    if (columnIdIndex >= 0) {
                        draft.Columns.splice(columnIdIndex, 1);
                        removed = true;
                    }
                });

                if (removed) {
                    draft.Columns.forEach((column, order) => {
                        column.Order = order;
                    });
                }
            });
        },
        [setDraft]
    );
};

export default useRemoveRightHandTotalsRow;
