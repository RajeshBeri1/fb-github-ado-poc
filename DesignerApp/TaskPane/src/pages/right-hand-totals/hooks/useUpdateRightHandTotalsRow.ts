import { useCallback } from 'react';
import * as API from '@omniflow/omni-webapi';
import { findIndex } from 'lodash';

import { useSetRightHandTotalsTemplateDraft } from '../states/RightHandTotalsState';

const useUpdateRightHandTotalsRow = (): any => {
    const setDraft = useSetRightHandTotalsTemplateDraft();

    return useCallback(
        (
            columnId: number | number[],
            updatedColumn: API.TotalsColumn | API.TotalsColumn[]
        ) => {
            setDraft(({ Definition: draft }) => {
                const columnIds = Array.isArray(columnId)
                    ? columnId
                    : [columnId];
                const updatedColumns = Array.isArray(updatedColumn)
                    ? updatedColumn
                    : [updatedColumn];

                let updated = false;
                columnIds.forEach((Id, index) => {
                    if (updatedColumns[index]) {
                        const columnIdIndex = findIndex(draft.Columns, {
                            Id,
                        });

                        if (columnIdIndex >= 0) {
                            draft.Columns[columnIdIndex] = {
                                ...draft.Columns[columnIdIndex],
                                ...updatedColumns[index],
                            };
                            updated = true;
                        }
                    }
                });

                if (updated) {
                    draft.Columns.sort((x, y) => (x.Order > y.Order ? 1 : -1));
                }
            });
        },
        [setDraft]
    );
};

export default useUpdateRightHandTotalsRow;
