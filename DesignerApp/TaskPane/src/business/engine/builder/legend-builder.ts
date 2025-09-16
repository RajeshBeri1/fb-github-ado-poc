import {
    ReferencedThemeDefinition,
    ReferencedMediaHierarchyDefinition,
    ReferencedCalendarDefinition,
    ThemeDefinition,
    Styling
} from '@omniflow/omni-webapi';
import { BuildEngine } from '../models/build-engine';
import { Block } from '../models/block';
import { StyleMapper } from '../models/styles';
import LegendBlock from '../blocks/theme/legend-block';
import { plainToClass } from 'class-transformer';
export interface ILegendBuilderProps {
    clientId: string;
    ReferencedThemeDefinition?: ReferencedThemeDefinition;
    ReferencedMediaHierarchyDefinition?: ReferencedMediaHierarchyDefinition;
    ReferencedCalendarDefinition?: ReferencedCalendarDefinition;
    leftMenuColumnLength: number;
    ThemeDefinition?: ThemeDefinition;
    calendarFullLength: number;
    rightHandTotalsColumnLength: number;
}

export class LegendBuilder extends BuildEngine {
    clientId: string;
    ReferencedThemeDefinition?: ReferencedThemeDefinition;
    ReferencedMediaHierarchyDefinition?: ReferencedMediaHierarchyDefinition;
    ReferencedCalendarDefinition?: ReferencedCalendarDefinition;
    ThemeDefinition?: ThemeDefinition;
    leftMenuColumnLength: number = 0;
    columnLength: number = 4;
    legendColumnLength: number = 0;
    legendRowLength: number = 0;
    calendarFullLength: number = 0;
    rightHandTotalsColumnLength: number = 0;
    constructor({
        clientId,
        ReferencedThemeDefinition,
        ReferencedCalendarDefinition,
        ReferencedMediaHierarchyDefinition,
        leftMenuColumnLength,
        ThemeDefinition,
        calendarFullLength,
        rightHandTotalsColumnLength,
    }: ILegendBuilderProps) {
        super();

        this.clientId = clientId;
        this.ReferencedThemeDefinition = ReferencedThemeDefinition;
        this.leftMenuColumnLength = leftMenuColumnLength;
        this.ReferencedCalendarDefinition = ReferencedCalendarDefinition;
        this.ReferencedMediaHierarchyDefinition = ReferencedMediaHierarchyDefinition;
        this.ThemeDefinition = ThemeDefinition;
        this.calendarFullLength = calendarFullLength;
        this.rightHandTotalsColumnLength = rightHandTotalsColumnLength;
    }

    build = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];
        if (
            this.ReferencedMediaHierarchyDefinition?.Definition &&
            this.ReferencedCalendarDefinition?.Definition &&
            this.ReferencedThemeDefinition?.Definition &&
            this.ThemeDefinition?.LegendTheme?.Settings?.filter(s => s?.Enabled)?.length > 0
        ) {
            rows = await this.buildLegends();
        }
        return rows;
    };

    buildLegends = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];
        let pad = 1;
        let isVertical = this.ThemeDefinition?.LegendTheme?.DisplayVertically;
        let legendRows = this.ThemeDefinition?.LegendTheme.Settings.filter(s => s.Enabled).map(
            ({ Name, Styling }, index) => {
                const legendBlock = new LegendBlock({
                    rowStart: isVertical ? (index == 0 ? pad : 0) : index < Math.floor(this.calendarFullLength / this.columnLength) ? pad : 0,
                    value: `'${Name}`,
                    columnLength: this.columnLength,
                    autofitColumns: false,
                });
                legendBlock.getDirectStyles = () =>
                    StyleMapper.mapStyleToExcelStyle(Styling ?? {} as Styling, false);
                return legendBlock;
            }
        ) || [];
        // Left space for menu
        const leftRows: Block[] = [];
        const leftRowsWithoutRowStart: Block[] = [];
        if (this.leftMenuColumnLength > 0) {
            Array.from({ length: this.leftMenuColumnLength }, () => {
                leftRows.push(new LegendBlock({
                    rowStart: pad,
                    columnLength: 1,
                    autofitColumns: false,
                }));
                leftRowsWithoutRowStart.push(new LegendBlock({
                    rowStart: 0,
                    columnLength: 1,
                    autofitColumns: false,
                }));
            })
        }
        // if horizontal
        if (!isVertical) {
            const numberOfLegendRows = this.calendarFullLength > 0 ? Math.ceil((this.ThemeDefinition?.LegendTheme.Settings.filter(s => s.Enabled).length) / (this.calendarFullLength / this.columnLength)) : 1;
            Array.from({ length: numberOfLegendRows }, (_, index) => {
                const row = legendRows.slice(index * Math.floor(this.calendarFullLength / this.columnLength), (index + 1) * Math.floor(this.calendarFullLength / this.columnLength));
                const spaceRows = index == 0 ? leftRows : leftRowsWithoutRowStart;
                //const updatedRow = index == 0 ? row.map((block, index) => { return plainToClass(LegendBlock, { ...block, rowStart: pad }) as LegendBlock }) : row;
                rows.push([...spaceRows, ...row]);
            });
        }
        else {  // if vertical
            legendRows.forEach((legendRow, index) => {
                const spaceRows = index == 0 ? leftRows : leftRowsWithoutRowStart;
                //const updatedRow = index == 0 ? plainToClass(LegendBlock, { ...legendRow, rowStart: pad }) as LegendBlock : legendRow;
                rows.push([...spaceRows, legendRow]);
            });
        }
        this.legendColumnLength = this.getColumnLength(rows);
        this.legendRowLength = this.getRowLength(rows);
        return rows;
    };
}
