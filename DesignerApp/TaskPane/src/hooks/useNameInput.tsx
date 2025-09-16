import React, { KeyboardEvent, useEffect, useRef } from 'react';
import { useState,useContext } from 'react';
import {
    RegisterOptions,
    UseFormRegisterReturn,
    useWatch
} from 'react-hook-form';
import Dialog, { ButtonType } from '../components/dialogs/Dialog';
import { Input } from '../components/form/input';
import useGoto from './useGoto';
import useEventLogger from './useEventLogger';
import { Action} from '../enums/event.enum';
import { Page } from '../enums/page.enum';
import { AppContext } from '../taskpane/contexts/AppContext';
import { ErrorMessage } from '../components/form/error-message';
import { FlowChartComponent, DuplicateNameCheckDto } from '@omniflow/omni-webapi';
import { commonApi } from '../lib/api';
import { plainToClass } from 'class-transformer';
import { useDebounce } from './useDebounce';
interface IUseNameInputProps {
    backUrl: string;
    register: (
        name: string,
        options?: RegisterOptions
    ) => UseFormRegisterReturn;
    formKey: string;
    control: any;
    title: string;
    inputLabel: string;
    setValue: any,
    initialValue?:string,
    okText: string,
    componentName:FlowChartComponent
}

/**
 * Shows a dialog to enter/edit template name
 */
const useNameInput = ({
            backUrl,
            register,
            formKey,
            control,
            title,
            inputLabel,
            setValue,
            initialValue,
            okText,
            componentName,
}: IUseNameInputProps) => {
    const { invalidCharacters, flowchartTemplateDefinition } = useContext(AppContext);
        const name = useWatch({ control, name: formKey });
        const [showNameDialog, setShowNameDialog] = useState(false);
        const goto = useGoto();
        const routePaths = backUrl.split('/');
    const currentModule = routePaths[routePaths.length - 1];
    const { logEvent } = useEventLogger(currentModule);
        const [invalidName, setInvalidName] = useState(false);
        const [tempName, setTempName] = useState(initialValue || '');
        const [invalidLength, setInvalidLength] = useState(false);
        const [isDuplicateName, setDuplicateName] = useState(false);
        const [processing, setProcessing] = useState(false);
        const maxLength: number = 100;
        const [query, setQuery] = useState('');
        const searchQuery = useDebounce(query, 1000);
        const controllerRef = useRef(new AbortController());
        useEffect(() => {
            if (!name) {
                setShowNameDialog(true);
            }
        }, []);

        useEffect(() => {
            if (showNameDialog) {
                setTempName(initialValue || '');
                renderDialog();
            }
        }, [showNameDialog]);

        const handleEnter = async (event: KeyboardEvent<HTMLInputElement>) => {
            setQuery(event.currentTarget.value);
            setInvalidName(invalidCharacters.test(event.currentTarget.value));
        setInvalidLength(event.currentTarget.value && event.currentTarget.value.length > maxLength);

            if (event.key === 'Enter') {
                event.preventDefault();
                // Add check for unchanged name
                if (event.currentTarget.value === initialValue) {
                    setDuplicateName(false);
                    handleOk();
                    return;
                }

            if (name &&
                    !invalidCharacters.test(event.currentTarget.value) &&
                    !processing &&
                    !invalidLength &&
                !isDuplicateName) {
                    handleOk();
                }
            }
        };

        useEffect(() => {
            (async () => {
                // Only check for duplicates if the name has actually changed
                if (searchQuery && searchQuery !== initialValue) {
                    try {
                    const { data } = await commonApi.commonCheckDuplicateComponentName(
                                plainToClass(DuplicateNameCheckDto, {
                            OmniClientId: flowchartTemplateDefinition.OmniClientId,
                                    Name: searchQuery,
                            ComponentName: componentName
                                })
                            );
                        setDuplicateName(data);
                        setProcessing(false);
                    } catch (e) {
                        console.error(e);
                        setDuplicateName(false);
                        setProcessing(false);
                    }
                } else {
                    setDuplicateName(false);
                    setProcessing(false);
                }
            })();

            return () => controllerRef.current.abort();
        }, [searchQuery]);

        const handleOk = async () => {
            // duplicate check if the name matches tempName
            if (name === tempName) {
                logEvent({ action: Action.OK, label: title });
                setShowNameDialog(false);
                return;
            }

        const { data: isDuplicate } = await commonApi.commonCheckDuplicateComponentName(
                    plainToClass(DuplicateNameCheckDto, {
                        OmniClientId: flowchartTemplateDefinition.OmniClientId,
                        Name: name,
                ComponentName: componentName
                    })
                );

            if (!isDuplicate) {
                logEvent({ action: Action.OK, label: title });
                setShowNameDialog(false);
            } else {
                setDuplicateName(true);
            }
        };

        const handleCancel = () => {
            setValue(formKey, tempName);
            logEvent({ action: Action.CANCEL, label: title });
            if (!tempName) {
                goto(backUrl);
            } else {
                setShowNameDialog(false);
            }
            setInvalidName(false);
            setInvalidLength(false);
            setDuplicateName(false);
        };

        const renderDialog = () => {
            return (
                showNameDialog && (
                    <Dialog
                        title={title}
                        okText={okText ?? 'Ok'}
                        icon="interactive:add"
                        okDisabled={!name || invalidName || invalidLength || processing || isDuplicateName}
                        onOk={handleOk}
                        onCancel={handleCancel}
                        buttonType={ButtonType.PRIMARY}
                        isTemplateDialog={true}
                        showDialog={showNameDialog}
                    >
                        <div className="d-flex">
                            <Input
                                label={inputLabel}
                                labelStyle="font-weight-bold w-100 no-capital"
                                placeholder="Enter component name"
                                register={register}
                                registerKey={formKey}
                                onKeyUp={handleEnter}
                                autoFocus
                            errorClass={invalidName || invalidLength || isDuplicateName ? 'error' : ''}
                                inputLabel="Component name"
                            />
                        </div>
                    {invalidName && <ErrorMessage message='A component name can only contain one of the following characters: A-Z a-z 0-9' ></ErrorMessage>}
                    {invalidLength && <ErrorMessage message='The component name must be 100 characters or less'></ErrorMessage>}
                    {isDuplicateName && <ErrorMessage message='The component name has already been used / unavailable'></ErrorMessage>}                    
                    </Dialog>
                )
            );
        };

        return { renderDialog, setShowNameDialog, name };
};

export default useNameInput;
