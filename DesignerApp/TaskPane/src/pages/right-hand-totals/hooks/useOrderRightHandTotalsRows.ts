import { useCallback } from 'react';

import { useSetRightHandTotalsTemplateDraft } from '../states/RightHandTotalsState';

const useOrderRightHandTotalsRows = (): any => {
    const setDraft = useSetRightHandTotalsTemplateDraft();

    return useCallback(
        (param: any) => {
            if (!param.destination) {
                return;
            }

            setDraft(({ Definition: draft }) => {
                const srcIdx = param.source.index;
                const desIdx = param.destination.index;
                let updatedColumns = [...draft.Columns];
                // Remove dragged item
                const [reorderedItem] = updatedColumns.splice(srcIdx, 1);
                // Re-Add dropped item
                updatedColumns.splice(desIdx, 0, reorderedItem);
                // Update order
                updatedColumns = updatedColumns.map((row, index) => ({
                    ...row,
                    Order: index,
                }));

                draft.Columns = updatedColumns;
            });
        },
        [setDraft]
    );
};

export default useOrderRightHandTotalsRows;
