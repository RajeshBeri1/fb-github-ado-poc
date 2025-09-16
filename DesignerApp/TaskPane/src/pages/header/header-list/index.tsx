import React, { useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router';
import {
    HeaderTemplateInfoDTO,
    HeaderTemplateSearchDTO,
    HeaderTemplateCreateDTO,
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
import { headerTemplateApi } from '../../../lib/api';
import useTemplateList from '../../../hooks/useTemplateList';
import useTemplateUpdateRender from '../../../hooks/useTemplateUpdateRender';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum'
import { template } from 'lodash';
import { OmniSwitchButton } from '../../../components/omniSwitchButton';


export const HeaderList: React.FC<{ params: any }> = ({ params }) => {
    const appContext = useContext(AppContext);
    const navigate = useNavigate();
    const pushNotification = useNotification();
    const [items, setItems] = useState([]);
    const [archivedItems, setArchivedItems] = useState([]);
    const [templateToCopy, setTemplateToCopy] = useState(null);
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(null);
    const { deselectTemplate } = useTemplateList();
    const { setUnsavedChanges } = useContext(AppContext);

    const templateRoutes = routes('Header', params);
    const { renderUpdates } = useTemplateUpdateRender();
    const { logEvent } = useEventLogger(Module.HEADER);
    useEffect(() => {
        loadTemplates();
        loadArchiveTemplates();
    }, []);

    const loadOrDeselectTemplateById = async (id: string) => {
        if (!id) return;
        const result = deselectTemplate(
            id,
            'HeaderDefinition',
            appContext.currentHeaderTemplateDetails
        );

        switch (result) {
            case 'Load':
                // load by id
                const { data: headerData } = await headerTemplateApi.headerTemplateGet(id);
                //.then(async (x) => {
                appContext.setCurrentHeaderTemplateDetails(headerData);
                //});
                break;
            case 'LoadFlowchart':
                // load "old" flowchart data as new context definition
                const newId =
                    appContext.flowchartTemplateDefinition?.Definition
                        ?.HeaderDefinition?.TemplateId;
                if (items.find((item) => item.Id === newId)) {
                    headerTemplateApi
                        .headerTemplateGet(newId)
                        .then(async (x) => {
                            appContext.setCurrentHeaderTemplateDetails(x.data);
                        });
                } else {
                    appContext.setCurrentHeaderTemplateDetails(undefined);
                }
                break;
            default:
                appContext.setCurrentHeaderTemplateDetails(undefined);
        }
      
        
        setUnsavedChanges((prevState) => ({ ...prevState, header: true }));
        
        appContext.setIsRenderClicked(!appContext.isRenderClicked);
    };

    const loadTemplates = (searchText = null, start = 0, count = 0) => { 
    appContext.setIsFlowchartSaving(true);
        headerTemplateApi
            .headerTemplateList(
                plainToClass(HeaderTemplateSearchDTO, {
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
                renderTemplateUpdates();
                appContext.setIsFlowchartSaving(false);
            });
    }
    const loadArchiveTemplates = (searchText = null, start = 0, count = 0) => { 
        appContext.setIsFlowchartSaving(true);
        headerTemplateApi
            .headerTemplateList(
                plainToClass(HeaderTemplateSearchDTO, {
                    OmniClientId:
                        appContext.flowchartTemplateDefinition.OmniClientId,
                    SearchText: searchText,
                    Start: start,
                    Count: count,
                    OrderAscending: true,
                    OrderBy: null,
                    Removed: true
                })
            )
            .then((x) => {
                const sorted = x.data.Items;

                setArchivedItems([...(sorted || [])]);
                renderTemplateUpdates();
                appContext.setIsFlowchartSaving(false);
            });
    }
    const edit = (
        template: HeaderTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
        isRestricted = false
    ) => {
        logEvent({ action: Action.EDIT });
        event.preventDefault();
        event.stopPropagation();
        if (isRestricted) return;

        headerTemplateApi
            .headerTemplateGet(template.Id)
            .then((x) => {
                appContext.setCurrentHeaderTemplateDetails(x.data);

                navigate(templateRoutes.update);
            })
            .catch((e) => console.log(e));
    };

    const copy = (
        template: HeaderTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>
    ) => {
        logEvent({ action: Action.DUPLICATE });
        setTemplateToCopy(template.Id);
        event.preventDefault();
        event.stopPropagation();
        headerTemplateApi
            .headerTemplateGet(template.Id)
            .then((x) => {
                const copy = plainToClass(HeaderTemplateCreateDTO, x.data);
                copy.Name += '_Copy';
                copy.OmniClientId =
                    appContext.flowchartTemplateDefinition.OmniClientId;
                copy.IsDefault = false;
                return headerTemplateApi.headerTemplateCreate(copy);
            })
            .then((x) => {
                setTemplateToCopy(null);
                pushNotification(
                    `Header Component "${x.data.Name}" was successfully created!`,
                    NotificationType.SUCCESS
                );
                loadTemplates();
                loadArchiveTemplates();
            })
            .catch(() => {
                setTemplateToCopy(null);
                pushNotification(
                    `Header Component could not be copied!`,
                    NotificationType.DANGER
                );
            });
    };

    const onDelete = (
        template: HeaderTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
        isRestricted = false
    ) => {
        logEvent({ action: Action.DELETE });
        event.preventDefault();
        event.stopPropagation();
        if (isRestricted) return;

        setIsDeleteModalOpen(template);
    };

    const deleteTemplate = (
        id: string,
        e: React.MouseEvent<HTMLButtonElement, MouseEvent>
    ) => {
        e.preventDefault();
        e.stopPropagation();
        headerTemplateApi
            .headerTemplateDelete(id)
            .then((x) => {
                setIsDeleteModalOpen(null);
                if (id === appContext.currentHeaderTemplateDetails?.Id) {
                    appContext.setCurrentHeaderTemplateDetails({ Definition: null });
                    setUnsavedChanges((prevState) => ({ ...prevState, headerDelete: true }));
                }
                pushNotification(
                    `Component "${x.data.Name}" was successfully archived!`,
                    NotificationType.SUCCESS
                );
                loadTemplates();
                loadArchiveTemplates();
            })
            .catch(() =>
                pushNotification(
                    `Component could not be archived.`,
                    NotificationType.DANGER
                )
        );
        
    };
    const onRestore = (
        template: HeaderTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
    ) => {
        event.preventDefault();
        event.stopPropagation();
       
        headerTemplateApi.headerTemplateRestore(template.Id).then((x) => {
            pushNotification(
                `Component "${x.data.Name}" was successfully restored!`,
                NotificationType.SUCCESS
            );
            loadTemplates();
            loadArchiveTemplates();
        }
        ).catch(() => {
            pushNotification(
                `Component could not be restored.`,
                NotificationType.DANGER
            )
        });
        }

   
    const [showArchiveTemplate, setShowArchiveTemplate] = useState<boolean>(false);
    const [showActiveTemplate, setShowActiveTemplate] = useState<boolean>(true);
    const showHideActiveTemplates = () => {
        setShowArchiveTemplate(false);
        setShowActiveTemplate(true);
    };
    const showHideArchiveTemplates = () => {
        setShowArchiveTemplate(true);
        setShowActiveTemplate(false);
    };
    const renderTemplateUpdates = () => {
        renderUpdates('headerDefinition').then();
    }
    const handleSearch = (searchTerm: string) => {
        const term = searchTerm?.trim() || null;
        if (term === lastSearchTerm) return; 
        setLastSearchTerm(term);
        if (!term) {
            loadTemplates(null, 0, 0);
            loadArchiveTemplates(null, 0, 0);
            return;
        }
        if (showActiveTemplate) {
            loadTemplates(term, 0, 0);
        }
        if (showArchiveTemplate) {
            loadArchiveTemplates(term, 0, 0);
        }
    };
    return (
        <Tile className="fb-list-container">
            <Toolbar slot="header">
                <div slot="start">
                    <button className={`tab calc-margin-tab ${showActiveTemplate ? "active" : ''}`} onClick={showHideActiveTemplates}>
                        Active
                    </button>
                    <button className={`tab calc-margin-tab ${showArchiveTemplate ? "active" : ''}`} onClick={showHideArchiveTemplates}>
                        Archived
                    </button>
                    <RenderButton
                        definitionName="headerDefinition"
                        definition={
                            appContext.currentHeaderTemplateDetails?.Definition
                        }
                        autoRender
                    />
                </div>
                
                <div slot="center-end">
                    <Search
                        onSearch={(e) => {
                            handleSearch(e);
                            logEvent({ action: Action.SEARCH });
                        }
                        }
                    />
                </div>
                <div slot="end" className="toolbar-divider"></div>
                <div slot="end">
                    <Button className="button secondary small" to={templateRoutes.create}>
                        New component
                    </Button>
                </div>
            </Toolbar>
            {showActiveTemplate &&
                <TemplateListEntries
                    items={items}
                    currentTemplate={appContext.currentHeaderTemplateDetails}
                    currentflowchartData={
                        appContext.flowchartTemplateDefinition?.Definition
                            ?.HeaderDefinition
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
                    currentTemplate={appContext.currentHeaderTemplateDetails}
                    currentflowchartData={
                        appContext.flowchartTemplateDefinition?.Definition
                            ?.HeaderDefinition
                    }
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
                        logEvent({ action: Action.CANCEL, subModule: SubModule.DELETE });
                    }}
                    showDialog={isDeleteModalOpen}
                >
                    Are you sure you want to archive the component?
                </Dialog>
            )}
        </Tile>
    );
};
