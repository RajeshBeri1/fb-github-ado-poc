import { useCallback, useState } from 'react';
import { createContainer } from 'react-tracked';
import * as API from '@omniflow/omni-webapi';
import { produce } from 'immer';
import { DefaultStyling } from '../../../business/engine/models/styles';

export type MediaHierarchySubLevelWithId = {
    Id: number;
    Settings?: MediaHierarchySubLevelSettingWithId[] | null;
} & Omit<API.MediaHierarchySubLevel, 'Settings'>;

export type MediaHierarchySubTotalWithId = {
    Id: number;
} & API.MediaHierarchySubTotal;

export type MediaHierarchySubTotalSummaryWithId = {
    Id: number;
    Settings?: MediaHierarchySubTotalSummarySettingWithId[] | null;
    SubTotals?: MediaHierarchySummarySubTotalWithId[] | null;
    DisplayNames?: {
        [key: string]: string
    } | null;
} & Omit<API.MediaHierarchySubTotalSummary, 'Settings,SubTotals,DisplayNames'>;

export type MediaHierarchySummarySubTotalWithId = {
    Id: number;
} & API.MediaHierarchySubTotalBase

export type MediaHierarchySubTotalSummarySettingWithId = {
    Id: number;
} & API.MediaHierarchySubTotalSummarySettingBase

export type MediaHierarchyInflightOverlayWithId = {
    Id: number;
} & API.MediaHierarchyInflightOverlay;

export type MediaHierarchyLevelSettingWithId = {
    Id: number;
    SubLevels?: MediaHierarchySubLevelWithId[] | null;
    SubTotals?: MediaHierarchySubTotalWithId[] | null;
    InflightOverlays?: MediaHierarchyInflightOverlayWithId[] | null;
    SubTotalSummary?: MediaHierarchySubTotalSummaryWithId[] | null;
} & Omit<API.MediaHierarchySetting, 'SubLevels,SubTotals,InflightOverlays,SubTotalSummary'>;

export type MediaHierarchySubLevelSettingWithId = {
    Id: number;
    SubTotals?: MediaHierarchySubTotalWithId[] | null;
} & Omit<API.MediaHierarchySubLevelSetting, 'SubTotals'>;

export type MediaHierarchyLevelWithId = {
    Id: number;
    Settings?: MediaHierarchyLevelSettingWithId[] | null;
} & Omit<API.MediaHierarchyLevel, 'Settings'>;

export type MediaHierarchyDefinitionWithId = {
    Levels?: MediaHierarchyLevelWithId[] | null;
} & Omit<API.MediaHierarchyDefinition, 'Levels'>;

export type MediaHierarchyTemplate = {
    Definition?: MediaHierarchyDefinitionWithId | null;
} & Omit<API.MediaHierarchyTemplateDetailsDTO, 'Definition'>;


export const initialMediaHierarchyTemplate: MediaHierarchyTemplate = {
    Id: null,
    Name: null,
    Definition: {
        Levels: [],
        Styling: DefaultStyling(),
    },
    Currency: "LLL",
    DisplaySource: API.DisplaySource.PlanitClientAlias.toString(),
    ShowSubTotalsAtBottom: false,
};

const useValue = () =>
    useState<MediaHierarchyTemplate>(initialMediaHierarchyTemplate);

const {
    Provider: MediaHierarchyTemplateProvider,
    useTrackedState: useMediaHierarchyTemplateState,
    useUpdate: useSetState,
} = createContainer(useValue);

const useSetMediaHierarchyTemplateDraft = () => {
    const setState = useSetState();
    return useCallback(
        (draftUpdater) => {
            setState(produce(draftUpdater));
        },
        [setState]
    );
};

export {
    MediaHierarchyTemplateProvider,
    useMediaHierarchyTemplateState,
    useSetMediaHierarchyTemplateDraft,
};
