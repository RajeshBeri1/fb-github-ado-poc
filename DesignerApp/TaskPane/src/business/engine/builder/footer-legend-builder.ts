import {
    FooterDefinition,
    MediaHierarchyDefinition,
    Styling,
} from '@omniflow/omni-webapi';

import { BuildEngine } from '../models/build-engine';
import { Block } from '../models/block';
import { TLegendData } from './media-hierarchy-builder';
import { StyleMapper } from '../models/styles';

export interface IFooterLegendBuilderProps {
    clientId: string;
    FooterDefinition?: FooterDefinition;
    templateColumnLength?: number;
    legendData?: TLegendData;
    MediaHierarchyDefinition?: MediaHierarchyDefinition;
}

export class FooterLegendBuilder extends BuildEngine {
    clientId: string;
    FooterDefinition: FooterDefinition;
    MediaHierarchyDefinition: MediaHierarchyDefinition;

    templateColumnLength: number = 20;
    footerLegendColumnLength: number = 0;
    footerLegendRowLength: number = 0;

    legendData: TLegendData;

    constructor({
        clientId,
        FooterDefinition,
        templateColumnLength,
        legendData,
        MediaHierarchyDefinition,
    }: IFooterLegendBuilderProps) {
        super();

        this.clientId = clientId;
        this.FooterDefinition = FooterDefinition;
        this.MediaHierarchyDefinition = MediaHierarchyDefinition;

        if (templateColumnLength > 0) {
            this.templateColumnLength = templateColumnLength;
        }
        this.legendData = legendData;
    }

    build = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];

        if (this.FooterDefinition) {
            rows = await this.buildFooterLegend();
        }

        return rows;
    };

    buildFooterLegend = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];
        let padBeforeLegend = 2;
        let padAdded = false;

        const config = this.FooterDefinition.Configuration;
        if (config.EnableLegend && this.legendData) {
            // Split legend array into smaller sizes (based on user chosen LegendColumnCount)
            const splitLegendData = this.legendData.reduce(
                (resultArray, item, index) => {
                    const chunkIndex = Math.floor(
                        index / config.LegendColumnCount
                    );

                    if (!resultArray[chunkIndex]) {
                        resultArray[chunkIndex] = []; // start a new chunk
                    }

                    resultArray[chunkIndex].push(item);

                    return resultArray;
                },
                []
            );

            // Create blocks
            let rowindex = 0;
            const legendRows = splitLegendData.map((legendData) => {
                let pad = 0;

                return legendData.map(({ name, color }) => {
                    if (rowindex === 0) {
                        pad = padBeforeLegend;
                        padAdded = true;
                    }

                    const legendBlock = new Block({
                        rowStart: pad,
                        value: name,
                        columnLength: 4,
                        autofitColumns: false,
                    });

                    // Map styling from media hierarchy values to legend
                    if (this.MediaHierarchyDefinition) {
                        this.MediaHierarchyDefinition.Levels?.forEach(
                            (level, levelIndex) => {
                                level.Settings?.forEach(
                                    (setting, settingsIndex) => {
                                        if (
                                            setting.Name === legendBlock.value
                                        ) {
                                            legendBlock.getDirectStyles = () =>
                                                StyleMapper.mapStyleToExcelStyle(
                                                    setting.Styling ??
                                                        ({} as Styling),
                                                    false
                                                );
                                        }
                                        if (setting.SubLevels?.length > 0) {
                                            setting.SubLevels.forEach(
                                                (subLevel) => {
                                                    if (
                                                        subLevel.ColumnName ===
                                                        legendBlock.value
                                                    ) {
                                                        legendBlock.getDirectStyles =
                                                            () =>
                                                                StyleMapper.mapStyleToExcelStyle(
                                                                    subLevel.Styling ??
                                                                        ({} as Styling),
                                                                    false
                                                                );
                                                    }
                                                    if (
                                                        subLevel.Settings
                                                            ?.length > 0
                                                    ) {
                                                        subLevel.Settings.forEach(
                                                            (
                                                                subLevelSetting
                                                            ) => {
                                                                if (
                                                                    subLevelSetting.Name ===
                                                                    legendBlock.value
                                                                ) {
                                                                    legendBlock.getDirectStyles =
                                                                        () =>
                                                                            StyleMapper.mapStyleToExcelStyle(
                                                                                subLevelSetting.Styling ??
                                                                                    ({} as Styling),
                                                                                false
                                                                            );
                                                                }
                                                            }
                                                        );
                                                    }
                                                }
                                            );
                                        }
                                    }
                                );
                            }
                        );
                    }
                    rowindex++;

                    return legendBlock;
                });
            });
            rows = legendRows;
        }

        this.footerLegendColumnLength = this.getColumnLength(rows);
        this.footerLegendRowLength = padAdded
            ? this.getRowLength(rows) + padBeforeLegend
            : this.getRowLength(rows);

        return rows;
    };
}
