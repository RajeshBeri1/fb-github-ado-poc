import { useCallback } from 'react';
import * as API from '@omniflow/omni-webapi';

import { useSetGrandTotalsTemplateDraft } from '../states/GrandTotalsState';
import { DefaultStyling } from '../../../business/engine/models/styles';

const useAddGrandTotalsRow = (): any => {
    const setDraft = useSetGrandTotalsTemplateDraft();

    return useCallback(
        (quantity: 1, reset: boolean = false) => {
            setDraft(({ Definition: draft }) => {
                if (reset || !draft.Selections) draft.Selections = [];

                let nextId =
                    Math.max(0, ...draft?.Selections?.map((o) => o.Id)) + 1;

                Array.from(Array(quantity).keys()).forEach(() => {
                    draft.Selections.push({
                        Id: nextId++,
                        FlightRange: API.FlightRange.Annually,
                        Order: draft.Selections.length,
                        Styling: DefaultStyling(),
                        LeftMenuStyling: DefaultStyling(),
                    });
                });
            });
        },
        [setDraft]
    );
};

export default useAddGrandTotalsRow;
