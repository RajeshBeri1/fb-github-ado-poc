export enum UrlAndQueryParamKey {
    /**
     * Flowchart template id accessor. (Url identifier)
     * Gets loaded on start up from custom properties.
     */
    FLOWCHART_TEMPLATE_ID = 'flowchartTemplateId',

    /**
     * Calendar template id accessor. (Url identifier)
     * Used if calendar template details are edited.
     */
    CALENDAR_TEMPLATE_ID = 'calendarTemplateId',

    /**
     * Calendar overlay template id accessor. (Url identifier)
     * Used if calendar overlay template details are edited.
     */
     CALENDAR_OVERLAY_TEMPLATE_ID = 'calendarOverlayTemplateId',

    /**
     * Media Hierarchy Template id accessor. (Url identifier)
     * Used if Media Hierarchy Details are edited.
     */
    MEDIA_HIERARCHY_TEMPLATE_ID = 'mediaHierarchyTemplateId',

    /**
     * Grand Totals Template id accessor. (Url identifier)
     * Used if Grand Totals Details are edited.
     */
    GRAND_TOTALS_TEMPLATE_ID = 'grandTotalsTemplateId',

    /**
     * Right Hand Totals Template id accessor. (Url identifier)
     * Used if Right Hand Totals Details are edited.
     */
    RIGHT_HAND_TOTALS_TEMPLATE_ID = 'rightHandTotalsTemplateId',

    /**
     * The ANsid query parameter key.
     */
    ANSID = 'ANsid',
}
