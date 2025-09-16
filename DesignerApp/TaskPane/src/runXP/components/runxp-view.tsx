import React, { useEffect, useRef, useState, useMemo } from 'react';
import {
    MediaHierarchyTemplateDetailsDTO,
    RunConfigurationApi,
    RunConfigurationCreateDTO, RunConfigurationDetailsDTO, RunRestriction as RestrictionSetting, MediaHierarchyLevel, CalendarType
} from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';
import { flow, isEqual, isEmpty } from 'lodash';
import * as API from '@omniflow/omni-webapi';

import usePublish from '../../hooks/usePublish';
import RunXPGroup from './runxp-group';
import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';
import RequestInterceptor from '../../business/request-interceptor';
import { AppContext } from '../../taskpane/contexts/AppContext';
import useRender from '../../hooks/useRender';
import { Tile } from '../../omni/tile';
import { Toolbar } from '../../omni/toolbar';
import useNotification, {
    NotificationType,
} from '../../components/notification/useNotification';
import { Switch } from "../../omni/switch";
import { CustomPropertyService } from '../../business/custom-property-service';
import { CustomPropertyKey } from '../../enums/custom-property-key.enum';
import Restriction from './run-restrictions/RunRestriction';
import { RunXPDialog } from './runxp-dialog';
import RunRestriction from './run-restrictions/RunRestriction';
import useEventLogger from '../../../src/hooks/useEventLogger';
import { Action, Module, SubModule } from '../../enums/event.enum'
import { CustomProperty } from '../../models/custom-property';
import {
    runConfigurationApi
} from '../../lib/api';

import Dialog, { DialogType, ButtonType } from '../../components/dialogs/Dialog';
import { OmniCheckBoxInput } from '../../omni/checkbox';
import { ExcelRenderer } from '../../business/engine/renderer/excel-renderer';
import { mediaHierarchyTemplateApi } from '../../lib/api';
import { DataContext } from '../../taskpane/contexts/DataContext';
import { CalendarHelper } from '../../../src/business/engine/helpers/calendar-helper';
import './runxp.css'
import { Icon } from '../../omni/icon';
import { Tooltip } from '../../omni/tooltip';



const RunXPView = () => {
    const [flowchartTemplateIdState, setFlowchartTemplateIdState] = useState<
        string | null
    >(null);
    const [mediaHierarchyLevels, setMediaHierarchyLevels] = useState<
        any | null
    >(null);
    const [headerData, setHeaderData] = useState<any | null>(null);
    const [runResctrictions, setRunResctrictions] = useState<any[]>([]);
    const [runResctrictionsWithValues, setRunResctrictionsWithValues] = useState<any[]>([]);
    const [selectionOpen, setSelectionOpen] = useState(false);
    const [isSaveLaunchModalOpen, setIsSaveLaunchModalOpen] = useState(false);
    const [isSaveDownloadModalOpen, setIsSaveDownloadModalOpen] = useState(false);
    const [reportName, setReportName] = useState<string | null>(null);
    const [currentMediaHierarchy, setCurrentMediaHierarchy] =
        useState<MediaHierarchyTemplateDetailsDTO | null>(null);
    const [isPublishing, setIsPublishing] = useState(false);

    const { logEvent } = useEventLogger(Module.RUNRESTRICTION);


    const {
        flowchartTemplateDefinition: flowChartData,
        currentMediaHierarchyDetails,
        currentCalendarTemplateDetails,
        runConfigurationId,
        mode,
        flowchartRunConfiguration,
        initialRenderDone,
        setCurrentMediaHierarchyDetails,
        onRun,
        runConfigLoaded,
        setRunConfigLoaded,
        setIsTrackFormattingRunConfig,
        isTrackFormattingRunConfig,
        manualFormattingStylesRunConfig,
        columns,
        staticFilter,
        setStaticFilter,
    } = React.useContext(AppContext);

    const { setIsGlobalRendering } = React.useContext(DataContext);
    const [flowchartTemplateDefinition, setFlowchartTemplateDefinition] = useState<any>(null);

    const runConfigApi = new RunConfigurationApi(
        WEBAPI_CONFIGURATION,
        '',
        RequestInterceptor
    );

    const { render, isRendering } = useRender();
    const pushNotification = useNotification();

    const { publishDocument } = usePublish({
        headerData,
        runResctrictions: runResctrictionsWithValues,
        reportName,
    });
    const [initialLoad, setLoaded] = useState(false);
    const [inputText, setInputText] = useState('');
    const [characterLimit] = useState(20);
    const [keepUserFormatting, setKeepUserFormatting] = useState(true);
    const appContext = React.useContext(AppContext);

    const [stopRender, setStopRender] = useState(false);
    const onChangeHandler = React.useCallback((event) => {
        setInputText(event.target.value);
    }, []);
    const isJsonObject = (strData) => {
        try {
            JSON.parse(strData);
        }
        catch (e) {
            return false;
        }
        return true;
    }
    const handleChange = React.useCallback((event) => {
        setKeepUserFormatting(Boolean(event?.currentTarget?.checked));
    }, []);
    const getRestrictions = () => {
        const runRestrictionsWithMultipleValues = Array<RestrictionSetting>();
        runResctrictions.forEach((restriction: RestrictionSetting) => {
            if (!isEmpty(restriction.ValueJson) && restriction.TableId != null && restriction.ColumnName != null) {
                const values = restriction.ValueJson.split("$");
                values.forEach(value => {
                    if (!isEmpty(value)) {
                        runRestrictionsWithMultipleValues.push(plainToClass(RestrictionSetting,
                            { TableId: restriction.TableId, ColumnName: restriction.ColumnName, ValueJson: isJsonObject(value) ? value : JSON.stringify(value) }));
                    }

                });
            }
        });
        return runRestrictionsWithMultipleValues;
    };
    useEffect(() => {
        if (isEmpty(inputText) && flowchartRunConfiguration?.Comments) {
            setInputText(flowchartRunConfiguration.Comments);
        }

    }, [flowchartRunConfiguration.Comments])
    useEffect(() => {
        if (flowchartTemplateDefinition?.[mode ? "FlowchartTemplateId" : "Id"] !== undefined) {
            setFlowchartTemplateIdState(flowchartTemplateDefinition?.[mode ? "FlowchartTemplateId" : "Id"]);
        }
    }, [flowchartTemplateDefinition?.[mode ? "FlowchartTemplateId" : "Id"]]);

    useEffect(() => {
        if (isEmpty(flowchartTemplateDefinition)) {
            setFlowchartTemplateDefinition(mode ? flowchartRunConfiguration : flowChartData);
        }
        if (isEmpty(runResctrictions) && !isEmpty(flowchartRunConfiguration)) {
            const parsedRestrictions = flowchartRunConfiguration.RunResctrictions.map((value) => {
                return plainToClass(API.RunRestriction, {
                    ColumnName: value.ColumnName,
                    TableId: value.TableId,
                    ValueJson: JSON.parse(value.ValueJson)

                });
            }

            )
            setRunResctrictions(parsedRestrictions || []);
        }
    }, [flowchartRunConfiguration, flowChartData]);




    useEffect(() => {
        setRunResctrictionsWithValues(getRestrictions());
    }, [runResctrictions]);

    // to set the run restriction
    useEffect(() => {
        if (initialRenderDone && !initialLoad && runResctrictionsWithValues && runResctrictionsWithValues.length && flowchartTemplateDefinition && !isRendering && currentMediaHierarchyDetails) {
            setTimeout(() => {
                (async () => {
                    setLoaded(true);
                    //await renderHandler();
                })();
            }, 0);
        }
    }, [initialRenderDone, runResctrictionsWithValues, flowchartTemplateDefinition, isRendering, currentMediaHierarchyDetails])

    useEffect(() => {
        if (flowchartTemplateDefinition !== undefined) {
            if (reportName === null || reportName === undefined) {
                setReportName(flowchartTemplateDefinition?.Name);
            }
        }
    }, [flowchartTemplateDefinition?.Name]);


    const saveRunConfigHandler = async (showNotification = true, comments?: string, isTrackFormatting?: boolean) => {
        logEvent({ action: Action.SAVELAUNCHVERSION });
        if (!reportName) {
            pushNotification(
                'Please enter a name first.',
                NotificationType.WARNING
            );
            return '';
        }
        let customStyles = null;
        if (isTrackFormatting) {
            //const excelRenderer = new ExcelRenderer();
            customStyles = sessionStorage.getItem("runCustomStyles");
            //if (customStyles != null) {
            //    customStyles= JSON.stringify(customStyles);

            //}
        }
        //await renderHandler(isTrackFormatting);

        // Get styles applied in sheet

        try {
            const { data } = await runConfigApi.runConfigurationCreate(
                plainToClass(RunConfigurationCreateDTO, {
                    FlowchartTemplateId: flowchartTemplateIdState,
                    CalendarDefinition: currentCalendarTemplateDetails,
                    MediaHierarchyLevels:
                        currentMediaHierarchyDetails?.Definition?.Levels ??
                        flowchartTemplateDefinition?.MediaHierarchyLevels,
                    Name: reportName,
                    RunRestrictions: getRestrictions(),
                    Comments: comments,
                    CustomStyleSettings: customStyles,
                    TrackFormatChanges: isTrackFormatting,
                })
            );
            if (data && data.Id) {
                appContext.setRunConfigurationId(data.Id);
            }
            showNotification &&
                pushNotification(
                    `The Run Configuration "${reportName}" was successfully created! You can check the Versioning History in the landing page.`,
                    NotificationType.SUCCESS
                );

            return data.Id;
        } catch {
            pushNotification(
                `The Run Configuration "${reportName}" could not be created. Please try again or change the settings.`,
                NotificationType.DANGER
            );
        }
        return '';
    };

    const publishReportHandler = async (
        e: React.MouseEvent<HTMLButtonElement, MouseEvent>,
        keepUserFormatting: boolean

    ) => {
        logEvent({ action: Action.SAVEDOWNLOADVERSION });
        e.preventDefault();
        if (!reportName) {
            pushNotification(
                'Please enter a name first.',
                NotificationType.WARNING
            );
            return;
        }

        setIsPublishing(true);
        if (runConfigurationId) {
            if (keepUserFormatting) {
                await publishDocument(null, inputText);
                pushNotification(
                    'The version has been saved with the manual applied format.',
                    NotificationType.INFO
                );
            }
            else {
                await renderHandler(keepUserFormatting);
                await publishDocument(null, inputText);
                pushNotification(
                    'The version has been saved with the manual applied format.',
                    NotificationType.INFO
                );
            }



        } else {
            // open selection list
            setSelectionOpen(false);
        }
        setIsPublishing(false);
    };

    const changeReportNameHandler = React.useCallback((e) => {
        setReportName(e.target.value);
    }, []);

    useEffect(() => {
        if (!isEqual(currentMediaHierarchyDetails, currentMediaHierarchy)) {
            setCurrentMediaHierarchy(currentMediaHierarchyDetails);
            setMediaHierarchyLevels(
                currentMediaHierarchyDetails?.Definition?.Levels
            );
        }
    }, [currentMediaHierarchyDetails]);

    const isFirstRefresh = useRef(true);
    const updateCurrentMediaHierarchyForRun = async (): Promise<API.MediaHierarchyDefinition | null | undefined> => {
        const flowchartHierarchyId =
            flowChartData?.Definition?.MediaHierarchyDefinition
                ?.TemplateId;
        let current = JSON.parse(JSON.stringify(currentMediaHierarchyDetails));

        if ((!currentMediaHierarchyDetails?.Id && flowchartHierarchyId) || !current?.Definition) {
            //load mh
            const { data } =
                await mediaHierarchyTemplateApi.mediaHierarchyTemplateGet(
                    flowchartHierarchyId
                );
            if (data) current = JSON.parse(JSON.stringify(data));
        }

        if (!current?.Definition?.Levels) return null;
        current.Definition.Levels.forEach((level: API.MediaHierarchyLevel) => {
            let settings = flowchartRunConfiguration?.MediaHierarchyLevels?.find(rl => rl.Order === level.Order)?.Settings;
            if (settings) {
                settings.forEach((setting) => {
                    let index = level.Settings.findIndex((item) => item.Name === setting.Name);
                    if (index > -1) {
                        level.Settings[index].Enabled = setting.Enabled;

                        // Sub levels
                        if (setting?.SubLevels && level.Settings[index]?.SubLevels) {
                            level.Settings[index].SubLevels.forEach((sublevel, sublevelIndex) => {
                                let subLevelSettings = setting.SubLevels.find(item => item.Order == sublevel.Order)?.Settings;
                                if (subLevelSettings) {
                                    subLevelSettings.forEach((subSetting) => {
                                        let subIndex = sublevel.Settings.findIndex((item) => item.Name === subSetting.Name);
                                        if (subIndex > -1) {
                                            level.Settings[index].SubLevels[sublevelIndex].Settings[subIndex].Enabled = subSetting.Enabled;
                                        }
                                    });
                                }
                            });
                        }
                    }
                });
            }
        })
        setCurrentMediaHierarchyDetails(current);
        return current.Definition;
    };
    useEffect(() => {
        if (isFirstRefresh.current == true && (mode || onRun) && !isRendering && flowchartRunConfiguration?.Id && flowChartData?.Definition?.MediaHierarchyDefinition
            ?.TemplateId && columns && columns.length) {
            isFirstRefresh.current = false;
            updateCurrentMediaHierarchyForRun().then((mediaHierarchy) => {
                if (runConfigLoaded == false) {
                setRunConfigLoaded(true);
                    processRendering(mediaHierarchy).then();
                }

            })
        }

    }, [mode, flowChartData, flowchartRunConfiguration, isRendering, onRun])



    const processRendering = async (mediaHierarchyDefinition?: API.MediaHierarchyDefinition) => {

        const runRestrictionsWithMultipleValues = getRestrictions();

        if (mediaHierarchyDefinition) {
            await render(
                'mediaHierarchyDefinition',
                mediaHierarchyDefinition,
                runRestrictionsWithMultipleValues
            );
        }
        else {
            // use deep equality comparison instead of JSON.stringify to avoid false negatives
            if (!isEqual(currentMediaHierarchyDetails, currentMediaHierarchy)) {
                await render(
                    'mediaHierarchyDefinition',
                    currentMediaHierarchyDetails?.Definition,
                    runRestrictionsWithMultipleValues
                );
                setCurrentMediaHierarchy(currentMediaHierarchyDetails);
            }

            await render(
                'calendarDefinition',
                currentCalendarTemplateDetails?.Definition,
                runRestrictionsWithMultipleValues
            );
        }
    };

    const renderHandler = async (isTrackFormatting?: boolean) => {
        logEvent({ action: Action.RENDER });


        setIsGlobalRendering(true)
        await processRendering();
        setIsGlobalRendering(false)



    };

    const preparePublish = async (id: string) => {
        setIsPublishing(true);
        await renderHandler();
        if (id === '') {
            const newId = await saveRunConfigHandler(false);
            if (newId === '') {
                pushNotification(
                    `Publish not possible. Run configuration Id is missing. Please try again or change the settings.`,
                    NotificationType.DANGER
                );
                return;
            }
            await publishDocument(newId);
        } else {
            await publishDocument(id);
        }
        setSelectionOpen(false);
        setIsPublishing(false);
    };

    const showSaveLaunchModal = (show: boolean) => {
        setIsSaveLaunchModalOpen(show);
    };
    const showSaveDownloadModal = (show: boolean) => {
        setIsSaveDownloadModalOpen(show);
    }
    const toggleSwitch = React.useCallback((event) => {
        setIsTrackFormattingRunConfig(event.target.checked);
    }, []);
    useEffect(() => {
        renderHandler(isTrackFormattingRunConfig).then();
    }, [isTrackFormattingRunConfig])    

    const [autoRefreshData, setAutoRefreshData] = useState<boolean>(!staticFilter);

    const toggleStaticFilterSwitch = React.useCallback((event) => {
        setStaticFilter(!event.target.checked);
        setAutoRefreshData(event.target.checked);
    }, []);

    return (
        <div>
            <Tile className="mb-1 runxp-main-tile">

                <Restriction
                    runResctrictions={runResctrictions}
                    setRunResctrictions={setRunResctrictions}
                    clientId={flowChartData.OmniClientId}
                />

                <div className="is-flex is-align-items-center py-2">
                    <p className="component-title mr-3">
                        Parameters
                    </p>
                    <input
                        defaultValue={reportName}
                        onChange={changeReportNameHandler}
                        className="input w-50"
                        placeholder="Name"
                    />
                </div>
                <div className="border-line"></div>

                <div className="mt-5">
                    <h3>Active Flows:</h3>


                    <RunXPGroup />
                </div>
                <div className="d-flex  mb-4">
                    <Switch onChange={(e) => toggleSwitch(e)} checked={isTrackFormattingRunConfig}>
                        <span slot="end"> Track Formatting</span>
                    </Switch>
                </div>

                <div className="d-flex  mb-4">
                    <Switch onChange={(e) => toggleStaticFilterSwitch(e)} checked={autoRefreshData}>
                        <span slot="end"> Auto Refesh Data  <Tooltip>
                            <Icon
                                iconId="omni:informative:info"
                                slot="invoker"
                            />
                            <div slot="content">Unselecting works with the existing data. Filters must be part of the media hierarchy; otherwise, live data is fetched.</div>
                        </Tooltip></span> 
                    </Switch>
                   
                </div>

            </Tile>
            <div className="d-flex is-justify-content-flex-end mt-5">
                <button
                    disabled={isRendering}
                    slot="end"
                    className="tertiary"
                    onClick={() => {
                        if (isTrackFormattingRunConfig) { setStopRender(true); }
                        else {
                            renderHandler(isTrackFormattingRunConfig).then();
                        }
                    }}>
                    Render
                </button>
                <button
                    slot="end"
                    className="tertiary"
                    onClick={() => showSaveLaunchModal(true)}>
                    Save Report Version
                </button>
                <button
                    slot="end"
                    className="tertiary"
                    onClick={() => showSaveDownloadModal(true)}
                    disabled={isPublishing}>
                    Save Download Version
                </button>


            </div>
            {selectionOpen && (
                <RunXPDialog
                    onClose={() => setSelectionOpen(false)}
                    onPublish={async (id) => await preparePublish(id)}
                    isPublishing={isPublishing}
                />
            )}

            {isSaveLaunchModalOpen && (

                <Dialog
                    icon="interactive:save"
                    type={DialogType.INFO}
                    title="Save report version"
                    okText="Save version"
                    onOk={(e) => {
                        showSaveLaunchModal(false);
                        saveRunConfigHandler(true, inputText, isTrackFormattingRunConfig);
                        setInputText('');
                    }}
                    onCancel={(e) => {
                        showSaveLaunchModal(false);
                        e.preventDefault();
                        setInputText('');
                    }}
                    okDisabled={!inputText.length}
                    className="run-config-dialog"
                    buttonType={ButtonType.PRIMARY}
                    showDialog={isSaveLaunchModalOpen}
                >
                    <div className="dataCard">
                        <label htmlFor="template-version" className="label font-bl-11"> * Version comment</label>

                        <input
                            id="template-version"
                            type="text"
                            placeholder="Label your new version"
                            value={inputText}
                            className="input text d-block "
                            onChange={onChangeHandler}
                            maxLength={20}
                            required
                        />
                        <div className="input-length-badge font-bl-11 text-opactity">
                            {inputText?.length || 0}/{characterLimit}
                        </div>
                        {/*                        <div className="is-flex-direction-row is-align-items-center runconfig-checkbox">
                            <OmniCheckBoxInput checked={keepUserFormatting} onClick={(e) => handleChange(e)}> <label className="keep-formatting">Keep manual formatting</label></OmniCheckBoxInput>
                            {keepUserFormatting && <p className="font-bl-12 pl-20pixel"> Any manual changes to the Excel file will be kept.Any un - rendered changes to the Excel file will be lost.</p>}
                        </div>*/}
                    </div>
                </Dialog>
            )}
            {isSaveDownloadModalOpen && (

                <Dialog
                    icon="interactive:save"
                    type={DialogType.INFO}
                    title="Save download version"
                    okText="Save version"
                    onOk={(e) => {
                        showSaveDownloadModal(false);
                        publishReportHandler(e, keepUserFormatting);
                        setInputText('');
                    }}
                    onCancel={(e) => {
                        showSaveDownloadModal(false);
                        e.preventDefault();
                        setInputText('');
                    }}
                    okDisabled={!inputText.length}
                    className="run-config-dialog"
                    buttonType={ButtonType.PRIMARY}
                    showDialog={isSaveDownloadModalOpen}
                >
                    <div className="dataCard">
                        <div className="label-container top">
                            <label htmlFor="template-version" className="label font-bl-11"> * Version comment</label>
                        </div>
                        <input
                            id="template-version"
                            type="text"
                            placeholder="Label your new version"
                            value={inputText}
                            className="input text d-block "
                            onChange={onChangeHandler}
                            maxLength={20}
                            required
                        />
                        <div className="input-length-badge font-bl-11 text-opactity">
                            {inputText?.length || 0}/{characterLimit}
                        </div>
                    </div>
                    <div className="is-flex-direction-row is-align-items-center runconfig-checkbox">
                        <OmniCheckBoxInput checked={keepUserFormatting} onClick={(e) => handleChange(e)}> <label className="keep-formatting">Keep manual formatting</label></OmniCheckBoxInput>
                        {keepUserFormatting && <p className="font-bl-12 pl-20pixel"> Any manual changes to the Excel file will be kept.Any un - rendered changes to the Excel file will be lost.</p>}
                    </div>
                </Dialog>
            )}
            {stopRender && (
                <Dialog
                    icon="interactive:save"
                    type={DialogType.INFO}
                    title="Excel Rendering"
                    okText="Render"
                    isRequired={false}
                    onOk={async (e) => {

                        setStopRender(false);
                        await renderHandler(isTrackFormattingRunConfig);

                    }}
                    onCancel={(e) => {
                        setStopRender(false);
                        e.preventDefault();

                    }}
                    className="run-config-dialog"
                    buttonType={ButtonType.PRIMARY}
                    showDialog={stopRender}
                >
                    <div className="dataCard">
                        <p>(Keep manual formatting) is ON. This might override Theme styles and Component styles.</p>
                        {/*        <div className="is-flex-direction-row is-align-items-center runconfig-checkbox">
                            <OmniCheckBoxInput checked={keepUserFormatting} onClick={(e) => handleChange(e)}> <label className="keep-formatting">Keep manual formatting</label></OmniCheckBoxInput>
                            {keepUserFormatting && <p className="font-bl-12 pl-20pixel"> Any manual changes to the Excel file will be kept. Any un-rendered changes to the Excel file will be lost.</p>}
                        </div>*/}

                    </div>

                </Dialog>
            )}


        </div>
    );
};
export default RunXPView;