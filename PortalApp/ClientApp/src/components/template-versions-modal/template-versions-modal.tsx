import React, { useImperativeHandle, forwardRef, JSX , useRef} from 'react';
import { useEffect, useState } from 'react';
import {
    RunConfigurationApi,
    RunConfigurationSearchDTO,
    RunConfigurationInfoListDTO,
    RunConfigurationInfoDTO,
    RunConfigurationDetailsDTO,
    ReportApi,
    FlowchartTemplatesHistortyApi,
    FlowchartTemplatesVersionHistortiesDTO,
    FlowchartTemplatesVersionHistortySearchDTO,
} from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';
import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';
import RequestInterceptor from '../../business/request-interceptor';
import useOpenDocument from '../../hooks/useOpenDocument';
import { ListTypes } from '../../enums/list-types.enum';
import Skeleton from '../custom/skeleton';
import { html } from 'lit';
import { Table } from '../../omni-ui-components/table';
import useEventLogger from '../../hooks/eventLogger';
import { Action, Module, SubModule } from '../../enums/event.enum';
import { Modal } from '../../omni-ui-components/modal'
import './template-versions-modal.scss';
interface CreateVerionTemplateModalFormProps {
    clientId: string;
    modalState: boolean;
    modalHandler: (a:any) => void;
    selectedTemplateVersion: string | null;
    type?: ListTypes;
    handleToggleReportDropdown?: (id: string) => void;
    activeReportDropdown?: string | null;
    setActiveReportDropdown?: React.Dispatch<React.SetStateAction<string | null>>;
    navigateToRunConfig?: (id: string, name: string) => void;
}
//export type childRef = {
//    close(): unknown;
//    showModal: () => void;
//}
const TemplateVersionsModal = forwardRef(({
    modalHandler,
    modalState,
    selectedTemplateVersion,
    type,
    handleToggleReportDropdown,
    activeReportDropdown,
    setActiveReportDropdown,
    navigateToRunConfig,
}: CreateVerionTemplateModalFormProps,ref): JSX.Element => {
    const runConfigApi = new RunConfigurationApi(
        WEBAPI_CONFIGURATION,
        '',
        RequestInterceptor
    );
    const reportApi = new ReportApi(
        WEBAPI_CONFIGURATION,
        '',
        RequestInterceptor
    );

    const flowchartTemplatesHistortyApis = new FlowchartTemplatesHistortyApi(
        WEBAPI_CONFIGURATION,
        '',
        RequestInterceptor
    );

    const [versions, setVersions] = useState<RunConfigurationInfoListDTO>({
        Items: [],
        TotalCount: 0,
    });

    const [selectedVersion] = useState<string | null>(null);
    const [, setSelectedRunConfig] = useState<RunConfigurationDetailsDTO>();
    const {
        openDocumentbyFlowchartTemplateVersionHistoryId,
        downloadDocument,
        runDocument,
    } = useOpenDocument();
    const [loading, SetLoading] = useState<boolean>(false);
    const { logEvent } = useEventLogger(Module.FLOWCHART);
    const modalChildRef = useRef<any>(null);

    useEffect(() => {
        if (selectedTemplateVersion && type) {
            if (type == ListTypes.Edit) {
                logEvent(Action.VIEWEDITHISTORYlIST, SubModule.HISTORY);
                loadAllFlowchartTemplatesHistortyVersions();
            }
            if (type == ListTypes.Launch) {
                logEvent(Action.VIEWLAUNCHHISTORYLIST, SubModule.HISTORY);
                loadAllRunConfigVersions();
            }
            if (type == ListTypes.Download) {
                logEvent(Action.VIEWDOWNLOADHISTORYLIST, SubModule.HISTORY);
                loadAllReportVersions();
            }
        }
    }, [selectedTemplateVersion, type]);

    useEffect(() => {
        if (selectedVersion !== null) {
            loadSelectedRunConfiguration();
        }
    }, [selectedVersion]);

    // Effect hook dropdown
    useEffect(() => {
        // Close dropdown when clicking outside
        const handleClickOutside = (event: MouseEvent) => {
            if (activeReportDropdown && setActiveReportDropdown) {
                const target = event.target as HTMLElement;
                if (!target.closest('.dropdown-menu') && !target.closest('omni-icon[icon-id="omni:informative:reports"]')) {
                    setActiveReportDropdown(null);
                }
            }
        };

        document.addEventListener('click', handleClickOutside);
        return () => {
            document.removeEventListener('click', handleClickOutside);
        };
    }, [activeReportDropdown, setActiveReportDropdown]);

    const closeForm = (): void => {
        logEvent(Action.CANCEL);
        if (modalChildRef.current) {
            modalChildRef.current.close();
        }
       
    };

    useImperativeHandle(ref, () => ({
        showModal: () => {
            if (modalChildRef.current) {
                modalChildRef.current.showModal();
            }
           
        },
        
    }));

    const loadAllReportVersions = () => {
        SetLoading(true);
        reportApi
            .reportList(
                plainToClass(RunConfigurationSearchDTO, {
                    FlowchartTemplateId: selectedTemplateVersion,
                    Start: 0,
                    Count: 0,
                    OrderAscending: true,
                })
            )
            .then((x) => {
                if (x.data.Items && x.data.TotalCount) {
                    const Items: RunConfigurationInfoDTO[] = x.data.Items;
                    const TotalCount: number | null = x.data.TotalCount;
                    setVersions({
                        Items,
                        TotalCount,
                    });
                }
                else {
                    setVersions({
                        Items: [],
                        TotalCount: 0,
                    });
                }

            })
            .finally(() => {
                SetLoading(false);
            });
    };

    const loadAllFlowchartTemplatesHistortyVersions = () => {
        SetLoading(true);
        flowchartTemplatesHistortyApis
            .flowchartTemplatesHistortyList(
                plainToClass(FlowchartTemplatesVersionHistortySearchDTO, {
                    FlowchartTemplateId: selectedTemplateVersion,
                    Start: 0,
                    Count: 0,
                    OrderAscending: true,
                })
            )
            .then((x) => {
                if (x.data.Items && x.data.TotalCount) {
                    const Items: FlowchartTemplatesVersionHistortiesDTO[] =
                        x.data.Items;
                    const TotalCount: number | null = x.data.TotalCount;
                    setVersions({
                        Items,
                        TotalCount,
                    });
                }
                else {
                    setVersions({
                        Items: [],
                        TotalCount: 0,
                    });
                }

            })
            .finally(() => {
                SetLoading(false);
            });
    };

    // Load all versions of a template
    const loadAllRunConfigVersions = (): void => {
        SetLoading(true);
        runConfigApi
            .runConfigurationList(
                plainToClass(RunConfigurationSearchDTO, {
                    FlowchartTemplateId: selectedTemplateVersion,
                    Start: 0,
                    Count: 0,
                })
            )
            .then((x) => {
                if (x.data.Items && x.data.TotalCount) {
                    const Items: RunConfigurationInfoDTO[] = x.data.Items;
                    const TotalCount: number | null = x.data.TotalCount;
                    setVersions({
                        Items,
                        TotalCount,
                    });
                } else {
                    setVersions({
                        Items: [],
                        TotalCount: 0,
                    });
                }
               
            })
            .finally(() => {
                SetLoading(false);
            });
    };

    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    const loadSelectedRunConfiguration = (id: string | null = null): void => {
        if (selectedVersion) {
            runConfigApi.runConfigurationGet(selectedVersion).then((x) => {
                if (x.data !== undefined) {
                    setSelectedRunConfig(x.data);
                }
            });
        }
    };

    const actionHandler = (item: any) => {
        if (item && type) {
            switch (type) {
                case ListTypes.Download:
                    // Replace with Report ID when available
                    downloadDocument(item.Id, item.Name);
                    break;
                case ListTypes.Launch:
                    runDocument(item.Id, item.Name);
                    break;
                case ListTypes.Edit:
                    openDocumentbyFlowchartTemplateVersionHistoryId(
                        item.Id,
                        item.Name
                    );
                    break;
            }
        }
    };
    const textWrap = html`<style>
        .text-wrap {
            white-space: nowrap !important;
            overflow: hidden;
            max-width: 150px;
            text-overflow: ellipsis;
            font-size: 14px;
            font-weight: 400;
            color: #686a6c;
        }
    </style>`;
    const tooltipItem = (value: any, isFirstColumn = false) => {
        const classes = isFirstColumn ? 'text-wrap first-column' : 'text-wrap';
        return html` ${textWrap}
            <omni-tooltip>
                <div slot="invoker" class="${classes}">${value}</div>
                <div slot="content">${value}</div>
            </omni-tooltip>`;
    };
    const getColumns = () => {
        const columns = [];
        columns.push({
            label: 'Name',
            passthrough: true,
            key: 'Name',
            template: (row: any) => {
                return html` <style>
                        .first-column {
                            font-size: 14px !important;
                            font-weight: 600 !important;
                            color: #3b3e3f !important;
                        }
                        .dropdown-menu-position {
                            display: block !important;
                            width:200px;
                            right:70px;
                            left:auto !important;
                            top:60% !important;
                            z-index: 99 !important;
                            position: absolute !important;
                        }
                         .icon-sm::part(icon){
                            width: 1rem !important;
                            height: 1rem !important;
                        }
                         .omni button.tertiary omni-icon{
                            --color-icon-lines: var(--color-almost-black) !important;
                        }
                    </style>
                    <td part="table-body-cell">
                        ${tooltipItem(row.Name, true)}
                    </td>`;
            },
        });
        /* columns.push({
            label: 'Version', passthrough: true, key: '', template: (row: any) => {
                return html`<td part="table-body-cell"><span>${type == ListTypes.Download ? row.FlowchartTemplateVersion : row.Version}</span></td>`
            }
        });*/

        if (type == ListTypes.Edit || type == ListTypes.Launch) {
            columns.push({
                label: 'Created Date',
                passthrough: true,
                key: 'CreatedDate',
                template: (row: any) => {
                    return html`<td part="table-body-cell">
                        ${tooltipItem(row.CreatedDate.slice(0, 10))}
                    </td>`;
                },
            });
        }
        if (type == ListTypes.Download) {
            columns.push({
                label: 'Modified By',
                passthrough: true,
                key: 'ModifiedByUser',
                template: (row: any) => {
                    return html`<td part="table-body-cell">
                        ${tooltipItem(row.ModifiedByUser.DisplayName)}
                    </td>`;
                },
            });
        }
        columns.push({
            label: 'Last Modified',
            passthrough: true,
            key: 'ModifiedDate',
            template: (row: any) => {
                return html`<td part="table-body-cell">
                    ${tooltipItem(row.ModifiedDate.slice(0, 10))}
                </td>`;
            },
        });
        columns.push({
            label: 'Created By',
            passthrough: true,
            key: 'CreatedByUser',
            template: (row: any) => {
                return html`<td part="table-body-cell">
                    ${tooltipItem(row.CreatedByUser.DisplayName)}
                </td>`;
            },
        });
        columns.push({
            label: 'Modified By',
            passthrough: true,
            key: 'ModifiedByUser',
            template: (row: any) => {
                return html`<td part="table-body-cell">
                    ${tooltipItem(row.ModifiedByUser.DisplayName)}
                </td>`;
            },
        });
        columns.push({
            label: 'Comments',
            passthrough: true,
            key: 'Comments',
            template: (row: any) => {
                return html`<td part="table-body-cell">
                    ${tooltipItem(row.Comments)}
                </td>`;
            },
        });
        columns.push({
            label: 'ACTIONS',
            passthrough: true,
            key: 'actions',
            template: (row: any) => {
                return html`<td part="table-body-cell">
            <div
    class=${`table-row-actions ${type == ListTypes.Edit ||
                        type == ListTypes.Download ||
                        type == ListTypes.Launch
                        ? 'alignLeftContainer'
                        : 'd-flex is-justify-content-end'}`}>
                <div>
                    <div
                        class="d-flex flex-row is-align-items-center mx-2">
                        <omni-tooltip>
                            ${type === ListTypes.Launch ? html`
                            <button
                                class="tertiary p-0 button-report-icon"
                                aria-haspopup="true" 
                                aria-controls="modal-report-dropdown-${row.Id}" 
                                @click=${() => handleToggleReportDropdown && handleToggleReportDropdown(row.Id)}
                                slot="invoker">
                                <omni-icon  class="icon-sm mr-0"
                                    icon-id="omni:informative:reports">
                                </omni-icon>
                            </button>
                            ` : html`
                            <omni-icon
                            class="icon-sm"
                                icon-id="omni:interactive:${type?.toLowerCase()}"
                                slot="invoker"
                                @click=${() => actionHandler(row)}>
                            </omni-icon>
                            `}
                            <div slot="content">
                                ${type?.toLowerCase()}
                            </div>
                        </omni-tooltip>
                    </div>
                    
                    ${type === ListTypes.Launch ? html`
                    <div 
                        class=${activeReportDropdown === row.Id ? 'dropdown-menu is-block dropdown-menu-position'  : 'is-hidden'} 
                        id="modal-report-dropdown-${row.Id}" 
                        role="menu">
                        <div class="dropdown-content">
                            <a class="dropdown-item is-flex is-align-items-center"
                                @click=${() => {
                                runDocument && runDocument (row.Id, row.Name);
                                setActiveReportDropdown && setActiveReportDropdown(null);
                        }}>
                                <span class="is-size-6 p-1">Download Document</span>
                            </a>
                            <a class="dropdown-item is-flex is-align-items-center"
                                @click=${() => {
                            navigateToRunConfig && navigateToRunConfig(row.Id, row.Name);
                            setActiveReportDropdown && setActiveReportDropdown(null);
                        }}>
                                <span class="is-size-6 p-1">Open Run Config Online</span>
                            </a>
                        </div>
                    </div>
                    ` : ''}
                </div>
            </div>
        </td>`;
            },
        });
        return columns;
    };

    return (
        <div>
            <Modal ref={modalChildRef} className="dialog-min-wd-and-ht">
                    {/*<div key="calendarConfigurator">*/}
                        <div slot="header" className="font-lg-bold">
                            Version History - {type?.toUpperCase()}
                </div>
                <div slot="" className="my-5">
                            {loading ? (
                                <Skeleton rowCount={4} />
                            ) : versions?.TotalCount &&
                                versions.TotalCount > 0 ? (
                                <>
                                    <div>
                                        <div className="body-1">
                                            Please select the version you would
                                            like to use.
                                        </div>
                                        <br />
                                        <div className="versionHistory">
                                            <Table
                                                // @ts-ignore
                                                data={versions?.Items}
                                                columns={getColumns()}></Table>
                                        </div>
                                    </div>
                                </>
                        ) : (
                            <p className="body-1">
                                    There are no versions available for this
                                    template
                                </p>
                            )}
                        {/*</div>*/}
                        <div slot="footer">
                            <div className="field is-grouped is-grouped-right">
                                <div className="control">
                                    <button                                       
                                        onClick={() => {
                                            closeForm();
                                        }}>
                                        Cancel
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
            </Modal>
        </div>
    );
});
export default TemplateVersionsModal;