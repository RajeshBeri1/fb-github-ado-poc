import React, {JSX, useCallback, useContext, useEffect, useRef, useState} from 'react';
import {Icon} from '../../omni/icon';
import useRender from '../../hooks/useRender';
import {TDefinition, TDefinitionName} from '../../interfaces/definition.type';
import {AppContext} from '../../taskpane/contexts/AppContext';
import { Tooltip } from "../../omni/tooltip";
import { DataContext } from '../../taskpane/contexts/DataContext';
import useNotification, { NotificationType } from '../notification/useNotification';
import { ExcelRenderer } from '../../business/engine/renderer/excel-renderer';
import { isEmpty } from 'lodash';

export type TRenderButtonProps = {
    definitionName?: TDefinitionName;
    definition?: TDefinition;
    disabled?: boolean;
    autoRender?: boolean;
};

const RenderButton = ({
    definitionName,
    definition,
    disabled,
    autoRender = false,
}: TRenderButtonProps): JSX.Element => {
    const {render, isRendering} = useRender();
    const appContext = useContext(AppContext);
    const timeOutRef = useRef(null);
    const pushNotification = useNotification();
    const {
        setIsGlobalRendering,
    } = useContext(DataContext);


    const handleRender = useCallback(async () => {
        render(definitionName, definition, []).then();
    }, [definitionName, definition, appContext.isRenderClicked, appContext.isTrackFormatting]);

    const first = useRef<boolean>(true);
    useEffect(() => {
        if (!first.current && autoRender) {
            timeOutRef.current = setTimeout(() => {
                if (!isRendering)
                    handleRender().then();
            }, 100);
        }

        if (first.current) {
            first.current = false;
        }
        return () => { if (timeOutRef.current) clearTimeout(timeOutRef.current); }
    }, [handleRender, autoRender, appContext.isTrackFormatting, appContext.isRenderClicked]);


    useEffect(() => {
        setIsGlobalRendering(isRendering)
    }, [isRendering])

    return (
        <Tooltip>
            <button
                slot="invoker"
                className={`icon ${isRendering ? 'spinner' : ''}`}
                onClick={handleRender}
                disabled={isRendering || disabled || appContext.levelLoading}>
                <Icon icon-id="omni:interactive:refresh"></Icon>
            </button>
            <div slot="content">Render</div>
        </Tooltip>
    );
};

export default RenderButton;