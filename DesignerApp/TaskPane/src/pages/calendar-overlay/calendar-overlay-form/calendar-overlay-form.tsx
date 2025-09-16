import React, { useContext, useEffect, useState, useRef } from 'react';
import { useForm, Control } from 'react-hook-form';
import { useNavigate } from 'react-router';
import { plainToClass } from 'class-transformer';
import {
    CalendarOverlayTemplateCreateDTO,
    CalendarOverlayTemplateDetailsDTO,
    CalendarOverlayTemplateUpdateDTO,
    FlowChartComponent,
} from '@omniflow/omni-webapi';
import moment from 'moment';

import { AppContext } from '../../../taskpane/contexts/AppContext';
import { Tile } from '../../../omni/tile';
import useNameInput from '../../../hooks/useNameInput';
import OnLeaveDialog from '../../../components/dialogs/OnLeaveDialog';
import { TemplateToolbar } from '../../../components/form/template-toolbar';
import Link from '../../../components/Link';
import { calendarOverlayTemplateApi } from '../../../lib/api';
import useNotification, {
    NotificationType,
} from '../../../components/notification/useNotification';
import calendarOverlayTemplateDetails from './calendar-overlay-default';
import { CalendarOverlaySections } from './calendar-overlay-sections/calendar-overlay-sections';
import { routes } from '../../routes';
import useRender from '../../../hooks/useRender';
import SaveTemplateButton from '../../../components/buttons/SaveTemplateButton';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum'
import '../../../pages/common-styles.css';
import './calendar-overlay-form.css';
import { isEmpty } from 'lodash';

export interface ICalendarOverlayFormProps {
    isUpdating?: boolean;
    params: any;
}

export const CalendarOverlayForm: React.FC<ICalendarOverlayFormProps> = ({
    isUpdating,
    params,
}) => {
    const { logEvent } = useEventLogger(Module.CALENDAROVERLAY);
    const appContext = useContext(AppContext);
    const goBack = routes('CalendarOverlay', params).list;
    const config =
        isUpdating && appContext.currentCalendarOverlayTemplateDetails;
    const [isSaving, setIsSaving] = useState(false);
    const [leaveDialog, setLeaveDialog] = useState(false);
    const { render } = useRender();
    const pushNotification = useNotification();
    const navigate = useNavigate();
    const { setUnsavedChanges } = appContext;

    const configPrev = appContext.currentCalendarOverlayTemplateDetails;
    const previousState = useRef<CalendarOverlayTemplateDetailsDTO | null>(configPrev);

    const {
        handleSubmit,
        watch,
        register,
        getValues,
        control,
        formState: { isDirty },
        setValue,
    } = useForm({
        mode: 'onChange',
        reValidateMode: 'onChange',
        defaultValues: calendarOverlayTemplateDetails(config),
    });

    const { renderDialog, setShowNameDialog, name } = useNameInput({
        backUrl: goBack,
        formKey: 'Name',
        register,
        control,
        title: `${isUpdating ? 'Update' : 'New'} calendar overlay component`,
        inputLabel: 'Please enter a name for the calendar overlay component.',
        setValue,
        initialValue: getValues('Name'),
        okText: `${isUpdating ? 'Update' : 'Create'} component`,
        componentName: FlowChartComponent.CalendarOverlay,
    });

    useEffect(() => {
        const subscription = watch((value) => {
            try {
                // format dates
                value.Definition.OverlaySections.forEach((section) => {
                    section.Rows.forEach((row) => {
                        row.StartDate = moment
                            .utc(row.StartDate)
                            .format('YYYY-MM-DD');
                        row.EndDate = moment
                            .utc(row.EndDate)
                            .format('YYYY-MM-DD');
                    });
                });
                   

                if (isEmpty(name) && isEmpty(getValues('Name'))) {
                    appContext.setCurrentCalendarOverlayTemplateDetails(
                        plainToClass(CalendarOverlayTemplateDetailsDTO, previousState.current)

                    );
                }
                else {
                    appContext.setCurrentCalendarOverlayTemplateDetails(
                        plainToClass(CalendarOverlayTemplateDetailsDTO, value)
                    );
                }
            } catch (e) {
                console.log(e);
            }
        });
        return () => subscription.unsubscribe();
    }, [watch]);

    const createUpdateSuccess = async (
        template: CalendarOverlayTemplateDetailsDTO
    ) => {
        pushNotification(
            `Calendar Overlay component "${template.Name}" was successfully ${isUpdating ? 'updated' : 'created'
            }!`,
            NotificationType.SUCCESS
        );

        // save in context here to access id!
        appContext.setCurrentCalendarOverlayTemplateDetails(
            plainToClass(CalendarOverlayTemplateDetailsDTO, template)
        );
        appContext.setTemplateChanged(true);
        navigate(goBack);
    };

    const createUpdateError = () => {
        pushNotification(
            `Calendar Overlay component could not be ${isUpdating ? 'updated' : 'created'
            }.`,
            NotificationType.DANGER
        );

        toggleSaving(false);
    };

    const updateTemplate = async (data) => {
        const overlayUpdate = plainToClass(
            CalendarOverlayTemplateUpdateDTO,
            data
        );

        await calendarOverlayTemplateApi
            .calendarOverlayTemplateUpdate(overlayUpdate)
            .then(async (x) => {
                await createUpdateSuccess(x.data);
            })
            .catch((e) => {
                console.error(e);
                createUpdateError();
            });
    };

    const createTemplate =  (data) => {
        const createDto = plainToClass(CalendarOverlayTemplateCreateDTO, data);

        (createDto.OmniClientId =
            appContext.flowchartTemplateDefinition.OmniClientId),
             calendarOverlayTemplateApi
                .calendarOverlayTemplateCreate(createDto)
                .then( (x) => {
                     createUpdateSuccess(x.data);
                })
                .catch(() => {
                    createUpdateError();
                });
    };

    const save = async (data) => {
        toggleSaving(true);
        appContext.setCurrentCalendarOverlayTemplateDetails(data);
        logEvent({ action: isUpdating ? Action.UPDATE : Action.CREATE });
        if (isUpdating) {
            await updateTemplate(data);
        } else {
            await createTemplate(data);
        }
    };
    const handleCancel = () => {
        if (previousState.current) {
            appContext.setCurrentCalendarOverlayTemplateDetails(plainToClass(CalendarOverlayTemplateDetailsDTO, previousState.current));
        }
    };

    const toggleSaving = (newState: boolean) => {
        setIsSaving(newState);
    };
    /* useEffect(() => {
         setUnsavedChanges((prevState) => ({ ...prevState, calendarOverlay: isDirty }));
     }, [isDirty])*/

    return (
        <Tile className="h-100">
            <TemplateToolbar
                goBack={goBack}
                definition={
                    appContext.currentCalendarOverlayTemplateDetails.Definition
                }
                definitionName="calendarOverlayDefinition"
                isUpdating={isUpdating}
                templateName={getValues('Name')}
                onEditTemplateName={() => setShowNameDialog(true)}
                title="Calendar Overlay Components"
            />
            <form
                key="calendarOverlayConfigurator"
                onSubmit={handleSubmit(async (data) => {
                    await save(data);
                    setUnsavedChanges((prevState) => ({ ...prevState, calendarOverlay: true }));

                })
                }>
                <CalendarOverlaySections
                    control={
                        control as unknown as Control<CalendarOverlayTemplateDetailsDTO>
                    }
                    register={register}
                />
                <div className="d-flex mt-3 is-justify-content-end">

                    <button onClick={(e) => { e.preventDefault(); logEvent({ action: Action.CANCEL }); setLeaveDialog(true); }} className="tertiary large">
                            Cancel
                        </button>
                   
                    <SaveTemplateButton
                        disabled={!isDirty}
                        type="submit"
                        loading={isSaving} className="large">
                        {isUpdating ? 'Save' : 'Create'} Calendar Overlay
                        Component
                    </SaveTemplateButton>
                </div>
            </form>
            {renderDialog()}
            <OnLeaveDialog when={leaveDialog} setDialog={(e) => { setLeaveDialog(e); } } navigatePath={goBack} handleOk={handleCancel} definitionName={'calendarOverlayDefinition'} definition={(previousState?.current?.Definition) ?? (appContext?.flowchartTemplateDefinition?.Definition?.CalendarOverlayDefinition?.Definition)} />
        </Tile>
    );
};
