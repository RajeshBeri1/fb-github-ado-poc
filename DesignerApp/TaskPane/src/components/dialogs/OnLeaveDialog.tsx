import React, { JSX, useCallback, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useBeforeUnload } from 'react-router-dom';
import { AppContext } from '../../taskpane/contexts/AppContext';
import Dialog, { DialogType } from './Dialog';

import { TDefinition, TDefinitionName } from '../../interfaces/definition.type';
import useRender from '../../hooks/useRender';
export type TOnLeaveDialogProps = {
    when?: boolean;
    title?: string;
    message?: string;
    okText?: string;
    cancelText?: string;
    navigatePath?: string;
    handleOk?: any;
    definitionName?: TDefinitionName;
    definition?: TDefinition;
    setDialog?: (e:boolean) => void,
};

// Todo: Need to fix this: "Warning: A history supports only one prompt at a time"
const OnLeaveDialog = ({
    when = true,
    title = 'Unsaved changes',
    message = 'There are unsaved changes. Are you sure you want to leave this page?',
    okText ="Ok",
    cancelText,
    navigatePath,
    handleOk,
    definitionName,
    definition,
    setDialog,
}: TOnLeaveDialogProps): JSX.Element | null => {
    const navigate = useNavigate();

    const [showPrompt, setShowPrompt] = useState(false);
    const [currentPath, setCurrentPath] = useState(navigatePath);
    const appContext = useContext(AppContext);
    const { setUnsavedChanges } = appContext;
    const { render } = useRender();

    useEffect(() => {
        if (when) {
      
            setShowPrompt(true);
            setCurrentPath(navigatePath);
 
        }
    },[when])
    const handleOkOnCancelClick = () => {
            handleOk();
        setShowPrompt(false);
        setDialog(false);
            navigate(currentPath);
            render(definitionName, definition, []).then();
    };

    const handleCancel = useCallback(async () => {
        setShowPrompt(false);
        setDialog(false);
    }, []);

    return (
        showPrompt && (
            <Dialog
                title={title}
                type={DialogType.WARNING}
                icon="informative:alert"
                okText={okText}
                onOk={handleOkOnCancelClick}
                cancelText={cancelText}
                onCancel={handleCancel}
                showDialog={showPrompt}
            >
                {message}
               
            </Dialog>
        )
    );
};

export default OnLeaveDialog;
