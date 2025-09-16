import {
    ReportPublishDTO,
    SaveFlowchartDTO,
    HeaderData,
    RunRestriction,
    FileType,
} from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';
import { useContext } from 'react';
import useNotification, {
    NotificationType,
} from '../components/notification/useNotification';
import { flowchartTemplateApi, reportApi } from '../lib/api';
import { AppContext } from '../taskpane/contexts/AppContext';
import { isEmpty } from 'lodash';
import Tools from '../business/tools';

interface IUsePublishProps {
    headerData: HeaderData;
    runResctrictions: RunRestriction[];
    reportName: string;
}

const usePublish = ({
    headerData,
    runResctrictions,
    reportName,
}: IUsePublishProps) => {
    const {
        flowchartTemplateDefinition,
        currentMediaHierarchyDetails,
        runConfigurationId,
        setFlowchartTemplateDefinition
    } = useContext(AppContext);
    const pushNotification = useNotification();
    let runId = '';
    const generateExcelFileName = (flowchartTemplateName, reportName) => {
        let baseName = generateReportDisplayName(
            flowchartTemplateName,
            reportName
        );
        return baseName + '.xlsx';
    };

    const generateReportDisplayName = (flowchartTemplateName, reportName) => {
        flowchartTemplateName.replace(' ', '');
        reportName.replace(' ', '');
        return flowchartTemplateName + '_' + reportName;
    };
    const publishDocumentAsTemplate = async (saveFlowchartData: SaveFlowchartDTO) => {
        runId = Tools.uuid();
        let fileName = generateExcelFileName(
            flowchartTemplateDefinition.Name,
            reportName
        );

        await Excel.run(async (context) => {
            context.workbook.load('name');
            await context.sync();
            fileName = context.workbook.name;
        });

        await Office.context.document.getFileAsync(
            Office.FileType.Compressed,
            async function (result) {
                if (result.status == Office.AsyncResultStatus.Succeeded) {
                    const file = await result.value;

                    const sliceCount = file.sliceCount;
                    const docdataSlices = [];
                    let slicesReceived = 0,
                        gotAllSlices = true;

                    // Get the file slices.
                    _getSliceAsync(
                        file,
                        0,
                        sliceCount,
                        gotAllSlices,
                        docdataSlices,
                        slicesReceived,
                        fileName,
                        saveFlowchartData.Comments,
                        saveFlowchartData.TrackFormatChanges,
                        saveFlowchartData.CustomStyleSettings,
                        saveFlowchartData
                    );
                } else {
                    pushNotification(
                        `Error during file content extraction!`,
                        NotificationType.DANGER
                    );
                }
            }
        );

    }
    const publishDocument = async (id?: string, comments?: string, trackFormatting?: boolean, customStyles?: string) => {
        runId = id;
        let fileName = generateExcelFileName(
            flowchartTemplateDefinition.Name,
            reportName
        );

        await Excel.run(async (context) => {
            context.workbook.load('name');
            await context.sync();
            fileName = context.workbook.name;
        });

        await Office.context.document.getFileAsync(
            Office.FileType.Compressed,
            async function (result) {
                if (result.status == Office.AsyncResultStatus.Succeeded) {
                    const file = await result.value;

                    const sliceCount = file.sliceCount;
                    const docdataSlices = [];
                    let slicesReceived = 0,
                        gotAllSlices = true;

                    // Get the file slices.

                    _getSliceAsync(
                        file,
                        0,
                        sliceCount,
                        gotAllSlices,
                        docdataSlices,
                        slicesReceived,
                        fileName,
                        comments,
                        trackFormatting,
                        customStyles,
                        
                    );
                } else {
                    pushNotification(
                        `Error during file content extraction!`,
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
        fileName,
        comments,
        trackFormatting,
        customStyles,
        saveFlowchartData?: SaveFlowchartDTO
    ) => {
        file.getSliceAsync(nextSlice, async function (sliceResult) {
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

                        // generate file
                        const _file = new File([blob], fileName, {
                            type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
                        });

                        // download file
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

                        let base64Content = (await _toBase64(_file)) as string;
                        base64Content = base64Content.split(',')[1];
                        if (saveFlowchartData) {
                            _publishAsTemplate(base64Content, saveFlowchartData)
                        }
                        else {
                            _publish(base64Content, comments, trackFormatting, customStyles);
                        }
                        
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

    const _toBase64 = (file) =>
        new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => resolve(reader.result);
            reader.onerror = (error) => reject(error);
        });
    const _publishAsTemplate = async (fileContent: any, saveFlowchartData: SaveFlowchartDTO) => {
        const mediaHierarchyLevels =
            currentMediaHierarchyDetails?.Definition?.Levels ??
            flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition
                ?.Definition?.Levels;
        if (!runId && !runConfigurationId) return;
        saveFlowchartData.ReportPublish = plainToClass(ReportPublishDTO, {
            Data: {
                // FlowchartData: flowchartTemplateDefinition.Definition,
                HeaderData: headerData,
            },
            RunConfigurationId: !runId ? runConfigurationId : runId,
            RunConfigurationData: {
                MediaHierarchyLevels: mediaHierarchyLevels,
                RunRestrictions: runResctrictions ?? [],
            },
            Name: generateReportDisplayName(
                saveFlowchartData.TemplateName,
                saveFlowchartData.TemplateName
            ),
            FileContent: fileContent,
            FileType: FileType.Excel,
            ...(!isEmpty(saveFlowchartData.Comments) ? { Comments: saveFlowchartData.Comments } : {}), ...(!isEmpty(saveFlowchartData.TrackFormatChanges) ? { TrackFormattingStyles: saveFlowchartData.TrackFormatChanges } : {}),
            ...(!isEmpty(saveFlowchartData.CustomStyleSettings) ? { CustomStyleSettings: saveFlowchartData.CustomStyleSettings } : {})
        });
        await flowchartTemplateApi.flowchartTemplateSaveAs(saveFlowchartData).then(async (x) => {
            pushNotification(
                `New Template with Download and Report created  successfully!!`,
                NotificationType.SUCCESS
            );
        })
            .catch((err) => {
                console.log(err);
                pushNotification(
                    `Error while creating new template!`,
                    NotificationType.DANGER
                );
            });
    }
    const _publish = async (fileContent: any, comments?: string, trackFormatting?: boolean, customStyles?: string) => {
        const mediaHierarchyLevels =
            currentMediaHierarchyDetails?.Definition?.Levels ??
            flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition
                ?.Definition?.Levels;
        if (!runId && !runConfigurationId) return;
        const publishData = plainToClass(ReportPublishDTO, {
            Data: {
                // FlowchartData: flowchartTemplateDefinition.Definition,
                HeaderData: headerData,
            },
            RunConfigurationId: !runId ? runConfigurationId : runId,
            RunConfigurationData: {
                MediaHierarchyLevels: mediaHierarchyLevels,
                RunRestrictions: runResctrictions ?? [],
            },
            Name: generateReportDisplayName(
                flowchartTemplateDefinition.Name,
                reportName
            ),
            FileContent: fileContent,
            FileType: FileType.Excel,
            ...(!isEmpty(comments) ? { Comments: comments } : {}), ...(!isEmpty(trackFormatting) ? { TrackFormattingStyles: trackFormatting } : {}),
            ...(!isEmpty(customStyles) ? { CustomStyleSettings: customStyles } : {})
        });
        await reportApi
            .reportPublish(publishData)
            .then(async (x) => {
                pushNotification(
                    `Report published successfully!`,
                    NotificationType.SUCCESS
                );
            })
            .catch((err) => {
                console.log(err);
                pushNotification(
                    `Error while publishing report!`,
                    NotificationType.DANGER
                );
            });
    };

    return {
        publishDocument,
        publishDocumentAsTemplate
    };
};

export default usePublish;
;