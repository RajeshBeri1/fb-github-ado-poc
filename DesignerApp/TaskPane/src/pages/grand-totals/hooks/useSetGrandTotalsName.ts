import { useCallback } from 'react';

import { useSetGrandTotalsTemplateDraft } from '../states/GrandTotalsState';

const useSetGrandTotalsName = (): any => {
    const setDraft = useSetGrandTotalsTemplateDraft();

    return useCallback(
        (name: string) => {
            setDraft((draft) => {
                const finalName = name.trim();
 
                draft.Name = finalName;
            });
        },
        [setDraft]
    );
};

export default useSetGrandTotalsName;
