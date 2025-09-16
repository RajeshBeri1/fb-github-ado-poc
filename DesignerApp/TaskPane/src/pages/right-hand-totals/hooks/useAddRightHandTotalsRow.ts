import { useCallback } from 'react';

import { DefaultStyling } from '../../../business/engine/models/styles';
import { useSetRightHandTotalsTemplateDraft } from '../states/RightHandTotalsState';

const useAddRightHandTotalsRow = (): any => {
    const setDraft = useSetRightHandTotalsTemplateDraft();

    return useCallback(
        (
            columnName: string | null | (string | null)[] = null,
            reset: boolean = false
        ) => {
            setDraft(({ Definition: draft }) => {
                const columnNames = Array.isArray(columnName)
                    ? columnName
                    : [columnName];

                if (reset || !draft.Columns) draft.Columns = [];

                let nextId =
                    Math.max(0, ...draft?.Columns?.map((o) => o.Id)) + 1;

                columnNames.forEach((ColumnName) => {
                    draft.Columns.push({
                        Id: nextId++,
                        ColumnName,
                        Name: ColumnName,
                        Order: draft.Columns.length,
                        TableId: null,
                        HeaderStyling: DefaultStyling(),
                        Styling: DefaultStyling(),
                        SumStyling: DefaultStyling(),
                    });
                });
            });
        },
        [setDraft]
    );
};

export default useAddRightHandTotalsRow;
