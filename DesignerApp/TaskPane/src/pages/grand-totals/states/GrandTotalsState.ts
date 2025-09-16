import { useCallback, useState } from 'react';
import { createContainer } from 'react-tracked';
import * as API from '@omniflow/omni-webapi';
import {produce} from 'immer';

export type GrandTotalsSelectionWithId = {
    Id: number;
} & API.GrandTotalSelection;

export type GrandTotalsDefinitionWithId = {
    Selections: GrandTotalsSelectionWithId[] | null;
} & Omit<API.GrandTotalDefinition, 'Selections'>;

export type GrandTotalsTemplate = {
    Definition?: GrandTotalsDefinitionWithId | null;
} & Omit<API.GrandTotalTemplateDetailsDTO, 'Definition'>;
 
export const initialGrandTotalsTemplate: GrandTotalsTemplate = {
    Id: null,
    Name: null,
    Definition: {
        Selections: null,
    },
};

const useValue = () =>
    useState<GrandTotalsTemplate>(initialGrandTotalsTemplate);

const {
    Provider: GrandTotalsTemplateProvider,
    useTrackedState: useGrandTotalsTemplateState,
    useUpdate: useSetState,
} = createContainer(useValue);

const useSetGrandTotalsTemplateDraft = () => {
    const setState = useSetState();
    return useCallback(
        (draftUpdater) => {
            setState(produce(draftUpdater));
        },
        [setState]
    );
};

export {
    GrandTotalsTemplateProvider,
    useGrandTotalsTemplateState,
    useSetGrandTotalsTemplateDraft,
};
