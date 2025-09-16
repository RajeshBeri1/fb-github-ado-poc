import { useCallback, useState } from 'react';
import { createContainer } from 'react-tracked';
import * as API from '@omniflow/omni-webapi';
import {produce} from 'immer';

export type RightHandTotalsColumnWithId = {
    Id: number;
} & API.TotalsColumn;

export type RightHandTotalsDefinitionWithId = {
    Columns: RightHandTotalsColumnWithId[] | null;
} & Omit<API.TotalsDefinition, 'Columns'>;

export type RightHandTotalsTemplate = {
    Definition?: RightHandTotalsDefinitionWithId | null;
} & Omit<API.TotalsTemplateDetailsDTO, 'Definition'>;

export const initialRightHandTotalsTemplate: RightHandTotalsTemplate = {
    Id: null,
    Name: null,
    Definition: {
        Columns: null,
    },
};

const useValue = () =>
    useState<RightHandTotalsTemplate>(initialRightHandTotalsTemplate);

const {
    Provider: RightHandTotalsTemplateProvider,
    useTrackedState: useRightHandTotalsTemplateState,
    useUpdate: useSetState,
} = createContainer(useValue);

const useSetRightHandTotalsTemplateDraft = () => {
    const setState = useSetState();
    return useCallback(
        (draftUpdater) => {
            setState(produce(draftUpdater));
        },
        [setState]
    );
};

export {
    RightHandTotalsTemplateProvider,
    useRightHandTotalsTemplateState,
    useSetRightHandTotalsTemplateDraft,
};
