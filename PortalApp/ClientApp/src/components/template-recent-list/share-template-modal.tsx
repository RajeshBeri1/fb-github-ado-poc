/* eslint-disable no-control-regex */
import { DropDownInput } from '../../omni-ui-components/dropdown';
import { Icon } from '../../omni-ui-components/icon';
import { useEffect, useState, useRef, JSX, useImperativeHandle, forwardRef } from 'react';
import { useDebounce } from '../../hooks/useDebounce';
import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';
import RequestInterceptor from '../../business/request-interceptor';
import { Modal } from '../../omni-ui-components/modal';
import {
    FlowchartTemplateApi,
    FlowchartTemplateDuplicateCheckDTO,
} from '@omniflow/omni-webapi';
type ShareTemplateModalPropsType = {
    templateName: string;
    selectedClientName: any;
    clientNameOptions: any;
    componentOptions: any;
    selectedComponentOptions: any;
    showClientName: (e: any) => void;
    showComponentName: (e: any) => void;
    setTemplateName: (e: any) => void;
    //setShareTemplateModal: (e: any) => void;
    setConfirmShareTemplate: (e: any) => void;
    duplicateTemplateNameCheck: boolean;
    setDuplicateTemplateNameCheck: (e: any) => void;
    handleConfirmModal: () => void;
    modalHandler: (a: any) => void;
};
const ShareTemplateModal = forwardRef(({
    templateName,
    selectedClientName,
    clientNameOptions,
    componentOptions,
    selectedComponentOptions,
    showClientName,
    showComponentName,
    setTemplateName,
    //setShareTemplateModal,
    setConfirmShareTemplate,
    duplicateTemplateNameCheck,
    setDuplicateTemplateNameCheck,
    handleConfirmModal,
    modalHandler,
}: ShareTemplateModalPropsType,ref): JSX.Element =>{
    const [buttonDisabled, setButtonDisabled] = useState(true);
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
    const [previousTemplateName, setPreviousTemplateName] = useState('');
    const [processing, setProcessing] = useState(false);
    const controllerRef = useRef(new AbortController());
    const templateApi = new FlowchartTemplateApi(
        WEBAPI_CONFIGURATION,
        '',
        RequestInterceptor
    );

    const searchQuery = useDebounce(query, 1000);
    const fileNameRegex = /[#$+%!`&'={}@<>:"|?*\u0000-\u001F^(),[\]]/; // =>  Regex = /[<>:"/\\|?*\u0000-\u001F]/g;
    const windowsRegex = /^(con|prn|aux|nul|com\d|lpt\d)$/i;
    const fullStopInBeginning = /^\.+/;
    const spaceAtBeginning = /^(?!\s)(.*[^\s])$/; // for not allowing neither at beginning nor at the end ==> Regex=  /^(?!\s).+(?<!\s)$/g;    || For not allowing at beginning only ==> Regex =  /^\ +/;
    const validator = (value: string) => {
        if (!value) return false;

        setIsValidFilename(!windowsRegex.test(value));
        setIsValidFilenameWithoutDot(!fullStopInBeginning.test(value));
        setIsValidFilenameWithoutSpace(spaceAtBeginning.test(value));
        setIsValidFilenameWithSpecialCharacters(!fileNameRegex.test(value));
    };

    const onChangeHandler = (event: { target: { value: any } }) => {
        const value = event.target.value;
        setTemplateName(event.target.value);
        setProcessing(true);
        setQuery(value);
        validator(value);
        setHasValues(value);
    };

    const modalChildRefShare = useRef<any>(null);
    const confirmModalRef = useRef<any>(null);

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
    useEffect(() => {
        if (
            templateName &&
            selectedClientName?.value?.length > 0 &&
            selectedComponentOptions?.length > 0 &&
            !duplicateTemplateNameCheck
        ) {
            setButtonDisabled(false);
        } else {
            setButtonDisabled(true);
        }
    }, [
        templateName,
        selectedClientName,
        selectedComponentOptions,
        duplicateTemplateNameCheck,
    ]);

    useImperativeHandle(ref, () => ({
        showModal: () => {
            if (modalChildRefShare.current) {
                modalChildRefShare.current.showModal();
            }

        },
        close: () => {
            if (modalChildRefShare.current) {
                modalChildRefShare.current.close();
            }

        },

    }));

    return (
        <Modal ref={modalChildRefShare} className="dialog-500px">
                <div slot="header" className="modal-header">
                    <h2 className="title">
                        Share Template :{' '}
                        <span className="fb-bold">{templateName} </span>
                    </h2>
                </div>
                <div className="">
                    <p className="text-msg mb-5 mt-3">
                        Please select client and component(s) to share
                    </p>
                    <div className="mb-5">
                        <div className="control">
                            <DropDownInput
                                label="Client name"
                                className="w-100 modal-dropdown"
                                placeholder="Select Client"
                                onValueChange={(e) => showClientName(e)}
                                value={[selectedClientName]}
                                options={clientNameOptions}
                                hidefooter
                                searchindropdown
                                typeahead
                                select={selectedClientName}
                                required></DropDownInput>
                        </div>
                    </div>
                    <div className="mb-5">
                        <div className="control">
                            <DropDownInput
                                className="w-100 modal-dropdown"
                                label="Component"
                                onValueChange={(e) => showComponentName(e)}
                                multiselect
                                options={componentOptions}
                                value={selectedComponentOptions}
                                selectall={true}
                                hidefooter
                                required></DropDownInput>
                        </div>
                    </div>
                    <div className="mb-5">
                        <div className="input-container">
                            <div className="label-container mb-2">
                                <label className="template-name-label">
                                    Template name
                                </label>
                            </div>
                            <input
                                required
                                className={
                                    'input modal-input ' +
                                    (duplicateTemplateNameCheck ? 'error' : '')
                                }
                                placeholder="Enter Text"
                                onChange={onChangeHandler}
                                value={templateName}
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
                                        The template name has already been used
                                        / unavailable
                                    </span>
                                </div>
                            )}
                        </div>
                    </div>
                    <div slot="footer" className="field is-grouped is-justify-content-end">
                        <div className="control">
                            <button
                                className="tertiary"
                            onClick={() => modalChildRefShare.current.close()}>
                                Cancel
                            </button>
                        </div>
                        <div className="control">
                            <button
                                disabled={buttonDisabled}
                                onClick={() => {
                                    modalChildRefShare.current.close();
                                    handleConfirmModal();
                                }}>
                                Share
                            </button>
                        </div>
                    </div>
                </div>
        </Modal>
    );
});
export default ShareTemplateModal;
