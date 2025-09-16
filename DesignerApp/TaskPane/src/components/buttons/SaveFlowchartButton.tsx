import React, { useState, useEffect , useRef} from 'react';
import useFlowchart from '../../hooks/useFlowchart';
import Dialog, { DialogType } from '../dialogs/Dialog';
import '../../components/dialogs/Dialog.css'
import Button from './Button';
import useEventLogger from '../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../enums/event.enum'
import { AppContext } from '../../taskpane/contexts/AppContext';
import { OmniCheckBoxInput } from '../../omni/checkbox';
import { isEmpty } from 'lodash';

import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';

import RequestInterceptor from '../../business/request-interceptor';
import { Icon } from '../../omni/icon'
import {
    FlowchartTemplateApi,
    FlowchartTemplateDuplicateCheckDTO


} from '@omniflow/omni-webapi';

import { useDebounce } from '../../hooks/useDebounce';

export const SaveFlowchartButton: React.FC = () => {
    const [isSaveModalOpen, setIsSaveModalOpen] = useState(false);
    const { isSavingFlowchart, saveFlowchart, saveAsTemplate, hasUpdate } = useFlowchart();
    const { logEvent } = useEventLogger(Module.FLOWCHART);

    const showSaveModal = (show: boolean) => {
        logEvent({ action: Action.SAVEFLOWCHART });
        setIsSaveModalOpen(show);
    };
    const appContext = React.useContext(AppContext);
    const { setUnsavedChanges, unsavedChanges, flowchartTemplateDefinition, user } = appContext;
    const saveFlowchartEnabled = Object.values(unsavedChanges).some(x => x);
    const [inputText, setInputText] = useState('');
    const [inputNewTemplateName, setInputNewTemplateName] = useState('');
    const [characterLimit] = useState(20);
    const [openRunConfig, setOpenRunConfig] = useState(false);
    const [isRestricted, setIsRestricted] = useState(false);

    const [duplicateTemplateNameCheck, setDuplicateTemplateNameCheck] = useState(false);
    const [hasValues, setHasValues] = useState(false);
    const [query, setQuery] = useState('');
    const [IsValidFilename, setIsValidFilename] = useState(true);
    const [IsValidFilenameWithoutDot, setIsValidFilenameWithoutDot] = useState(true);
    const [IsValidFilenameWithoutSpace, setIsValidFilenameWithoutSpace] = useState(true);
    const [IsValidFilenameWithSpecialCharacters, setIsValidFilenameWithSpecialCharacters] = useState(true);
    const [previousTemplateName, setPreviousTemplateName] = useState('');
    const [processing, setProcessing] = useState(false);

    const controllerRef = useRef(new AbortController());

    const templateApi = new FlowchartTemplateApi(
        WEBAPI_CONFIGURATION,
        '',
        RequestInterceptor
    );

    const onChangeHandler = (event) => {
        setInputText(event.target.value);

    }
   
    const searchQuery = useDebounce(query, 1000);
    const fileNameRegex = /[#$+%!`&'={}@<>:"/\\|?*\u0000-\u001F^(),\[\]]/;          // =>  Regex = /[<>:"/\\|?*\u0000-\u001F]/g;  
    const windowsRegex = /^(con|prn|aux|nul|com\d|lpt\d)$/i;
    const fullStopInBeginning = /^\.+/;
    const spaceAtBeginning = /^(?!\s)(.*[^\s])$/;   // for not allowing neither at beginning nor at the end ==> Regex=  /^(?!\s).+(?<!\s)$/g;    || For not allowing at beginning only ==> Regex =  /^\ +/; 
    const validator = (value: string) => {
        if (!value) return false;

        setIsValidFilename(!windowsRegex.test(value));
        setIsValidFilenameWithoutDot(!fullStopInBeginning.test(value));
        setIsValidFilenameWithoutSpace(spaceAtBeginning.test(value));
        setIsValidFilenameWithSpecialCharacters(!fileNameRegex.test(value));
        return true;
    };
    const onNameChangeHandler = (event: { target: { value: any; }; }) => {
        setInputNewTemplateName(event.target.value);
        const value = event.target.value;

        setQuery(value);
        validator(value);
        setHasValues(value)

    }

  
    useEffect(() => {
        (async () => {
            const queryString = window.location.search;
            const urlParams = new URLSearchParams(queryString);

            if (searchQuery || query.trim().length < 0) {
                try {
                    const templateCheckDTO = new FlowchartTemplateDuplicateCheckDTO();
                    templateCheckDTO.OmniClientId = appContext.flowchartTemplateDefinition.OmniClientId;
                    templateCheckDTO.TemplateName = searchQuery;
                    const { data } = await templateApi.flowchartTemplateCheckDuplicateTemplateName(templateCheckDTO);
                    setDuplicateTemplateNameCheck(data);
                    setProcessing(false);
                } catch (e) {
                    setDuplicateTemplateNameCheck(false);
                    setProcessing(false);
                }
            }
        })();

        return controllerRef.current.abort();
    }, [searchQuery])
    useEffect(() => {
        if (isEmpty(inputText) && flowchartTemplateDefinition?.Comments) {
            setInputText(flowchartTemplateDefinition.Comments);
        }

    }, [flowchartTemplateDefinition.Comments])
    useEffect(() => {
        if (!isEmpty(flowchartTemplateDefinition?.CreatedByUser?.Id) && (flowchartTemplateDefinition?.CreatedByUser?.Id !== user?.Id)) {
            setIsRestricted(true);
        }
        else {
            setIsRestricted(false);
        }

    }, [flowchartTemplateDefinition?.CreatedByUser])

    const handleChange = (event) => {
        setOpenRunConfig(Boolean(event?.currentTarget?.checked));
    }
    return (
        <>{isRestricted ?
            <Button
                disabled={isSavingFlowchart || !saveFlowchartEnabled}
                onClick={() => showSaveModal(true)}>
                Save As
            </Button>
            :
            <Button
                disabled={isSavingFlowchart || !saveFlowchartEnabled}
                onClick={() => showSaveModal(true) }>
                Save
            </Button>
            }
            {isSaveModalOpen && (

                <Dialog
                    icon="interactive:save"
                    type={DialogType.INFO}
                    title="Save to flowchart"
                    okText={`${isRestricted ? "Save template" : "Save version"}`}
                    onOk={(e) => {
                        if (isRestricted) {
                            logEvent({ action: Action.OK });
                            showSaveModal(false);
                            setUnsavedChanges({});
                            appContext.setIsFlowchartSaving(true)
                            saveAsTemplate(null, inputText, openRunConfig, appContext?.isTrackFormatting, inputNewTemplateName);
                        }
                        else {
                            logEvent({ action: Action.OK });
                            showSaveModal(false);
                            setUnsavedChanges({});
                            appContext.setIsFlowchartSaving(true)
                            saveFlowchart(null, inputText, openRunConfig, appContext?.isTrackFormatting);
                        }

                    }}
                    onCancel={(e) => {
                        logEvent({ action: Action.CANCEL });
                        showSaveModal(false);
                        e.preventDefault();
                    }}
                    okDisabled={isRestricted ? Boolean(!inputNewTemplateName?.length || !inputText?.length) : Boolean(!inputText?.length)}
                    showDialog={isSaveModalOpen }
                >
                    <div className="dataCard">
                        {isRestricted && (
                            <>
                             <div className="label-container top">
                                <div className="label dialog-font-12"> * Template Name</div>
                            </div>
                            <input
                                id="template-name"
                                type="text"
                                placeholder="Label your new template name"
                                    value={inputNewTemplateName}
                                    className={"inputfield text d-block " + (duplicateTemplateNameCheck ? 'error' : '')}
                                onChange={onNameChangeHandler}
                                required
                                />
                                {duplicateTemplateNameCheck && <div className="error-msg"> <div className="mr-1 error-icon"> <Icon icon-id="omni:informative:error" slot='invoker'></Icon></div> <span>The template name has already been used / unavailable</span></div>}
                        </>
                        )}
                       
                        <div className="label-container top">
                            <div className="label dialog-font-12"> * Version comment</div>
                        </div>
                        <input
                            id="template-version"
                            type="text"
                            placeholder="Label your new version"
                            value={inputText}
                            className="inputfield text d-block "
                            onChange={onChangeHandler}
                            maxLength={20}
                            required
                        />
                        <div className="input-length-badge ">
                            {/* {inputText?.length || 0}/{characterLimit} */}
                           <Icon icon-id="omni:informative:info" slot='invoker'></Icon><div className='ml-2'> Max 20 characters allowed</div>
                        </div>
                       
                    </div>
                    {(!isRestricted && flowchartTemplateDefinition.Version === 0 )&&
                        <div className="is-flex-direction-row is-align-items-center runconfig-checkbox">
                            <OmniCheckBoxInput checked={openRunConfig} onClick={(e) => handleChange(e)}> <label>View Run Configuration after saving</label></OmniCheckBoxInput>
                        </div>
                    }
                </Dialog>
            )}
        </>
    );
};
