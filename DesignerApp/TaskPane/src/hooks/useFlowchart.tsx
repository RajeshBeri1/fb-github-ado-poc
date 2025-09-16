import {
    FlowchartTemplateDetailsDTO,
    FlowchartTemplateUpdateDTO,
    RunConfigurationCreateDTO,
    FlowchartTemplatesVersionHistortiesDTO,
    RunConfigurationInfoDTO,
    RunConfigurationSearchDTO,
    RunConfigurationInfoListDTO,
    FlowchartData,
    SaveFlowchartDTO,
    MediaHierarchyLevel,
    ReportPublishDTO
    
} from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';
import { isEmpty, isEqual, update } from 'lodash';
import { useContext, useEffect, useState } from 'react';
import useNotification, {
    NotificationType,
} from '../components/notification/useNotification';
import { flowchartTemplateApi, runConfigurationApi, calendarTemplateApi } from '../lib/api';
import App from '../taskpane/components/App';
import { AppContext } from '../taskpane/contexts/AppContext';
import usePublish from './usePublish';
import useRenderWithDefinition from './useRenderWithDefinition';
import { useNavigate } from 'react-router';
import { ExcelRenderer } from '../business/engine/renderer/excel-renderer';

function useFlowchart() {

    const [isSavingFlowchart, setIsSavingFlowchart] = useState(false);
    const [hasUpdate, setHasUpdate] = useState(true);
    const appContext = useContext(AppContext);
    const pushNotification = useNotification();

    const [headerData, setHeaderData] = useState<any | null>(null);
    const [runResctrictions, setRunResctrictions] = useState<any[]>([]);
    const [runResctrictionsWithValues, setRunResctrictionsWithValues] = useState<any[]>([]);

    const [reportName, setReportName] = useState<string | null>(null);

    const { publishDocument, publishDocumentAsTemplate } = usePublish({
        headerData,
        runResctrictions: runResctrictionsWithValues,
        reportName,
    });
    const { render, isRenderingWithDefinition, setFlowchartTemplateDefinition } = useRenderWithDefinition();

    const navigate = useNavigate();

    const getStorageKey = (key: string) => {
        return sessionStorage.getItem(key);
    };
    const setStorageKey = (key: string, value: any) => {
        return sessionStorage.setItem(key, value);
    };

    useEffect(() => {
        const savedIsSavingFlowchart = getStorageKey('isSavingFlowchart');
        if (savedIsSavingFlowchart !== null) {
            setIsSavingFlowchart(savedIsSavingFlowchart === 'true');
        }
    }, []);

    useEffect(() => {
        setStorageKey('isSavingFlowchart', isSavingFlowchart.toString());
    }, [isSavingFlowchart]);

    useEffect(() => {
        if (appContext.flowchartTemplateDefinition?.Name !== undefined) {
            if (reportName === null || reportName === undefined) {
                setReportName(appContext.flowchartTemplateDefinition?.Name);
            }
        }
    }, [appContext.flowchartTemplateDefinition?.Name]);

    useEffect(() => {
        if (!appContext.currentCalendarTemplateDetails?.Name?.trim()) {
            getCurrentCalendar();
        }
    }, [appContext.currentCalendarTemplateDetails?.Name]);

    // Optimized getCurrentCalendar
    const getCurrentCalendar = () => {
        const flowchartCalendarId = appContext.flowchartTemplateDefinition?.Definition?.CalendarDefinition?.TemplateId;
        if (flowchartCalendarId) {
            calendarTemplateApi.calendarTemplateGet(flowchartCalendarId)
                .then((x) => {
                    appContext.setCurrentCalendarTemplateDetails(x.data);
                })
                .catch(() => {
                    pushNotification('Failed to load calendar template.', NotificationType.DANGER);
                });
        }
    };

    // TODO: currently deselection can't be registered as change
    // useEffect(() => {
    //     const flowchart = appContext.flowchartTemplateDefinition?.Definition;
    //     if (!flowchart && _oneOrMoreDefinitionsAvailable()) {
    //         setHasUpdate(true);
    //         return;
    //     }
    //     setHasUpdate(_oneOrMoreDefinitionsChanged());
    // }, [
    //     appContext.flowchartTemplateDefinition,
    //     appContext.currentCalendarTemplateDetails,
    //     appContext.currentHeaderTemplateDetails,
    //     appContext.currentThemeTemplateDetails,
    //     appContext.currentMediaHierarchyDetails,
    //     appContext.currentTotalsDetails,
    //     appContext.currentCalendarOverlayTemplateDetails,
    //     appContext.currentGrandTotalsDetails,
    //     appContext.currentFooterTemplateDetails,
    // ]);
    const _oneOrMoreDefinitionsChanged = () => {
        const flowchart = {
            ...appContext.flowchartTemplateDefinition?.Definition,
        };

        const calendarUpdate = _hasDefinitionChanged(
            flowchart?.CalendarDefinition?.Definition,
            appContext.currentCalendarTemplateDetails?.Definition
        );
        const mediaHierarchyUpdate = _hasDefinitionChanged(
            flowchart?.MediaHierarchyDefinition?.Definition,
            appContext.currentMediaHierarchyDetails?.Definition
        );
        const headerUpdate = _hasDefinitionChanged(
            flowchart?.HeaderDefinition?.Definition,
            appContext.currentHeaderTemplateDetails?.Definition
        );
        const themeUpdate = _hasDefinitionChanged(
            flowchart?.ThemeDefinition?.Definition,
            appContext.currentThemeTemplateDetails?.Definition
        );
        const totalsUpdate = _hasDefinitionChanged(
            flowchart?.TotalsDefinition?.Definition,
            appContext.currentTotalsDetails?.Definition
        );
        const calendarOverlayUpdate = _hasDefinitionChanged(
            flowchart?.CalendarOverlayDefinition?.Definition,
            appContext.currentCalendarOverlayTemplateDetails?.Definition
        );
        const grandTotalsUpdate = _hasDefinitionChanged(
            flowchart?.GrandTotalDefinition?.Definition,
            appContext.currentGrandTotalsDetails?.Definition
        );
        const footerUpdate = _hasDefinitionChanged(
            flowchart?.FooterDefinition?.Definition,
            appContext.currentFooterTemplateDetails?.Definition
        );

        return (
            calendarUpdate ||
            mediaHierarchyUpdate ||
            headerUpdate ||
            themeUpdate ||
            totalsUpdate ||
            calendarOverlayUpdate ||
            grandTotalsUpdate ||
            footerUpdate
        );
    };

    const _hasDefinitionChanged = (flowchartDefinition, update) => {
        if (update && !isEqual(flowchartDefinition, update)) return true;
        return false;
    };

    const _oneOrMoreDefinitionsAvailable = () => {
        // returns true when at least one current definition is available
        return (
            !!appContext.currentCalendarTemplateDetails ||
            !!appContext.currentHeaderTemplateDetails ||
            !!appContext.currentMediaHierarchyDetails ||
            !!appContext.currentThemeTemplateDetails ||
            !!appContext.currentTotalsDetails ||
            !!appContext.currentCalendarOverlayTemplateDetails ||
            !!appContext.currentGrandTotalsDetails ||
            !!appContext.currentFooterTemplateDetails
        );
    };


    const renderHandler = async () => {
        setFlowchartTemplateDefinition(getCurrentFlowchart());
        await render(
            'mediaHierarchyDefinition',
            appContext.currentMediaHierarchyDetails?.Definition,
            []
        );
        await render(
            'calendarDefinition',
            appContext.currentCalendarTemplateDetails?.Definition,
            []
        );
    };

    const createRunConfiguration = async (comments: string, trackFormatting?: boolean, customStyles?: string) => {
        return runConfigurationApi.runConfigurationCreate(
            plainToClass(RunConfigurationCreateDTO, {
                FlowchartTemplateId: appContext.flowchartTemplateId,
                CalendarDefinition: appContext.currentCalendarTemplateDetails,
                MediaHierarchyLevels: appContext.currentMediaHierarchyDetails?.Definition?.Levels || getCurrentFlowchart().Definition.MediaHierarchyDefinition.Definition.Levels,
                Name: appContext.flowchartTemplateDefinition.Name,
                RunRestrictions: [],
                Comments: comments || 'Initial Version',
                CustomStyleSettings: customStyles,
                TrackFormatChanges: trackFormatting
            })
        );

    };

    const loadRunConfiguratonList = async (comments?: string, trackFormatting?: boolean, customStyles?: string) => {
        return createRunConfiguration(comments, trackFormatting, customStyles).then(() => {
            return runConfigurationApi
                .runConfigurationList(
                    plainToClass(RunConfigurationSearchDTO, {
                        FlowchartTemplateId: appContext.flowchartTemplateId,
                        Start: 0,
                        Count: 0,
                    })
                );
        });
    };

    const saveFlowchartWithReportAndRunConfiguration = async (
        flowchartDefinition?: FlowchartTemplateDetailsDTO,
        comments?: string,
        trackFormatting?: boolean
    ) => {
        const value = getStorageKey('isSavingFlowchart');
        if (isSavingFlowchart || (value && value == "true")) return;
        setStorageKey('isSavingFlowchart', true);
        setIsSavingFlowchart(true);
        //await renderHandler();
        let updateWith = getCurrentFlowchart();
        if (flowchartDefinition) {
            updateWith = flowchartDefinition;
        }
        const update = plainToClass(FlowchartTemplateUpdateDTO, updateWith);

        update.Comments = comments;
        update.TrackFormatChanges = trackFormatting;
        let customStyles = null;
        if (trackFormatting) {
            //const excelRenderer = new ExcelRenderer();
            customStyles = sessionStorage.getItem("customstyles"); // await excelRenderer.getAppliedStyles(appContext.manualFormattingStyles);
            if (customStyles != null) {
                //customStyles = JSON.stringify(customStyles);
                update.CustomStyleSettings = customStyles;
            }
        }

        flowchartTemplateApi
            .flowchartTemplateUpdate(update)
            .then(async (x) => {
                appContext.setFlowchartTemplateDefinition(x.data);
                await _storeDocument(x.data.Id);

                // Create Run Configuration and Publish Report
                loadRunConfiguratonList(comments, trackFormatting, customStyles).then(async (runConfiguration) => {
                    await publishDocument(runConfiguration.data.Items[0].Id, comments, trackFormatting, customStyles);
                    appContext.setRunConfigurationId(runConfiguration.data.Items[0].Id);
                });
                pushNotification(
                    `Flowchart "${x.data.Name}" was successfully saved!`,
                    NotificationType.SUCCESS
                );
                setStorageKey('isSavingFlowchart', false);
                setIsSavingFlowchart(false);
                appContext.setIsFlowchartSaving(false);
            })
            .catch((e) => {
                console.error(e);
                pushNotification(
                    `Flowchart could not be saved.`,
                    NotificationType.DANGER
                );
                setStorageKey('isSavingFlowchart', false);
                setIsSavingFlowchart(false);
                appContext.setIsFlowchartSaving(false);
            });

    };

    const saveFlowchart = async (
        flowchartDefinition?: FlowchartTemplateDetailsDTO,
        comments?: string,
        openRunConfig?: boolean,
        trackFormatting?: boolean
    ) => {
        //let { path } = useRouteMatch();
        if (appContext.flowchartTemplateDefinition.Version == 0) {
            await saveFlowchartWithReportAndRunConfiguration(null, comments, trackFormatting);
            if (openRunConfig) {
                navigate(`/Dashboard/${appContext.flowchartTemplateDefinition.Id}/runxp`);
                appContext.setOnRun(true);
            }

        } else {
            const value = getStorageKey('isSavingFlowchart');
            if (isSavingFlowchart || (value && value == "true")) return;
            setStorageKey('isSavingFlowchart', true);
            setIsSavingFlowchart(true);
            let updateWith = getCurrentFlowchart();

            if (flowchartDefinition) {
                updateWith = flowchartDefinition;
            }
            const update = plainToClass(FlowchartTemplateUpdateDTO, updateWith);
            if (!isEmpty(comments)) {
                update.Comments = comments;
            }
            update.TrackFormatChanges = trackFormatting;

            let customStyles = null;
            if (trackFormatting) {
                const excelRenderer = new ExcelRenderer();
                customStyles = sessionStorage.getItem("customstyles");;
                if (customStyles != null) {
                    //customStyles = JSON.stringify(customStyles);
                    update.CustomStyleSettings = customStyles;
                }
            }

            flowchartTemplateApi
                .flowchartTemplateUpdate(update)
                .then(async (x) => {
                    appContext.setFlowchartTemplateDefinition(x.data);
                    await _storeDocument(x.data.Id);
                    pushNotification(
                        `Flowchart "${x.data.Name}" was successfully saved!`,
                        NotificationType.SUCCESS
                    );

                    setStorageKey('isSavingFlowchart', false);
                    setIsSavingFlowchart(false);
                    appContext.setIsFlowchartSaving(false);
                })
                .catch((e) => {
                    console.error(e);
                    pushNotification(
                        `Flowchart could not be saved.`,
                        NotificationType.DANGER
                    );
                    setStorageKey('isSavingFlowchart', false);
                    setIsSavingFlowchart(false);
                    appContext.setIsFlowchartSaving(false);
                    console.error(e);
                });
        }

    };

    const saveAsTemplate = async (
        flowchartDefinition?: FlowchartTemplateDetailsDTO,
        comments?: string,
        openRunConfig?: boolean,
        trackFormatting?: boolean,
        templateName?: string
    ) => {
        const value = getStorageKey('isSavingFlowchart');
        if (isSavingFlowchart || (value && value == "true")) return;
        setStorageKey('isSavingFlowchart', true);
        setIsSavingFlowchart(true);
        await renderHandler();
        let updateWith = getCurrentFlowchart();
        if (flowchartDefinition) {
            updateWith = flowchartDefinition;
        }

        let customStyles = null;
        let customStyleSettings = null;
        if (trackFormatting) {
            //const excelRenderer = new ExcelRenderer();
            customStyles = sessionStorage.getItem("customstyles"); // await excelRenderer.getAppliedStyles(appContext.manualFormattingStyles);
            if (customStyles != null) {
                //customStyles = JSON.stringify(customStyles);
                customStyleSettings = customStyles;
            }
        }
       
        const saveFlowchartData = plainToClass(SaveFlowchartDTO, {
            Definition: updateWith.Definition,
            OmniClientId: updateWith.OmniClientId,
            Comments: comments,
            TrackFormatChanges: trackFormatting,
            TemplateName: templateName,
            CalendarDefinition: appContext.currentCalendarTemplateDetails,
            MediaHierarchyLevels: appContext.currentMediaHierarchyDetails?.Definition?.Levels || getCurrentFlowchart().Definition.MediaHierarchyDefinition.Definition.Levels,
            RunRestrictions: [],
            TemplateId: appContext.flowchartTemplateId,
            Version: 1,
            CustomStyleSettings: customStyleSettings,
            ReportPublish: new ReportPublishDTO(),

        });

        await publishDocumentAsTemplate(saveFlowchartData).then(async () => {
            setStorageKey('isSavingFlowchart', false);
            setIsSavingFlowchart(false);
            appContext.setIsFlowchartSaving(false);
        }).catch((e) => {
            console.error(e);
           
            setStorageKey('isSavingFlowchart', false);
            setIsSavingFlowchart(false);
            appContext.setIsFlowchartSaving(false);
            console.error(e);
        });;
       
    };

    const getReferencedDefinition = (contextDefinition: any) => {
        return {
            Definition: contextDefinition?.Definition,
            TemplateId: contextDefinition?.Id,
            TemplateVersion: contextDefinition?.Version,
        };
    };

    const hasReferenceValues = (contextData) => {
        return (
            contextData?.Definition && contextData?.Id && contextData?.Version
        );
    };

    const getCurrentFlowchart = (): FlowchartTemplateDetailsDTO => {
        const currentFlowchartDefinition = { ...appContext.flowchartTemplateDefinition?.Definition };
        const assignIfHasRef = (key: string, contextValue: any) => {
            if (hasReferenceValues(contextValue)) {
                currentFlowchartDefinition[key] = getReferencedDefinition(contextValue);
            }
        };
        assignIfHasRef('CalendarDefinition', appContext.currentCalendarTemplateDetails);
        assignIfHasRef('HeaderDefinition', appContext.currentHeaderTemplateDetails);
        assignIfHasRef('MediaHierarchyDefinition', appContext.currentMediaHierarchyDetails);
        assignIfHasRef('ThemeDefinition', appContext.currentThemeTemplateDetails);
        assignIfHasRef('TotalsDefinition', appContext.currentTotalsDetails);
        assignIfHasRef('CalendarOverlayDefinition', appContext.currentCalendarOverlayTemplateDetails);
        assignIfHasRef('GrandTotalDefinition', appContext.currentGrandTotalsDetails);
        assignIfHasRef('FooterDefinition', appContext.currentFooterTemplateDetails);

        return {
            ...appContext.flowchartTemplateDefinition,
            Definition: currentFlowchartDefinition,
        };
    };

    const _storeDocument = async (templateId: string) => {
        let fileName = 'placeholder_name.xlsx';
        await Excel.run(async (context) => {
            context.workbook.load('name');
            await context.sync();
            fileName = context.workbook.name;
        });

        await Office.context.document.getFileAsync(
            Office.FileType.Compressed,
            async function(result) {
                if (result.status == Office.AsyncResultStatus.Succeeded) {
                    const file = await result.value;

                    const sliceCount = file.sliceCount;
                    const docdataSlices = [];
                    let slicesReceived = 0, gotAllSlices = true;

                    // Get the file slices.
                    _getSliceAsync(
                        file,
                        0,
                        sliceCount,
                        gotAllSlices,
                        docdataSlices,
                        slicesReceived,
                        templateId,
                        fileName
                    );
                } else {
                    pushNotification(
                        `Flowchart could not be saved.`,
                        NotificationType.DANGER
                    );
                }
            }
        );
    };

    const _getSliceAsync = (
        file,
        nextSlice,
        sliceCount,
        gotAllSlices,
        docdataSlices,
        slicesReceived,
        storeId,
        fileName
    ) => {
        file.getSliceAsync(nextSlice, async function(sliceResult) {
            if (sliceResult.status == 'succeeded') {
                if (!gotAllSlices) {
                    file.closeAsync();
                }
                docdataSlices[sliceResult.value.index] = sliceResult.value.data;

                if (++slicesReceived == sliceCount) {
                    // All slices have been received.
                    try {
                        var docdata = [];
                        for (var i = 0; i < docdataSlices.length; i++) {
                            docdata = docdata.concat(docdataSlices[i]);
                        }
                        let fileContent = new String();
                        for (var j = 0; j < docdata.length; j++) {
                            fileContent += String.fromCharCode(docdata[j]);
                        }
                        let arrayBuffer = new ArrayBuffer(fileContent.length);
                        var unsignedIntBuffer = new Uint8Array(arrayBuffer);

                        for (var i = 0; i < fileContent.length; i++) {
                            unsignedIntBuffer[i] = fileContent.charCodeAt(i);
                        }
                        const blob = new Blob([arrayBuffer], {
                            type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
                        });
                        const uploadedFile = new File([blob], fileName, {
                            type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
                        });

                        // // code for file download
                        // const blobUrl = window.URL.createObjectURL(blob);
                        // const a = document.createElement('a');
                        // a.href = blobUrl;
                        // a.download = fileName;
                        // document.body.appendChild(a);
                        // a.click();
                        // setTimeout(function () {
                        //     document.body.removeChild(a);
                        //     window.URL.revokeObjectURL(blobUrl);
                        // }, 0);
                        // Had to comment that out as it was removed from the API
                        // Todo: Work out alternative solution
                        // Info: Was replaced with reportApi.reportPublish
                        // Todo: Must still be adapted to the new API
                        // await documentApi.documentStoreForm(
                        /* await reportApi.reportPublish(
                            storeId,
                            uploadedFile
                        );*/
                        file.closeAsync();
                    } catch {
                        file.closeAsync();

                        pushNotification(
                            `Flowchart could not be saved.`,
                            NotificationType.DANGER
                        );
                    }
                }
            } else {
                // console.log('data slices error');
                gotAllSlices = false;
                file.closeAsync();
            }
        });
    };

    return {
        isSavingFlowchart,
        saveFlowchart,
        getCurrentFlowchart,
        hasUpdate,
        saveAsTemplate
    };
}

export default useFlowchart;
