import React, { useEffect, useState, useRef, ChangeEvent, JSX } from 'react';
import {
    FlowchartTemplateApi,
    FlowchartTemplateInfoDTO,
    FlowchartTemplateSearchDTO,
    DataApi,
    FlowchartTemplateDuplicateCheckDTO,
    FlowchartTemplateShareInfoDTO,
} from '@omniflow/omni-webapi';
import { html } from 'lit';

import {
    Icon,
    SearchInput,
    Tile,
    Toolbar,
} from '../../omni-ui-components';
import { plainToClass } from 'class-transformer';
import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';
import RequestInterceptor from '../../business/request-interceptor';
import useOpenDocument from '../../hooks/useOpenDocument';
import CreateFlowchartTemplateModalForm from '../createTemplate-Form/createTemplate-ModalForm';
// import DeleteFlowchartTemplateDialog from '../deleteTemplate/deleteTemplate-ModalDialog';
// import { OmniTableElement as OmniTable} from 'omni-ui';
import { Table } from '../../omni-ui-components/table';

import { ClientApi, OmniClientSearchDTO } from '@omniflow/omni-webapi';
import TemplateVersionsModal from '../template-versions-modal/template-versions-modal';
import { ListTypes } from '../../enums/list-types.enum';
import Skeleton from '../custom/skeleton';
import useEventLogger from '../../hooks/eventLogger';
import { Action, Module } from '../../enums/event.enum';
import './template-recent-list.scss';
import useNotification, {
    NotificationType,
} from '../notification/useNotification';
import ShareTemplateModal from './share-template-modal';
import ConfirmShareTemplateModal from './confirm-template';
import { useNavigate } from 'react-router-dom';

interface TemplateRecentListProps {
    ClientId: string;
    NumberOfItemsToLoad: number;
    LoggedInUserId: any;
}

interface TemplateRecentListState {
    loadedTemplates: number;
    rows: FlowchartTemplateInfoDTO[];
    showMore: boolean;
    totalTemplates: number;
}

interface UpdateFlowchartTemplateNameDto {
    OmniClientId: string;
    FlowchartTemplateId: string;
    TemplateName: string;
}

export default function TemplateRecentList(
    props: TemplateRecentListProps
): JSX.Element {
    const { logEvent } = useEventLogger(Module.FLOWCHART);
    const [state, setState] = useState<TemplateRecentListState>({
        loadedTemplates: 0,
        rows: [],
        showMore: false,
        totalTemplates: 0,
    });

    const [archivedState, setArchivedState] = useState<TemplateRecentListState>(
        {
            loadedTemplates: 0,
            rows: [],
            showMore: false,
            totalTemplates: 0,
        }
    );
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
    const disableStyle = html`<style>
        .omni input:disabled,
        .omni input[disabled],
        .omni .input:disabled,
        .omni .input[disabled] {
            opacity: 1;
        }
        .omni .input[disabled] {
            color: var(--color-gray-36);
        }
        .omni .input {
            font-size: 14px !important;
            font-weight: 600;
            color: #242526 !important;
        }     
         .omni .input.table-input:focus {
            padding-left: 15px !important;
            width:calc(100% - 15px) !important;
        }
        .omni .input.table-input:focus:has(+.shared-indicator) {
            padding-left: 15px !important;
            width:calc(100% - 31px) !important;
        }
    </style>`;
    const pushNotification = useNotification();
    const [isValidFileLength, setIsValidFileLength] = useState<boolean>(true);
    const [IsShareTemplateInProgress, setIsShareTemplateInProgress] =
        useState<boolean>(false);
    const [activeModalData, setActiveModalData] = useState<any>({
        show: false,
        row: '',
    });
    const [activeReportDropdown, setActiveReportDropdown] = useState<string | null>(null);


    const checkLength = (value: string) => {
        if (!value) {
            setIsValidFileLength(false);
            return;
        }

        const validLength = value.length <= 202;
        setIsValidFileLength(validLength);

        if (!validLength) {
            pushNotification(
                'Template name exceeds the maximum length limit (202 characters).',
                NotificationType.DANGER
            );
        }
    };

    const onChangeHandler = async (
        event: React.ChangeEvent<HTMLInputElement>,
        value: string,
        index: number
    ) => {
        const newValue = event.target.value.trim();

        // Check for empty value
        if (!newValue) {
            pushNotification(
                'The template name should not be blank.',
                NotificationType.DANGER
            );
            return; // Stop further processing
        }

        if (newValue !== value) {
            checkLength(newValue);
            await validateAndSaveTemplateName(newValue, index);
        }
    };

    const validateAndSaveTemplateName = async (
        newValue: string,
        index: number
    ) => {
        const { rows } = state;
        const updatedRows = [...rows];
        const template = updatedRows[index];

        // Validate template name length
        if (!isValidFileLength) {
            pushNotification(
                'Template name exceeds the maximum length limit (202 characters).',
                NotificationType.DANGER
            );
            return;
        }

        // Validate template name
        const isValid = validateTemplateName(newValue);
        if (!isValid) {
            pushNotification(
                'A template name can only contain one of the following characters A-Z a-z 0-9.',
                NotificationType.DANGER
            );
            return;
        }

        // To check for duplicate template name only if value has changed
        if (newValue !== template.Name) {
            try {
                const templateCheckDTO =
                    new FlowchartTemplateDuplicateCheckDTO();
                templateCheckDTO.OmniClientId = props.ClientId;
                templateCheckDTO.TemplateName = newValue;

                const { data } =
                    await templateApi.flowchartTemplateCheckDuplicateTemplateName(
                        templateCheckDTO
                    );

                if (data) {
                    pushNotification(
                        'The template name has already been used / unavailable.',
                        NotificationType.DANGER
                    );
                    return;
                }

                // If validation and duplicate check pass, proceed with update
                if (template && template.Id) {
                    const updateData: UpdateFlowchartTemplateNameDto = {
                        OmniClientId: props.ClientId,
                        FlowchartTemplateId: template.Id,
                        TemplateName: newValue,
                    };

                    await templateApi.flowchartTemplateUpdateFlowChartTemplateNameById(
                        updateData
                    );
                    pushNotification(
                        'Template name updated successfully.',
                        NotificationType.SUCCESS
                    );
                }
            } catch (error) {
                console.error('Error updating template name:', error);
                pushNotification(
                    'Failed to update template name.',
                    NotificationType.DANGER
                );
            }
        }
    };

    const validateTemplateName = (value: string): boolean => {
        if (!value) return false;

        const isValidLength = value.length <= 202;
        // eslint-disable-next-line no-control-regex
        const invalidCharacters = /[#$+%!`&'={}@<>:"|?*\u0000-\u001F^(),[\]]/;
        const windowsReservedNamesRegex = /^(con|prn|aux|nul|com\d|lpt\d)$/i;
        const fullStopInBeginning = /^\.+/;
        const spaceAtBeginning = /^(?!\s)(.*[^\s])$/;

        const isValidFormat =
            !windowsReservedNamesRegex.test(value) &&
            !fullStopInBeginning.test(value) &&
            spaceAtBeginning.test(value) &&
            !invalidCharacters.test(value);

        const isValid = isValidLength && isValidFormat;

        return isValid;
    };

    const isEditingDisabled = (index: number): boolean => {
        const row = state.rows[index];
        return row.CreatedByUser?.Id !== props.LoggedInUserId;
    };

    const isRestoreDisabled = (index: number): boolean => {
        const row = archivedState.rows[index];
        return row.CreatedByUser?.Id !== props.LoggedInUserId;
    };

    const tooltipItem = (value: any, isFirstColumn = false) => {
        const classes = isFirstColumn ? 'text-wrap first-column' : 'text-wrap';
        return html` ${textWrap}
            <omni-tooltip>
                <div slot="invoker" class="${classes}">${value}</div>
                <div slot="content">${value}</div>
            </omni-tooltip>`;
    };

    // Navigate Run Config Page
    const navigateToRunConfig = (id: string, name: string) => {
        const currentParams = new URLSearchParams(window.location.search);
        const clientId = currentParams.get('clientid');
        const ansid = currentParams.get('ANsid');

        console.log('Navigating to Run Config with document:', { id, name });

        navigate(`/run-config?clientid=${clientId}&ANsid=${ansid}&runConfigId=${id}`);

        // Close any open dropdowns
        setActiveReportDropdown(null);
    };

    //  handler function
    const handleToggleReportDropdown = (id: string) => {
        setActiveReportDropdown(prev => prev === id ? null : id);
        // Close other dropdowns when opening this one
        if (activeModalData.show) {
            setActiveModalData({
                show: false,
                row: '',
            });
        }
    };

    const inputChangeTemplateName = (
        value: any,
        isFirstColumn = false,
        index = 0
    ) => {
        return isEditingDisabled(index)
            ? tooltipItem(value, isFirstColumn)
            : html`${disableStyle}<input
                      id="template-name-${index}"
                      part="cell-input"
                      class="input is-static is-editable table-input"
                      type="text"
                      .value=${value}
                      ?disabled=${isEditingDisabled(index)}
                      @change=${(event: ChangeEvent<HTMLInputElement>) =>
                    onChangeHandler(event, value, index)} />`;
    };
    const archiveTemplate = (templateId: string) => {
        templateApi
            .flowchartTemplateDelete(templateId)
            .then(() => {
                pushNotification(
                    `Template is successfully archived!`,
                    NotificationType.SUCCESS
                );
                loadTemplates(null, true);
                loadArchiveTemplates(null, true);
            })
            .catch(() => {
                pushNotification(
                    `Error occured while archiving template. Please try again later.`,
                    NotificationType.DANGER
                );
            })
            .finally(() => {
                setActiveModalData({
                    show: false,
                    row: '',
                });
            });
    };

    const restoreTemplate = (templateId: string) => {
        templateApi
            .flowchartTemplateRestore(templateId)
            .then(() => {
                pushNotification(
                    `Template is successfully restored!`,
                    NotificationType.SUCCESS
                );
                loadTemplates(null, true);
                loadArchiveTemplates(null, true);
            })
            .catch(() => {
                pushNotification(
                    `Error occured while restoring template. Please try again later.`,
                    NotificationType.DANGER
                );
            });
    };
    const handleToggle = (id: string) => {
        setActiveModalData((v: { row: string; show: any }) => {
            return {
                show: v.row === id ? !v.show : true,
                row: id,
            };
        });
    };
    const archiveColumn = [
        {
            label: 'Name',
            key: 'Name',
            customClass: 'table-header',
            template: (row: FlowchartTemplateInfoDTO) => {
                return html` <td part="table-body-cell">
                    ${tooltipItem(row)}
                </td>`;
            },
        },
        {
            label: 'Created Date',
            key: 'CreatedDate',
            template: (row: any) => {
                return html`<td part="table-body-cell">
                    ${tooltipItem(new Date(row || Date.now())?.toDateString())}
                </td>`;
            },
        },
        {
            label: 'Created By',
            key: 'CreatedByUser',
            template: (row: any) => {
                return html` <td part="table-body-cell">
                    ${tooltipItem(row?.DisplayName)}
                </td>`;
            },
        },
        {
            label: 'Modified Date',
            key: 'ModifiedDate',
            template: (row: any) => {
                return html`<td part="table-body-cell">
                    ${tooltipItem(new Date(row || Date.now())?.toDateString())}
                </td>`;
            },
        },
        {
            label: 'Modified By',
            key: 'ModifiedByUser',
            template: (row: any) => {
                return html` <td part="table-body-cell">
                    ${tooltipItem(row?.DisplayName)}
                </td>`;
            },
        },
        {
            label: 'Actions',
            key: 'actions',
            passthrough: true,
            shouldClamp: false,
            template: (row: any, index: any) => {
                const isDisabled = isRestoreDisabled(index);
                return html`<td part="table-body-cell">
                    ${textWrap}
                    <div class="table-row-actions">
                        <div class="mx-3">
                            <div
                                class="is-flex is-align-items-center table-row-actions">
                                <button
                                    style="width:100%"                                    
                                    @click=${() => restoreTemplate(row.Id)}
                                    ?disabled=${isDisabled}>
                                    Restore
                                </button>
                            </div>
                        </div>
                    </div>
                </td>`;
            },
        },
    ];


    const columns = [
        {
            label: 'Name',
            key: 'Name',
            passthrough: true,
            shouldClamp: false,
            customClass: 'table-header',
            template: (row: FlowchartTemplateInfoDTO, index: any) => {
                return html` <style>
                        .first-column {
                            font-size: 14px !important;
                            font-weight: 600 !important;
                            color: #3b3e3f !important;
                            font-family: var(--family-sans-serif) !important;
                        }
                        .shared-indicator {
                            width: 8px;
                            height: 8px;
                            border-radius: 20px;
                            margin: 0 6px !important;
                            padding: 4px;
                        }
                        .shared-indicator.Shared {
                            background: #03bbe3;
                        }
                        .shared-indicator.Received {
                            background: #1a8978;
                        }
                        .report-dropdown-position {
                            display: block !important;
                            width:200px;
                            right:200px;
                            left:auto !important;
                            top:60% !important;
                            z-index: 99 !important;
                            position: absolute !important;
                        }
                        .more-actions-dropdown {
                            right: 100px !important;
                            left: auto !important;
                            z-index: 99 !important;
                            position: absolute !important;
                            top:60% !important;
                            width:200px;
                        }
                        .icon-sm::part(icon){
                            width: 1rem !important;
                            height: 1rem !important;
                        }
                        .omni button.tertiary omni-icon{
                          --color-icon-lines: var(--color-almost-black) !important;
                         }
                         .table-cell{
                               border-top-left-radius:8px !important;
                               border-bottom-left-radius: 8px !important;
                            }
                    </style>
                    <td part="table-body-cell" class="table-cell">
                        <omni-tooltip>
                            <div slot="invoker" style="display:flex">
                                ${inputChangeTemplateName(
                    row.Name,
                    true,
                    index
                )}
                                <div
                                    class=${row.ShareStatus != 'None'
                        ? `shared-indicator ${row.ShareStatus} `
                        : ''}></div>
                            </div>

                            <div slot="content">${row.Name}</div>
                        </omni-tooltip>
                    </td>`;
            },
        },
        {
            label: 'Created Date',
            key: 'CreatedDate',
            template: (row: any) => {
                return html`<td part="table-body-cell">
                    ${tooltipItem(new Date(row || Date.now())?.toDateString())}
                </td>`;
            },
        },
        {
            label: 'Created By',
            key: 'CreatedByUser',
            template: (row: any) => {
                return html` <td part="table-body-cell">
                    ${tooltipItem(row?.DisplayName)}
                </td>`;
            },
        },
        {
            label: 'Modified Date',
            key: 'ModifiedDate',
            template: (row: any) => {
                return html`<td part="table-body-cell">
                    ${tooltipItem(new Date(row || Date.now())?.toDateString())}
                </td>`;
            },
        },
        {
            label: 'Modified By',
            key: 'ModifiedByUser',
            template: (row: any) => {
                return html` <td part="table-body-cell">
                    ${tooltipItem(row?.DisplayName)}
                </td>`;
            },
        },
        {
            label: 'Actions',
            key: 'actions',
            passthrough: true,
            shouldClamp: false,
            template: (row: any, index: any) => {
                const isDisabled = isEditingDisabled(index);
                const isDefaultTemplate = row.IsDefault;
                return html`<td part="table-body-cell">
            ${textWrap}
            <div class="is-flex is-align-items-center table-row-actions">
               <div class="mx-1">
                  <div class="is-flex is-align-items-center">
                     <div class="row-action-container">
                        <omni-tooltip>
                           <omni-icon
                              class="icon-sm mr-1"
                              icon-id="omni:interactive:edit"
                              slot='invoker'
                              @click="${() =>
                        openDocument(row.Id, row.Name)}"></omni-icon>
                           <div slot='content'>edit</div>
                        </omni-tooltip>
                     </div>
                     <omni-icon
                        class="icon-sm mt-1"
                        icon-id="omni:interactive:down"
                        @click="${() =>
                        openModalHandler(
                            row?.Id || '',
                            ListTypes.Edit
                        )}"></omni-icon>
                  </div>
               </div>
               
               <div class="mx-1">
                  <div class="is-flex is-align-items-center table-row-actions">
                      <div class="report-button-container">
                        <omni-tooltip>
                  <button
                   class="tertiary p-0 report-button-height"
                   ?disabled=${row.IsRunConfigurationAvailable != true}
                   aria-haspopup="true" 
                   aria-controls="report-dropdown-menu" 
                   @click=${() => row.IsRunConfigurationAvailable == true ? handleToggleReportDropdown(row.Id) : null}>
                   <omni-icon  class="icon-sm mr-0"
                    part=${row.IsRunConfigurationAvailable != true ? 'omni-icon-disabled' : ''}
                    icon-id="omni:informative:reports">
                  </omni-icon>
                 </button>
            ${row.IsRunConfigurationAvailable == true
                        ? html`<div slot="content">Report Options</div>`
                        : html`<div slot="content">Save Flowchart to create the version report</div>`
                    }
                        </omni-tooltip>
                     </div>
      
                     <omni-icon  class="icon-sm mt-2"
                        icon-id="omni:interactive:down"
                  @click="${() => openModalHandler(row?.Id || '', ListTypes.Launch)}">
                 </omni-icon>
               </div>
   
                <div class=${activeReportDropdown === row.Id ? 'dropdown-menu is-block report-dropdown-position' : 'is-hidden'}
                    id="report-dropdown-menu" 
                    role="menu">
                  <div class="dropdown-content">
                     <a class="dropdown-item is-flex is-align-items-center"
                          @click=${() => {
                        runDocument(row.Id, row.Name);
                        setActiveReportDropdown(null);
                    }}>
                      <span class="is-size-6 p-1">Download Document</span>
                          </a>
                             <a class="dropdown-item is-flex is-align-items-center"
                                @click=${() => {
                        navigateToRunConfig(row.Id, row.Name);
                        setActiveReportDropdown(null);
                    }}>
                             <span class="is-size-6 p-1">Open Run Config Online</span>
                            </a>
                        </div>
                  </div>
               </div>
            <div class="mx-1">
                  <div  class="is-flex is-align-items-center" class="table-row-actions">
                       <div class="action-button-container">
                        <omni-tooltip>
                           <omni-icon  class="icon-sm mr-1"
                            part=${row.IsReportAvailable != true
                        ? 'omni-icon-disabled'
                        : ''
                    }
                              icon-id="omni:interactive:download"
                              slot="invoker"
                              @click="${() =>
                        row.IsReportAvailable == true
                            ? downloadDocumentByFlowChartTemplateId(
                                row.Id,
                                row.Name
                            )
                            : null}"></omni-icon>         
                             ${row.IsReportAvailable == true
                        ? html`
                                           <div slot="content">download</div>
                                       `
                        : html`
                                           <div slot="content">
                                               Save Flowchart to create the
                                               version
                                           </div>
                                       `
                    }
                        </omni-tooltip>
                     </div>
                     <omni-icon  class="icon-sm mt-2"
                        icon-id="omni:interactive:down"
                         class="icon-sm mt-2"
                        @click="${() =>
                        openModalHandler(
                            row?.Id || '',
                            ListTypes.Download
                        )}"></omni-icon>
                  </div>
               </div>
                
             <div>
                  <div class="is-flex is-align-items-center test-data">
                     <div class="action-button-container">
                        <omni-tooltip>
                      <button  class="tertiary px-0 action-button-height" ?disabled=${hideShareTemplate && isDefaultTemplate
                    } aria-haspopup="true" aria-controls="dropdown-menu" @click=${() =>
                        handleToggle(row.Id)} >
                           <omni-icon  class="mr-0 icon-sm"
                              icon-id="omni:interactive:actions"
                            ></omni-icon>
                          </button>
                           <div slot="content">${isDisabled && hideShareTemplate
                        ? 'No Actions Available'
                        : 'More Actions'
                    }</div>
                          
                        </omni-tooltip>
                     </div>
                  </div>
                      <div slot="dropdown-menu"
                      class=${activeModalData.show && activeModalData.row === row.Id ? 'dropdown-menu is-block more-actions-dropdown' : 'is-hidden'} 
                      id="dropdown-menu" role="menu">
                      ${isDisabled || (!isDisabled && isDefaultTemplate)
                        ? html``
                        : html`<div
                                    slot="dropdown-content"
                                    class="dropdown-content"
                                    @click=${() => archiveTemplate(row.Id)}>
                                    <a
                                        class="dropdown-item is-flex is-align-items-center"
                                        ><span id="custom" class="is-size-6 p-1"
                                            >Archive Template</span
                                        ></a
                                    >
                                </div> `
                    }
                        ${hideShareTemplate
                        ? html``
                        : html`<div
                                      slot="dropdown-content"
                                      class="dropdown-content"
                                      @click=${() => shareTemplate(row.Id)}>
                                      <a
                                          class="dropdown-item is-flex is-align-items-center"
                                          ><span
                                              id="custom"
                                              class="is-size-6 p-1"
                                              >Share Template</span
                                          ></a
                                      >
                                  </div>`
                    }
                    </div>
               </div>
              <div class=${!row.IsDefault ? 'is-hidden' : 'ml-3'} >
                
                        <omni-tooltip>
                            <omni-icon  class="icon-sm mr-0"
                                    icon-id="omni:informative:info"
                                    slot='invoker'
                                    ></omni-icon>
                            <div slot='content'>Default Template</div>
                        </omni-tooltip>
                      
                </div>  
            </div>
            </div>
         </td>`;
            },
        },
    ];

    // Save search text in state to be able to show more items, when search exceeds 20 items.
    const [searchText, setSearchText] = useState<string>('');
    const [searchTextArchive, setSearchTextArchive] = useState<string>('');
    const templateApi = new FlowchartTemplateApi(
        WEBAPI_CONFIGURATION,
        '',
        RequestInterceptor
    );
    const {
        openDocument,
        //openDocumentbyReportId,
        //openFlowchartDocumentbyReportId,
        //downloadDocument,
        downloadDocumentByFlowChartTemplateId,
        runDocument,
    } = useOpenDocument();
    const searchRef = useRef<any>(null);
    const searchArchiveRef = useRef<any>(null);
    // Template Versions
    const [selectedVersion, setSelectedVersion] = useState<string | null>(null);
    const [showModal, setShowModal] = useState<boolean>(false);
    const [btnType, setBtnType] = useState<ListTypes>();
    const [loading, SetLoading] = useState<boolean>(false);
    const [showGuidModal, setShowGuidModal] = useState<boolean>(false);
    const [clientName, setClientName] = useState<null | string | undefined>('');
    const [showArchiveTemplate, setShowArchiveTemplate] =
        useState<boolean>(false);
    const [showActiveTemplate, setShowActiveTemplate] = useState<boolean>(true);
    //const [shareTemplateModal, setShareTemplateModal] =
    //    useState<boolean>(false);
    const [confirmShareTemplate, setConfirmShareTemplate] =
        useState<boolean>(false);

    const [clientNameOptions, setClientNameOptions] = useState<any>([]);
    const [selectedClientName, setSelectedClientName] = useState<any>([]);
    const [componentOptions, setComponentOptions] = useState<any>([]);
    const [selectedComponentOptions, setSelectedComponentOptions] =
        useState<any>([]);
    const [templateName, setTemplateName] = useState<string>('');
    const [templateId, setTemplateId] = useState<string>('');
    const [duplicateTemplateNameCheck, setDuplicateTemplateNameCheck] =
        useState(false);
    const [hideShareTemplate, setHideShareTemplate] = useState(true);
    const [defaultVal, setDefaultVal] = useState<any>([]);

    const navigate = useNavigate();
    const modalRef = useRef<any>(null);
    const previousModalRef = useRef<any>(null);
    const shareModalRef = useRef<any>(null);
    const confirmModalRef = useRef<any>(null);
    const loadClients = (
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        searchText: string | null = null,
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        start = 0,
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        count = 100
    ): void => {
        const clientApi = new ClientApi(WEBAPI_CONFIGURATION, '', RequestInterceptor);
        clientApi.clientGet(props.ClientId).then((x) => {
            setClientName(x.data.Client?.Name);
        });
    };

    const loadClientList = (searchText = null, start = 0, count = 100) => {
        const clientApi = new ClientApi(WEBAPI_CONFIGURATION, '', RequestInterceptor);
        clientApi
            .clientList(
                plainToClass(OmniClientSearchDTO, {
                    SearchText: searchText,
                    Start: start,
                    Count: count,
                    OrderAscending: true,
                    OrderBy: null,
                })
            )
            .then((x) => {
                if (x && x?.data && x.data?.Items && x.data.Items?.length > 1) {
                    setHideShareTemplate(false);
                }
            });
    };

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

    // Debounced effect for loading templates and archive templates
    useEffect(() => {
        // Debounce logic
        const handler = setTimeout(() => {
            // Only call if ClientId is present
            if (props.ClientId) {
                loadTemplates(searchText || null, true);
                loadArchiveTemplates(searchTextArchive || null, true);
                loadClientList();
                loadClients();
                searchRef.current?.toggleSearch(false);
                searchArchiveRef.current?.toggleSearch(false);
            }
        }, 300); // 300ms debounce
        return () => clearTimeout(handler);
    }, [props.ClientId, searchText, searchTextArchive]);

    const clearAll = () => {
        if (modalRef.current !== previousModalRef.current) {
            setSelectedClientName([]);
            setSelectedComponentOptions([]);
            setDuplicateTemplateNameCheck(false);
            previousModalRef.current = modalRef.current;
        }
    };

    const onSearchUpdate = (data: any): void => {
        if (
            data &&
            data.detail &&
            data.detail.value &&
            data.detail.value !== ''
        ) {
            // Allow 'Show More' to work correctly with a search with more than 20 results.
            setSearchText(data.detail.value);
        } else {
            setSearchText('');
        }
    };
    const onArchiveSearchUpdate = (data: any): void => {
        if (
            data &&
            data.detail &&
            data.detail.value &&
            data.detail.value !== ''
        ) {
            // Allow 'Show More' to work correctly with a search with more than 20 results.
            setSearchTextArchive(data.detail.value);
        } else {
            setSearchTextArchive('');
        }
    };

    const loadMore = (): void => {
        logEvent(Action.NEXTRECORD);
        if (searchText !== '') {
            loadTemplates(searchText, false, state.loadedTemplates);
        } else {
            loadTemplates(null, false, state.loadedTemplates);
        }
    };
    const loadMoreArchive = (): void => {
        if (searchTextArchive !== '') {
            loadArchiveTemplates(
                searchTextArchive,
                false,
                archivedState.loadedTemplates
            );
        } else {
            loadArchiveTemplates(null, false, archivedState.loadedTemplates);
        }
    };

    const loadTemplates = (
        searchText: string | null = null,
        clearRows = false,
        skip = 0,
        orderBy = 'ModifiedDate',
        sortAscending = false
    ): void => {
        SetLoading(true);
        logEvent(Action.SEARCH);
        templateApi
            .flowchartTemplateList(
                plainToClass(FlowchartTemplateSearchDTO, {
                    OmniClientId: props.ClientId,
                    SearchText: searchText,
                    Start: skip,
                    Count: props.NumberOfItemsToLoad,
                    OrderBy: orderBy,
                    OrderAscending: sortAscending,
                    Removed: false,
                })
            )
            .then((x) => {
                if (x.data.Items) {
                    const items = x.data.Items;
                    const loadedTemplates: number = clearRows
                        ? items.length
                        : state.loadedTemplates + items.length;
                    let totalTemplates = 0;
                    let showMore = false;
                    let rows: FlowchartTemplateInfoDTO[] = [];

                    if (x.data.TotalCount) {
                        totalTemplates = x.data.TotalCount;
                    }

                    if (loadedTemplates < totalTemplates) {
                        showMore = true;
                    }

                    if (clearRows) {
                        rows = [...items];
                    } else {
                        rows = [...state.rows, ...items];
                    }

                    setState({
                        loadedTemplates,
                        rows,
                        showMore,
                        totalTemplates,
                    });
                }
            })
            .finally(() => {
                SetLoading(false);
            });
    };

    const loadArchiveTemplates = (
        searchText: string | null = null,
        clearRows = false,
        skip = 0,
        orderBy = 'ModifiedDate',
        sortAscending = false
    ): void => {
        SetLoading(true);
        logEvent(Action.SEARCH);
        templateApi
            .flowchartTemplateList(
                plainToClass(FlowchartTemplateSearchDTO, {
                    OmniClientId: props.ClientId,
                    SearchText: searchText,
                    Start: skip,
                    Count: props.NumberOfItemsToLoad,
                    OrderBy: orderBy,
                    OrderAscending: sortAscending,
                    Removed: true,
                })
            )
            .then((x) => {
                if (x.data.Items) {
                    const items = x.data.Items;
                    const loadedTemplates: number = clearRows
                        ? items.length
                        : archivedState.loadedTemplates + items.length;
                    let totalTemplates = 0;
                    let showMore = false;
                    let rows: FlowchartTemplateInfoDTO[] = [];

                    if (x.data.TotalCount) {
                        totalTemplates = x.data.TotalCount;
                    }

                    if (loadedTemplates < totalTemplates) {
                        showMore = true;
                    }

                    if (clearRows) {
                        rows = [...items];
                    } else {
                        rows = [...archivedState.rows, ...items];
                    }

                    setArchivedState({
                        loadedTemplates,
                        rows,
                        showMore,
                        totalTemplates,
                    });
                }
            })
            .finally(() => {
                SetLoading(false);
            });
    };

    const onTemplateCreated = (newTemplate: FlowchartTemplateInfoDTO): void => {
        // New template has been created. Update template list.
        loadTemplates(null, true);

        openDocument(newTemplate.Id || '', newTemplate.Name || '');
    };

    // Dev only: delete template row
    // const deleteTemplate = (templateId: string): void => {
    //     templateApi
    //         .flowchartTemplateDelete(templateId, props.ClientId)
    //         .then((response) => {
    //             loadTemplates(null, true);
    //         })
    // }

    const openModalHandler = (id: string, type: ListTypes) => {
        if (modalRef.current) {
            modalRef.current.showModal();
        }

        setSelectedVersion(id);
        setBtnType(type);
    };
    const showHideActiveTemplates = () => {
        setShowArchiveTemplate(false);
        setShowActiveTemplate(true);
    };
    const showHideArchiveTemplates = () => {
        setShowArchiveTemplate(true);
        setShowActiveTemplate(false);
    };
    const renderActiveTemplate = () => {
        if (state.totalTemplates === 0) {
            return (
                <div className="empty alignCenterContainer">
                    {`There are no flowchart templates for this client. Click "New Template" to add a new template.`}
                </div>
            );
        }
        return (
            <Table
                className="table-fb"
                // @ts-ignore
                data={state.rows}
                columns={columns}>
                <div slot="table-footer">
                    <div className="tableFooterContainer">
                        <div className="count">
                            Showing 1-{state.loadedTemplates} of{' '}
                            {state.totalTemplates}
                        </div>
                        {state.showMore && (
                            <button
                                className="button is-default is-small"
                                onClick={() => loadMore()}>
                                Show More
                            </button>
                        )}
                        <div></div>
                    </div>
                </div>
            </Table>
        );
    };

    const renderArchiveTemplate = () => {
        if (archivedState.totalTemplates === 0) {
            return (
                <div className="empty alignCenterContainer">
                    There are no archive flowchart templates for this client.
                </div>
            );
        }
        return (
            <Table
                className="table-fb"
                // @ts-ignore
                data={archivedState.rows}
                columns={archiveColumn}>
                <div slot="table-footer">
                    <div className="tableFooterContainer">
                        <div className="count">
                            Showing 1-{archivedState.loadedTemplates} of{' '}
                            {archivedState.totalTemplates}
                        </div>
                        {archivedState.showMore && (
                            <button
                                className="button is-default is-small"
                                onClick={() => loadMoreArchive()}>
                                Show More
                            </button>
                        )}
                        <div></div>
                    </div>
                </div>
            </Table>
        );
    };

    const shareTemplate = async (templateId: string) => {
        await templateApi
            .flowchartTemplateFlowchartTemplateShareInfo(
                plainToClass(FlowchartTemplateShareInfoDTO, {
                    OmniClientId: props.ClientId,
                    TemplateId: templateId,
                })
            )
            .then((x) => {
                if (shareModalRef.current) {
                    shareModalRef.current.showModal();
                }
                clearAll();
                if (x && x.data && x.data.ClientDetailsList) {
                    const clientDetailsList = x.data.ClientDetailsList;
                    setClientNameOptions(
                        clientDetailsList.map((opt) => ({
                            id: opt.ClientId,
                            value: opt.Name,
                        }))
                    );
                }
                if (x && x.data && x.data.ComponentDetailsList) {
                    const componentDetailsList = x.data.ComponentDetailsList;
                    const filteredList = componentDetailsList
                        .map((comp) => {
                            if (
                                comp.ComponentName === 'Calendar' ||
                                comp.ComponentName === 'MediaHierarchy'
                            ) {
                                return {
                                    id: comp.TemplateId,
                                    value: `${comp.ComponentName} -  ${comp.Name}`,
                                    name: comp.Name,
                                };
                            }
                        })
                        .filter((x) => x);
                    if (filteredList.length != 2) {
                        pushNotification(
                            'There are no Calendar and Media Hierarchy component definitions in the template. Please add both component definitions if you want to share.',
                            NotificationType.DANGER
                        );
                        if (shareModalRef.current) {
                            shareModalRef.current.close();
                        }
                    } else {
                        const completeOptionsList = componentDetailsList.map(
                            (opt) => ({
                                id: opt.TemplateId,
                                value: `${opt.ComponentName} -  ${opt.Name}`,
                                name: opt.Name,
                                attributes: !!filteredList.find(
                                    (f: any) => f.id === opt.TemplateId
                                ),
                            })
                        );
                        const optionsSelectedByDefault =
                            completeOptionsList.filter((aa) => {
                                if (
                                    filteredList.find(
                                        (f: any) => f.id === aa.id
                                    )
                                ) {
                                    return true;
                                }
                                return false;
                            });
                        setComponentOptions(completeOptionsList);
                        setSelectedComponentOptions(optionsSelectedByDefault);
                        setDefaultVal([...optionsSelectedByDefault]);
                    }
                }

                if (x && x.data && x.data.TemplateId && x.data.TemplateName) {
                    setTemplateName(x.data.TemplateName);
                    setTemplateId(x.data.TemplateId);
                }
            })
            .catch(() => {
                pushNotification(
                    'There are no components in the template. Please add component if you want to share.',
                    NotificationType.DANGER
                );
                if (shareModalRef.current) {
                    shareModalRef.current.close();
                }
            });
    };

    const checkDuplicateTemplate = async (val: any) => {
        try {
            const { data } =
                await templateApi.flowchartTemplateCheckDuplicateTemplateName(
                    plainToClass(FlowchartTemplateDuplicateCheckDTO, {
                        OmniClientId: val.id,
                        TemplateName: templateName,
                    })
                );
            setDuplicateTemplateNameCheck(data);
        } catch (e) {
            setDuplicateTemplateNameCheck(false);
        }
    };
    const showClientName = (value: any) => {
        setSelectedClientName(value.detail);
        checkDuplicateTemplate(value.detail);
    };

    const showComponentName = (event: any) => {
        const list = event.detail;
        const defaultList = defaultVal;
        const newList = [...list];
        for (let i = 0; i < defaultList.length; i++) {
            let defaultValExists = false;
            for (let j = 0; j < list.length; j++) {
                if (defaultList[i].id == list[j].id) {
                    defaultValExists = true;

                    break;
                }
            }
            if (!defaultValExists) {
                pushNotification(
                    'Cannot deselect Calendar and Media Hierarchy components.',
                    NotificationType.WARNING
                );
                newList.push(defaultList[i]);
            }
        }

        setSelectedComponentOptions(newList);
    };
    const shareTemplateMethod = async () => {
        setIsShareTemplateInProgress(true);
        await templateApi
            .flowchartTemplateSaveSharetemplate(
                plainToClass(FlowchartTemplateShareInfoDTO, {
                    currentOmniClientId: props.ClientId,
                    newOmniClientId: selectedClientName.id,
                    NewTemplateName: templateName,
                    TemplateId: templateId,
                    ComponentInfosList: selectedComponentOptions
                        .filter((x: any) => x.id !== undefined)
                        .map((x: any) => {
                            return { TemplateId: x.id };
                        }),
                })
            )
            .then(() => {
                setIsShareTemplateInProgress(false);
                pushNotification(
                    'Template shared successfully',
                    NotificationType.SUCCESS
                );
            })
            .catch(() => {
                setIsShareTemplateInProgress(false);
                pushNotification(
                    'Error while sharing template',
                    NotificationType.DANGER
                );
            });

        if (confirmModalRef.current) {
            confirmModalRef.current.close();
        }
    };

    return (
        <>
            {showGuidModal && (
                <div className={'modal ' + (showGuidModal ? 'is-active' : '')}>
                    <div className="modal-content">
                        <article className="notification is-info">
                            <button
                                className="delete"
                                aria-label="delete"
                                onClick={() =>
                                    setShowGuidModal(false)
                                }></button>
                            {clientName} is not mapped to any Guid in Planit.
                            Please contact support.
                        </article>
                    </div>
                </div>
            )}

            <ShareTemplateModal
                templateName={templateName}
                selectedClientName={selectedClientName}
                clientNameOptions={clientNameOptions}
                componentOptions={componentOptions}
                selectedComponentOptions={selectedComponentOptions}
                showClientName={showClientName}
                showComponentName={showComponentName}
                setTemplateName={setTemplateName}
                setConfirmShareTemplate={setConfirmShareTemplate}
                duplicateTemplateNameCheck={duplicateTemplateNameCheck}
                setDuplicateTemplateNameCheck={
                    setDuplicateTemplateNameCheck
                }
                handleConfirmModal={() => {
                    confirmModalRef.current.showModal();
                }}
                ref={shareModalRef}
                modalHandler={(ref) => ref.current.showModal()}
            />


            <ConfirmShareTemplateModal
                confirmShareTemplate={confirmShareTemplate}
                templateName={templateName}
                selectedClientName={selectedClientName}
                selectedComponentOptions={selectedComponentOptions}
                shareTemplateMethod={shareTemplateMethod}
                setConfirmShareTemplate={setConfirmShareTemplate}
                isShareTemplateInProgress={IsShareTemplateInProgress}
                ref={confirmModalRef}
            />

            <div className="d-flex app-container-fb is-justify-content-flex-end mb-2">
                <div className="sent-recieved-info mr-3">
                    <p className="shared-indicator Shared"></p> Sent
                </div>
                <div className="sent-recieved-info">
                    <p className="shared-indicator Recieved"></p> Recieved
                </div>
            </div>
            <Tile className="app-container-fb">

                <TemplateVersionsModal
                    clientId={props.ClientId}
                    modalState={showModal}
                    modalHandler={(ref) => ref.current.showModal()}
                    selectedTemplateVersion={selectedVersion}
                    type={btnType}
                    ref={modalRef}
                    handleToggleReportDropdown={handleToggleReportDropdown}
                    activeReportDropdown={activeReportDropdown}
                    setActiveReportDropdown={setActiveReportDropdown}
                    navigateToRunConfig={navigateToRunConfig}
                />


                <Toolbar
                    slot="header"
                    style={{ paddingLeft: '8px', paddingRight: '4px' }}>
                    <div slot="start">
                        <button
                            className={`tab ${showActiveTemplate
                                    ? 'active'
                                    : ''
                                }`}
                            onClick={showHideActiveTemplates}>
                            Active templates
                        </button>
                        <button
                            style={{ marginLeft: '8px' }}
                            className={`tab ${showArchiveTemplate
                                    ? 'active'
                                : ''
                                }`}
                            onClick={showHideArchiveTemplates}>
                            Archived templates
                        </button>
                    </div>

                    {showActiveTemplate && (
                        <>
                            <div slot="end">
                                <SearchInput
                                    className="search-icon search-md search-sm search-lg"
                                    ref={searchRef}
                                    onSearchUpdate={onSearchUpdate}
                                    value={searchText}
                                    isExpanded
                                />
                            </div>
                            <div slot="end">
                                <button
                                    className="secondary fixed-size-small h-24"
                                    onClick={() => {
                                        loadTemplates(null, true);
                                    }}>
                                    Refresh
                                    <span>
                                        <Icon
                                            icon-id="omni:interactive:refresh"
                                        />
                                    </span>
                                </button>
                            </div>

                            <div slot="end" className="toolbar-divider"></div>

                            <div slot="end">
                                <CreateFlowchartTemplateModalForm
                                    callback={onTemplateCreated}
                                    clientId={props.ClientId}
                                />
                            </div>
                        </>
                    )}
                    {showArchiveTemplate && (
                        <>
                            <div slot="center-end">
                                <SearchInput
                                    className="search-icon search-md search-sm search-lg"
                                    ref={searchArchiveRef}
                                    onSearchUpdate={onArchiveSearchUpdate}
                                    value={searchTextArchive}
                                />
                            </div>
                        </>
                    )}
                </Toolbar>
                {loading ? (
                    <Skeleton rowCount={4} />
                ) : (
                    <>
                        {showActiveTemplate && renderActiveTemplate()}
                        {showArchiveTemplate && renderArchiveTemplate()}
                    </>
                )}
            </Tile>
        </>
    );
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
//function setModalActive(arg0: boolean) {
//    throw new Error('Function not implemented.');
//}
