import React, { useContext, useEffect, useState, useRef } from 'react';
import { useNavigate } from 'react-router';
import { useForm } from 'react-hook-form';
import { FlowChartComponent,
    ThemeTemplateCreateDTO,
    ThemeTemplateDetailsDTO,
    ThemeTemplateUpdateDTO,
} from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';

import { AppContext } from '../../../../taskpane/contexts/AppContext';
import { Title } from '../../../../components/form/title';
import { Icon } from '../../../../omni/icon';
import { Tile } from '../../../../omni/tile';
import { Toolbar } from '../../../../omni/toolbar';
import useNameInput from '../../../../hooks/useNameInput';
import { routes } from '../../../routes';
import { Page } from '../../../../enums/page.enum';
import OnLeaveDialog from '../../../../components/dialogs/OnLeaveDialog';
import { TemplateToolbar } from '../../../../components/form/template-toolbar';
import Link from '../../../../components/Link';
import { themeTemplateApi } from '../../../../lib/api';
import useNotification, {
    NotificationType,
} from '../../../../components/notification/useNotification';
import getThemeTemplateDetails from './theme-default';
import { StylingConfig } from '../../../../components/styling/styling-config';
import SaveTemplateButton from '../../../../components/buttons/SaveTemplateButton';

import '../theme-configurator/theme-configurator';
import './theme-form.css';
import useRender from '../../../../hooks/useRender';
import '../../../../pages/common-styles.css';
import Button from '../../../../components/buttons/Button';
import { SelectWithCommonFieldsTop } from '../../../../components/form/select-with-common-fields-Top';
import { OmniDropDownInput } from '../../../../omni/dropdown';
import { LegendDefault } from '../legend-form/legend-default';
import { isEmpty } from 'lodash';
import { SplitByDefault } from '../splitby-form/splitby-default';

export enum EditComponentSection {
    General = 'General',
    Hierarchy = 'Hierarchy',
    Header = 'Header 2',
    HeaderRow = 'Header Row Styling',
    Caledar = 'Calendar',
    CalendarOverlay = 'CalendarOverlay',
    MediaHierarchy = 'Media Hierarchy',
    GrandTotals = 'Grand Totals',
    Totals = 'Right Hand Totals',
    Footer = 'Footer',
    Legend = 'Legend',
    SplitBy = 'Split By',
}

export interface IThemeFormProps {
    isUpdating?: boolean;
    params: any;
}

export const ThemeForm: React.FC<IThemeFormProps> = ({
    isUpdating,
    params,
}) => {
    const appContext = useContext(AppContext);
    const goBack = routes(Page.Theme, params).list;
    const [editComponentSectionOpen, setEditCompponentSectionOpen] =
        useState<EditComponentSection>(EditComponentSection.General);
    const config = isUpdating && appContext.currentThemeTemplateDetails;
    const configPrev = appContext.currentThemeTemplateDetails;
    const previousState = useRef<ThemeTemplateDetailsDTO | null>(configPrev);
    const [isSaving, setIsSaving] = useState(false);
    const pushNotification = useNotification();
    const navigate = useNavigate();
    const { render } = useRender();
    const { setUnsavedChanges } = appContext;
    const [showSettings, setShowSettings] = useState<boolean>(false);

    const [leaveDialog, setLeaveDialog] = useState(false);
    const {
        handleSubmit,
        watch,
        register,
        setValue,
        getValues,
        control,
        reset,
        resetField,
        formState: { isDirty },
    } = useForm({
        mode: 'onChange',
        reValidateMode: 'onChange',
        defaultValues: getThemeTemplateDetails(config),
    });
    
    const { renderDialog, setShowNameDialog, name } = useNameInput({
        backUrl: goBack,
        formKey: 'Name',
        register,
        control,
        title: `${isUpdating ? 'Update' : 'New'} Theme Component`,
        inputLabel: 'Please enter a name for the Theme Component.',
        setValue,
        initialValue: getValues('Name'),
        //okText:'Ok',
        okText: 'Ok',
        componentName: FlowChartComponent.Theme,
    });

    useEffect(() => {
        const subscription = watch((value) => {

            if (isEmpty(name) && isEmpty(getValues('Name'))) {
                appContext.setCurrentThemeTemplateDetails(
                    plainToClass(ThemeTemplateDetailsDTO, previousState.current)

                );
            }
            else {
                appContext.setCurrentThemeTemplateDetails(
                    plainToClass(ThemeTemplateDetailsDTO, value)
                );
            }
        });
        return () => subscription.unsubscribe();
    }, [watch]);

    const createUpdateSuccess = (template: ThemeTemplateDetailsDTO) => {
        pushNotification(
            `Theme component "${template.Name}" was successfully ${isUpdating ? 'updated' : 'created'
            }!`,
            NotificationType.SUCCESS
        );

        // save in context here to access id!
        appContext.setCurrentThemeTemplateDetails(
            plainToClass(ThemeTemplateDetailsDTO, template)
        );
        appContext.setTemplateChanged(true);
        navigate(goBack);
    };

    const createUpdateError = () => {
        pushNotification(
            `Theme component could not be ${isUpdating ? 'updated' : 'created'
            }.`,
            NotificationType.DANGER
        );

        toggleSaving(false);
    };

    const updateTemplate = (data) => {
        const headerUpdate = plainToClass(ThemeTemplateUpdateDTO, data);

        themeTemplateApi
            .themeTemplateUpdate(headerUpdate)
            .then((x) => {
                createUpdateSuccess(x.data);
            })
            .catch((e) => {
                console.error(e);
                createUpdateError();
            });
    };

    const createTemplate = (data) => {
        const createDto = plainToClass(ThemeTemplateCreateDTO, data);

        (createDto.OmniClientId =
            appContext.flowchartTemplateDefinition.OmniClientId),
            themeTemplateApi
                .themeTemplateCreate(createDto)
                .then((x) => {
                    createUpdateSuccess(x.data);
                })
                .catch(() => {
                    createUpdateError();
                });
    };

    const save = (data) => {
        toggleSaving(true);
        appContext.setCurrentThemeTemplateDetails(data);
        if (isUpdating) {
            updateTemplate(data);
        } else {
            createTemplate(data);
        }
    };

    const handleCancel = () => {
        if (previousState.current) {
            appContext.setCurrentThemeTemplateDetails(previousState.current);
        }
    };

    const toggleSaving = (newState: boolean) => {
        setIsSaving(newState);
    };

    const resetForm = () => {
        reset();
    };

    const onChange = (key, value) => {
        setValue(key, value, {
            shouldDirty: true,
        });
    };


    /* useEffect(() => {
         setUnsavedChanges((prevState) => ({ ...prevState, theme: isDirty }));
     }, [isDirty])*/

    return (
        <Tile className="h-100">
            <TemplateToolbar
                goBack={goBack}
                definition={appContext.currentThemeTemplateDetails?.Definition}
                definitionName="themeDefinition"
                isUpdating={isUpdating}
                templateName={getValues('Name')}
                onEditTemplateName={() => setShowNameDialog(true)}
                title="Theme Components"
            /*              isKeepManualFormatting={appContext.isKeepManualFormatting}*/
            />
            <form
                key="themeConfigurator"
                onSubmit={handleSubmit((data) => {
                    save(data);
                    setUnsavedChanges((prevState) => ({ ...prevState, theme: true }));
                })}
                className="d-flex-">
                {/** START GENERAL THEME */}
                <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.General
                                        ? null
                                        : EditComponentSection.General
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.General
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark"> General</p>
                        </div>
                        {/* <div slot="end" className="toolbar-divider"></div>
                        <div slot="end">
                            <Button className="is-primary" onClick={() => resetForm()}>
                                Reset
                            </Button>
                        </div> */}
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.General ? (

                        <StylingConfig
                            styling={getValues('Definition.Styling')}
                            showInherited={false}
                            formKey={'Definition.Styling'}
                            onChange={onChange}
                            showPosition={true}
                        />

                    ) : (
                        <></>
                    )}
                </Tile>
                {/** START GENERAL THEME */}
                {/** START SPLITBY THEME */}
                <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.SplitBy
                                        ? null
                                        : EditComponentSection.SplitBy
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.SplitBy
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark">Split By</p>
                        </div>
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.SplitBy ? (
                        <SplitByDefault formKey={'Definition.SplitByColumn'} onChange={onChange}
                        splitByColumn={getValues('Definition.SplitByColumn')} />
                    ) : (
                        <></>
                    )}
                </Tile>

                {/** END SPLITBY THEME */}
                {/** START HIERARCHY THEME */}
                { /**  <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.Hierarchy
                                        ? null
                                        : EditComponentSection.Hierarchy
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.Hierarchy
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark">Hierarchy</p>
                        </div>
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.Hierarchy ? (
                            <>
                        <div className='d-flex is-justify-content-flex-end mb-3'>
                        <Button
                            className="is-outlined button-font-12 h-24"
                           >
                            Add level
                        </Button>
                            </div>
                            <div className="d-flex w-100">
                                <div className="w-5"> </div>
                            </div>
                            <div className="d-flex w-100 mb-2">
                                <div className="w-5"> </div>
                                    <div className="w-12 text-sm">Level</div>
                                <div className="w-60 text-sm"></div>
                                <div className="w-25 text-center text-sm">
                                        Actions
                                    </div>
                                </div>
                                <div className="d-flex w-100 align-items-center pb-3">
                                    <div className="w-5">
                                        <Icon
                                           
                                            icon-id="omni:interactive:reorder"></Icon>
                                    </div>
                                    <div className="w-12">
                                        <span className='text-md'>Level 1</span>
                                    </div>
                                    <div className="w-60 mr-2">
                                       
                                    </div>
                                    <div className="w-23">
                                        <div className=" d-flex is-justify-content-space-around">
                                            <Button
                                                className="is-text"
                                                tooltip="format style"
                                               >
                                                <Icon icon-id={`omni:informative:theme`}></Icon>
                                            </Button>
                                            <Button
                                                className="is-text"
                                                tooltip={showSettings ? 'back' : 'edit'}
                                                >
                                                <Icon
                                                    icon-id={`omni:interactive:${showSettings ? 'left' : 'edit'
                                                        }`}></Icon>
                                            </Button>
                                            <Button
                                                className="is-text"
                                                tooltip="delete"
                                                >
                                                <Icon icon-id="omni:interactive:trash"></Icon>
                                            </Button>
                                        </div>
                                    </div>
                                </div>
                            </>
                    ) : (
                        <></>
                    )}
                </Tile>**/}
                {/** START HIERARCHY THEME */}

                {/** START HEADER THEME */}
                <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.Header
                                        ? null
                                        : EditComponentSection.Header
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.Header
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark">Header Rows</p>
                        </div>
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.Header ? (
                        <StylingConfig
                            styling={getValues(
                                'Definition.HeaderTheme.Styling'
                            )}
                            formKey={'Definition.HeaderTheme.Styling'}
                            onChange={onChange}
                        />
                    ) : (
                        <></>
                    )}
                </Tile>
                {/** END HEADER THEME */}

                {/** START HEADER ROW THEME */}
                <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.HeaderRow
                                        ? null
                                        : EditComponentSection.HeaderRow
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.HeaderRow
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark">Header Detail Row</p>
                        </div>
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.HeaderRow ? (
                        <StylingConfig
                            styling={getValues(
                                'Definition.HeaderTheme.RowStyling'
                            )}
                            formKey={'Definition.HeaderTheme.RowStyling'}
                            onChange={onChange}
                        />
                    ) : (
                        <></>
                    )}
                </Tile>
                {/** END HEADER ROW THEME */}

                {/** START CALENDAR THEME */}
                <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.Caledar
                                        ? null
                                        : EditComponentSection.Caledar
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.Caledar
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark">Calendar</p>
                        </div>
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.Caledar ? (
                        <StylingConfig
                            styling={getValues(
                                'Definition.CalendarTheme.Styling'
                            )}
                            formKey={'Definition.CalendarTheme.Styling'}
                            onChange={onChange}
                        />
                    ) : (
                        <></>
                    )}
                </Tile>
                {/** END CALENDAR THEME */}

                {/** START CALENDAR OVERLAY THEME */}
                <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.CalendarOverlay
                                        ? null
                                        : EditComponentSection.CalendarOverlay
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.CalendarOverlay
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark">Calendar Overlay</p>
                        </div>
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.CalendarOverlay ? (
                        <div>
                            <header>
                                <p className="text-md">Section</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.CalendarOverlayTheme.SectionStyling'
                                )}
                                formKey={
                                    'Definition.CalendarOverlayTheme.SectionStyling'
                                }
                                onChange={onChange}
                            />
                            <header>
                                <p className="text-md">Row</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.CalendarOverlayTheme.RowStyling'
                                )}
                                formKey={
                                    'Definition.CalendarOverlayTheme.RowStyling'
                                }
                                onChange={onChange}
                            />
                        </div>
                    ) : (
                        <></>
                    )}
                </Tile>
                {/** END CALENDAR OVERLAY THEME */}

                {/** START MEDIA HIERARCHY THEME */}
                <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.MediaHierarchy
                                        ? null
                                        : EditComponentSection.MediaHierarchy
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.MediaHierarchy
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark">Media Hierarchy</p>
                        </div>
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.MediaHierarchy ? (
                        <div>
                            <header>
                                <p className="text-md">Media Hierarchy</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.MediaHierarchyTheme.Styling'
                                )}
                                formKey={
                                    'Definition.MediaHierarchyTheme.Styling'
                                }
                                onChange={onChange}
                            />
                            <header>
                                <p className="text-md">Media Hierarchy Left Menu</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.MediaHierarchyTheme.LeftMenuStyling'
                                )}
                                formKey={
                                    'Definition.MediaHierarchyTheme.LeftMenuStyling'
                                }
                                onChange={onChange}
                            />
                            <header>
                                <p className="text-md">Media Hierarchy Flightbars</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.MediaHierarchyTheme.FlightBarStyling'
                                )}
                                formKey={
                                    'Definition.MediaHierarchyTheme.FlightBarStyling'
                                }
                                onChange={onChange}
                            />
                            <header>
                                <p className="text-md">Sub Totals Title</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.MediaHierarchyTheme.SubTotalTitleStyling'
                                )}
                                formKey={
                                    'Definition.MediaHierarchyTheme.SubTotalTitleStyling'
                                }
                                onChange={onChange}
                            />
                            <header>
                                <p className="text-md">Sub Totals</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.MediaHierarchyTheme.SubTotalStyling'
                                )}
                                formKey={
                                    'Definition.MediaHierarchyTheme.SubTotalStyling'
                                }
                                onChange={onChange}
                            />
                            <header>
                                <p className="text-md">Inflight Overlay</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.MediaHierarchyTheme.InflightOverlayStyling'
                                )}
                                formKey={
                                    'Definition.MediaHierarchyTheme.InflightOverlayStyling'
                                }
                                onChange={onChange}
                            />
                        </div>
                    ) : (
                        <></>
                    )}
                </Tile>
                {/** END MEDIA HIERARCHY THEME */}

                {/** START LEGEND THEME */}
                <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.Legend
                                        ? null
                                        : EditComponentSection.Legend
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.Legend
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark">Legend</p>
                        </div>
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.Legend ? (
                        <LegendDefault formKey={'Definition.LegendTheme'} onChange={onChange} legend={getValues(
                            'Definition.LegendTheme'
                        )} />
                    ) : (
                        <></>
                    )}
                </Tile>
                {/** END LEGEND THEME */}

                {/** START GRAND TOTALS THEME */}
                <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.GrandTotals
                                        ? null
                                        : EditComponentSection.GrandTotals
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.GrandTotals
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark">Grand Totals</p>
                        </div>
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.GrandTotals ? (
                        <div>
                            <header>
                                <p className="text-md">Left menu</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.GrandTotalTheme.LeftMenuStyling'
                                )}
                                formKey={
                                    'Definition.GrandTotalTheme.LeftMenuStyling'
                                }
                                onChange={onChange}
                            />
                            <header>
                                <p className="text-md">Totals</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.GrandTotalTheme.Styling'
                                )}
                                formKey={'Definition.GrandTotalTheme.Styling'}
                                onChange={onChange}
                            />
                        </div>
                    ) : (
                        <></>
                    )}
                </Tile>
                {/** END GRAND TOTALS THEME */}

                {/** START TOTALS THEME */}
                <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.Totals
                                        ? null
                                        : EditComponentSection.Totals
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.Totals
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark">Right Hand Totals</p>
                        </div>
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.Totals ? (
                        <div>
                            <header>
                                <p className="text-md">Title</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.TotalsTheme.MainHeaderStyling'
                                )}
                                formKey={'Definition.TotalsTheme.MainHeaderStyling'}
                                onChange={onChange}
                            />
                            <header>
                                <p className="text-md">Header</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.TotalsTheme.HeaderStyling'
                                )}
                                formKey={'Definition.TotalsTheme.HeaderStyling'}
                                onChange={onChange}
                            />

                            <header>
                                <p className="text-md">Totals</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.TotalsTheme.Styling'
                                )}
                                formKey={'Definition.TotalsTheme.Styling'}
                                onChange={onChange}
                            />

                            <header>
                                <p className="text-md">Totals sum</p>
                            </header>
                            <StylingConfig
                                styling={getValues(
                                    'Definition.TotalsTheme.SumStyling'
                                )}
                                formKey={'Definition.TotalsTheme.SumStyling'}
                                onChange={onChange}
                            />
                        </div>
                    ) : (
                        <></>
                    )}
                </Tile>
                {/** END TOTALS THEME */}

                {/** START FOOTER THEME */}
                <Tile className="section-container">
                    <Toolbar slot="header" className="theme-toolbar">
                        <div
                            className="d-flex theme-section"
                            onClick={() =>
                                setEditCompponentSectionOpen(
                                    editComponentSectionOpen ===
                                        EditComponentSection.Footer
                                        ? null
                                        : EditComponentSection.Footer
                                )
                            }>
                            <Icon
                                icon-id={
                                    'omni:interactive:' +
                                    (editComponentSectionOpen ===
                                        EditComponentSection.Footer
                                        ? 'up'
                                        : 'down')
                                }
                                className="me-2"></Icon>
                            <p className="is-size-6 text-uppercase text-core-dark">Footer</p>
                        </div>
                    </Toolbar>
                    {editComponentSectionOpen ===
                        EditComponentSection.Footer ? (
                        <StylingConfig
                            styling={getValues(
                                'Definition.FooterTheme.Styling'
                            )}
                            formKey={'Definition.FooterTheme.Styling'}
                            onChange={onChange}
                        />
                    ) : (
                        <></>
                    )}
                </Tile>
                {/** END FOOTER THEME */}

                <div className="d-flex mt-3 is-justify-content-end">
                    <button className="tertiary large" onClick={(e) => {
                        e.preventDefault(); setLeaveDialog(true);
                    }}>
                        Cancel
                    </button>
                    <SaveTemplateButton
                        disabled={!isDirty}
                        type="submit"
                        loading={isSaving} className="large">
                        {isUpdating ? 'Save' : 'Create'} Theme Component
                    </SaveTemplateButton>
                </div>
            </form>
            {renderDialog()}
            <OnLeaveDialog when={leaveDialog} setDialog={(e) => { setLeaveDialog(e); }} navigatePath={goBack} handleOk={handleCancel} definitionName={'themeDefinition'} definition={(previousState?.current?.Definition) ?? (appContext?.flowchartTemplateDefinition?.Definition?.ThemeDefinition?.Definition)} />
        </Tile>
    );
};
