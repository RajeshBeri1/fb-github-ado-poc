import React, { useEffect, useState, useRef } from 'react';
import { Control, useForm } from 'react-hook-form';
import { useNavigate } from 'react-router';
import {
    FlowChartComponent,
    HeaderTemplateCreateDTO,
    HeaderTemplateDetailsDTO,
    HeaderTemplateUpdateDTO,
} from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';
import moment from 'moment/moment';

import Link from '../../../components/Link';
import useNotification, {
    NotificationType,
} from '../../../components/notification/useNotification';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import { routes } from '../../routes';
import { headerTemplateApi } from '../../../lib/api';
import Divider from '../../../components/divider/divider';
import { HeaderRows } from './header-rows/default-rows/header-rows';
import { HeaderDetailRows } from './header-rows/detail-rows/header-detail-rows';
import { Tile } from '../../../omni/tile';
import { TemplateToolbar } from '../../../components/form/template-toolbar';
import useRender from '../../../hooks/useRender';
import useNameInput from '../../../hooks/useNameInput';
import OnLeaveDialog from '../../../components/dialogs/OnLeaveDialog';
import { HeaderLogoRows } from './header-rows/logo-rows/header-logo-rows';
import { headerTemplateDetails } from './header-default-form';
import SaveTemplateButton from '../../../components/buttons/SaveTemplateButton';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum'
import { isEmpty } from 'lodash';
export interface IHeaderFormProps {
    isUpdating?: boolean;
    params: any;
}

export const HeaderForm: React.FC<IHeaderFormProps> = ({
    isUpdating,
    params,
}) => {
    const navigate = useNavigate();
    const appContext = React.useContext(AppContext);
    const { flowchartTemplateDefinition, setUnsavedChanges } = appContext;
    const { OmniClientId } = flowchartTemplateDefinition;
    const goBack = routes('Header', params).list;
    const config = isUpdating && appContext.currentHeaderTemplateDetails;
    const pushNotification = useNotification();
    const [isSaving, setIsSaving] = useState(false);
    const { render } = useRender();
    const { logEvent } = useEventLogger(Module.HEADER);
    const configPrev = appContext.currentHeaderTemplateDetails;
    const previousState = useRef<HeaderTemplateDetailsDTO | null>(configPrev);

    const [leaveDialog, setLeaveDialog] = useState(false);
    const {
        handleSubmit,
        register,
        control,
        watch,
        getValues,
        formState: { isDirty },
        setValue,
    } = useForm({
        mode: 'onChange',
        reValidateMode: 'onChange',
        defaultValues: headerTemplateDetails(config),
    });
    const { renderDialog, setShowNameDialog, name } = useNameInput({
        backUrl: goBack,
        formKey: 'Name',
        register,
        control,
        title: `${isUpdating ? 'Update' : 'New'} header component`,
        inputLabel: 'Please enter a name for the header component.',
        setValue,
        initialValue: getValues('Name'),
        okText: `${isUpdating ? 'Update' : 'Create'} component`,
        componentName: FlowChartComponent.Header,
    });
    /*useEffect(() => {
        setUnsavedChanges((prevState) => ({ ...prevState, header: isDirty }));
    }, [isDirty])*/

    useEffect(() => {
        const subscription = watch((value) => {
            try {
                //transform dates
                value?.Definition?.Configuration?.Rows?.forEach((row) => {
                    row.Date = moment
                        .utc(row?.Date ?? moment.utc())
                        .format('YYYY-MM-DD');
                });

                //remove empty detail rows to prevent errors
                const details = value?.Definition?.Configuration?.Details;
                if (details && details.Rows && !details.Rows.length) {
                    value.Definition.Configuration.Details = undefined;
                }
                if (!isUpdating && !value.Id) {
                    value.Id = flowchartTemplateDefinition.Id;
                }

                if (isEmpty(name) && isEmpty(getValues('Name'))) {
                    appContext.setCurrentHeaderTemplateDetails(
                        plainToClass(HeaderTemplateDetailsDTO, previousState.current)

                    );
                }
                else {
                    appContext.setCurrentHeaderTemplateDetails(
                        plainToClass(HeaderTemplateDetailsDTO, value)
                    );
                }
            } catch (e) {
                console.log(e);
            }
        });
        return () => subscription.unsubscribe();
    }, [watch]);

    const createUpdateSuccess = async (template: HeaderTemplateDetailsDTO) => {
        pushNotification(
            `Header component "${template.Name}" was successfully ${isUpdating ? 'updated' : 'created'
            }!`,
            NotificationType.SUCCESS
        );

        // save in context here to access id!
        appContext.setCurrentHeaderTemplateDetails(
            plainToClass(HeaderTemplateDetailsDTO, template)
        );
        appContext.setTemplateChanged(true);
        navigate(goBack);
    };

    const createUpdateError = () => {
        pushNotification(
            `Header component could not be ${isUpdating ? 'updated' : 'created'
            }.`,
            NotificationType.DANGER
        );

        toggleSaving(false);
    };

    const updateTemplate = async (data) => {
        const headerUpdate = plainToClass(HeaderTemplateUpdateDTO, data);

        await headerTemplateApi
            .headerTemplateUpdate(headerUpdate)
            .then(async (x) => {
                await createUpdateSuccess(x.data);
            })
            .catch((e) => {
                console.error(e);
                createUpdateError();
            });
    };

    const createTemplate = (data) => {
        const headerCreate = plainToClass(HeaderTemplateCreateDTO, data);

        (headerCreate.OmniClientId = OmniClientId),
            headerTemplateApi
                .headerTemplateCreate(headerCreate)
                .then((x) => {
                    createUpdateSuccess(x.data);
                })
                .catch(() => {
                    createUpdateError();
                });
    };

    const pruneData = (data) => {
        if (data.Definition?.Configuration?.Logos.length) {
            //remove empty logo rows
            const filteredLogos = data.Definition.Configuration.Logos.filter(
                (logo) => logo.Image?.Content
            );
            return {
                ...data,
                Definition: {
                    ...data.Definition,
                    Configuration: {
                        ...data.Definition.Configuration,
                        Logos: filteredLogos,
                    },
                },
            };
        }

        return data;
    };

    const save = async (data) => {
        logEvent({ action: isUpdating ? Action.UPDATE : Action.CREATE });
        if (!getValues('Definition.Configuration.Rows').length) {
            pushNotification(
                `Header rows are missing. Component could not be ${isUpdating ? 'updated' : 'created'
                }.`,
                NotificationType.DANGER
            );
            return;
        }

        toggleSaving(true);
        data = pruneData(data);
        appContext.setCurrentHeaderTemplateDetails(data);
        if (isUpdating) {
            await updateTemplate(data);
        } else {
            await createTemplate(data);
        }
    };

    const handleCancel = () => {
        if (previousState.current) {
            appContext.setCurrentHeaderTemplateDetails(previousState.current);
        }
    };

    const toggleSaving = (newState: boolean) => {
        setIsSaving(newState);
    };

    return (
        <Tile className="h-100">
            <TemplateToolbar
                goBack={goBack}
                isUpdating={isUpdating}
                templateName={getValues('Name')}
                onEditTemplateName={() => setShowNameDialog(true)}
                definitionName="headerDefinition"
                definition={appContext.currentHeaderTemplateDetails?.Definition}
                title="Header Components"
            />
            <form
                key="headerConfigurator"
                onSubmit={handleSubmit(async (data) => {
                    await save(data);
                    setUnsavedChanges((prevState) => ({ ...prevState, header: true }));
                })}>
                <HeaderRows
                    clientId={OmniClientId}
                    control={
                        control as unknown as Control<HeaderTemplateDetailsDTO>
                    }
                    register={register}
                />

                <Divider />
                <HeaderDetailRows
                    clientId={OmniClientId}
                    title="Detail Rows"
                    control={
                        control as unknown as Control<HeaderTemplateDetailsDTO>
                    }
                />

                <Divider />
                <HeaderLogoRows
                    title="Logos"
                    control={
                        control as unknown as Control<HeaderTemplateDetailsDTO>
                    }
                />

                <div className="d-flex mt-3 is-justify-content-end">
                    <button onClick={(e) => { e.preventDefault(); logEvent({ action: Action.CANCEL }); setLeaveDialog(true); }} className="tertiary large">
                        Cancel
                    </button>

                    <SaveTemplateButton
                        disabled={!isDirty || isSaving}
                        type="submit"
                        loading={isSaving} className="large">
                        {isUpdating ? 'Save' : 'Create'} Header Component
                    </SaveTemplateButton>
                </div>
            </form>

            {renderDialog()}
            <OnLeaveDialog when={leaveDialog} setDialog={(e) => { setLeaveDialog(e); }} navigatePath={goBack} handleOk={handleCancel} definitionName={'headerDefinition'} definition={(previousState?.current?.Definition) ?? (appContext?.flowchartTemplateDefinition?.Definition?.HeaderDefinition?.Definition)} />
        </Tile>
    );
};
