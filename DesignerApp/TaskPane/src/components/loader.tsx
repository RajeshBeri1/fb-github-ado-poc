import React, { JSX, useCallback, useContext, useEffect, useRef } from 'react';
import useRender from '../hooks/useRender';
import { TDefinition, TDefinitionName } from '../interfaces/definition.type';
import { AppContext } from '../taskpane/contexts/AppContext';
import { DataContext } from '../taskpane/contexts/DataContext';

import { OmniLoadingIndicator } from "../components/spinner/Spinner";
import '../pages/common-styles'

export type TLoaderProps = {
    definitionName?: TDefinitionName;
    definition?: TDefinition;
    disabled?: boolean;
    autoRender?: boolean;
}

const Loader = ({
  
}: TLoaderProps): JSX.Element => {

    const dataContext = useContext(DataContext);
    const appContext = useContext(AppContext);
    return (
        <>
            {(dataContext.isGlobalRendering || appContext.isFlowchartSaving || appContext.levelLoading) && 
                <OmniLoadingIndicator>
                    <p>Please wait while the content loads</p>
                </OmniLoadingIndicator>
            }

        </>
    );

};
export default Loader;