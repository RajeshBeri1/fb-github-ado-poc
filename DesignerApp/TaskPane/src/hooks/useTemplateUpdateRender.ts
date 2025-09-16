import { useCallback, useContext } from 'react';
import { AppContext } from '../taskpane/contexts/AppContext';
import { TDefinition, TDefinitionName } from '../interfaces/definition.type';
import useRender from './useRender';

const useTemplateUpdateRender = ()=> {
    const appContext = useContext(AppContext);
    const { render } = useRender();
    const renderUpdates = async (definitionName: TDefinitionName) => {
        try {
            if (appContext.isTemplateChanged) {
                appContext.setTemplateChanged(false);
                render(
                    definitionName,
                    getDefinition(definitionName),
                    []
                ).then();
            }
        } catch (e) {
            console.error(e);
        }
    };

    const getDefinition = (definitionName: TDefinitionName)=> {
        let definition: TDefinition= null;
        switch (definitionName) {
            case 'calendarDefinition':
                definition = appContext.currentCalendarTemplateDetails?.Definition;
                break;
            case 'calendarOverlayDefinition':
                definition = appContext.currentCalendarOverlayTemplateDetails?.Definition;
                break;
            case 'headerDefinition':
                definition = appContext.currentHeaderTemplateDetails?.Definition;
                break;
            case 'mediaHierarchyDefinition':
                definition = appContext.currentMediaHierarchyDetails?.Definition;
                //debugger;
                break;
            case 'totalsDefinition':
                definition = appContext.currentTotalsDetails?.Definition;
                break;
            case 'grandTotalsDefinition':
                definition = appContext.currentGrandTotalsDetails?.Definition;
                break;
            case 'themeDefinition':
                definition = appContext.currentThemeTemplateDetails?.Definition;
                break;
            case 'footerDefinition':
                definition = appContext.currentFooterTemplateDetails?.Definition;
                break;
        }
        return definition;
    }

    return { renderUpdates };
};

export default useTemplateUpdateRender;
