import React, {
    JSX,
    useCallback,
    useContext,
    useEffect,
    useRef,
    useState,
} from 'react';
import {produce} from 'immer';
import * as API from '@omniflow/omni-webapi';

import { AppContext } from '../../../taskpane/contexts/AppContext';
import { mediaHierarchyTemplateApi } from '../../../lib/api';
import MediaHierarchyTemplate from './MediaHierarchyTemplate';
import Spinner from '../../../components/spinner/Spinner';
import useNotification, {
    NotificationType,
} from '../../../components/notification/useNotification';
import useTemplateList from '../../../hooks/useTemplateList';
import { MediaHierarchyTemplateInfoDTO } from '@omniflow/omni-webapi';
import useTemplateUpdateRender from '../../../hooks/useTemplateUpdateRender';

export enum TMediaHierarchyTemplateListOrderBy {
    'Id' = 'Id',
    'Name' = 'Name',
    'CreatedDate' = 'CreatedDate',
    'ModifiedDate' = 'ModifiedDate',
}

export type TMediaHierarchyTemplateListProps = {
    search?: string | null;
    showActiveTemplate?: boolean;
    showArchiveTemplate?: boolean;
    order?: TMediaHierarchyTemplateListOrderBy;
    orderAsc?: boolean;
};

const MediaHierarchyTemplateList = ({
    search,
    showActiveTemplate,
    showArchiveTemplate,
    order = TMediaHierarchyTemplateListOrderBy.ModifiedDate,
    orderAsc = false,
}: TMediaHierarchyTemplateListProps): JSX.Element => {
    const isSubscribed = useRef(false);
    const { deselectTemplate, getListItemBorder } = useTemplateList();
    const pushNotification = useNotification();
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [templates, setTemplates] = useState<
        API.MediaHierarchyTemplateInfoDTO[]
        >([]);
    const [archivedTemplates, setArchivedTemplates] = useState<
        API.MediaHierarchyTemplateInfoDTO[]
    >([]);
    const {
        user,
        flowchartTemplateDefinition,
        currentMediaHierarchyDetails,
        setCurrentMediaHierarchyDetails,
        setUnsavedChanges,
        setIsFlowchartSaving,
        setIsRenderClicked,
        isRenderClicked,
    } = useContext(AppContext);
    const { OmniClientId } = flowchartTemplateDefinition;

    const { renderUpdates } = useTemplateUpdateRender();

    const loadMediaHierarchyTemplates = useCallback(async () => {
        if (OmniClientId) {
            if (isSubscribed.current) setIsLoading(true);
            setIsFlowchartSaving(true);
            try {
                const { data } =
                    await mediaHierarchyTemplateApi.mediaHierarchyTemplateList({
                        OmniClientId: OmniClientId,
                        SearchText: search || null,
                        Start: 0,
                        Count: 0,
                        OrderAscending: orderAsc,
                        OrderBy: order,
                        Removed: false,
                    });

                if (data && isSubscribed.current) {
                    const { Items } = data;
                    setTemplates(Items || []);
                    setIsLoading(false);
                }
                renderTemplateUpdates();
                setIsFlowchartSaving(false);

            } catch (e) {
                pushNotification(
                    `Media Hierarchy Components could not be loaded!`,
                    NotificationType.DANGER
                );
                if (isSubscribed.current) setIsLoading(false);
                setIsFlowchartSaving(false);
                console.error(e);
            }
        }
    }, [OmniClientId, search, order, orderAsc]);
    const loadMediaHierarchyArchivedTemplates = useCallback(async () => {
        if (OmniClientId) {
            if (isSubscribed.current) setIsLoading(true);
            setIsFlowchartSaving(true);
            try {
                const { data } =
                    await mediaHierarchyTemplateApi.mediaHierarchyTemplateList({
                        OmniClientId: OmniClientId,
                        SearchText: search || null,
                        Start: 0,
                        Count: 0,
                        OrderAscending: orderAsc,
                        OrderBy: order,
                        Removed: true,
                    });

                if (data && isSubscribed.current) {
                    const { Items } = data;
                    setArchivedTemplates(Items || []);
                    setIsLoading(false);
                }
                renderTemplateUpdates();
                setIsFlowchartSaving(false);

            } catch (e) {
                pushNotification(
                    `Media Hierarchy Archived Components could not be loaded!`,
                    NotificationType.DANGER
                );
                if (isSubscribed.current) setIsLoading(false);
                setIsFlowchartSaving(false);
                console.error(e);
            }
        }
    }, [OmniClientId, search, order, orderAsc]);
    useEffect(() => {
        isSubscribed.current = true;

        loadMediaHierarchyTemplates().then();
        loadMediaHierarchyArchivedTemplates().then();
        return () => {
            isSubscribed.current = false;
        };
    }, [loadMediaHierarchyTemplates]);

    const loadTemplateById = async (id: string) => {
        try {
            if (!id) return;
            const result = deselectTemplate(
                id,
                'MediaHierarchyDefinition',
                currentMediaHierarchyDetails
            );

            switch (result) {
                case 'Load':
                    // load by id
                    const { data: mediaHierarchyData } = await mediaHierarchyTemplateApi.mediaHierarchyTemplateGet(id);
                       //.then(async (x) => {
                    setCurrentMediaHierarchyDetails(mediaHierarchyData);
                       // });
                    break;
                case 'LoadFlowchart':
                    // load "old" flowchart data as new context definition
                    const newId =
                        flowchartTemplateDefinition?.Definition
                            ?.MediaHierarchyDefinition?.TemplateId;
                    if (templates.find((item) => item.Id === newId)) {
                        mediaHierarchyTemplateApi
                            .mediaHierarchyTemplateGet(newId)
                            .then(async (x) => {
                                setCurrentMediaHierarchyDetails(x.data);
                            });
                    } else {
                        setCurrentMediaHierarchyDetails(undefined);
                    }
                    break;
                default:
                    setCurrentMediaHierarchyDetails(undefined);
            }
        } catch (e) {
            pushNotification(
                `Media Hierarchy Component "${id}" could not be loaded!`,
                NotificationType.DANGER
            );
            console.error(e);
        }
       
        setUnsavedChanges((prevState) => ({ ...prevState, mediaHierarchy: true }));
        
        setIsRenderClicked(!isRenderClicked);
    };

    const editTemplate = useCallback(
        async (template: MediaHierarchyTemplateInfoDTO) => {
            const isRestricted = template.CreatedByUser.Id !== user.Id;
            if (isRestricted) return;

            mediaHierarchyTemplateApi
                .mediaHierarchyTemplateGet(template.Id)
                .then((x) => {
                    setCurrentMediaHierarchyDetails(x.data);
                })
                .catch((e) => console.log(e));
        },
        [user]
    );

    const handleCopyAfter = useCallback(
        (template) => {
            setTemplates(
                produce((draft) => {
                    draft.push(template);
                    draft.sort(
                        (a, b) =>
                            (a[order] < b[order] ? 1 : -1) * (orderAsc ? -1 : 1)
                    );
                })
            );
        },
        [order, orderAsc]
    );

    const handleDeleteAfter = useCallback(
        (id) => {
            setTemplates(
                produce((draft) => {
                    draft.splice(
                        draft.findIndex((template) => template.Id === id),
                        1
                    );
                })
            );
            if (id === currentMediaHierarchyDetails?.Id) {
                setCurrentMediaHierarchyDetails({ Definition: null });
            }
        },
        [currentMediaHierarchyDetails?.Id]
    );
    const onRestore = async (template: MediaHierarchyTemplateInfoDTO) => {
        try {
            await mediaHierarchyTemplateApi.mediaHierarchyTemplateRestore(template.Id).then((x) => {
                pushNotification(
                    `Media Hierarchy Component "${template.Name}" restored successfully!`,
                    NotificationType.SUCCESS
                );
            }
            );
            loadMediaHierarchyTemplates().then();
            loadMediaHierarchyArchivedTemplates().then();
        } catch (e) {
            pushNotification(
                `Media Hierarchy Component "${template.Name}" could not be restored!`,
                NotificationType.DANGER
            );
            console.error(e);
        }
    };

    const renderTemplateUpdates = () => {
        renderUpdates('mediaHierarchyDefinition').then();
    }

    return (
        <>
           
            <div className="item-wrapper">
                {showActiveTemplate &&
                
                    templates.map((template) => (
                        <MediaHierarchyTemplate
                            key={template.Id}
                            template={template}
                            border={getListItemBorder(
                                template,
                                currentMediaHierarchyDetails,
                                flowchartTemplateDefinition?.Definition
                                    ?.MediaHierarchyDefinition
                            )}
                            onClick={loadTemplateById}
                            onEdit={editTemplate}
                            onCopyAfter={handleCopyAfter}
                            onDeleteAfter={handleDeleteAfter}
                            currentUser={user}
                        />
                    ))
                
                }
                {showArchiveTemplate &&
                
                    archivedTemplates.map((template) => (
                        <MediaHierarchyTemplate
                            key={template.Id}
                            template={template}
                            border={getListItemBorder(
                                template,
                                currentMediaHierarchyDetails,
                                flowchartTemplateDefinition?.Definition
                                    ?.MediaHierarchyDefinition
                            )}
                            onClick={() => { } }
                            isArchived = {true}
                            currentUser={user}
                            onRestore={onRestore}
                        />
                    ))
                
                 }
            </div>
            
              
        </>
    );
};

export default MediaHierarchyTemplateList;
