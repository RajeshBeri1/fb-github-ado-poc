import React, { useContext, useEffect, useState, useRef } from 'react';
import { useNavigate } from 'react-router';
import {
    CalendarTemplateInfoDTO,
    CalendarTemplateSearchDTO,
    CalendarTemplateCreateDTO,
} from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';

import { Toolbar } from '../../../omni/toolbar';
import Search from '../../../components/Search';
import { Tile } from '../../../omni/tile';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import { TemplateListEntries } from '../../../components/template-list/template-list-entries';
import useNotification, {
    NotificationType,
} from '../../../components/notification/useNotification';
import Button from '../../../components/buttons/Button';
import { routes } from '../../routes';
import Dialog, { DialogType } from '../../../components/dialogs/Dialog';
import RenderButton from '../../../components/buttons/RenderButton';
import { SaveFlowchartButton } from '../../../components/buttons/SaveFlowchartButton';
import { calendarTemplateApi } from '../../../lib/api';
import useTemplateList from '../../../hooks/useTemplateList';
import useTemplateUpdateRender from '../../../hooks/useTemplateUpdateRender';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum'
import '../../../pages/common-styles.css';
import { OmniSwitchButton } from '../../../components/omniSwitchButton';

export const CalendarList: React.FC<{ params: any }> = ({ params }) => {
    const { logEvent } = useEventLogger(Module.CALENDAR);
    const appContext = useContext(AppContext);
    const navigate = useNavigate();
    const pushNotification = useNotification();
    const [items, setItems] = useState<CalendarTemplateInfoDTO[]>([]);
    const [archivedItems, setArchivedItems] = useState<CalendarTemplateInfoDTO[]>([]);
    const [templateToCopy, setTemplateToCopy] = useState(null);
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(null);
    const { deselectTemplate } = useTemplateList();
    const { setUnsavedChanges } = useContext(AppContext);
    const templateRoutes = routes('Calendar', params);
    const { renderUpdates } = useTemplateUpdateRender();
    const [showArchiveTemplate, setShowArchiveTemplate] = useState<boolean>(false);
    const [showActiveTemplate, setShowActiveTemplate] = useState<boolean>(true);

    const [lastSearchTerm, setLastSearchTerm] = useState<string | null>(null);
    const deleteDialogRef = useRef<any>(null);
    useEffect(() => {
        if (appContext.flowchartTemplateDefinition.OmniClientId) {
            loadTemplates();
            loadArchivedTemplates();
        }
    }, [appContext.flowchartTemplateDefinition.OmniClientId]);

    const loadOrDeselectTemplateById = async (id: string) => {
        if (!id) return;
      
        const result = deselectTemplate(
            id,
            'CalendarDefinition',
            appContext.currentCalendarTemplateDetails
        );

        switch (result) {
            case 'Load':
                // load by id
              
                const { data: calendarData } = await calendarTemplateApi.calendarTemplateGet(id);
                appContext.setCurrentCalendarTemplateDetails(calendarData);
                
                break;
            case 'LoadFlowchart':
                // load "old" flowchart data as new context definition
                const newId =
                    appContext.flowchartTemplateDefinition?.Definition
                        ?.CalendarDefinition?.TemplateId;
                if (items.find((item) => item.Id === newId)) {
                    calendarTemplateApi
                        .calendarTemplateGet(newId)
                        .then(async (x) => {
                            appContext.setCurrentCalendarTemplateDetails(
                                x.data
                            );
                        });
                } else {
                    appContext.setCurrentCalendarTemplateDetails(undefined);
                }
                break;
            default:
                appContext.setCurrentCalendarTemplateDetails(undefined);
        }
        
        setUnsavedChanges((prevState) => ({ ...prevState, calendar: true }));
      
        appContext.setIsRenderClicked(!appContext.isRenderClicked);
    };

    const loadTemplates = (searchText = null, start = 0, count = 0) => { 
    appContext.setIsFlowchartSaving(true);
        calendarTemplateApi
            .calendarTemplateList(
                plainToClass(CalendarTemplateSearchDTO, {
                    OmniClientId:
                        appContext.flowchartTemplateDefinition.OmniClientId,
                    SearchText: searchText,
                    Start: start,
                    Count: count,
                    OrderAscending: true,
                    OrderBy: null,
                    Removed: false,
                })
            )
            .then((x) => {
                const sorted = x.data.Items;

                setItems([...(sorted || [])]);

                //render updated details
                renderTemplateUpdates();
                appContext.setIsFlowchartSaving(false);
            });
    }
    const loadArchivedTemplates = (searchText = null, start = 0, count = 0) => { 
    appContext.setIsFlowchartSaving(true);
        calendarTemplateApi
            .calendarTemplateList(
                plainToClass(CalendarTemplateSearchDTO, {
                    OmniClientId:
                        appContext.flowchartTemplateDefinition.OmniClientId,
                    SearchText: searchText,
                    Start: start,
                    Count: count,
                    OrderAscending: true,
                    OrderBy: null,
                    Removed: true,
                })
            )
            .then((x) => {
                const sorted = x.data.Items;

                setArchivedItems([...(sorted || [])]);
               
                //render updated details
                renderTemplateUpdates();

                appContext.setIsFlowchartSaving(false);
            }); 
    }
    const edit = (
        template: CalendarTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
        isRestricted = false
    ) => {
        event.preventDefault();
        event.stopPropagation();
        if (isRestricted) return;
        logEvent({ action: Action.EDIT });
        calendarTemplateApi
            .calendarTemplateGet(template.Id)
            .then((x) => {
                appContext.setCurrentCalendarTemplateDetails(x.data);

                navigate(templateRoutes.update);
            })
            .catch((e) => console.log(e));
    };

    const copy = (
        template: CalendarTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>
    ) => {
        setTemplateToCopy(template.Id);
        logEvent({ action: Action.DUPLICATE });
        event.preventDefault();
        event.stopPropagation();
        calendarTemplateApi
            .calendarTemplateGet(template.Id)
            .then((x) => {
                const copy = plainToClass(CalendarTemplateCreateDTO, x.data);
                copy.Name += '_Copy';
                copy.OmniClientId =
                    appContext.flowchartTemplateDefinition.OmniClientId;
                copy.IsDefault = false;
                return calendarTemplateApi.calendarTemplateCreate(copy);
            })
            .then((x) => {
                setTemplateToCopy(null);
                pushNotification(
                    `Calendar Component "${x.data.Name}" was successfully created!`,
                    NotificationType.SUCCESS
                );
                loadTemplates();
            })
            .catch(() => {
                setTemplateToCopy(null);
                pushNotification(
                    `Calendar Component could not be copied!`,
                    NotificationType.DANGER
                );
            });
    };

    const onDelete = (
        template: CalendarTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
        isRestricted = false
    ) => {
        event.preventDefault();
        event.stopPropagation();
        
        if (isRestricted) return;

        setIsDeleteModalOpen(template);
        logEvent({ action: Action.DELETE });
    };


    const deleteTemplate = (
        id: string,
        e: React.MouseEvent<HTMLButtonElement, MouseEvent>
    ) => {
        e.preventDefault();
        e.stopPropagation();
        calendarTemplateApi
            .calendarTemplateDelete(id)
            .then((x) => {
                setIsDeleteModalOpen(null);
                if (id === appContext.currentCalendarTemplateDetails?.Id) {
                    appContext.setCurrentCalendarTemplateDetails({ Definition: null });
                    setUnsavedChanges((prevState) => ({ ...prevState, calendarDelete: true }));
                }
                pushNotification(
                    `Template "${x.data.Name}" was successfully archived!`,
                    NotificationType.SUCCESS
                );
                loadTemplates();
                loadArchivedTemplates();
            })
            .catch(() =>
                pushNotification(
                    `Template could not be archived.`,
                    NotificationType.DANGER
                )
        );
        
    };
    const onRestore = (
        template: CalendarTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
    ) => {
        event.preventDefault();
        event.stopPropagation();
        calendarTemplateApi
            .calendarTemplateRestore(template.Id)
            .then((x) => {
                pushNotification(
                    `Template "${x.data.Name}" was successfully restored!`,
                    NotificationType.SUCCESS
                );
                loadTemplates();
                loadArchivedTemplates();
            })
            .catch(() =>
                pushNotification(
                    `Template could not be restored.`,
                    NotificationType.DANGER
                ))
    };


    const renderTemplateUpdates = () => {
        renderUpdates('calendarDefinition').then();
    }
    const showHideActiveTemplates = () => {
        setShowArchiveTemplate(false);
        setShowActiveTemplate(true);
    };
    const showHideArchiveTemplates = () => {
        setShowArchiveTemplate(true);
        setShowActiveTemplate(false);
    };
    const handleSearch = (searchTerm: string) => {
        const term = searchTerm?.trim() || null;
        if (term === lastSearchTerm) return; 
        setLastSearchTerm(term);
        if (!term) {
            loadTemplates(null, 0, 0);
            loadArchivedTemplates(null, 0, 0);
            return;
        }
        if (showActiveTemplate) {
            loadTemplates(term, 0, 0);
        }
        if (showArchiveTemplate) {
            loadArchivedTemplates(term, 0, 0);
        }
    };

    return (
        <Tile className="fb-list-container">
            <Toolbar slot="header">
                <div slot="start">
                    <button className={`tab calc-margin-tab ${showActiveTemplate ? "active " : ' '}`} onClick={showHideActiveTemplates}>
                        Active
                    </button>
                    <button className={`tab calc-margin-tab ${showArchiveTemplate ? " active" : ' '}`} onClick={showHideArchiveTemplates}>
                        Archived 
                    </button>
                    <RenderButton
                        definitionName="calendarDefinition"
                        definition={
                            appContext.currentCalendarTemplateDetails?.Definition
                        }
                        autoRender
                    />
                </div>
               
                <div slot="end">
                    <Search
                        onSearch={handleSearch}
                    />
                </div>
                <div slot="end" className="toolbar-divider"></div>
                <div slot="end">
                    <Button className="button secondary small" to={templateRoutes.create}>
                        New component
                    </Button>
                </div>
            </Toolbar>
            {(showActiveTemplate )&&
            <TemplateListEntries
                items={items}
                currentTemplate={appContext.currentCalendarTemplateDetails}
                currentflowchartData={
                    appContext.flowchartTemplateDefinition?.Definition
                        ?.CalendarDefinition
                }
                templateToCopy={templateToCopy}
                onClick={async (x) => await loadOrDeselectTemplateById(x.Id)}
                onEdit={edit}
                onCopy={copy}
                onDelete={onDelete}
                currentUser={appContext.user}
                isArchived = {false}
            />
            }
            {showArchiveTemplate &&
                <TemplateListEntries
                    items={archivedItems}
                    currentTemplate={appContext.currentCalendarTemplateDetails}
                    currentflowchartData={
                        appContext.flowchartTemplateDefinition?.Definition
                            ?.CalendarDefinition
                    }
                    onClick={() => { } }
                    onRestore={onRestore}
                    isArchived = {true}
                    currentUser={appContext.user}
                />
              
            }
            <div className="d-flex mt-5 is-justify-content-space-between">
                <OmniSwitchButton></OmniSwitchButton>
                <SaveFlowchartButton />
            </div>
            {isDeleteModalOpen && (
                <Dialog
                    type={DialogType.DANGER}
                    icon="interactive:archive"
                    title="Archive component?"
                    okText='Ok'
                    onOk={(e) => {
                        deleteTemplate(isDeleteModalOpen.Id, e);
                    }}
                    onCancel={(e) => {
                        e.preventDefault();
                        setIsDeleteModalOpen(null);
                    }}
                    showDialog={isDeleteModalOpen }
                >
                    Are you sure you want to archive the component?
                </Dialog>
            )}
       
        </Tile>
    );
};
