import React, { useEffect, useState, useRef } from 'react';
import { useNavigate } from 'react-router';
import { Control, useForm } from 'react-hook-form';
import {
    FlowChartComponent,
    FooterTemplateCreateDTO,
    FooterTemplateDetailsDTO,
    FooterTemplateUpdateDTO,
} from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';

import Link from '../../../components/Link';
import useNotification, {
    NotificationType,
} from '../../../components/notification/useNotification';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import { routes } from '../../routes';
import { Tile } from '../../../omni/tile';
import { TemplateToolbar } from '../../../components/form/template-toolbar';
import useNameInput from '../../../hooks/useNameInput';
import OnLeaveDialog from '../../../components/dialogs/OnLeaveDialog';
import { defaultFooterDetails } from './footer-default-form';
import useRender from '../../../hooks/useRender';
import { footerTemplateApi } from '../../../lib/api';
import { FooterGeneralSettings } from './footer-general-settings/footer-general-settings';
import Divider from '../../../components/divider/divider';
import { FooterRows } from './footer-rows/footer-rows';
import SaveTemplateButton from '../../../components/buttons/SaveTemplateButton';
import { isEmpty } from 'lodash';

export interface IFooterFormProps {
    isUpdating?: boolean;
    params: any;
}

export const FooterForm: React.FC<IFooterFormProps> = ({
    isUpdating,
    params,
}) => {
    const navigate = useNavigate();
    const appContext = React.useContext(AppContext);
    const goBack = routes('Footer', params).list;
    const config: FooterTemplateUpdateDTO | undefined =
        isUpdating && appContext.currentFooterTemplateDetails;
    const configPrev = appContext.currentFooterTemplateDetails;
    const previousState = useRef<FooterTemplateDetailsDTO | null>(configPrev);
    const pushNotification = useNotification();
    const [isSaving, setIsSaving] = useState(false);
    const { render } = useRender();
    const { setUnsavedChanges } = React.useContext(AppContext);
    const [leaveDialog, setLeaveDialog] = useState(false);
    const {
        formState: { isDirty },
        control,
        handleSubmit,
        register,
        watch,
        getValues,
        setValue,
    } = useForm({
        mode: 'onChange',
        reValidateMode: 'onChange',
        defaultValues: defaultFooterDetails(config),
    });

    const { renderDialog, setShowNameDialog, name } = useNameInput({
        backUrl: goBack,
        formKey: 'Name',
        register,
        control,
        title: `${isUpdating ? 'Update' : 'New'} footer component`,
        inputLabel: 'Please enter a name for the footer component.',
        setValue,
        initialValue: getValues('Name'),
        okText: `${isUpdating ? 'Update' : 'Create'} component`,
        componentName: FlowChartComponent.Footer,
    });

    useEffect(() => {
        const subscription = watch((value) => {
                
            if (isEmpty(name) && isEmpty(getValues('Name'))) {
                appContext.setCurrentFooterTemplateDetails(
                    plainToClass(FooterTemplateDetailsDTO, previousState.current)

                );
            }
            else {
                appContext.setCurrentFooterTemplateDetails(
                    plainToClass(FooterTemplateDetailsDTO, value)
                );  
            }
        });
        return () => subscription.unsubscribe();
    }, [watch]);

    const createUpdateSuccess = async (template: FooterTemplateDetailsDTO) => {
        pushNotification(
            `Footer component "${template.Name}" was successfully ${
                isUpdating ? 'updated' : 'created'
            }!`,
            NotificationType.SUCCESS
        );

        // save in context here to access id!
        appContext.setCurrentFooterTemplateDetails(
            plainToClass(FooterTemplateDetailsDTO, template)
        );
        appContext.setTemplateChanged(true);
        navigate(goBack);
    };

    const createUpdateError = () => {
        pushNotification(
            `Footer component could not be ${
                isUpdating ? 'updated' : 'created'
            }.`,
            NotificationType.DANGER
        );

        toggleSaving(false);
    };

    const updateTemplate = async (data) => {
        const footerUpdate = plainToClass(FooterTemplateUpdateDTO, data);

        await footerTemplateApi
            .footerTemplateUpdate(footerUpdate)
            .then(async (x) => {
                await createUpdateSuccess(x.data);
            })
            .catch((e) => {
                console.error(e);
                createUpdateError();
            });
    };

    const createTemplate =  (data) => {
        const footerCreate = plainToClass(FooterTemplateCreateDTO, data);

        (footerCreate.OmniClientId =
            appContext.flowchartTemplateDefinition.OmniClientId),
             footerTemplateApi
                .footerTemplateCreate(footerCreate)
                .then( (x) => {
                     createUpdateSuccess(x.data);
                })
                .catch(() => {
                    createUpdateError();
                });
    };

    const save = async (data) => {
        toggleSaving(true);
        appContext.setCurrentFooterTemplateDetails(data);
        if (isUpdating) {
            await updateTemplate(data);
        } else {
            await createTemplate(data);
        }
    };
    const handleCancel = () => {
        if (previousState.current) {
            appContext.setCurrentFooterTemplateDetails(plainToClass(FooterTemplateCreateDTO,previousState.current));
        }
    };
    const toggleSaving = (newState: boolean) => {
        setIsSaving(newState);
    };
   /* useEffect(() => {
        setUnsavedChanges((prevState) => ({ ...prevState, footer: isDirty }));
    }, [isDirty])*/

    return (
        <Tile className="h-100">
            <TemplateToolbar
                goBack={goBack}
                isUpdating={isUpdating}
                templateName={getValues('Name')}
                onEditTemplateName={() => setShowNameDialog(true)}
                definitionName="footerDefinition"
                definition={appContext.currentFooterTemplateDetails?.Definition}
                title="Footer Components"
            />
            <form
                key="footerConfigurator"
                onSubmit={handleSubmit(async (data) => {
                    await save(data);
                    setUnsavedChanges((prevState) => ({ ...prevState, footer: true }));
                })}>
                {/* <FooterGeneralSettings
                    register={register}
                    getValues={getValues}
                    setValue={setValue}
                />

                <Divider />*/}
                <FooterRows
                    control={
                        control as unknown as Control<FooterTemplateDetailsDTO>
                    }
                />
                <div className="d-flex mt-3 is-justify-content-end">
                   
                    <button className="tertiary large" onClick={(e) => {
                        e.preventDefault(); 
                        setLeaveDialog(true);
                    }}>
                            Cancel
                        </button>
                    
                    <SaveTemplateButton
                        disabled={!isDirty || isSaving}
                        type="submit"
                        loading={isSaving} className="large">
                        {isUpdating ? 'Save' : 'Create'} Footer Component
                    </SaveTemplateButton>
                </div>
            </form>

            {renderDialog()}
            <OnLeaveDialog when={leaveDialog} setDialog={(e) => { setLeaveDialog(e); }} navigatePath={goBack} handleOk={handleCancel} definitionName={'footerDefinition'} definition={(previousState?.current?.Definition) ?? (appContext?.flowchartTemplateDefinition?.Definition?.FooterDefinition?.Definition)} />
        </Tile>
    );
};
