import { useCallback } from 'react';
import { cloneDeep } from 'lodash';

import {
    RightHandTotalsDefinitionWithId,
    RightHandTotalsTemplate,
    useSetRightHandTotalsTemplateDraft,
} from '../states/RightHandTotalsState';

const useSetRightHandTotalsTemplate = (): any => {
    const setDraft = useSetRightHandTotalsTemplateDraft();

    return useCallback(
        (
            template: RightHandTotalsTemplate
        ): RightHandTotalsDefinitionWithId => {
            let nextColumnId = 1;
            const columnsWithIds = template?.Definition?.Columns?.map(
                (column) => ({
                    ...column,
                    Id: nextColumnId++,
                })
            );

            const Definition = {
                ...template?.Definition,
                Columns: columnsWithIds,
            };
            setDraft(() => ({
                ...template,
                Definition,
            }));

            return cloneDeep(Definition ?? null);
        },
        [setDraft]
    );
};

export default useSetRightHandTotalsTemplate;
