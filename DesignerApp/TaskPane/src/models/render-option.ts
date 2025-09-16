export class RenderOptions {
    columnWidth?: number;
    rowHeight?: number;
    background?: string = '#ffffff';
    borders?: {
        bottom?: {
            style: Excel.BorderLineStyle;
        };
        left?: {
            style?: Excel.BorderLineStyle;
        };
        right?: {
            style?: Excel.BorderLineStyle;
        };
        top?: {
            style?: Excel.BorderLineStyle;
        };
    };
    fill?: {
        color?: string;
        alternate?: boolean;
    };
    horizontalAlignment?: Excel.HorizontalAlignment;
    font?: {
        color?: string;
        bold?: boolean;
    };
}
