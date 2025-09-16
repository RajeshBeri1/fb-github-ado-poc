import React, { useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router';
import {
    FooterTemplateCreateDTO,
    FooterTemplateInfoDTO,
    FooterTemplateSearchDTO,
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
import { footerTemplateApi } from '../../../lib/api';
import useTemplateList from '../../../hooks/useTemplateList';
import useTemplateUpdateRender from '../../../hooks/useTemplateUpdateRender';
import '../../../pages/common-styles.css';
import { OmniSwitchButton } from '../../../components/omniSwitchButton';

export const FooterList: React.FC<{ params: any }> = ({ params }) => {
    const appContext = useContext(AppContext);
    const navigate = useNavigate();
    const pushNotification = useNotification();
    const [items, setItems] = useState([]);
    const [archivedItems, setArchivedItems] = useState([]);
    const { deselectTemplate } = useTemplateList();
    const [templateToCopy, setTemplateToCopy] = useState(null);
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(null);
    const { setUnsavedChanges } = useContext(AppContext);
    const templateRoutes = routes('Footer', params);
    const { renderUpdates } = useTemplateUpdateRender();

    const [lastSearchTerm, setLastSearchTerm] = useState<string | null>(null);
    useEffect(() => {
        loadTemplates();
      loadArchivedTemplates();
    }, []);

    const loadOrDeselectTemplateById = async (id: string) => {
        if (!id) return;
        const result = deselectTemplate(
            id,
            'FooterDefinition',
            appContext.currentFooterTemplateDetails
        );

        switch (result) {
            case 'Load':
                // load by id
                const { data: footerData } = await footerTemplateApi.footerTemplateGet(id);
               // .then(async (x) => {
                appContext.setCurrentFooterTemplateDetails(footerData);
               // });
                break;
            case 'LoadFlowchart':
                // load "old" flowchart data as new context definition
                const newId =
                    appContext.flowchartTemplateDefinition?.Definition
                        ?.FooterDefinition?.TemplateId;
                if (items.find((item) => item.Id === newId)) {
                    footerTemplateApi
                        .footerTemplateGet(newId)
                        .then(async (x) => {
                            appContext.setCurrentFooterTemplateDetails(x.data);
                        });
                } else {
                    appContext.setCurrentFooterTemplateDetails(undefined);
                }
                break;
            default:
                appContext.setCurrentFooterTemplateDetails(undefined);
        }
       
        setUnsavedChanges((prevState) => ({ ...prevState, footer: true }));
        
        appContext.setIsRenderClicked(!appContext.isRenderClicked);
    };

    const loadTemplates = (searchText = null, start = 0, count = 0) => { 
    appContext.setIsFlowchartSaving(true);
        footerTemplateApi
            .footerTemplateList(
                plainToClass(FooterTemplateSearchDTO, {
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
    const loadArchivedTemplates = (searchText = null, start = 0, count = 0) => { 
    appContext.setIsFlowchartSaving(true);
        footerTemplateApi
            .footerTemplateList(
                plainToClass(FooterTemplateSearchDTO, {
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
                renderTemplateUpdates();
                appContext.setIsFlowchartSaving(false);
            });
    }

    const edit = (
        template: FooterTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
        isRestricted = false
    ) => {
        event.preventDefault();
        event.stopPropagation();
        if (isRestricted) return;

        footerTemplateApi
            .footerTemplateGet(template.Id)
            .then((x) => {
                appContext.setCurrentFooterTemplateDetails(x.data);

                navigate(templateRoutes.update);
            })
            .catch((e) => console.log(e));
    };

    const copy = (
        template: FooterTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>
    ) => {
        setTemplateToCopy(template.Id);
        event.preventDefault();
        event.stopPropagation();

        footerTemplateApi
            .footerTemplateGet(template.Id)
            .then((x) => {
                const copy = plainToClass(FooterTemplateCreateDTO, x.data);
                copy.Name += '_Copy';
                copy.OmniClientId =
                    appContext.flowchartTemplateDefinition.OmniClientId;
                copy.IsDefault = false;
                return footerTemplateApi.footerTemplateCreate(copy);
            })
            .then((x) => {
                setTemplateToCopy(null);
                pushNotification(
                    `Footer Component "${x.data.Name}" was successfully created!`,
                    NotificationType.SUCCESS
                );
                loadTemplates();
                loadArchivedTemplates();
            })
            .catch(() => {
                setTemplateToCopy(null);
                pushNotification(
                    `Footer Component could not be copied!`,
                    NotificationType.DANGER
                );
            });
    };

    const onDelete = (
        template: FooterTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
        isRestricted = false
    ) => {
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

        footerTemplateApi
            .footerTemplateDelete(id)
            .then((x) => {
                setIsDeleteModalOpen(null);
                loadTemplates();
                loadArchivedTemplates();
                pushNotification(
                    `Component "${x.data.Name}" was successfully archived!`,
                    NotificationType.SUCCESS
                );
                if (id === appContext.currentFooterTemplateDetails?.Id) {
                    appContext.setCurrentFooterTemplateDetails({ Definition: null });
                    setUnsavedChanges((prevState) => ({ ...prevState, footerDelete: true }));
                }
            })
            .catch(() =>
                pushNotification(
                    `Component could not be archived.`,
                    NotificationType.DANGER
                )
        );
       
    };
    const onRestore = (
        template: FooterTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
        isRestricted = false
    ) => {
        event.preventDefault();
        event.stopPropagation();
        if (isRestricted) return;

        footerTemplateApi
            .footerTemplateRestore(template.Id)
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
                )
            );
    }
    const renderTemplateUpdates = () => {
        renderUpdates('footerDefinition').then();
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
                    <button className={`tab calc-margin-tab ${showActiveTemplate ? "active" : ''}`} onClick={showHideActiveTemplates}>
                        Active
                    </button>
                    <button className={`tab calc-margin-tab ${showArchiveTemplate ? "active" : ''}`} onClick={showHideArchiveTemplates}>
                        Archived
                    </button>
                    <RenderButton
                        definitionName="footerDefinition"
                        definition={
                            appContext.currentFooterTemplateDetails?.Definition
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
                    <Button className="button secondary small" to={templateRoutes.create}>
                        New component
                    </Button>
                </div>
            </Toolbar>
            {showActiveTemplate &&
                <TemplateListEntries
                    items={items}
                    currentTemplate={appContext.currentFooterTemplateDetails}
                    currentflowchartData={
                        appContext.flowchartTemplateDefinition?.Definition
                            ?.FooterDefinition
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
                    currentTemplate={appContext.currentFooterTemplateDetails}
                    currentflowchartData={
                        appContext.flowchartTemplateDefinition?.Definition
                            ?.FooterDefinition
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
