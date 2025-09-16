import { HeaderDefinition } from '@omniflow/omni-webapi';

import { BuildEngine } from '../models/build-engine';
import { Block } from '../models/block';
import * as HeaderHelper from '../helpers/header-helper';
import HeaderDetailsBlock from '../blocks/header/header-details-block';
import { StyleMapper } from '../models/styles';
import { THeaderRowData } from '../helpers/header-helper';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';

export interface IHeaderDetailsBuilderProps {
    clientId: string;
    HeaderDefinition?: HeaderDefinition;
    headerRowData?: THeaderRowData;
    headerDetailsLength?: number;
    dataColumns?: TColumn[];
}

export class HeaderDetailsBuilder extends BuildEngine {
    clientId: string;
    HeaderDefinition: HeaderDefinition;
    headerRowData: THeaderRowData;

    headerColumnLength: number = 0;
    headerRowLength: number = 0;

    headerDetailsLength: number = 10;

    dataColumns: TColumn[] = [];

    constructor({
        clientId,
        HeaderDefinition,
        headerRowData,
        headerDetailsLength,
        dataColumns,
    }: IHeaderDetailsBuilderProps) {
        super();

        this.clientId = clientId;
        this.HeaderDefinition = HeaderDefinition;
        this.headerRowData = headerRowData;

        if (headerDetailsLength > 0) {
            this.headerDetailsLength = headerDetailsLength;
        }
        if (dataColumns) {
            this.dataColumns = dataColumns;
        }
    }

    build = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];
        if (
            this.HeaderDefinition?.Configuration?.Details?.Rows.length &&
            this.headerRowData
        ) {
            rows = await this.buildHeaderDetails();
        }

        return rows;
    };

    buildHeaderDetails = async (): Promise<Block[][]> => {
        const { rowDetailValues } = this.headerRowData;

        let rows: Block[][] = [];
        await this.HeaderDefinition.Configuration.Details.Rows.forEach(
            (row) => {
                const formattedValues = rowDetailValues
                    ? HeaderHelper.findAndFormatValues(
                          row,
                          rowDetailValues,
                          this.dataColumns
                      ) + ''
                    : 'Placeholder until Calendar and Media Hierarchy are defined.';

                const titleLength = Math.floor(this.headerDetailsLength / 4);
                const blocks = [
                    new HeaderDetailsBlock({
                        value: row.Text,
                        columnLength: Math.floor(this.headerDetailsLength / 4),
                    }),
                    new HeaderDetailsBlock({
                        value: formattedValues,
                        columnLength: this.headerDetailsLength - titleLength,
                    }),
                ];
                blocks.map(
                    (block) =>
                        (block.getDirectStyles = () =>
                            StyleMapper.mapStyleToExcelStyle(
                                row.Styling,
                                false
                            ))
                );

                rows.push(blocks);
            }
        );

        this.headerColumnLength = this.getColumnLength(rows);
        this.headerRowLength = this.getRowLength(rows);

        return rows;
    };
}
