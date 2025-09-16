import {
    BorderLineStyle,
    HorizontalAlignment,
    VerticalAlignment,
    FontWeight,
    RangeUnderlineStyle,
    ThemeTemplateDetailsDTO, ThemeDefinition, Styling, LegendTheme
} from '@omniflow/omni-webapi';

import { FontFamilyType } from '../../../../enums/font-family.enum';
import { precisionValue } from '../../../../enums/precision-value.enum';
import { isEmpty } from 'lodash';
const defaultColor = {
    Alpha: 1,
    Blue: 1,
    Green: 1,
    Red: 1,
};

const getThemeTemplateDetails = (config: ThemeTemplateDetailsDTO) => ({
    Id: config?.Id,
    Name: config?.Name,
    Definition: {
        Styling: getDefaultStyling(config),
        HeaderTheme: {
            Styling: getDefaultStyling(
                config,
                'Definition.HeaderTheme.Styling'
            ),
            RowStyling: getDefaultStyling(
                config,
                'Definition.HeaderTheme.RowStyling'
            ),
        },
        CalendarTheme: {
            Styling: getDefaultStyling(
                config,
                'Definition.CalendarTheme.Styling'
            ),
            RowStyling: getDefaultStyling(
                config,
                'Definition.CalendarTheme.RowStyling'
            ),
        },
        CalendarOverlayTheme: {
            SectionStyling: getDefaultStyling(
                config,
                'Definition.CalendarOverlayTheme.SectionStyling'
            ),
            RowStyling: getDefaultStyling(
                config,
                'Definition.CalendarOverlayTheme.RowStyling'
            ),
        },
        GrandTotalTheme: {
            Styling: getDefaultStyling(
                config,
                'Definition.GrandTotalTheme.Styling'
            ),
            LeftMenuStyling: getDefaultStyling(
                config,
                'Definition.GrandTotalTheme.LeftMenuStyling'
            ),
        },
        MediaHierarchyTheme: {
            Styling: getDefaultStyling(
                config,
                'Definition.MediaHierarchyTheme.Styling'
            ),
            LeftMenuStyling: getDefaultStyling(
                config,
                'Definition.MediaHierarchyTheme.LeftMenuStyling'
            ),
            SubTotalStyling: getDefaultStyling(
                config,
                'Definition.MediaHierarchyTheme.SubTotalStyling'
            ),
            SubTotalTitleStyling: getDefaultStyling(
                config,
                'Definition.MediaHierarchyTheme.SubTotalTitleStyling'
            ),
            FlightBarStyling: getDefaultStyling(
                config,
                'Definition.MediaHierarchyTheme.FlightBarStyling'
            ),
            InflightOverlayStyling: getDefaultStyling(
                config,
                'Definition.MediaHierarchyTheme.InflightOverlayStyling'
            ),
        },
        TotalsTheme: {
            Styling: getDefaultStyling(
                config,
                'Definition.TotalsTheme.Styling'
            ),
            HeaderStyling: getDefaultStyling(
                config,
                'Definition.TotalsTheme.HeaderStyling'
            ),
            SumStyling: getDefaultStyling(
                config,
                'Definition.TotalsTheme.SumStyling'
            ),
            MainHeaderStyling: getDefaultStyling(
                config,
                'Definition.TotalsTheme.MainHeaderStyling'
            ),
        },
        FooterTheme: {
            Styling: getDefaultStyling(
                config,
                'Definition.FooterTheme.Styling'
            ),
        },
        LegendTheme: config?.Definition?.LegendTheme ?? getDefaultLegendStyling(config,'Definition.LegendTheme'),
        SplitByColumn: config?.Definition?.SplitByColumn ?? { TableId: null, ColumnName: null },
    },
});

const getDefaultStyling = (config, key = null) => {
    const styling = key ? getValue(config, key) : config?.Definition?.Styling;
/*    const isCustomStyleSettingValue = styling?.CustomStyleSetting?.CustomStyleSettingValue;
*/    const getDefaultColor = (color) => {
        // Check if all RGB values are null or undefined
        const isDefaultColor = [color?.Red, color?.Green, color?.Blue].every((c) => c === null || c === undefined);
        return {
            Alpha: color?.Alpha || 1,
            Blue: isDefaultColor ? 255 : color?.Blue,
            Green: isDefaultColor ? 255 : color?.Green,
            Red: isDefaultColor ? 255 : color?.Red,
        };
    };
    return {
        Alignment: {
            Horizontal:
                styling?.Alignment?.Horizontal || HorizontalAlignment.Center,
            Vertical: styling?.Alignment?.Vertical || VerticalAlignment.Center,
        },
        Border: {
            Bottom: {
                Style:
                    styling?.Border?.Bottom?.Style ||
                    BorderLineStyle.Continuous,
                Color: styling?.Border?.Bottom?.Color || defaultColor,
            },
            Left: {
                Style:
                    styling?.Border?.Left?.Style || BorderLineStyle.Continuous,
                Color: styling?.Border?.Left?.Color || defaultColor,
            },
            Right: {
                Style:
                    styling?.Border?.Right?.Style || BorderLineStyle.Continuous,
                Color: styling?.Border?.Right?.Color || defaultColor,
            },
            Top: {
                Style:
                    styling?.Border?.Top?.Style || BorderLineStyle.Continuous,
                Color: styling?.Border?.Top?.Color || defaultColor,
            },
        },
        Fill: {},
        Font: {
            Color: {
                Alpha: styling?.Font?.Color?.Alpha || 1,
                Blue: styling?.Font?.Color?.Blue || 0,
                Green: styling?.Font?.Color?.Green || 0,
                Red: styling?.Font?.Color?.Red || 0,
            },
            Background: getDefaultColor(styling?.Font?.Background),
            ColorAlternate: {
                Alpha: styling?.Font?.ColorAlternate?.Alpha || 1,
                Blue: styling?.Font?.ColorAlternate?.Blue || 0,
                Green: styling?.Font?.ColorAlternate?.Green || 0,
                Red: styling?.Font?.ColorAlternate?.Red || 0,
            },
            BackgroundAlternate: getDefaultColor(styling?.Font?.BackgroundAlternate),
            Family: styling?.Font?.Family ?? FontFamilyType.Calibri,
            Size: styling?.Font?.Size || 12,
            Weight: styling?.Font?.Weight || FontWeight.Regular,
            Underline: styling?.Font?.Underline || RangeUnderlineStyle.None,
        },
        Position: {
            Start: styling?.Position?.Start || "A1",
        },
        Precision: {
            PrecisionValue: styling?.Precision?.PrecisionValue || precisionValue.Actual,
            ApplyToCurrencyMetric: styling?.Precision?.ApplyToCurrencyMetric ?? true,
            ApplyToNonCurrencyMetric: styling?.Precision?.ApplyToNonCurrencyMetric ?? true,

        },
        ShowBriefedCTC: {
            ShowBriefedCTCValue: Boolean(styling?.ShowBriefedCTC?.ShowBriefedCTCValue),
        },
/*        CustomStyleSetting: {
            CustomStyleSettingValue: isCustomStyleSettingValue === undefined ? false : Boolean(isCustomStyleSettingValue),
        }*/

    };
};

const getValue = (obj, path) => {
    if (!obj) {
        return;
    }

    let pathSegments = path.split('.');
    let resultObject = obj;

    while (pathSegments.length) {
        let n = pathSegments.shift();
        if (isEmpty(resultObject[n])) return;
        resultObject = resultObject[n];
    }
    return resultObject;
};

const getDefaultLegendStyling = (config: ThemeTemplateDetailsDTO, key = null): LegendTheme => {
    const styling = key ? getValue(config, key) : config?.Definition?.LegendTheme;
    return {
        TableId: styling?.TableId,
        ColumnName: styling?.ColumnName,
        Settings: styling?.Settings ?? [],
        DisplayVertically: styling?.DisplayVertically ?? false,
    }
}

export default getThemeTemplateDetails;
