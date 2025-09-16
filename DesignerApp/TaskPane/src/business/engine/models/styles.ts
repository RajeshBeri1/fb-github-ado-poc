import {
    BorderLineStyle,
    RangeUnderlineStyle,
    HorizontalAlignment,
    VerticalAlignment,
    Styling,
    FontWeight,
} from '@omniflow/omni-webapi';
import { FontFamilyType } from '../../../enums/font-family.enum';
import Tools from '../../tools';

export class Styles {
    columnWidth?: number;
    rowHeight?: number;
    font?: {
        color?: string;
        bold?: boolean;
        underline?: RangeUnderlineStyle;
        italic?: boolean;
        name?: string;
        size?: number;
    };
    horizontalAlignment?: HorizontalAlignment;
    verticalAlignment?: VerticalAlignment;
    wrapText?: boolean;
    fill?: {
        color?: string;
        alternate?: boolean;
    };
    borders?: {
        top?: {
            style?: BorderLineStyle;
            color?: string;
        };
        right?: {
            style?: BorderLineStyle;
            color?: string;
        };
        bottom?: {
            style: BorderLineStyle;
            color?: string;
        };
        left?: {
            style?: BorderLineStyle;
            color?: string;
        };
    };
}

export class StyleMapper {
    private static defaultColorPrimary = '#000';
    private static defaultColorSeconary = '#fff';

    static mapStyleToExcelStyle(style: Styling, setDefaults = true): Styles {
        let bold = !setDefaults ? null : false;
        let italic = !setDefaults ? null : false;

        switch (style?.Font?.Weight) {
            case FontWeight.Bold:
                bold = true;
                break;
            case FontWeight.BoldItalic:
                bold = true;
                italic = true;
                break;           
            case FontWeight.Italic:
                italic = true;
                break;
        }

        const s: Styles = {
            // columnWidth: 15,
            // rowHeight: 15,
            font: {
                color: Tools.rgbColorToHex(style?.Font?.Color),
                bold: bold,
                italic: italic,
                size: style?.Font?.Size || (!setDefaults ? null : 14),
                name:
                    style?.Font?.Family ||
                    (!setDefaults ? null : FontFamilyType.Calibri),
               underline:
                    style?.Font?.Underline ||
                    (!setDefaults ? null : RangeUnderlineStyle.None),
            },
            horizontalAlignment:
                style?.Alignment?.Horizontal ||
                (!setDefaults ? null : HorizontalAlignment.Center),
            verticalAlignment:
                style?.Alignment?.Vertical ||
                (!setDefaults ? null : VerticalAlignment.Center),
            fill: {
                color:
                    Tools.rgbColorToHex(style?.Font?.Background) ||
                    (!setDefaults ? null : this.defaultColorSeconary),
                alternate: !setDefaults ? null : false,
                // pattern: Excel.FillPattern.vertical,
            },
            borders: {
                top: {
                    style:
                        style?.Border?.Top?.Style ||
                        (!setDefaults ? null : BorderLineStyle.Continuous),
                    color:
                        Tools.rgbColorToHex(style?.Border?.Top?.Color) ??
                        (!setDefaults ? null : this.defaultColorPrimary),
                },
                right: {
                    style:
                        style?.Border?.Right?.Style ||
                        (!setDefaults ? null : BorderLineStyle.Continuous),
                    color:
                        Tools.rgbColorToHex(style?.Border?.Right?.Color) ??
                        (!setDefaults ? null : this.defaultColorPrimary),
                },
                bottom: {
                    style:
                        style?.Border?.Bottom?.Style ||
                        (!setDefaults ? null : BorderLineStyle.Continuous),
                    color:
                        Tools.rgbColorToHex(style?.Border?.Bottom?.Color) ??
                        (!setDefaults ? null : this.defaultColorPrimary),
                },
                left: {
                    style:
                        style?.Border?.Left?.Style ||
                        (!setDefaults ? null : BorderLineStyle.Continuous),
                    color:
                        Tools.rgbColorToHex(style?.Border?.Left?.Color) ??
                        (!setDefaults ? null : this.defaultColorPrimary),
                },
            },
        };
        return s;
    }

    static mergeStyle(currentStyle: Styles, newStyle: Styles): Styles {
        return Tools.mergeDeep<Styles>(currentStyle, newStyle);
    }
}

const defaultColor = (inherit: boolean) => {
    return {
        Alpha: 1,
        Blue: inherit ? -1 : 0,
        Green: inherit ? -1 : 0,
        Red: inherit ? -1 : 0,
    };
};
const defaultBackground = (inherit: boolean) => {
    return {
        Alpha: 1,
        Blue: inherit ? -1 : 255,
        Green: inherit ? -1 : 255,
        Red: inherit ? -1 : 255,
    };
};

export const CalendarStyling = (inherit: boolean = false) => {
    return {
        Alignment: {
            Horizontal: HorizontalAlignment.Center,
            Vertical: VerticalAlignment.Center,
        },
        //Border: {
        //    Bottom: {
        //        Style: BorderLineStyle.None,
        //        Color: defaultColor(inherit),
        //    },
        //    Left: {
        //        Style: BorderLineStyle.None,
        //        Color: defaultColor(inherit),
        //    },
        //    Right: {
        //        Style: BorderLineStyle.None,
        //        Color: defaultColor(inherit),
        //    },
        //    Top: {
        //        Style: BorderLineStyle.None,
        //        Color: defaultColor(inherit),
        //    }
        //}
    }
}
        


export const DefaultStyling = (inherit: boolean = true) => {
    return {
        Border: {
            Bottom: {
                Style: inherit ? null : BorderLineStyle.Continuous,
                Color: defaultColor(inherit),
            },
            Left: {
                Style: inherit ? null : BorderLineStyle.Continuous,
                Color: defaultColor(inherit),
            },
            Right: {
                Style: inherit ? null : BorderLineStyle.Continuous,
                Color: defaultColor(inherit),
            },
            Top: {
                Style: inherit ? null : BorderLineStyle.Continuous,
                Color: defaultColor(inherit),
            },
        },
        Fill: {},
        Font: {
            Color: defaultColor(inherit),
            Background: defaultBackground(inherit),
            ColorAlternate: defaultColor(inherit),
            BackgroundAlternate: defaultBackground(inherit),
            Family: 'Calibri',
            Size: 12,
            Weight: FontWeight.Regular,
            Underline: RangeUnderlineStyle.None,
        },
        Alignment: {
            Horizontal: inherit ? null : HorizontalAlignment.Center,
            Vertical: inherit ? null : VerticalAlignment.Center,
        },
    };
};
