import { IBlock } from '../interfaces/block.interface';
import { Styles } from './styles';

export interface IBlockProps {
    value?: any;
    columnStart?: number;
    rowStart?: number;
    columnLength?: number;
    rowLength?: number;
    columnAfter?: number;
    rowAfter?: number;
    merge?: boolean;
    containsImage?: boolean;
    imageMaxHeight?: number;
    autofitColumns?: boolean;
    isMetricNone?: boolean;
    legendColumn?: string;
}

export class Block implements IBlock {
    value;
    columnStart;
    rowStart;
    columnLength;
    rowLength;
    columnAfter;
    rowAfter;
    merge;
    containsImage;
    imageMaxHeight;
    autofitColumns;
    styles: Styles = {
        wrapText: false,
    };
    isMetricNone;
    legendColumn;
    constructor({
        value = '',
        columnStart = 0,
        rowStart = 0,
        columnLength = 1,
        rowLength = 1,
        columnAfter = 0,
        rowAfter = 0,
        merge = true,
        containsImage = false,
        imageMaxHeight,
        autofitColumns = true,
        isMetricNone = false,
        legendColumn = '',

    }: IBlockProps = {}) {
        this.value = value;
        this.columnStart = columnStart;
        this.rowStart = rowStart;
        this.columnLength = columnLength;
        this.rowLength = rowLength;
        this.columnAfter = columnAfter;
        this.rowAfter = rowAfter;
        this.merge = merge;
        this.containsImage = containsImage;
        this.imageMaxHeight = imageMaxHeight;
        this.autofitColumns = autofitColumns;
        this.isMetricNone = isMetricNone;
        this.legendColumn = legendColumn;
    }

    getValue = (columnIndex: number = 0, rowIndex: number = 0): string =>
        columnIndex === 0 && rowIndex === 0 ? `${this.value}` : '';

    getStyles = (columnIndex: number = 0, rowIndex: number = 0): Styles => {
        return this.styles;
    };
    getGeneralStyles = () => null;

    getComponentStyles = () => null;

    getDirectStyles = () => null;

    getLegendStyles = () => null;
}
