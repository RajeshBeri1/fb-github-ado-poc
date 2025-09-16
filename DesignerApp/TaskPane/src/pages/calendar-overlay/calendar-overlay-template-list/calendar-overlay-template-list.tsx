import React, { useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router';
import * as API from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';
import {
    CalendarOverlayTemplateApi,
    CalendarOverlayTemplateCreateDTO,
    CalendarOverlayTemplateInfoDTO,
    CalendarOverlayTemplateSearchDTO,
} from '@omniflow/omni-webapi';

import { Tile } from '../../../omni/tile';
import Button from '../../../components/buttons/Button';
import { TemplateListEntries } from '../../../components/template-list/template-list-entries';
import useNotification, {
    NotificationType,
} from '../../../components/notification/useNotification';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import { Toolbar } from '../../../omni/toolbar';
import Search from '../../../components/Search';
import { WEBAPI_CONFIGURATION } from '../../../config/webapi.config';
import RequestInterceptor from '../../../business/request-interceptor';
import Dialog, { DialogType } from '../../../components/dialogs/Dialog';
import RenderButton from '../../../components/buttons/RenderButton';
import { SaveFlowchartButton } from '../../../components/buttons/SaveFlowchartButton';
import { routes } from '../../routes';
import useTemplateList from '../../../hooks/useTemplateList';

import './calendar-overlay-template-list.css';
import useTemplateUpdateRender from '../../../hooks/useTemplateUpdateRender';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum'
import { OmniSwitchButton } from '../../../components/omniSwitchButton';
export interface ICalendarOverlayTemplateListProps {
    onClick?: (x: API.CalendarOverlayTemplateInfoDTO) => void;
    onDelete?: (
        x: API.CalendarOverlayTemplateInfoDTO,
        event?: React.MouseEvent<HTMLButtonElement>
    ) => void;
    onEdit?: (
        x: API.CalendarOverlayTemplateInfoDTO,
        event?: React.MouseEvent<HTMLButtonElement>
    ) => void;
    onCopy?: (
        x: API.CalendarOverlayTemplateInfoDTO,
        event?: React.MouseEvent<HTMLButtonElement>
    ) => void;
    items?: API.CalendarOverlayTemplateInfoDTO[];
    CalendarOverlayToCopy?: string;
    currentCalendarOverlayTemplate: API.CalendarOverlayTemplateInfoDTO;
}

export const CalendarOverlayTemplateList: React.FC<
    ICalendarOverlayTemplateListProps | any
    > = ({ params }) => {
    const { logEvent } = useEventLogger(Module.CALENDAROVERLAY);
    const appContext = useContext(AppContext);
        const { setUnsavedChanges } = useContext(AppContext);
        const navigate = useNavigate();
    const { deselectTemplate } = useTemplateList();
    const pushNotification = useNotification();
    const [items, setItems] = useState([]);
    const [archivedItems, setArchivedItems] = useState([]);
    const [templateToCopy, setTemplateToCopy] = useState(null);
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(null);
    const calendarOverlayTemplateApi = new CalendarOverlayTemplateApi(
        WEBAPI_CONFIGURATION,
        null,
        RequestInterceptor
    );
    const templateroutes = routes('CalendarOverlay', params);
    const { renderUpdates } = useTemplateUpdateRender();
    const [showArchiveTemplate, setShowArchiveTemplate] = useState<boolean>(false);
    const [showActiveTemplate, setShowActiveTemplate] = useState<boolean>(true);
    const [lastSearchTerm, setLastSearchTerm] = useState<string | null>(null);
    const edit = (
        template: CalendarOverlayTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
        isRestricted = false
    ) => {
        event.preventDefault();
        event.stopPropagation();
        if (isRestricted) return;
        logEvent({ action: Action.EDIT });
        calendarOverlayTemplateApi
            .calendarOverlayTemplateGet(template.Id)
            .then((x) => {
                appContext.setCurrentCalendarOverlayTemplateDetails(x.data);

                navigate(templateroutes.update);
            })
            .catch((e) => console.log(e));
    };
    const copy = (
        template: CalendarOverlayTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>
    ) => {
        setTemplateToCopy(template.Id);
        logEvent({ action: Action.DUPLICATE });
        event.preventDefault();
        event.stopPropagation();
        calendarOverlayTemplateApi
            .calendarOverlayTemplateGet(template.Id)
            .then((x) => {
                const copy = plainToClass(
                    CalendarOverlayTemplateCreateDTO,
                    x.data
                );
                copy.Name += '_Copy';
                copy.OmniClientId =
                    appContext.flowchartTemplateDefinition.OmniClientId;
                copy.IsDefault = false;
                return calendarOverlayTemplateApi.calendarOverlayTemplateCreate(
                    copy
                );
            })
            .then((x) => {
                setTemplateToCopy(null);
                pushNotification(
                    `CalendarOverlay Component "${x.data.Name}" was successfully created!`,
                    NotificationType.SUCCESS
                );
                loadTemplates();
                loadArchivedTemplates();
            })
            .catch(() => {
                setTemplateToCopy(null);
                pushNotification(
                    `CalendarOverlay Component could not be copied!`,
                    NotificationType.DANGER
                );
            });
    };
    const onDelete = (
        template: CalendarOverlayTemplateInfoDTO,
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
        calendarOverlayTemplateApi
            .calendarOverlayTemplateDelete(id)
            .then((x) => {
                setIsDeleteModalOpen(null);
               
                if (id===appContext.currentCalendarOverlayTemplateDetails?.Id) {
                    appContext.setCurrentCalendarOverlayTemplateDetails({ Definition: null });
                    setUnsavedChanges((prevState) => ({ ...prevState, calendarOverlayDelete: true }));
                }
                pushNotification(
                    `Component "${x.data.Name}" was successfully archived!`,
                    NotificationType.SUCCESS
                );
                loadTemplates();
                loadArchivedTemplates();
            })
            .catch(() =>
                pushNotification(
                    `Component could not be archived.`,
                    NotificationType.DANGER
                )
        );

    };

   

        const loadTemplates = (searchText = null, start = 0, count = 0) => {
            appContext.setIsFlowchartSaving(true);
        return calendarOverlayTemplateApi
            .calendarOverlayTemplateList(
                plainToClass(CalendarOverlayTemplateSearchDTO, {
                    OmniClientId:
                        appContext.flowchartTemplateDefinition.OmniClientId,
                    SearchText: searchText,
                    Start: start,
                    Count: count,
                    OrderAscending: true,
                    OrderBy: null,
                    Removed: false
                })
            )
            .then((x) => {
                const sorted = x.data.Items;

                setItems([...(sorted || [])]);
                renderTemplateUpdates();
                appContext.setIsFlowchartSaving(false);
            });
        };
        const loadArchivedTemplates = (searchText = null, start = 0, count = 0) => {
            appContext.setIsFlowchartSaving(true);
            return calendarOverlayTemplateApi
                .calendarOverlayTemplateList(
                    plainToClass(CalendarOverlayTemplateSearchDTO, {
                        OmniClientId:
                            appContext.flowchartTemplateDefinition.OmniClientId,
                        SearchText: searchText,
                        Start: start,
                        Count: count,
                        OrderAscending: true,
                        OrderBy: null,
                        Removed:true
                    })
                )
                .then((x) => {
                    const sorted = x.data.Items;

                    setArchivedItems([...(sorted || [])]);
                    renderTemplateUpdates();
                    appContext.setIsFlowchartSaving(false);
                });
        };

    useEffect(() => {
        loadTemplates();
        loadArchivedTemplates();
    }, []);

        const loadOrDeselectTemplateById = async (id: string) => {
        if (!id) return;
        const result = deselectTemplate(
            id,
            'CalendarOverlayDefinition',
            appContext.currentCalendarOverlayTemplateDetails
        );

            switch (result) {
                case 'Load':
                    // load by id
                    const { data: calendarOverlayData } = await calendarOverlayTemplateApi.calendarOverlayTemplateGet(id);
                    //.then(async (x) => {
                        appContext.setCurrentCalendarOverlayTemplateDetails( calendarOverlayData);
                   // });
                break;
            case 'LoadFlowchart':
                // load "old" flowchart data as new context definition
                const newId =
                    appContext.flowchartTemplateDefinition?.Definition
                        ?.CalendarOverlayDefinition?.TemplateId;
                if (items.find((item) => item.Id === newId)) {
                    calendarOverlayTemplateApi
                        .calendarOverlayTemplateGet(newId)
                        .then(async (x) => {
                            appContext.setCurrentCalendarOverlayTemplateDetails(
                                x.data
                            );
                        });
                } else {
                    appContext.setCurrentCalendarOverlayTemplateDetails(
                        undefined
                    );
                }
                break;
            default:
                appContext.setCurrentCalendarOverlayTemplateDetails(undefined);
            }
            
               
            
            setUnsavedChanges((prevState) => ({ ...prevState, calendarOverlay: true }));
            
            appContext.setIsRenderClicked(!appContext.isRenderClicked);
        };
        const onRestore = (template: CalendarOverlayTemplateInfoDTO,
            event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
            ) => {
            event.preventDefault();
            event.stopPropagation();
            calendarOverlayTemplateApi.calendarOverlayTemplateRestore(template.Id)
                .then((x) => {
                    pushNotification(
                        `Component "${x.data.Name}" was successfully restored!`,
                        NotificationType.SUCCESS
                    );
                    loadTemplates();
                    loadArchivedTemplates();
                })
                .catch(() =>
                    pushNotification(
                        `Component could not be restored.`,
                        NotificationType.DANGER
                    ))
        }

    const renderTemplateUpdates = () => {
        renderUpdates('calendarOverlayDefinition').then();
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
                    <button className={`tab calc-margin-tab  ${showActiveTemplate ? " active" : ''}`} onClick={showHideActiveTemplates}>
                        Active
                    </button>
                    <button className={`tab calc-margin-tab  ${showArchiveTemplate ? " active" : ''}`} onClick={showHideArchiveTemplates}>
                        Archived
                    </button>
                    <RenderButton
                        definitionName="calendarOverlayDefinition"
                        definition={
                            appContext.currentCalendarOverlayTemplateDetails
                                ?.Definition
                        }
                        autoRender
                    />
                </div>
                
                <div slot="center-end">
                    <Search
                        onSearch={handleSearch}
                    />
                </div>
                <div slot="end" className="toolbar-divider"></div>
                <div slot="end">
                    <Button className="button secondary small" to={templateroutes.create}>
                        New component
                    </Button>
                </div>
            </Toolbar>
            {showActiveTemplate &&
                <TemplateListEntries
                    items={items}
                    currentTemplate={
                        appContext.currentCalendarOverlayTemplateDetails
                    }
                    currentflowchartData={
                        appContext.flowchartTemplateDefinition?.Definition
                            ?.CalendarOverlayDefinition
                    }
                    templateToCopy={templateToCopy}
                    onClick={async (x) => await loadOrDeselectTemplateById(x.Id)}
                    onEdit={edit}
                    onCopy={copy}
                    onDelete={onDelete}
                    currentUser={appContext.user}
                />
            }
            {showArchiveTemplate &&
                <TemplateListEntries
                items={archivedItems}
                    currentTemplate={
                        appContext.currentCalendarOverlayTemplateDetails
                    }
                    currentflowchartData={
                        appContext.flowchartTemplateDefinition?.Definition
                            ?.CalendarOverlayDefinition
                    }
                    templateToCopy={templateToCopy}
                    onClick={() => { }}
                     onRestore={onRestore}
                    isArchived={true}
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
                    showDialog={isDeleteModalOpen}
                >
                    Are you sure you want to archive the component?
                </Dialog>
            )}
        </Tile>
    );
};
