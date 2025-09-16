import { useCallback } from 'react';
import { cloneDeep } from 'lodash';

import {
    GrandTotalsDefinitionWithId,
    GrandTotalsTemplate,
    useSetGrandTotalsTemplateDraft,
} from '../states/GrandTotalsState';

const useSetGrandTotalsTemplate = (): any => {
    const setDraft = useSetGrandTotalsTemplateDraft();

    return useCallback(
        (template: GrandTotalsTemplate): GrandTotalsDefinitionWithId => {
            let nextSelectionId = 1;
            const selectionsWithIds = template?.Definition?.Selections?.map(
                (selection) => ({
                    ...selection,
                    Id: nextSelectionId++,
                })
            );

            const Definition = {
                ...template?.Definition,
                Selections: selectionsWithIds,
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

export default useSetGrandTotalsTemplate;
