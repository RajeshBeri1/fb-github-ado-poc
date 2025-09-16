import { FlowchartDefinition } from '@omniflow/omni-webapi';
import { useContext } from 'react';
import { TBorder } from '../components/template-list/template-list.types';
import {
    TemplateDetailsDTO,
    TemplateInfoDTO,
} from '../interfaces/definition.type';
import { TValueOf } from '../interfaces/valueOf.type';
import { AppContext } from '../taskpane/contexts/AppContext';

export type TDeselectReturn = 'Deselect' | 'Load' | 'LoadFlowchart';
const useTemplateList = () => {
    const appContext = useContext(AppContext);

    const deselectTemplate = (
        id: string,
        definitionKey: string,
        contextDef: TemplateDetailsDTO
    ): TDeselectReturn => {
        const currFlowchartDef: TValueOf<FlowchartDefinition> =
            appContext.flowchartTemplateDefinition?.Definition?.[definitionKey];
        const bothDefined = contextDef?.Id && currFlowchartDef?.TemplateId;
        const sameIds = contextDef?.Id === currFlowchartDef?.TemplateId;
        const contextNewerVersion =
            contextDef?.Version &&
            contextDef?.Version > currFlowchartDef?.TemplateVersion;
        const contextSelected = contextDef?.Id === id;

        if (
            (bothDefined && !sameIds && contextSelected) ||
            (bothDefined && sameIds && contextNewerVersion && contextSelected)
        )
            //in this case fall back to flowchart
            return 'LoadFlowchart';

        if (
            contextDef?.Id &&
            !currFlowchartDef?.TemplateId &&
            contextSelected
        ) {
            return 'Deselect';
        }

        const flowchartSelected =
            currFlowchartDef?.TemplateId && currFlowchartDef.TemplateId === id;
        if (
            (!contextDef?.Id && flowchartSelected) ||
            (bothDefined && sameIds && !contextNewerVersion && contextSelected)
        ) {
            // remove definition from flowchart
            const flowchart = { ...appContext.flowchartTemplateDefinition };
            delete flowchart.Definition[definitionKey]; //= undefined;
            appContext.setFlowchartTemplateDefinition(flowchart);
            return 'Deselect';
        }

        return 'Load';
    };

    const getListItemBorder = (
        listItem: TemplateInfoDTO,
        currentContext?: TemplateDetailsDTO,
        currentflowchart?: TValueOf<FlowchartDefinition>
    ): TBorder => {
        const {
            Id: contextId,
            Version: contextVersion,
            Definition: constextDef,
        } = currentContext ?? {};
        const {
            TemplateId: flowchartId,
            TemplateVersion: flowchartVersion,
            Definition: flowchartDef,
        } = currentflowchart ?? {};

        if (
            (!contextId || contextId === flowchartId) &&
            flowchartId === listItem.Id
        ) {
            if (
                flowchartVersion <= contextVersion ||
                flowchartVersion === listItem.Version
            ) {
                return 'active';
            } else {
                return 'warning';
            }
        }

        if (contextId === listItem.Id) {
            return 'active';
        }
        return '';
    };

    return {
        deselectTemplate,
        getListItemBorder,
    };
};

export default useTemplateList;
