import React, {
    JSX,
    useCallback,
    useContext,
    useEffect,
    useRef,
    useState,
} from 'react';
import { produce } from 'immer';
import { GrandTotalTemplateInfoDTO } from '@omniflow/omni-webapi';
import * as API from '@omniflow/omni-webapi';

import { AppContext } from '../../../taskpane/contexts/AppContext';
import { grandTotalTemplateApi } from '../../../lib/api';
import GrandTotalsTemplate from './GrandTotalsTemplate';
import Spinner from '../../../components/spinner/Spinner';
import useNotification, {
    NotificationType,
} from '../../../components/notification/useNotification';
import useTemplateList from '../../../hooks/useTemplateList';
import useTemplateUpdateRender from '../../../hooks/useTemplateUpdateRender';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum'

export enum TGrandTotalsTemplateListOrderBy {
    'Id' = 'Id',
    'Name' = 'Name',
    'CreatedDate' = 'CreatedDate',
    'ModifiedDate' = 'ModifiedDate',
}

export type TGrandTotalsTemplateListProps = {
    search?: string | null;
    showArchiveTemplate?: boolean;
    showActiveTemplate?: boolean;
    order?: TGrandTotalsTemplateListOrderBy;
    orderAsc?: boolean;
};
const { logEvent } = useEventLogger(Module.GRANDTOTALS);
const GrandTotalsTemplateList = ({
    search,
    showArchiveTemplate,
    showActiveTemplate,
    order = TGrandTotalsTemplateListOrderBy.ModifiedDate,
    orderAsc = false,
}: TGrandTotalsTemplateListProps): JSX.Element => {
    const isSubscribed = useRef(false);
    const { deselectTemplate, getListItemBorder } = useTemplateList();
    const pushNotification = useNotification();
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [templates, setTemplates] = useState<API.GrandTotalTemplateInfoDTO[]>(
        []
    );
    const [archivedTemplates, setArchivedTemplates] = useState<API.GrandTotalTemplateInfoDTO[]>(
        []
    );
    const {
        user,
        flowchartTemplateDefinition,
        currentGrandTotalsDetails,
        setCurrentGrandTotalsDetails,
        setUnsavedChanges,
        setIsFlowchartSaving,
        isRenderClicked,
        setIsRenderClicked
    } = useContext(AppContext);
    const { OmniClientId } = flowchartTemplateDefinition;
    const { renderUpdates } = useTemplateUpdateRender();
    const loadGrandTotalsTemplates = useCallback(async () => {
        
        if (OmniClientId) {
            if (isSubscribed.current) setIsLoading(true);

            try {
                setIsFlowchartSaving(true);
                const { data } =
                    await grandTotalTemplateApi.grandTotalTemplateList({
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
                    `Grand Totals Components could not be loaded!`,
                    NotificationType.DANGER
                );
                if (isSubscribed.current) setIsLoading(false);
                console.error(e);
            }
        }
    }, [OmniClientId, search, order, orderAsc]);
    const loadGrandTotalsArchivedTemplates = useCallback(async () => {

        if (OmniClientId) {
            if (isSubscribed.current) setIsLoading(true);

            try {
                setIsFlowchartSaving(true);
                const { data } =
                    await grandTotalTemplateApi.grandTotalTemplateList({
                        OmniClientId: OmniClientId,
                        SearchText: search || null,
                        Start: 0,
                        Count: 0,
                        OrderAscending: orderAsc,
                        OrderBy: order,
                        Removed: true
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
                    `Grand Totals Archived Components could not be loaded!`,
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
        loadGrandTotalsTemplates().then();
        loadGrandTotalsArchivedTemplates().then();
        return () => {
            isSubscribed.current = false;
        };
    }, [loadGrandTotalsTemplates]);

    const loadTemplateById = async (id: string) => {
                if (!id) return;
        try {


            const result = deselectTemplate(
                id,
                'GrandTotalDefinition',
                currentGrandTotalsDetails
            );

            switch (result) {
                case 'Load':
                    // load by id
                    const { data: grandTotalsData } = await grandTotalTemplateApi.grandTotalTemplateGet(id);
                    //.then(async (x) => {
                    setCurrentGrandTotalsDetails(grandTotalsData);
                    // });
                    break;
                case 'LoadFlowchart':
                    // load "old" flowchart data as new context definition
                    const newId =
                        flowchartTemplateDefinition?.Definition
                            ?.GrandTotalDefinition?.TemplateId;
                    if (templates.find((item) => item.Id === newId)) {
                        grandTotalTemplateApi
                            .grandTotalTemplateGet(newId)
                            .then(async (x) => {
                                setCurrentGrandTotalsDetails(x.data);
                            });
                    } else {
                        setCurrentGrandTotalsDetails(undefined);
                    }
                    break;
                default:
                    setCurrentGrandTotalsDetails(undefined);
            }
            
              setUnsavedChanges((prevState) => ({ ...prevState, grandTotals: true }));
                
            } catch (e) {
                pushNotification(
                    `Grand Totals Component "${id}" could not be loaded!`,
                    NotificationType.DANGER
                );
                console.error(e);
        }
        setIsRenderClicked(!isRenderClicked);          
        }

    const editTemplate = useCallback(
        async (template: GrandTotalTemplateInfoDTO) => {
            const isRestricted = template.CreatedByUser.Id !== user.Id;
            if (isRestricted) return;
            logEvent({ action: Action.EDIT });
            grandTotalTemplateApi
                .grandTotalTemplateGet(template.Id)
                .then((x) => {
                    setCurrentGrandTotalsDetails(x.data);
                })
                .catch((e) => console.log(e));
        },
        [user]

    );

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

    const handleDeleteAfter = useCallback((id) => {
        logEvent({ action: Action.DELETE });
        setTemplates(
            produce((draft) => {
                draft.splice(
                    draft.findIndex((template) => template.Id === id),
                    1
                );
            })
        );
        if (id === currentGrandTotalsDetails?.Id) {
            setCurrentGrandTotalsDetails({ Definition: null });
        }
    }, [currentGrandTotalsDetails?.Id]);

    const onRestore = async (template: GrandTotalTemplateInfoDTO) => {
       
        try {
            await grandTotalTemplateApi.grandTotalTemplateRestore(template.Id).then((x) => {
                pushNotification(
                    `Grand Totals Component "${template.Name}" restored successfully!`,
                    NotificationType.SUCCESS
                );
            }
            );
            loadGrandTotalsTemplates().then();
            loadGrandTotalsArchivedTemplates().then();
        } catch (e) {
            pushNotification(
                `Grand Totals Component "${template.Id}" could not be restored!`,
                NotificationType.DANGER
            );
            console.error(e);
        }
    }

    const renderTemplateUpdates = () => {
        renderUpdates('grandTotalsDefinition').then();
    }

    return (
        <>
            
            <div className="item-wrapper">
                {showActiveTemplate && 
                        templates.map((template) => (
                            <GrandTotalsTemplate
                                key={template.Id}
                                template={template}
                                border={getListItemBorder(
                                    template,
                                    currentGrandTotalsDetails,
                                    flowchartTemplateDefinition?.Definition
                                        ?.GrandTotalDefinition
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
                            <GrandTotalsTemplate
                                key={template.Id}
                                template={template}
                                border={getListItemBorder(
                                    template,
                                    currentGrandTotalsDetails,
                                    flowchartTemplateDefinition?.Definition
                                        ?.GrandTotalDefinition
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

export default GrandTotalsTemplateList;
