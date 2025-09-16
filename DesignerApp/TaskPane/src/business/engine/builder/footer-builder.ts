import { FooterDefinition } from '@omniflow/omni-webapi';

import { BuildEngine } from '../models/build-engine';
import { Block } from '../models/block';
import FooterBlock from '../blocks/footer/footer-block';
import { StyleMapper } from '../models/styles';

export interface IFooterBuilderProps {
    clientId: string;
    FooterDefinition?: FooterDefinition;
    templateColumnLength?: number;
}

export class FooterBuilder extends BuildEngine {
    clientId: string;
    FooterDefinition: FooterDefinition;

    templateColumnLength: number = 20;
    footerColumnLength: number = 0;
    footerRowLength: number = 0;

    constructor({
        clientId,
        FooterDefinition,
        templateColumnLength,
    }: IFooterBuilderProps) {
        super();

        this.clientId = clientId;
        this.FooterDefinition = FooterDefinition;

        if (templateColumnLength > 0) {
            this.templateColumnLength = templateColumnLength;
        }
    }

    build = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];

        if (this.FooterDefinition) {
            rows = await this.buildFooter();
        }

        return rows;
    };

    buildFooter = async (): Promise<Block[][]> => {
        const rows: Block[][] = [];
        const padBeforeFooter = 3;

        let index = 0;
        this.FooterDefinition.Configuration.Rows.forEach(
            ({ Text, Styling }) => {
                let pad = 0;

                // First footnote needs some padding
                if (index === 0) pad = padBeforeFooter;

                const footerBlock = new FooterBlock({
                    rowStart: pad,
                    value: Text,
                    columnLength: this.templateColumnLength,
                    autofitColumns: false,
                });
                footerBlock.getDirectStyles = () =>
                    StyleMapper.mapStyleToExcelStyle(Styling, false);
                rows.push([footerBlock]);

                index++;
            }
        );

        this.footerColumnLength = this.getColumnLength(rows);
        this.footerRowLength = this.getRowLength(rows) + padBeforeFooter;

        return rows;
    };
}
