import React, {
    JSX,
    useCallback,
    useContext,
    useEffect,
    useRef,
    useState,
} from 'react';
import * as API from '@omniflow/omni-webapi';
import { TotalsTemplateInfoDTO } from '@omniflow/omni-webapi';
import {produce} from 'immer';

import { AppContext } from '../../../taskpane/contexts/AppContext';
import { totalsTemplateApi } from '../../../lib/api';
import RightHandTotalsTemplate from './RightHandTotalsTemplate';
import Spinner from '../../../components/spinner/Spinner';
import useNotification, {
    NotificationType,
} from '../../../components/notification/useNotification';
import useTemplateList from '../../../hooks/useTemplateList';
import useTemplateUpdateRender from '../../../hooks/useTemplateUpdateRender';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum';

export enum TRightHandTotalsTemplateListOrderBy {
    'Id' = 'Id',
    'Name' = 'Name',
    'CreatedDate' = 'CreatedDate',
    'ModifiedDate' = 'ModifiedDate',
}

export type TRightHandTotalsTemplateListProps = {
    search?: string | null;
    showArchiveTemplate?: boolean;
    showActiveTemplate?: boolean;
    order?: TRightHandTotalsTemplateListOrderBy;
    orderAsc?: boolean;
};
const { logEvent } = useEventLogger(Module.RIGHTHANDTOTALS);
const RightHandTotalsTemplateList = ({
    search,
    showArchiveTemplate,
    showActiveTemplate,
    order = TRightHandTotalsTemplateListOrderBy.ModifiedDate,
    orderAsc = false,
}: TRightHandTotalsTemplateListProps): JSX.Element => {
    const isSubscribed = useRef(false);
    const pushNotification = useNotification();
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [templates, setTemplates] = useState<API.TotalsTemplateInfoDTO[]>([]);
    const [archivedTemplates, setArchivedTemplates] = useState<API.TotalsTemplateInfoDTO[]>([]);
    const {
        user,
        flowchartTemplateDefinition,
        currentTotalsDetails,
        setCurrentTotalsDetails,
        setUnsavedChanges,
        setIsFlowchartSaving,
        setIsRenderClicked,
        isRenderClicked,
    } = useContext(AppContext);
    const { OmniClientId } = flowchartTemplateDefinition;
    const { deselectTemplate, getListItemBorder } = useTemplateList();

    const { renderUpdates } = useTemplateUpdateRender();
    const loadRightHandTotalsTemplates = useCallback(async () => {
        if (OmniClientId) {
            if (isSubscribed.current) setIsLoading(true);

            try {
               setIsFlowchartSaving(true);
                const { data } = await totalsTemplateApi.totalsTemplateList({
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

                    setIsFlowchartSaving(false);
                }
                renderTemplateUpdates();
            } catch (e) {
                pushNotification(
                    `Right Hand Totals Components could not be loaded!`,
                    NotificationType.DANGER
                );
                if (isSubscribed.current) setIsLoading(false);
                setIsFlowchartSaving(false);
                console.error(e);
            }
        }
    }, [OmniClientId, search, order, orderAsc]);
    const loadRightHandTotalsArchivedTemplates = useCallback(async () => {
        if (OmniClientId) {
            if (isSubscribed.current) setIsLoading(true);
            setIsFlowchartSaving(true);
            try {
               
                const { data } = await totalsTemplateApi.totalsTemplateList({
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
                    setIsFlowchartSaving(false);
                }
                renderTemplateUpdates();
                
            } catch (e) {
                pushNotification(
                    `Right Hand Totals Archived Components could not be loaded!`,
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
        loadRightHandTotalsTemplates().then();
        loadRightHandTotalsArchivedTemplates().then();
        return () => {
            isSubscribed.current = false;
        };
    }, [loadRightHandTotalsTemplates, loadRightHandTotalsArchivedTemplates]);

    const editTemplate = useCallback(
        async (template: TotalsTemplateInfoDTO) => {
            const isRestricted = template.CreatedByUser.Id !== user.Id;
            if (isRestricted) return;
            logEvent({ action: Action.EDIT })
            totalsTemplateApi
                .totalsTemplateGet(template.Id)
                .then((x) => {
                    setCurrentTotalsDetails(x.data);
                })
                .catch((e) => console.log(e));
        },
        [user]
    );

    const loadTemplateById = async (id: string) => {
            try {
                if (!id) return;
                const result = deselectTemplate(
                    id,
                    'TotalsDefinition',
                    currentTotalsDetails
                );

                switch (result) {
                    case 'Load':
                        // load by id
                        const {data:rightHandTotalsData}=await totalsTemplateApi.totalsTemplateGet(id)
                           // .then(async (x) => {
                        setCurrentTotalsDetails(rightHandTotalsData);
                           // });
                        break;
                    case 'LoadFlowchart':
                        // load "old" flowchart data as new context definition
                        const newId =
                            flowchartTemplateDefinition?.Definition
                                ?.TotalsDefinition?.TemplateId;

                        if (templates.find((item) => item.Id === newId)) {
                            totalsTemplateApi
                                .totalsTemplateGet(newId)
                                .then(async (x) => {
                                    setCurrentTotalsDetails(x.data);
                                });
                        } else {
                            setCurrentTotalsDetails(undefined);
                        }
                        break;
                    default:
                        setCurrentTotalsDetails(undefined);
                }
            } catch (e) {
                pushNotification(
                    `Right Hand Totals Component "${id}" could not be loaded!`,
                    NotificationType.DANGER
                );
                console.error(e);
            }
        
        setUnsavedChanges((prevState) => ({ ...prevState, rightHandTotals: true }));
        
        setIsRenderClicked(!isRenderClicked);
        }

    const handleCopyAfter = useCallback(
        (template) => {
            logEvent({ action: Action.DUPLICATE })
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
        [user, order, orderAsc]
    );

    const handleDeleteAfter = useCallback(
        (id) => {
            logEvent({ action: Action.DELETE })
            setTemplates(
                produce((draft) => {
                    draft.splice(
                        draft.findIndex((template) => template.Id === id),
                        1
                    );
                })
            );
            if (id === currentTotalsDetails?.Id) {
                setCurrentTotalsDetails({ Definition: null });
            }
        },
        [currentTotalsDetails?.Id]
    );

    const onRestore = async (template: TotalsTemplateInfoDTO) => {
      
        try {
            await totalsTemplateApi.totalsTemplateRestore(template.Id).then((x) => {
                pushNotification(
                    `Right Hand Totals Component "${template.Name}" restored successfully!`,
                    NotificationType.SUCCESS
                );
            });
            loadRightHandTotalsTemplates();
            loadRightHandTotalsArchivedTemplates();
            
        } catch (e) {
            pushNotification(
                `Right Hand Totals Component "${template.Name}" could not be restored!`,
                NotificationType.DANGER
            );
            console.error(e);
        }
    }

    const renderTemplateUpdates = () => {
        renderUpdates('totalsDefinition').then();
    }

    return (
        <>
          
            <div className="item-wrapper">
                {showActiveTemplate && 
                        templates.map((template) => (
                            <RightHandTotalsTemplate
                                key={template.Id}
                                template={template}
                                border={getListItemBorder(
                                    template,
                                    currentTotalsDetails,
                                    flowchartTemplateDefinition?.Definition
                                        ?.TotalsDefinition
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
                        <RightHandTotalsTemplate
                            key={template.Id}
                            template={template}
                            border={getListItemBorder(
                                template,
                                currentTotalsDetails,
                                flowchartTemplateDefinition?.Definition
                                    ?.TotalsDefinition
                            )}
                            onClick={() => { } }
                            onRestore={onRestore}
                            isArchived={true}
                            currentUser={user}
                        />
                    ))
                    }
                    </div>
               
        </>
    );
};

export default RightHandTotalsTemplateList;
