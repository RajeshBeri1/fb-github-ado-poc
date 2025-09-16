import React, { useContext, useEffect, useState } from 'react';
import { useNavigate } from 'react-router';
import * as API from '@omniflow/omni-webapi';
import {
    ThemeTemplateApi,
    ThemeTemplateCreateDTO,
    ThemeTemplateInfoDTO,
    ThemeTemplateSearchDTO,
} from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';

import { Tile } from '../../../../omni/tile';
import Button from '../../../../components/buttons/Button';
import { TemplateListEntries } from '../../../../components/template-list/template-list-entries';
import useNotification, {
    NotificationType,
} from '../../../../components/notification/useNotification';
import { AppContext } from '../../../../taskpane/contexts/AppContext';
import { Toolbar } from '../../../../omni/toolbar';
import Search from '../../../../components/Search';
import { routes } from '../../../routes';
import { Page } from '../../../../enums/page.enum';
import { WEBAPI_CONFIGURATION } from '../../../../config/webapi.config';
import RequestInterceptor from '../../../../business/request-interceptor';
import Dialog, { DialogType } from '../../../../components/dialogs/Dialog';
import RenderButton from '../../../../components/buttons/RenderButton';
import { SaveFlowchartButton } from '../../../../components/buttons/SaveFlowchartButton';
import useTemplateList from '../../../../hooks/useTemplateList';

import './theme-template-list.css';
import useTemplateUpdateRender from '../../../../hooks/useTemplateUpdateRender';
import useEventLogger from '../../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../../enums/event.enum';
import '../../../../pages/common-styles.css';
import { OmniSwitchButton } from '../../../../components/omniSwitchButton';
export interface IThemeTemplateListProps {
    onClick?: (x: API.ThemeTemplateInfoDTO) => void;
    onDelete?: (
        x: API.ThemeTemplateInfoDTO,
        event?: React.MouseEvent<HTMLButtonElement>
    ) => void;
    // onEdit?: (
    //     x: API.ThemeTemplateInfoDTO,
    //     event?: React.MouseEvent<HTMLButtonElement>
    // ) => void;
    onCopy?: (
        x: API.ThemeTemplateInfoDTO,
        event?: React.MouseEvent<HTMLButtonElement>
    ) => void;
    items?: API.ThemeTemplateInfoDTO[];
    themeToCopy?: string;
    currentThemeTemplate: API.ThemeTemplateInfoDTO;
}

export const ThemeTemplateList: React.FC<IThemeTemplateListProps | any> = ({
    params,
}) => {
    const appContext = useContext(AppContext);
    const { setUnsavedChanges } = useContext(AppContext);
    const navigate = useNavigate();
    const pushNotification = useNotification();
    const [items, setItems] = useState([]);
    const [archivedItems, setArchivedItems] = useState([]);
    const [templateToCopy, setTemplateToCopy] = useState(null);
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(null);
    const templateRoutes = routes(Page.Theme, params);
    const themeTemplateApi = new ThemeTemplateApi(
        WEBAPI_CONFIGURATION,
        null,
        RequestInterceptor
    );
    const { deselectTemplate } = useTemplateList();
    const { renderUpdates } = useTemplateUpdateRender();
    const { logEvent } = useEventLogger(Module.GRANDTOTALS);

    const [lastSearchTerm, setLastSearchTerm] = useState<string | null>(null);
    const edit = (
        template: ThemeTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
        isRestricted = false
    ) => {
        logEvent({ action: Action.EDIT });
        event.preventDefault();
        event.stopPropagation();
        if (isRestricted) return;

        themeTemplateApi
            .themeTemplateGet(template.Id)
            .then((x) => {
                appContext.setCurrentThemeTemplateDetails(x.data);
/*                appContext.setIsKeepManualFormatting(x.data?.Definition?.Styling?.CustomStyleSetting?.CustomStyleSettingValue)
*/                navigate(templateRoutes.update);
            })
            .catch((e) => console.log(e));
    };
    const copy = (
        template: ThemeTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>
    ) => {
        logEvent({ action: Action.DUPLICATE });
        setTemplateToCopy(template.Id);
        event.preventDefault();
        event.stopPropagation();
        themeTemplateApi
            .themeTemplateGet(template.Id)
            .then((x) => {
                const copy = plainToClass(ThemeTemplateCreateDTO, x.data);
                copy.Name += '_Copy';
                copy.OmniClientId =
                    appContext.flowchartTemplateDefinition.OmniClientId;
                copy.IsDefault = false;
                return themeTemplateApi.themeTemplateCreate(copy);
            })
            .then((x) => {
                setTemplateToCopy(null);
                pushNotification(
                    `Theme Component "${x.data.Name}" was successfully created!`,
                    NotificationType.SUCCESS
                );
                loadTemplates();
                loadArchivedTemplates();
            })
            .catch(() => {
                setTemplateToCopy(null);
                pushNotification(
                    `Theme Component could not be copied!`,
                    NotificationType.DANGER
                );
            });
    };
    const onDelete = (
        template: ThemeTemplateInfoDTO,
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
        themeTemplateApi
            .themeTemplateDelete(id)
            .then((x) => {
                setIsDeleteModalOpen(null);
                loadTemplates();
                loadArchivedTemplates();
                pushNotification(
                    `Component "${x.data.Name}" was successfully archived!`,
                    NotificationType.SUCCESS
                );
                if (id === appContext.currentThemeTemplateDetails?.Id) {
                    appContext.setCurrentThemeTemplateDetails({ Definition: null });
                    setUnsavedChanges((prevState) => ({ ...prevState, themeDelete: true }));
                }
            })
            .catch(() =>
                pushNotification(
                    `Component could not be archived.`,
                    NotificationType.DANGER
                )
        );
    };

    useEffect(() => {
        loadTemplates();
        loadArchivedTemplates();
    }, [appContext.flowchartTemplateDefinition.OmniClientId]);

    const loadTemplates = (searchText = null, start = 0, count = 0) => {
        appContext.setIsFlowchartSaving(true);
        return themeTemplateApi
            .themeTemplateList(
                plainToClass(ThemeTemplateSearchDTO, {
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
                const sorted = x.data.Items.sort(
                    (a, b) =>
                        new Date(b.ModifiedDate).getTime() -
                        new Date(a.ModifiedDate).getTime()
                );

                setItems([...(sorted || [])]);
                renderTemplateUpdates();
                appContext.setIsFlowchartSaving(false);
            });
    };
    const loadArchivedTemplates = (searchText = null, start = 0, count = 0) => {
        appContext.setIsFlowchartSaving(true);
        return themeTemplateApi
            .themeTemplateList(
                plainToClass(ThemeTemplateSearchDTO, {
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
                const sorted = x.data.Items.sort(
                    (a, b) =>
                        new Date(b.ModifiedDate).getTime() -
                        new Date(a.ModifiedDate).getTime()
                );

                setArchivedItems([...(sorted || [])]);
                renderTemplateUpdates();
                appContext.setIsFlowchartSaving(false);
            });
    };

    const loadTemplateById = async (id: string) => {
        if (!id) return;
        const result = deselectTemplate(
            id,
            'ThemeDefinition',
            appContext.currentThemeTemplateDetails
        );

        switch (result) {
            case 'Load':
                // load by id
                const { data: themeData } = await themeTemplateApi.themeTemplateGet(id);
                //.then(async (x) => {
                appContext.setCurrentThemeTemplateDetails(themeData); 
/*                appContext.setIsKeepManualFormatting(themeData?.Definition?.Styling?.CustomStyleSetting?.CustomStyleSettingValue)
*/               // });
                break;
            case 'LoadFlowchart':
                // load "old" flowchart data as new context definition
                const newId =
                    appContext.flowchartTemplateDefinition?.Definition
                        ?.ThemeDefinition?.TemplateId;
                if (items.find((item) => item.Id === newId)) {
                    themeTemplateApi.themeTemplateGet(newId).then(async (x) => {
                        appContext.setCurrentThemeTemplateDetails(x.data);
                    });
                } else {
                    appContext.setCurrentThemeTemplateDetails(undefined);
                }
                break;
            default:
                appContext.setCurrentThemeTemplateDetails(undefined);
        }
        
        setUnsavedChanges((prevState) => ({ ...prevState, theme: true }));
        
        appContext.setIsRenderClicked(!appContext.isRenderClicked);
    };
    const onRestore = (
        template: ThemeTemplateInfoDTO,
        event: React.MouseEvent<HTMLButtonElement, MouseEvent>,
        isRestricted = false
    ) => {
        event.preventDefault();
        event.stopPropagation();
        if (isRestricted) return;

        themeTemplateApi
            .themeTemplateRestore(template.Id)
            .then((x) => {
                loadTemplates();
                loadArchivedTemplates();
                pushNotification(
                    `Component "${x.data.Name}" was successfully restored!`,
                    NotificationType.SUCCESS
                );
            })
            .catch(() =>
                pushNotification(
                    `Component could not be restored.`,
                    NotificationType.DANGER
                )
            );
    }
    const renderTemplateUpdates = () => {
        renderUpdates('themeDefinition').then();
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
                        definitionName="themeDefinition"
                        definition={
                            appContext.currentThemeTemplateDetails?.Definition
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
                    currentTemplate={appContext.currentThemeTemplateDetails}
                    currentflowchartData={
                        appContext.flowchartTemplateDefinition?.Definition
                            ?.ThemeDefinition
                    }
                    templateToCopy={templateToCopy}
                    onClick={async (x) => await loadTemplateById(x.Id)}
                    onEdit={edit}
                    onCopy={copy}
                    onDelete={onDelete}
                    currentUser={appContext.user}
                />
            }
            {showArchiveTemplate &&
                <TemplateListEntries
                items={archivedItems}
                    currentTemplate={appContext.currentThemeTemplateDetails}
                    currentflowchartData={
                        appContext.flowchartTemplateDefinition?.Definition
                            ?.ThemeDefinition
                    }
                    templateToCopy={templateToCopy}
                    onClick={() => {}}
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
