import moment from 'moment';

import { AddressConverter } from '../../business/address-converter';
import { RenderPosition } from '../models/render-position';
import InfoBoxStrings from '../constants/info-box-strings';
import { TestCase } from '../models/test-case';
import { TestRenderDict } from '../types/test-render-dict.type';
import { ExpectedResult } from '../models/expected-result';
import { DESCRIPTION_WIDTH, UNKNOWN_COLOR } from '../constants/info-box';
import { TestOutcome } from '../../enums/test-outcome.enum';
import { IRenderAreas, RenderAreas } from '../models/render-areas';
import { CalendarBuilder } from '../../business/engine/builder/calendar-builder';
import { ExcelRenderer } from '../../business/engine/renderer/excel-renderer';
import { HeaderBuilder } from '../../business/engine/builder/header-builder';
import { HeaderDetailsBuilder } from '../../business/engine/builder/header-details-builder';
import { MediaHierarchyBuilder } from '../../business/engine/builder/media-hierarchy-builder';
import {
    DUMMY_CALENDAR_DEFINITION,
    DUMMY_HEADER_DEFINITION,
} from '../constants/dummy-definitions';
import { RightHandTotalsBuilder } from '../../business/engine/builder/right-hand-totals-builder';
import { FooterBuilder } from '../../business/engine/builder/footer-builder';
import { CalendarOverlayBuilder } from '../../business/engine/builder/calendar-overlay-builder';
import { IRenderSize, RenderSize } from '../models/render-size';
import { BuildEngine } from '../../business/engine/models/build-engine';
import { Block } from '../../business/engine/models/block';
import { FooterLegendBuilder } from '../../business/engine/builder/footer-legend-builder';
import { GrandTotalsBuilder } from '../../business/engine/builder/grand-totals-builder';
import React, { useContext } from 'react';
import { AppContext } from '../../taskpane/contexts/AppContext';
import { CalendarHelper } from '../../business/engine/helpers/calendar-helper';
import * as HeaderHelper from '../../business/engine/helpers/header-helper';
import {
    flatLevels,
    TFlowchartDataLevelFlat,
} from '../../business/engine/helpers/level-helper';
import * as API from '@omniflow/omni-webapi';
import { dataApi } from '../../lib/api';
import { CLIENT_ID, OMNI_CLIENT_ID, TEMPLATE_ID } from '../constants/ids';
import { getDataCached } from '../../lib/data-cache';

export class TestRenderer extends React.Component {
    private renderPos: RenderPosition = new RenderPosition();
    private addressConverter = new AddressConverter();
    private results: { [key: string]: ExpectedResult } = {};
    private appContext = useContext(AppContext);

    private testRenders: TestRenderDict = {
        calendar: [],
        footer: [],
        header: [],
        mediaHierarchy: [],
        summary: [],
        fullReport: [],
    };

    public get Results(): { [key: string]: ExpectedResult } {
        return this.results;
    }
    /**
     * Renders all testCases. Calls renderTest() for each testCase
     * @param  {TestCase[]} testCases
     * @param  {ExpectedResult[]} expectedResults
     */
    public async renderTests(
        testCases: TestCase[],
        expectedResults: ExpectedResult[]
    ): Promise<void> {
        this.testRenders = {
            calendar: [],
            footer: [],
            header: [],
            mediaHierarchy: [],
            summary: [],
            fullReport: [],
        };

        this.results = {};

        for await (const testCase of testCases) {
            if (this.testRenders && this.testRenders[testCase.component]) {
                this.testRenders[testCase.component].push(testCase);

                const result = expectedResults.find(
                    (x) =>
                        x.testId === testCase.id &&
                        x.component === testCase.component
                );
                if (result) {
                    this.results[`${testCase.component}${testCase.id}`] =
                        result;
                }
            }
        }

        if (this.testRenders) {
            const startTime = performance.now();
            for await (const i of Object.keys(this.testRenders)) {
                this.renderPos.setCoords(0, 0);

                if (this.testRenders[i].length === 0) {
                    this.deactivateSheet(i);
                    continue;
                }

                await this.activateSheet(i);
                await this.renderSheetHeader(i);

                for await (const testCase of this.testRenders[i]) {
                    await this.renderTest(testCase);
                }
            }

            const endTime = performance.now();
            console.log(
                `Running tests took ${(endTime - startTime) / 1000} seconds`
            );
        }
    }
    /**
     * Renders a single testCase
     * @param  {TestCase} testCase
     */
    private async renderTest(testCase: TestCase): Promise<void> {
        const allBlocks = await this.buildComponents(testCase);
        const size = await this.calculateSize(allBlocks, testCase);
        const renderAreas = await this.getRenderAreas(size);
        testCase.renderAreas = renderAreas;

        await this.renderInfoBox(testCase, renderAreas);
        await this.renderActualResult(allBlocks, testCase, renderAreas);
        await this.renderExpectedResult(testCase, renderAreas);
    }
    /**
     * Uses the builder-classes to get blocks out of the testCase.
     * The logic in this Method must match the logic in the useRender-Hook
     * @param  {TestCase} testCase
     */
    async buildComponents(testCase: TestCase): Promise<Block[][]> {
        const { columns } = this.appContext;
        // Dummy References:
        const referencedMediaHierarchyDefinition = {
            Definition: testCase.mediaHierarchyDefinition,
            TemplateId: TEMPLATE_ID,
            TemplateVersion: 1,
        };
        const referencedCalendarDefinition = {
            Definition:
                testCase.calendarDefinition ?? DUMMY_CALENDAR_DEFINITION,
            TemplateId: TEMPLATE_ID,
            TemplateVersion: null,
        };
        const referencedHeaderDefinition = {
            Definition: testCase.headerDefinition ?? DUMMY_HEADER_DEFINITION,
            TemplateId: TEMPLATE_ID,
            TemplateVersion: null,
        };
        const referencedTotalsDefinition = {
            Definition: testCase.totalsDefinition,
            TemplateId: TEMPLATE_ID,
            TemplateVersion: null,
        };
        const referencedGrandTotalDefinition = {
            Definition: testCase.grandTotalsDefinition,
            TemplateId: TEMPLATE_ID,
            TemplateVersion: null,
        };

        let flatLevelData: TFlowchartDataLevelFlat[];
        let levelFlowchartData: API.FlowchartData;
        if (testCase.mediaHierarchyDefinition) {
            const levelApiData = await dataApi.getDataCached({
                OmniClientId: OMNI_CLIENT_ID,
                FlowchartDefinition: {
                    HeaderDefinition: testCase.headerDefinition
                        ? referencedHeaderDefinition
                        : null,
                    CalendarDefinition: referencedCalendarDefinition,
                    MediaHierarchyDefinition: testCase.mediaHierarchyDefinition
                        ? referencedMediaHierarchyDefinition
                        : null,
                    TotalsDefinition: testCase.totalsDefinition
                        ? referencedTotalsDefinition
                        : null,
                    GrandTotalDefinition: testCase.grandTotalsDefinition
                        ? referencedGrandTotalDefinition
                        : null,
                },
                RunRestrictions: [],
            });

            levelFlowchartData = levelApiData?.data;

            flatLevelData = flatLevels(
                levelFlowchartData?.Levels || [],
                referencedMediaHierarchyDefinition
            );
        }

        //Helpers:
        const calendarHelper = new CalendarHelper(
            CLIENT_ID,
            testCase.calendarDefinition?.Configuration
                ? testCase.calendarDefinition.Configuration
                : DUMMY_CALENDAR_DEFINITION.Configuration
        );
        calendarHelper.data = await calendarHelper.getData();

        const headerRowValues = await HeaderHelper.getRowValues(CLIENT_ID, {});

        // Building Components
        const builder = new BuildEngine();
        const mediaHierarchyBuilder = new MediaHierarchyBuilder({
            FlatLevelData: flatLevelData,
            clientId: CLIENT_ID,
            columns,
            ReferencedMediaHierarchyDefinition:
                referencedMediaHierarchyDefinition,
            ReferencedCalendarDefinition: referencedCalendarDefinition,
            ReferencedHeaderDefinition: referencedHeaderDefinition,
            CalendarHelper: calendarHelper,
        });
        let mediaHierarchyBlocks = await mediaHierarchyBuilder.build();
        const { leftMenuColumnLength, flightBarWallColumnLength, legendData } =
            mediaHierarchyBuilder;

        const calendarOverlayBuilder = new CalendarOverlayBuilder({
            clientId: CLIENT_ID,
            CalendarOverlayDefinition: testCase.calendarOverlayDefinition,
            CalendarDefinition:
                testCase.calendarDefinition ?? DUMMY_CALENDAR_DEFINITION,

            titleColumns: leftMenuColumnLength,
            CalendarHelper: calendarHelper,
        });
        let calendarOverlayBlocks = await calendarOverlayBuilder.build();
        const { overlayFlightBarWallRowLength, overlayLeftMenuColumnLength } =
            calendarOverlayBuilder;

        const leftSpace =
            overlayLeftMenuColumnLength > leftMenuColumnLength
                ? overlayLeftMenuColumnLength
                : leftMenuColumnLength;

        const totalsBuilder = new RightHandTotalsBuilder({
            clientId: CLIENT_ID,
            ReferencedTotalsDefinition: referencedTotalsDefinition,
            ReferencedMediaHierarchyDefinition:
                referencedMediaHierarchyDefinition,
            ReferencedCalendarDefinition: referencedCalendarDefinition,
            calendarOverlayHeight: overlayFlightBarWallRowLength,
            CalendarHelper: calendarHelper,
            FlatLevelData: flatLevelData,
            dataColumns: [],
        });
        let totalsBlocks = await totalsBuilder.build();
        const { rightHandTotalsColumnLength } = totalsBuilder;

        const mediaHierarchyTotalsBlocks = flightBarWallColumnLength
            ? builder.merge([mediaHierarchyBlocks, totalsBlocks], 0)
            : [];

        const calendarBuilder = new CalendarBuilder({
            clientId: CLIENT_ID,
            CalendarDefinition: testCase.calendarDefinition,
            CalendarHelper: calendarHelper,
        });
        let calendarBlocks = await calendarBuilder.build();
        const { calendarColumnLength } = calendarBuilder;
        calendarBlocks = builder.move(calendarBlocks, {
            left: leftSpace,
        });

        const headerBuilder = new HeaderBuilder({
            clientId: CLIENT_ID,
            HeaderDefinition: testCase.headerDefinition,
            headerColumns: calendarColumnLength,
            logoLeftColumns: leftSpace,
            logoRightColumns: rightHandTotalsColumnLength,
            headerRowData: headerRowValues,
        });
        let headerBlocks = await headerBuilder.build();
        const { logoLeftColumns } = headerBuilder;

        let headerDetailsBuilder = new HeaderDetailsBuilder({
            clientId: CLIENT_ID,
            HeaderDefinition: testCase.headerDefinition,
            headerDetailsLength: Math.floor((calendarColumnLength ?? 20) / 2),
            headerRowData: headerRowValues,
        });
        let headerDetailsBlocks = await headerDetailsBuilder.build();

        const containsLogo =
            !!testCase.headerDefinition?.Configuration?.Logos?.length;

        headerBlocks = builder.move(headerBlocks, {
            left: leftSpace && !containsLogo ? leftSpace : 0,
        });
        headerDetailsBlocks = builder.move(headerDetailsBlocks, {
            left: leftSpace ? leftSpace : logoLeftColumns,
        });

        const grandTotalsBuilder = new GrandTotalsBuilder({
            clientId: CLIENT_ID,
            ReferencedGrandTotalDefinition: referencedGrandTotalDefinition,
            ReferencedCalendarDefinition: referencedCalendarDefinition,
            ReferencedMediaHierarchyDefinition:
                referencedMediaHierarchyDefinition,
            leftMenuColumnLength: leftSpace,
            columns,
            CalendarHelper: calendarHelper,
            LevelFlowchartData: levelFlowchartData,
            totalsBlocks,
        });
        let grandTotalsBlocks = await grandTotalsBuilder.build();

        const footerBuilder = new FooterBuilder({
            clientId: CLIENT_ID,
            FooterDefinition: testCase.footerDefinition,
            templateColumnLength:
                leftSpace + calendarColumnLength + rightHandTotalsColumnLength,
        });
        let footerBlocks = await footerBuilder.build();
        const footerLegendBuilder = new FooterLegendBuilder({
            clientId: CLIENT_ID,
            FooterDefinition: testCase.footerDefinition,
            templateColumnLength:
                leftSpace + calendarColumnLength + rightHandTotalsColumnLength,
            legendData: legendData,
            MediaHierarchyDefinition: testCase.mediaHierarchyDefinition,
        });
        let footerLegendBlocks = await footerLegendBuilder.build();
        footerLegendBlocks = builder.move(footerLegendBlocks, {
            left: leftSpace ? leftSpace : 0,
        });

        const allBlocks = [
            ...headerBlocks,
            ...headerDetailsBlocks,
            ...calendarBlocks,
            ...calendarOverlayBlocks,
            ...mediaHierarchyTotalsBlocks,
            ...grandTotalsBlocks,
            ...footerLegendBlocks,
            ...footerBlocks,
        ];

        return allBlocks;
    }
    /**
     * Calculates the size of the actual result
     * @param  {Block[][]} allBlocks
     * @param  {TestCase} testCase
     * @returns size
     */
    async calculateSize(
        allBlocks: Block[][],
        testCase: TestCase
    ): Promise<RenderSize> {
        let totalRowsOfHeaderCalendarAndOverlay: number = 0;
        const size = new RenderSize({
            rows: 0,
            columns: 0,
            startAdjustment: 0,
        });

        for (const row of allBlocks) {
            let columnLength = 0;
            let columnStart = 0;
            let columnAfter = 0;
            for (const block of row) {
                columnLength += block.columnLength;
                columnStart += block.columnStart;
                columnAfter += block.columnAfter;
            }
            const length = columnLength + columnStart + columnAfter;
            size.columns = size.columns > length ? size.columns : length;
        }

        size.rows = allBlocks.length;
        for (const row of allBlocks) {
            for (const block of row) {
                if (block.rowStart > 0) {
                    size.rows += block.rowStart;
                }
                if (block.rowStart < 0) {
                    size.startAdjustment += block.rowStart * -1;
                }
            }
        }

        // bad temp fix for the weird space-rows above grad totals:
        // adding 2 rows and another row for each calendarOverlay row
        if (
            testCase.mediaHierarchyDefinition &&
            testCase.grandTotalsDefinition
        ) {
            size.rows +=
                2 +
                (testCase.calendarOverlayDefinition
                    ? testCase.calendarOverlayDefinition.OverlaySections.length
                    : 0);
        }

        totalRowsOfHeaderCalendarAndOverlay = testCase.headerDefinition
            ? testCase.headerDefinition.Configuration.Rows.length
            : 0;
        totalRowsOfHeaderCalendarAndOverlay += testCase.calendarDefinition
            ? testCase.calendarDefinition.Configuration.Rows.length
            : 0;
        totalRowsOfHeaderCalendarAndOverlay +=
            testCase.calendarOverlayDefinition
                ? testCase.calendarOverlayDefinition.OverlaySections.length
                : 0;

        size.startAdjustment -= totalRowsOfHeaderCalendarAndOverlay;
        size.startAdjustment =
            size.startAdjustment < 0 ? 0 : size.startAdjustment;

        return size;
    }
    /**
     * Gets the RenderAreas for the infobox and both results
     * @param  {IRenderSize} size
     */
    private async getRenderAreas(size: IRenderSize): Promise<RenderAreas> {
        this.renderPos.addRows(2);

        const cell = this.renderPos.Cell;

        const infoBox = {
            from: {
                x: cell.x,
                y: cell.y,
            },
            to: {
                x: cell.x + 2,
                y: cell.y + size.rows * 2 + 5,
            },
        };

        const actualResult = {
            from: {
                x: infoBox.from.x + 4,
                y: infoBox.from.y + 4,
            },
            to: {
                x: infoBox.to.x + size.columns + 1,
                y: infoBox.from.y + 4 + size.rows - 1,
            },
        };

        const expectedResult = {
            from: {
                x: actualResult.from.x,
                y: actualResult.from.y + (size.rows + 2),
            },
            to: {
                x: actualResult.to.x,
                y: actualResult.to.y + (size.rows + 2),
            },
        };

        const renderAreas = new RenderAreas({
            infoBox: infoBox,
            actualResult: actualResult,
            expectedResult: expectedResult,
            componentSize: size,
        });

        return renderAreas;
    }
    /**
     * Renders the Info-box on the left of the Excel-Worksheet
     * @param  {TestCase} testCase
     * @param  {RenderAreas} renderAreas
     */
    private async renderInfoBox(
        testCase: TestCase,
        renderAreas: RenderAreas
    ): Promise<void> {
        const { id, description, outcome } = testCase;
        const infoBoxRange = this.renderPos.getRange(
            renderAreas.infoBox.from,
            renderAreas.infoBox.to
        );

        await Excel.run(async (context) => {
            try {
                const ws = context.workbook.worksheets.getActiveWorksheet();
                const range = ws.getRange(infoBoxRange);
                range.format.fill.color = UNKNOWN_COLOR;

                const idTagCell = ws.getCell(this.renderPos.Y, 0);
                idTagCell.values = [[InfoBoxStrings.ID]];
                idTagCell.format.font.bold = true;
                const idValueCell = ws.getCell(this.renderPos.Y, 2);
                idValueCell.values = [[id]];

                this.renderPos.addRows(1);

                const descriptionTagCell = ws.getCell(this.renderPos.Y, 0);
                descriptionTagCell.values = [[InfoBoxStrings.Description]];
                descriptionTagCell.format.font.bold = true;
                descriptionTagCell.format.verticalAlignment = 'Top';
                descriptionTagCell.format.horizontalAlignment = 'Left';

                const descriptionValueCell = ws.getCell(this.renderPos.Y, 1);
                descriptionValueCell.values = [[description]];
                descriptionValueCell.format.wrapText = true;
                descriptionValueCell.format.verticalAlignment = 'Top';
                descriptionValueCell.format.horizontalAlignment = 'Left';

                const descriptionColumn = descriptionValueCell.getLastColumn();
                descriptionColumn.format.columnWidth = DESCRIPTION_WIDTH;

                this.renderPos.addRows(1);

                const outcomeTagCell = ws.getCell(this.renderPos.Y, 0);
                outcomeTagCell.values = [[InfoBoxStrings.Status]];
                outcomeTagCell.format.font.bold = true;
                const outcomeValueCell = ws.getCell(this.renderPos.Y, 1);
                outcomeValueCell.values = [[outcome]];

                this.renderPos.addRows(1);

                const actualResultTagCell = ws.getCell(this.renderPos.Y, 0);
                actualResultTagCell.values = [[InfoBoxStrings.ActualResult]];
                actualResultTagCell.format.font.bold = true;

                const componentHeight = renderAreas.componentSize.rows;
                this.renderPos.addRows(componentHeight + 2);

                const expectedResultTagCell = ws.getCell(this.renderPos.Y, 0);
                expectedResultTagCell.values = [
                    [InfoBoxStrings.ExpectedResult],
                ];
                expectedResultTagCell.format.font.bold = true;

                this.renderPos.addRows(componentHeight);
                this.renderPos.addRows(1);
                range.getLastCell().select();

                await context.sync();
            } catch (error) {
                console.error(error);
            }
        });
    }
    /**
     * Renders the actual result
     * @param  {Block[][]} componenetRows
     * @param  {TestCase} testCase
     * @param  {IRenderAreas} renderAreas
     */
    async renderActualResult(
        componenetRows: Block[][],
        testCase: TestCase,
        renderAreas: IRenderAreas
    ) {
        const renderer = new ExcelRenderer();
        const name = testCase.component.toString();
        let rowStart = renderAreas.actualResult.from.y;
        rowStart += renderAreas.componentSize.startAdjustment;

        // Setting the renderRangeString property to be able to extract and download the results if desired.
        testCase.renderRangeString = `${testCase.component}!${
            this.addressConverter.getAddressFromCoords(
                renderAreas.actualResult.from.x + 1,
                renderAreas.actualResult.from.y + 1
            ).text
        }:${
            this.addressConverter.getAddressFromCoords(
                renderAreas.actualResult.to.x + 1,
                renderAreas.actualResult.to.y + 1
            ).text
        }`;

        await renderer.render(componenetRows, {
            rowStart: rowStart,
            columnStart: renderAreas.actualResult.from.x,
            name: name,
            deleteSheet: false,
        });
    }
    /**
     * Renders the expected result
     * @param  {TestCase} testCase
     * @param  {RenderAreas} renderAreas
     */
    private async renderExpectedResult(
        testCase: TestCase,
        renderAreas: RenderAreas
    ): Promise<void> {
        let expectedResultRange = this.renderPos.getRange(
            renderAreas.expectedResult.from,
            renderAreas.expectedResult.to
        );

        // Get the top-left Coordinates of the Range where the expected result will be rendered.
        const addressConverter = this.addressConverter;
        const expectedResultStartingCellString =
            expectedResultRange.split(':')[0];
        const expectedResultStartingCellAddress =
            addressConverter.getAddressFromString(
                expectedResultStartingCellString
            );
        const expectedResultStartingCellCoords =
            addressConverter.getCoordsFromAddress(
                expectedResultStartingCellAddress.column,
                Number.parseInt(
                    expectedResultStartingCellAddress.row.toString()
                )
            );

        const expectedResult =
            this.results[`${testCase.component}${testCase.id}`];

        if (expectedResult) {
            await Excel.run(async function (context) {
                try {
                    const ws = context.workbook.worksheets.getActiveWorksheet();

                    const range = ws.getRange(expectedResultRange);
                    range.clear();

                    if (
                        expectedResult.areas &&
                        expectedResult.areas.length > 0
                    ) {
                        for (let range of expectedResult.areas) {
                            context.application.suspendScreenUpdatingUntilNextSync();
                            const relativeCoords =
                                expectedResult.areaPositions.filter(
                                    (x) => x.address === range.address
                                )[0];
                            const startCellOfMergeAreaCoords = {
                                y:
                                    expectedResultStartingCellCoords.y +
                                    relativeCoords.startCoords.y,
                                x:
                                    expectedResultStartingCellCoords.x +
                                    relativeCoords.startCoords.x,
                            };
                            const endCellOfMergeAreaCoords = {
                                y:
                                    startCellOfMergeAreaCoords.y +
                                    range.rowCount -
                                    1,
                                x:
                                    startCellOfMergeAreaCoords.x +
                                    range.columnCount -
                                    1,
                            };
                            const startCellOfMergeAreaAddress =
                                addressConverter.getAddressFromCoords(
                                    startCellOfMergeAreaCoords.x,
                                    startCellOfMergeAreaCoords.y
                                );
                            const endCellOfMergeAreaAddress =
                                addressConverter.getAddressFromCoords(
                                    endCellOfMergeAreaCoords.x,
                                    endCellOfMergeAreaCoords.y
                                );
                            const mergeAreaRangeString = `${startCellOfMergeAreaAddress.column}${startCellOfMergeAreaAddress.row}:${endCellOfMergeAreaAddress.column}${endCellOfMergeAreaAddress.row}`;
                            const rangeToMerge =
                                ws.getRange(mergeAreaRangeString);
                            rangeToMerge.merge(false);
                        }
                    }
                    range.numberFormat = expectedResult.range.numberFormat;
                    range.values = expectedResult.range.values;

                    range.setColumnProperties(expectedResult.columns);
                    range.setRowProperties(expectedResult.rows);
                    range.setCellProperties(expectedResult.cells);

                    await context.sync();
                } catch (error) {
                    // ToDo: Catch RichApi Inputmatrix
                    console.error(error);
                }
            });
        } else {
            console.error('No expected result found for testCase', testCase.id);
            testCase.outcome = TestOutcome.Review;
        }
    }
    /**
     * Deactivates the Excel-Worksheet
     * @param  {string} sheetName
     */
    private async deactivateSheet(sheetName: string): Promise<void> {
        await Excel.run(async (context) => {
            try {
                const worksheets = context.workbook.worksheets;

                let oldWorksheet = worksheets.getItemOrNullObject(sheetName);

                if (oldWorksheet) {
                    oldWorksheet.delete();
                }

                await context.sync();
            } catch (error) {
                console.error(error);
            }
        });
    }
    /**
     * Activates the Excel-Worksheet
     * @param  {string} sheetName
     */
    private async activateSheet(sheetName: string): Promise<void> {
        await Excel.run(async (context) => {
            try {
                const worksheets = context.workbook.worksheets;

                let oldWorksheet = worksheets.getItemOrNullObject(sheetName);

                if (oldWorksheet) {
                    oldWorksheet.delete();
                }

                const ws = worksheets.add(sheetName);

                ws.load('name, position');
                ws.activate();

                await context.sync();
            } catch (error) {
                console.error(error);
            }
        });
    }
    /**
     * Renders the Sheet-Header with Information on the tested component and the current time
     * @param  {string} component
     */
    private async renderSheetHeader(component: string): Promise<void> {
        const componentTitle =
            component.charAt(0).toUpperCase() + component.slice(1);

        await Excel.run(async (context) => {
            try {
                const ws = context.workbook.worksheets.getActiveWorksheet();

                const headerCell = ws.getCell(this.renderPos.Cell.y, 0);
                headerCell.values = [
                    [componentTitle + ' ' + InfoBoxStrings.SheetHeader],
                ];
                headerCell.format.font.bold = true;
                headerCell.format.font.size = 14;

                this.renderPos.addRows(1);

                const dateTagCell = ws.getCell(this.renderPos.Cell.y, 0);
                dateTagCell.values = [[InfoBoxStrings.LastRun]];
                dateTagCell.format.font.bold = true;

                const dateValueCell = ws.getCell(this.renderPos.Cell.y, 1);
                dateValueCell.values = [
                    [moment.utc().toDate().toLocaleString()],
                ];

                this.renderPos.addRows(1);

                await context.sync();
            } catch (error) {
                console.error(error);
            }
        });
    }
}
