import {
    CalendarDefinition,
    CalendarOverlayDefinition,
    FooterDefinition,
    GrandTotalDefinition,
    HeaderDefinition,
    MediaHierarchyDefinition,
    ThemeDefinition,
    TotalsDefinition,
} from '@omniflow/omni-webapi';
import { TestComponent } from '../../enums/test-component.enum';
import { TestOutcome } from '../../enums/test-outcome.enum';
import { IRenderAreas } from './render-areas';

export interface ITestCase {
    id: string;
    component: TestComponent;
    description: string;
    outcome: TestOutcome;
    renderRangeString?: string;
    renderAreas?: IRenderAreas;

    calendarDefinition?: CalendarDefinition;
    mediaHierarchyDefinition?: MediaHierarchyDefinition;
    headerDefinition?: HeaderDefinition;
    totalsDefinition?: TotalsDefinition;
    grandTotalsDefinition?: GrandTotalDefinition;
    footerDefinition?: FooterDefinition;
    calendarOverlayDefinition?: CalendarOverlayDefinition;
    themeDefinition?: ThemeDefinition;
}

export class TestCase implements ITestCase {
    id: string;
    component: TestComponent;
    description: string;
    outcome: TestOutcome;
    renderRangeString?: string;
    renderAreas?: IRenderAreas;

    calendarDefinition?: CalendarDefinition;
    mediaHierarchyDefinition?: MediaHierarchyDefinition;
    headerDefinition?: HeaderDefinition;
    totalsDefinition?: TotalsDefinition;
    grandTotalsDefinition?: GrandTotalDefinition;
    footerDefinition?: FooterDefinition;
    calendarOverlayDefinition?: CalendarOverlayDefinition;
    themeDefinition?: ThemeDefinition;
    // TODO: Add other definitions

    constructor(testCase: TestCase | ITestCase) {
        this.id = testCase.id;
        this.component = testCase.component;
        this.description = testCase.description;
        this.outcome = testCase.outcome;
        this.renderRangeString = testCase.renderRangeString;
        this.renderAreas = testCase.renderAreas;

        // Component definitions
        this.calendarDefinition = testCase.calendarDefinition;
        this.mediaHierarchyDefinition = testCase.mediaHierarchyDefinition;
        this.headerDefinition = testCase.headerDefinition;
        this.totalsDefinition = testCase.totalsDefinition;
        this.grandTotalsDefinition = testCase.grandTotalsDefinition;
        this.footerDefinition = testCase.footerDefinition;
        this.calendarOverlayDefinition = testCase.calendarOverlayDefinition;

        this.themeDefinition = testCase.themeDefinition;
    }
}
