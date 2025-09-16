import {
    CalendarDefinition,
    CalendarOverlayDefinition,
    CalendarOverlayTemplateDetailsDTO,
    CalendarOverlayTemplateInfoDTO,
    CalendarTemplateDetailsDTO,
    CalendarTemplateInfoDTO,
    FooterDefinition,
    FooterTemplateDetailsDTO,
    FooterTemplateInfoDTO,
    GrandTotalDefinition,
    GrandTotalTemplateDetailsDTO,
    GrandTotalTemplateInfoDTO,
    HeaderDefinition,
    HeaderTemplateDetailsDTO,
    HeaderTemplateInfoDTO,
    MediaHierarchyDefinition,
    MediaHierarchyTemplateDetailsDTO,
    MediaHierarchyTemplateInfoDTO,
    ThemeDefinition,
    ThemeTemplateDetailsDTO,
    ThemeTemplateInfoDTO,
    TotalsDefinition,
    TotalsTemplateDetailsDTO,
    TotalsTemplateInfoDTO,
} from '@omniflow/omni-webapi';

export type TDefinitionName =
    | 'headerDefinition'
    | 'calendarDefinition'
    | 'calendarOverlayDefinition'
    | 'mediaHierarchyDefinition'
    | 'totalsDefinition'
    | 'grandTotalsDefinition'
    | 'themeDefinition'
    | 'footerDefinition';

export type TDefinition =
    | CalendarDefinition
    | CalendarOverlayDefinition
    | HeaderDefinition
    | FooterDefinition
    | MediaHierarchyDefinition
    | TotalsDefinition
    | GrandTotalDefinition
    | ThemeDefinition;

export type TemplateInfoDTO =
    | CalendarTemplateInfoDTO
    | CalendarOverlayTemplateInfoDTO
    | MediaHierarchyTemplateInfoDTO
    | GrandTotalTemplateInfoDTO
    | HeaderTemplateInfoDTO
    | ThemeTemplateInfoDTO
    | FooterTemplateInfoDTO
    | TotalsTemplateInfoDTO;

export type TemplateDetailsDTO =
    | CalendarTemplateDetailsDTO
    | CalendarOverlayTemplateDetailsDTO
    | MediaHierarchyTemplateDetailsDTO
    | GrandTotalTemplateDetailsDTO
    | HeaderTemplateDetailsDTO
    | ThemeTemplateDetailsDTO
    | FooterTemplateDetailsDTO
    | TotalsTemplateDetailsDTO;
