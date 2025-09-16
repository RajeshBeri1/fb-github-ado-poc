/* eslint-disable no-control-regex */
import React, { JSX, useRef } from 'react';
import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import {
    FlowchartTemplateApi,
    FlowchartTemplateCreateDTO,
    FlowchartTemplateDuplicateCheckDTO,
    FlowchartTemplateInfoDTO,
} from '@omniflow/omni-webapi';
import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';
import RequestInterceptor from '../../business/request-interceptor';
import useNotification, {
    NotificationType,
} from '../../components/notification/useNotification';
import useEventLogger from '../../hooks/eventLogger';
import { Action, Module } from '../../enums/event.enum';
import { useDebounce } from '../../hooks/useDebounce';
import './createTemplate-ModalForm.scss';
import { Icon } from '../../omni-ui-components/icon';
import { Modal } from '../../omni-ui-components/modal';
interface CreateFlowchartTemplateModalFormProps {
    callback: (templateInfo: FlowchartTemplateInfoDTO) => void;
    clientId: string;
}

export default function CreateFlowchartTemplateModalForm(
    props: CreateFlowchartTemplateModalFormProps
): JSX.Element {
    //const [isModalOpen, toggleModal] = useState(false);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const modalRef = useRef<any>(null);

    const { logEvent } = useEventLogger(Module.FLOWCHART);
    const pushNotification = useNotification();
    const templateApi = new FlowchartTemplateApi(
        WEBAPI_CONFIGURATION,
        '',
        RequestInterceptor
    );
    const formDefaultValues: FlowchartTemplateCreateDTO = {
        TemplateName: '',
        OmniClientId: '',
    };

    const { handleSubmit, register, reset } = useForm({
        mode: 'onChange',
        reValidateMode: 'onChange',
        defaultValues: formDefaultValues,
    });

    const [hasValues, setHasValues] = useState(false);
    const [query, setQuery] = useState('');
    const [IsValidFilename, setIsValidFilename] = useState(true);
    const [IsValidFilenameWithoutDot, setIsValidFilenameWithoutDot] =
        useState(true);
    const [IsValidFilenameWithoutSpace, setIsValidFilenameWithoutSpace] =
        useState(true);
    const [
        IsValidFilenameWithSpecialCharacters,
        setIsValidFilenameWithSpecialCharacters,
    ] = useState(true);
    const [isValidFileLength, setIsValidFileLength] = useState(false);
    const [previousTemplateName, setPreviousTemplateName] = useState('');
    const [duplicateTemplateNameCheck, setDuplicateTemplateNameCheck] =
        useState(false);
    const [processing, setProcessing] = useState(false);
    const controllerRef = useRef(new AbortController());
    const searchQuery = useDebounce(query, 1000);
    useEffect(() => {
        (async () => {
            const queryString = window.location.search;
            const urlParams = new URLSearchParams(queryString);

            if (searchQuery || query.trim().length < 0) {
                try {
                    const templateCheckDTO =
                        new FlowchartTemplateDuplicateCheckDTO();
                    templateCheckDTO.OmniClientId = urlParams.get('clientid');
                    templateCheckDTO.TemplateName = searchQuery;
                    const { data } =
                        await templateApi.flowchartTemplateCheckDuplicateTemplateName(
                            templateCheckDTO
                        );
                    setDuplicateTemplateNameCheck(data);
                    setProcessing(false);
                } catch (e) {
                    setDuplicateTemplateNameCheck(false);
                    setProcessing(false);
                }
            }
        })();

        return controllerRef.current.abort();
    }, [searchQuery]);
    const checKLength = (value: string) => {
        if (!value) return false;

        const validLength = value.length > 202 ? false : true;

        setIsValidFileLength(validLength);
    };

    const fileNameRegex = /[#$+%!`&'={}@<>:"|?*\u0000-\u001F^(),[\]]/; // =>  Regex = /[<>:"/\\|?*\u0000-\u001F]/g;
    const windowsRegex = /^(con|prn|aux|nul|com\d|lpt\d)$/i;
    const fullStopInBeginning = /^\.+/;
    const spaceAtBeginning = /^(?!\s)(.*[^\s])$/; // for not allowing neither at beginning nor at the end ==> Regex=  /^(?!\s).+(?<!\s)$/g;    || For not allowing at beginning only ==> Regex =  /^\ +/;
    const validator = (value: string) => {
        if (!value) return false;

        checKLength(value);
        setIsValidFilename(!windowsRegex.test(value));
        setIsValidFilenameWithoutDot(!fullStopInBeginning.test(value));
        setIsValidFilenameWithoutSpace(spaceAtBeginning.test(value));
        setIsValidFilenameWithSpecialCharacters(!fileNameRegex.test(value));
    };

    const onChangeHandler = (event: { target: { value: any } }) => {
        const value = event.target.value;
        setProcessing(true);
        setQuery(value);
        validator(value);
        setHasValues(value);
    };

    const closeForm = (): void => {
        modalRef.current.close();
        
        setHasValues(false);
        logEvent(Action.CANCEL);
        reset();
    };

    const sendDataToBackend = (data: any): void => {
        if (data.TemplateName != previousTemplateName) {
            const requestData: FlowchartTemplateCreateDTO = data;
            // Use currently selected ClientId.
            requestData.OmniClientId = props.clientId;
            logEvent(Action.CREATE);
            templateApi
                .flowchartTemplateCreate(requestData)
                .then((x) => {
                    const newTemplateInfo: FlowchartTemplateInfoDTO =
                        x.data as FlowchartTemplateInfoDTO;
                    props.callback(newTemplateInfo);

                    pushNotification(
                        `New flowchart template "${requestData.TemplateName}" got created.`,
                        NotificationType.SUCCESS
                    );
                    closeForm();
                })
                .catch((x) => {
                    //ToDo: add server response to notification.
                    pushNotification(
                        `The flowchart template named "${requestData.TemplateName}" couldn't get created. ${x.response.data.Message}`,
                        NotificationType.DANGER
                    );
                });
        }
        setPreviousTemplateName(data.TemplateName);
    };
    const openModal = () => {
        if (modalRef.current) {
            modalRef.current.showModal();
        }
    };
    
    return (
        <div>
            <button
                className="secondary fixed-size-small pr-5  h-24"
                onClick={openModal}>
                {' '}
                New template{' '}
            </button>
           
          
            <Modal ref={modalRef} className="dialog-500px">
                <div slot="header" className="font-lg-bold">
                 New template
                </div>
                    <form
                        className="message"
                        key="calendarConfigurator"
                        onSubmit={handleSubmit((data) => {
                            sendDataToBackend(data);
                        })}>
                       
                        <div className="py-4">
                            <div>
                                <div className="font-gr-14">
                                    Please select the name for your new
                                    template.
                                </div>
                                <div>
                                    {/* Error should be displayed in case user enters the same template name that has already been used before */}
                                    <div className="dataCard">
                                        <label
                                            htmlFor="template-name"
                                        className="label font-bl-11 font-opacity">
                                            {' '}
                                            Template name{' '}
                                        </label>

                                        <input
                                            {...register('TemplateName', {
                                                required:
                                                    'Please enter a template name.',
                                            })}
                                            id="template-name"
                                            type="text"
                                            placeholder="Enter template name"
                                            className={
                                                'input text d-block ' +
                                                (duplicateTemplateNameCheck ||
                                                (hasValues &&
                                                    (!isValidFileLength ||
                                                        !IsValidFilenameWithoutDot ||
                                                        !IsValidFilenameWithoutSpace ||
                                                        !IsValidFilenameWithSpecialCharacters ||
                                                        !IsValidFilename))
                                                    ? 'error'
                                                    : '')
                                            }
                                            onChange={onChangeHandler}
                                            required
                                        />

                                        {duplicateTemplateNameCheck && (
                                            <div className="error-msg">
                                                {' '}
                                                <div className="mr-1 error-icon">
                                                    {' '}
                                                    <Icon
                                                        icon-id="omni:informative:error"
                                                        slot="invoker"></Icon>
                                                </div>{' '}
                                                <span>
                                                    The template name has
                                                    already been used /
                                                    unavailable
                                                </span>
                                            </div>
                                        )}
                                        {hasValues &&
                                            (!isValidFileLength ||
                                                !IsValidFilenameWithoutDot ||
                                                !IsValidFilenameWithoutSpace ||
                                                !IsValidFilenameWithSpecialCharacters ||
                                                !IsValidFilename) && (
                                                <div className="error-msg">
                                                    <div className="mr-2 error-icon">
                                                        <Icon
                                                            icon-id="omni:informative:error"
                                                            slot="invoker"></Icon>
                                                    </div>{' '}
                                                    <span>
                                                        A template name can only
                                                        contain one of the
                                                        following characters:
                                                        A-Z a-z 0-9
                                                    </span>
                                                </div>
                                            )}
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div slot="footer" >
                            <div className="field is-grouped is-grouped-right">
                                <div className="control">
                                    <button
                                        className="button is-text"
                                        onClick={() => {
                                            closeForm();
                                        }}
                                        type="button">
                                        Cancel
                                    </button>
                                </div>
                                <div className="control">
                                    <button
                                        //disabled={!isDirty || !isValid}
                                        disabled={
                                            processing ||
                                            duplicateTemplateNameCheck ||
                                            !IsValidFilename ||
                                            !hasValues ||
                                            !isValidFileLength ||
                                            !IsValidFilenameWithoutDot ||
                                            !IsValidFilenameWithoutSpace ||
                                            !IsValidFilenameWithSpecialCharacters
                                        }
                                        type="submit">
                                        Create template
                                    </button>
                                </div>
                            </div>
                        </div>
                    </form>
            </Modal>
        </div>
    );
}
