import { Styles } from '../models/styles';

export interface IBlock {
    value: any;
    columnStart: number;
    rowStart: number;
    columnLength: number;
    rowLength: number;
    columnAfter: number;
    rowAfter: number;
    merge: boolean;
    containsImage: boolean;
    styles: Styles;
    getValue: () => string;
    getStyles: (index?: number) => Styles;
    getGeneralStyles: () => Styles;
    getComponentStyles: () => Styles;
    legendColumn: string;
    getLegendStyles: () => Styles;
}
