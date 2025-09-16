import React, { useEffect, useState, useMemo, useContext, useRef } from 'react';
import { useNavigate } from 'react-router';
import { Control, useForm, useWatch } from 'react-hook-form';
import { plainToClass } from 'class-transformer';
import {
    CalendarTemplateCreateDTO,
    CalendarTemplateDetailsDTO,
    CalendarTemplateUpdateDTO,
    CalendarType,
    FlowChartComponent,
} from '@omniflow/omni-webapi';
import moment from 'moment';

import { CalendarRowsSetting } from './calendar-rows';
import { CalendarTypeMap } from '../../../business/calendar-type-map';
import { Title } from '../../../components/form/title';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import Link from '../../../components/Link';
import { routes } from '../../routes';
import useNotification, {
    NotificationType,
} from '../../../components/notification/useNotification';
import { calendarTemplateApi } from '../../../lib/api';
import useNameInput from '../../../hooks/useNameInput';
import { TemplateToolbar } from '../../../components/form/template-toolbar';
import { Tile } from '../../../omni/tile';
import OnLeaveDialog from '../../../components/dialogs/OnLeaveDialog';
import { CalendarTimelineSetting } from './calendar-timeline-setting/calendar-timeline-setting';
import { calendarTemplateDetails } from './calendar-default-form';
import SaveTemplateButton from '../../../components/buttons/SaveTemplateButton';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum'
import '../calendar-form/calendar-rows/calendar-rows.css';
import { OmniDropDownInput } from '../../../omni/dropdown';
import { isEmpty } from 'lodash';
import { OmniCheckBoxInput } from '../../../omni/checkbox';
export interface ICalendarFormProps {
    isUpdating?: boolean;
    params: any;
}

export const CalendarForm: React.FC<ICalendarFormProps> = ({
    isUpdating = false,
    params,
}) => {
    const { logEvent } = useEventLogger(Module.CALENDAR);
    const [isSaving, setIsSaving] = useState(false);
    const navigate = useNavigate();
    const appContext = React.useContext(AppContext);
    const { flowchartTemplateDefinition, setUnsavedChanges } = appContext;
    const { OmniClientId } = flowchartTemplateDefinition;
    const goBack = routes('Calendar', params).list;
    const pushNotification = useNotification();

    const config = isUpdating && appContext.currentCalendarTemplateDetails;
    const configPrev = appContext.currentCalendarTemplateDetails;
    const previousState = useRef<CalendarTemplateDetailsDTO | null>(configPrev);

    const [leaveDialog, setLeaveDialog] = useState(false);
    const {
        handleSubmit,
        watch,
        register,
        setValue,
        getValues,
        formState: { isDirty },
        control,
    } = useForm({
        mode: 'onChange',
        reValidateMode: 'onChange',
        defaultValues: calendarTemplateDetails(config),
    });
    const { renderDialog, setShowNameDialog, name } = useNameInput({
        backUrl: goBack,
        formKey: 'Name',
        register,
        control,
        title: `${isUpdating ? 'Update' : 'New'} calendar component`,
        inputLabel: 'Please enter a name for the calendar component.',
        setValue,
        initialValue: getValues('Name'),
        okText: `${isUpdating ? 'Update' : 'Create'} component`,
        componentName: FlowChartComponent.Calendar,
    });
    /* useEffect(() => {
         setUnsavedChanges((prevState) => ({ ...prevState, calendar: isDirty }));
     }, [isDirty])*/

    useEffect(() => {
        const subscription = watch((value) => {
            // TODO this is a bit hacky refactor soon! -> ensures that dates are set
            try {
                value.Definition.Configuration.CustomStartDate = moment
                    .utc(
                        value?.Definition?.Configuration?.CustomStartDate ??
                        moment.utc().startOf('year')
                    )
                    .format('YYYY-MM-DD');

                value.Definition.Configuration.CustomEndDate = moment
                    .utc(
                        value?.Definition?.Configuration?.CustomEndDate ??
                        moment.utc().endOf('year')
                    )
                    .format('YYYY-MM-DD');

                if (isEmpty(name) && isEmpty(getValues('Name'))) {
                    appContext.setCurrentCalendarTemplateDetails(
                        plainToClass(CalendarTemplateDetailsDTO, previousState.current)

                    );
                }
                else {
                    appContext.setCurrentCalendarTemplateDetails(
                        plainToClass(CalendarTemplateDetailsDTO, value)

                    );
                }

            } catch (e) {
                console.log(e);
            }
        });
        return () => subscription.unsubscribe();
    }, [watch]);

    const toggleSaving = (newState: boolean) => {
        setIsSaving(newState);
    };

    const createUpdateSuccess = async (
        template: CalendarTemplateDetailsDTO
    ) => {
        pushNotification(
            `Calendar component "${template.Name}" was successfully ${isUpdating ? 'updated' : 'created'
            }!`,
            NotificationType.SUCCESS
        );

        // save in context here to access id!
        appContext.setCurrentCalendarTemplateDetails(
            plainToClass(CalendarTemplateDetailsDTO, template)
        );
        appContext.setTemplateChanged(true);
        navigate(goBack);
    };

    const createUpdateError = () => {
        pushNotification(
            `Calendar component could not be ${isUpdating ? 'updated' : 'created'
            }.`,
            NotificationType.DANGER
        );

        toggleSaving(false);
    };

    const updateTemplate = async (data) => {
        const calendarUpdate = plainToClass(CalendarTemplateUpdateDTO, data);

        await calendarTemplateApi
            .calendarTemplateUpdate(calendarUpdate)
            .then(async (x) => {
                await createUpdateSuccess(x.data);
            })
            .catch(() => {
                createUpdateError();
            });
    };

    const createTemplate = (data) => {
        const calendarCreate = plainToClass(CalendarTemplateCreateDTO, data);

        (calendarCreate.OmniClientId =
            appContext.flowchartTemplateDefinition.OmniClientId),
            calendarTemplateApi
                .calendarTemplateCreate(calendarCreate)
                .then((x) => {
                    createUpdateSuccess(x.data);
                })
                .catch(() => {
                    createUpdateError();
                });
    };

    const isValidForm = () => {
        let isValid = true;

        try {
            const endDate = moment
                .utc(getValues('Definition.Configuration.CustomEndDate'))
                .toDate();
            const startDate = moment
                .utc(getValues('Definition.Configuration.CustomStartDate'))
                .toDate();

            if (startDate > endDate) {
                pushNotification(
                    `Calendar dates are invalid. Component could not be ${isUpdating ? 'updated' : 'created'
                    }.`,
                    NotificationType.DANGER
                );
                isValid = false;
            }
        } catch {
            pushNotification(
                `Calendar dates are invalid. Component could not be ${isUpdating ? 'updated' : 'created'
                }.`,
                NotificationType.DANGER
            );
            isValid = false;
        }

        if (!getValues('Definition.Configuration.Rows').length) {
            pushNotification(
                `Calendar rows are missing. Component could not be ${isUpdating ? 'updated' : 'created'
                }.`,
                NotificationType.DANGER
            );
            isValid = false;
        }
        return isValid;
    };

    const save = async (data) => {
        if (!isValidForm()) {
            return;
        }
        toggleSaving(true);
        logEvent({ action: isUpdating ? Action.UPDATE : Action.CREATE });
        if (isUpdating) {
            await updateTemplate(data);
        } else {
            await createTemplate(data);
        }
    };


    const handleCancel = async () => {
        if (previousState.current) {
            appContext.setCurrentCalendarTemplateDetails(
                plainToClass(CalendarTemplateDetailsDTO, previousState.current)
            );
        }
    };

    const calendarTypes = Object.keys(CalendarType).map((t) => {
        return { id: t, value: CalendarTypeMap(CalendarType[t]) }

    });

    const calendarTypeValue = useMemo(() => {
        if (getValues('Definition.Configuration.Type') && CalendarTypeMap(CalendarType[getValues('Definition.Configuration.Type')])) {
            return [calendarTypes.find(c => c.id == getValues('Definition.Configuration.Type'))]

        }
        else {
            setValue('Definition.Configuration.Type', CalendarType.Standard, { shouldDirty: true });
            return [calendarTypes.find(c => c.value == CalendarTypeMap(CalendarType.Standard))]
        }


    }, [getValues('Definition.Configuration.Type')]
    )

    const currentCalendarType = useWatch({
        control,
        name: 'Definition.Configuration.Type',
    });

    const isUseCustomStartDayEnabled = useWatch({
        control,
        name: 'Definition.Configuration.UseCustomStartDay',
    });

    const handleChange = (e) => {
        setValue('Definition.Configuration.IsNormalizedEnabled', e.target.checked, { shouldDirty: true });
    }

    const handleUseCustomStartDayChange = (e) => {
        setValue('Definition.Configuration.UseCustomStartDay', e.target.checked, { shouldDirty: true });
    }

    useEffect(() => {
        if (currentCalendarType == CalendarType.Standard && isUseCustomStartDayEnabled) {
            moment.updateLocale('en', {
                week: {
                    dow: 1
                }
            });
        } else {
            moment.updateLocale('en', {
                week: {
                    dow: 0
                }
            });
        }
    }, [currentCalendarType, isUseCustomStartDayEnabled])


    const isNormalizedRequired =getValues('Definition.Configuration.Type') == CalendarType.Standard;
    const isCustomStartDayRequired = getValues('Definition.Configuration.Type') == CalendarType.Standard;


    useEffect(() => {
        if (currentCalendarType && currentCalendarType != CalendarType.Standard) {
            setValue('Definition.Configuration.IsNormalizedEnabled', false);
            setValue('Definition.Configuration.UseCustomStartDay', false);
        }
    }, [currentCalendarType])

    return (
        <Tile className="h-100">
            <TemplateToolbar
                title="Calendar Components"
                isUpdating={isUpdating}
                templateName={getValues('Name')}
                onEditTemplateName={() => setShowNameDialog(true)}
                definitionName="calendarDefinition"
                definition={
                    appContext.currentCalendarTemplateDetails?.Definition
                }
                goBack={goBack}
            />

            <form
                key="calendarConfigurator"
                onSubmit={handleSubmit(async (data) => {
                    await save(data);
                    setUnsavedChanges((prevState) => ({ ...prevState, calendar: true }));
                })}>
                <div className="d-flex mb-3">
                    <p className="component-title">General calendar settings</p>
                </div>
                <div className="d-flex mb-2">
                    <OmniDropDownInput
                        label="Calendar type"
                        className="w-100"
                        onValueChange={(e: CustomEvent) => { setValue('Definition.Configuration.Type', e.detail.id, { shouldDirty: true }) }}
                        value={calendarTypeValue}
                        options={calendarTypes} hidefooter>
                    </OmniDropDownInput>
                </div>

                <CalendarTimelineSetting
                    register={register}
                    getValues={getValues}
                    setValue={setValue}
                />
                {isNormalizedRequired && (
                    <div className="d-flex mb-2 mt-2">
                        <OmniCheckBoxInput checked={getValues('Definition.Configuration.IsNormalizedEnabled')} onClick={(e) => handleChange(e)} className="checkbox"> <label className="mb-0 text-core-dark">Enable Normalized View</label></OmniCheckBoxInput>
                    </div>
                )}

                {isCustomStartDayRequired && (
                    <div className="d-flex mb-2 mt-2">
                        <OmniCheckBoxInput checked={getValues('Definition.Configuration.UseCustomStartDay')} onClick={(e) => handleUseCustomStartDayChange(e)} className="checkbox"> <label className="mb-0 text-core-dark"> Start Week From Monday</label></OmniCheckBoxInput>
                    </div>
                )}

                <CalendarRowsSetting
                    clientId={OmniClientId}
                    control={
                        control as unknown as Control<CalendarTemplateDetailsDTO>
                    }
                />

                <div className="d-flex is-justify-content-end">

                    <button onClick={(e) => {
                        e.preventDefault();
                        logEvent({ action: Action.CANCEL });
                        setLeaveDialog(true);
                    }} className="tertiary large">
                        Cancel
                    </button>

                    <SaveTemplateButton
                        disabled={!isDirty || isSaving}
                        type="submit"
                        loading={isSaving}
                        className="large"
                    >
                        {isUpdating ? 'Save' : 'Create'} calendar component
                    </SaveTemplateButton>
                </div>
            </form>
            {renderDialog()}
            <OnLeaveDialog when={leaveDialog && Boolean(getValues('Name'))} setDialog={(e) => { setLeaveDialog(e); }} navigatePath={goBack} handleOk={handleCancel} definitionName={'calendarDefinition'} definition={(previousState?.current?.Definition) ?? (appContext?.flowchartTemplateDefinition?.Definition?.CalendarDefinition?.Definition)} />
        </Tile>
    );
};
