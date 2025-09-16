import { useCallback } from 'react';

import { useSetRightHandTotalsTemplateDraft } from '../states/RightHandTotalsState';
 
const useSetRightHandTotalsName = (): any => {
    const setDraft = useSetRightHandTotalsTemplateDraft();

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

export default useSetRightHandTotalsName;
