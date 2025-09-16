import {
    HeaderDefinition,
    HeaderLogoAlignment,
    HeaderRowType,
} from '@omniflow/omni-webapi';
import moment from 'moment';

import { BuildEngine } from '../models/build-engine';
import { Block } from '../models/block';
import * as HeaderHelper from '../helpers/header-helper';
import { DateFormat } from '../../../enums/dateFormat.enum';
import { StyleMapper } from '../models/styles';
import { THeaderRowData } from '../helpers/header-helper';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import HeaderDetailsBlock from '../blocks/header/header-details-block';

export interface IHeaderBuilderProps {
    clientId: string;
    HeaderDefinition?: HeaderDefinition;
    headerRowData?: THeaderRowData;
    headerColumns?: number;
    logoLeftColumns?: number;
    logoRightColumns?: number;
    logoCenterColumns?: number;
    dataColumns?: TColumn[];
}

export class HeaderBuilder extends BuildEngine {
    clientId: string;
    HeaderDefinition: HeaderDefinition;
    headerRowData: THeaderRowData;

    headerColumnLength: number = 0;
    headerRowLength: number = 0;
    headerColumns: number = 20;

    logoLeftColumns: number = 0;
    logoRightColumns: number = 0;
    logoCenterColumns: number = 0;

    dataColumns: TColumn[] = [];

    constructor({
        clientId,
        HeaderDefinition,
        headerRowData,
        headerColumns,
        logoLeftColumns,
        logoRightColumns,
        logoCenterColumns,
        dataColumns,
    }: IHeaderBuilderProps) {
        super();

        this.clientId = clientId;
        this.HeaderDefinition = HeaderDefinition;
        this.headerRowData = headerRowData;

        if (logoLeftColumns > 0) {
            this.logoLeftColumns = logoLeftColumns;
        }
        if (logoRightColumns > 0) {
            this.logoRightColumns = logoRightColumns;
        }
        if (logoCenterColumns > 0) {
            this.logoCenterColumns = logoCenterColumns;
        }
        if (headerColumns > 0) {
            this.headerColumns = headerColumns;
        }
        if (dataColumns) {
            this.dataColumns = dataColumns;
        }
    }

    build = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];

        if (this.HeaderDefinition?.Configuration?.Rows && this.headerRowData) {
            rows = await this.buildHeader();
        }

        return rows;
    };

    buildHeader = async (): Promise<Block[][]> => {
        const { rowValues } = this.headerRowData;
        const headerConfig = this.HeaderDefinition.Configuration;

        let logoLeft,
        logoCenter,
            logoRight = null;
        if (headerConfig.Logos && headerConfig.Logos.length) {
            headerConfig.Logos.forEach(logo => {
                switch (logo.Alignment) {
                    case HeaderLogoAlignment.Right:
                        logoRight = {
                            content: logo.Image.Content, width: logo.Width|4, height: logo.Height|4
                        };
                        break;
                    case HeaderLogoAlignment.Left:
                        logoLeft = {
                            content: logo.Image.Content, width: logo.Width | 4, height: logo.Height | 4
                        };
                        break;
                    case HeaderLogoAlignment.Center:
                        logoCenter = {
                            content: logo.Image.Content, width: logo.Width | 4, height: logo.Height | 4
                        };
                        break;
                    default:
                        console.log('No alignment')

                }
            });
            //logoLeft = headerConfig.Logos[0].Image.Content;
            //logoRight = headerConfig.Logos[1]?.Image?.Content;
            //logoCenter = headerConfig.Logos[2]?.Image?.Content;
            //if (headerConfig.Logos[0].Alignment === HeaderLogoAlignment.Right) {
            //    logoRight = headerConfig.Logos[0].Image.Content;
            //    logoLeft = headerConfig.Logos[1]?.Image?.Content;
            //    logoCenter = headerConfig.Logos[2]?.Image?.Content;
            // //  if (headerConfig.Logos[2].Alignment === HeaderLogoAlignment.Center) {
            //  //    logoCenter = headerConfig.Logos[2].Image?.Content;
            //     // logoLeft = headerConfig.Logos[1]?.Image?.Content;
            //    //  logoRight = headerConfig.Logos[0]?.Image.Content;
            //   //}
            //}
        }

        let rows: Block[][] = [];
        headerConfig.Rows.forEach((row, index) => {
            let value = null;
            switch (row.Type) {
                case HeaderRowType.Text:
                    value = row.Text;
                    break;
                case HeaderRowType.Date:
                    const format =
                        DateFormat[row.DateFormat] ?? DateFormat.Default;
                    value = `'${moment.utc(row.Date).format(format)}`;
                    break;
                case HeaderRowType.Value:
                    value = rowValues
                        ? HeaderHelper.findAndFormatValues(
                            row,
                            rowValues,
                            this.dataColumns
                        ) + ''
                        : 'Placeholder until Calendar and Media Hierarchy are defined.';
                    break;
                case HeaderRowType.Blank:
                    value = ' ';
                    break;
            }

            let block = HeaderHelper.getHeaderBlock(value, this.headerColumns);
            let headerContentBlock = null;

            if (logoLeft || logoRight || logoCenter) {
                const defaultWidth = 6;
               
                this.logoLeftColumns =
                    logoLeft && this.logoLeftColumns === 0
                        ? defaultWidth 
                        : this.logoLeftColumns;
                this.logoRightColumns =
                    logoRight && this.logoRightColumns === 0
                        ? defaultWidth
                        : this.logoRightColumns;
                this.logoCenterColumns =
                    logoCenter && this.logoCenterColumns === 0
                        ? defaultWidth
                        : this.logoCenterColumns;

                if (index === 0) {
                    const detailRows = headerConfig.Details?.Rows?.length;
                    let logoHeight = 2;
                    if (detailRows) {
                     logoHeight += detailRows;
                      }

                    block = HeaderHelper.getHeaderBlockWithLogo({
                        value,
                        length: this.headerColumns,
                        logoLeft: JSON.stringify(logoLeft),
                        logoLeftColumns: this.logoLeftColumns,
                        logoRight: logoRight!=null? JSON.stringify(logoRight):null,
                        logoRightColumns: this.logoRightColumns,
                        logoCenter: JSON.stringify(logoCenter),
                        logoCenterColumns: this.logoCenterColumns,
                        logoHeight,
                    });
                    if (logoCenter) {
                        headerContentBlock = HeaderHelper.getHeaderBlockWithLogo({
                            value,
                            length: this.headerColumns,
                            logoLeft: null,
                            logoLeftColumns: this.logoLeftColumns,
                            logoRight: null,
                            logoRightColumns: this.logoRightColumns,
                            logoCenter: null,
                            logoCenterColumns: this.logoCenterColumns,
                            logoHeight: null,
                        });
                    }
                } else {
                    block = HeaderHelper.getHeaderBlock(
                        value,
                        this.headerColumns,
                        this.logoLeftColumns
                    );
                }
            }

            rows.push(block);
            if (index == 0 && headerContentBlock) {
                rows.push([new HeaderDetailsBlock({ columnStart: 1, columnLength:0 })]);
                rows.push(headerContentBlock);
            }
            

            [block, ...(headerContentBlock ? [headerContentBlock] : [])].forEach((blocks) => {
                blocks.forEach((x) => {
                    x.getDirectStyles = () => {
                        if (!x.containsImage) {
                            return StyleMapper.mapStyleToExcelStyle(row.Styling, false);
                        }
                        return null;
                    };
                });
            });
        });

        this.headerColumnLength = this.getColumnLength(rows);
        this.headerRowLength = this.getRowLength(rows);

        return rows;
    };
}
