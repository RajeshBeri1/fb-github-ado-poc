import { useCallback } from 'react';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useSetMediaHierarchyName = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (name: string, currency: string, showSubTotalAtBottom?: boolean,displaySource?:string) => {
            setDraft((draft) => {
                const finalName = name.trim();

                draft.Name = finalName;
                draft.Currency = currency;
                draft.ShowSubTotalsAtBottom = showSubTotalAtBottom;
                draft.DisplaySource = displaySource;
            });
        },
        [setDraft]
    );
};

export default useSetMediaHierarchyName;
