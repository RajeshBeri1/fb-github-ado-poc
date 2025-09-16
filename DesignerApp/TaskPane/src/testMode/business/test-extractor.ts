import { HeaderConfiguration } from '@omniflow/omni-webapi';
import moment from 'moment';

import { AddressConverter } from '../../business/address-converter';
import { ExpectedResult } from '../models/expected-result';
import { AreaPosition } from '../models/area-position';
import { TestCase } from '../models/test-case';
import { DateFormat } from '../../enums/dateFormat.enum';
import { TestComponent } from '../../enums/test-component.enum';
import { range } from 'lodash';

export class TestExtractor {
    private addressConverter: AddressConverter = new AddressConverter();

    constructor() {}

    /**
     * Extract ExpectedResult from range
     * @param rangeString
     * @returns ExpectedResult
     */
    public async extractExpected(
        rangeString: string,
        testCase: TestCase
    ): Promise<ExpectedResult> {
        let expectedResult: ExpectedResult;

        await Excel.run(async (context) => {
            try {
                const ws = context.workbook.worksheets.getItemOrNullObject(
                    testCase.component.toString()
                );

                const actualRange = ws.getRange(rangeString);
                actualRange.load({
                    address: true,
                    values: true,
                    format: {
                        rowHeight: true,
                        columnWidth: true,
                    },
                    numberFormat: true,
                    rowCount: true,
                    columnCount: true,
                });

                const cells = actualRange.getCellProperties({
                    format: {
                        horizontalAlignment: true,
                        verticalAlignment: true,
                        fill: {
                            color: true,
                            pattern: true,
                            patternColor: true,
                            patternTintAndShade: true,
                            tintAndShade: true,
                        },
                        font: {
                            bold: true,
                            color: true,
                            italic: true,
                            size: true,
                            name: true,
                            strikethrough: true,
                            tintAndShade: true,
                            underline: true,
                        },
                        borders: {
                            color: true,
                            weight: true,
                            style: true,
                            tintAndShade: true,
                        },
                    },
                });

                const columns = actualRange.getColumnProperties({
                    format: { columnWidth: true },
                });

                const rows = actualRange.getRowProperties({
                    format: { rowHeight: true },
                });

                const mergedAreas = actualRange.getMergedAreasOrNullObject();
                mergedAreas.areas.load({
                    address: true,
                    rowCount: true,
                    columnCount: true,
                });

                await context.sync();

                expectedResult = new ExpectedResult({
                    testId: null,
                    component: null,
                    areas: mergedAreas.areas.toJSON().items,
                    areaPositions: [],
                    range: actualRange.toJSON(),
                    rows: rows.value,
                    columns: columns.value,
                    cells: cells.value,
                });
            } catch (error) {
                console.error(error);
            }
        });

        // Fix displaying wrong borders
        try {
            for await (const row of expectedResult.cells) {
                for await (const cell of row) {
                    const borders = cell.format.borders;
                    const newBorders = {};

                    for await (const key of Object.keys(borders)) {
                        const border = borders[key];

                        if (
                            key === '@odata.type' ||
                            (border && border.style !== 'None')
                        ) {
                            newBorders[key] = border;
                        }
                    }

                    cell.format.borders = newBorders;
                }
            }
        } catch (error) {
            console.log(error);
        }

        if (expectedResult.areas) {
            for await (const area of expectedResult.areas) {
                const mergedAreaRelativeAddress = await this.getRelativeCoords(
                    area,
                    expectedResult.range
                );
                expectedResult.areaPositions.push(mergedAreaRelativeAddress);
            }
        }

        expectedResult.testId = testCase.id;
        expectedResult.component = testCase.component;

        // WhiteFill+Gridline Fix:
        // when using getCellProperties on an empty cell without filling the following is returned:
        // patternColor: ""
        // color "#FFFFFF"
        // tintAndShade: null
        //
        // When using those props to draw the expectedResult this leads to:
        // 1. the empty string crashes the API and it stops setting cell props.
        // 2. the color #FFFFFF sets a white filling which makes all adjacent gridlines dissapear.
        //
        // To draw cells correctly the following must be done:
        // All cells: patterncolor "" must be set to null.
        // Cells without filling and visible gridlines: color FFFFFF must be set to null.
        // Cells with filling (white of any other color) are extracted correctly and nothing must be done.

        try {
            const rows = expectedResult.cells.length;
            for (let i = 0; i < rows; i++) {
                const row = expectedResult.cells[i];
                const newRow = row.map((cell) => {
                    if (cell.format.fill) {
                        if (cell.format.fill.patternColor.trim() === '') {
                            return {
                                ...cell,
                                format: {
                                    ...cell.format,
                                    fill: {
                                        ...cell.format.fill,
                                        patternColor: null,
                                    },
                                },
                            };
                        }
                    }
                    return cell;
                });
                const newerRow = newRow.map((cell) => {
                    if (
                        cell.format.fill.color === '#FFFFFF' &&
                        cell.format.fill.tintAndShade === null
                    ) {
                        return {
                            ...cell,
                            format: {
                                ...cell.format,
                                fill: {
                                    ...cell.format.fill,
                                    color: null,
                                },
                            },
                        };
                    }
                    return cell;
                });
                expectedResult.cells[i] = newerRow;
            }
        } catch (error) {
            console.log(error);
        }

        // Fixing Number Formats:
        // The number-format of all cells is set to "text". The format of the cells containing a number is set back to "General".
        // This avoids errors with date and currency
        // numbers should be kept in the general-format to avoid changing alignments

        // determine which cells contain a number
        const rows = expectedResult.range.rowCount;
        const cols = expectedResult.range.columnCount;
        const cellsToChange = [];
        for (let r = 0; r < rows; r++) {
            for (let c = 0; c < cols; c++) {
                const cellValue = expectedResult.range.values[r][c];
                if (typeof cellValue === 'number') {
                    const cellIndex = { rowIndex: r - 1, columnIndex: c };
                    cellsToChange.push(cellIndex);
                }
            }
        }

        // Setting all cells number-format to Text
        for (let r = 0; r < rows; r++) {
            for (let c = 0; c < cols; c++) {
                expectedResult.range.numberFormat[r][c] = '@';
            }
        }
        // setting the number-containing cells back to "General"
        for (const index of cellsToChange) {
            expectedResult.range.numberFormat[index.rowIndex + 1][
                index.columnIndex
            ] = 'General';
        }

        return expectedResult;
    }

    /**
     * Gets the relative coordinates of a subrange to its parent ranges top left cell
     * @param  {Excel.Interfaces.RangeData} rangeToMerge
     * @param  {Excel.Interfaces.RangeData} parentRange
     */
    private async getRelativeCoords(
        rangeToMerge: Excel.Interfaces.RangeData,
        parentRange: Excel.Interfaces.RangeData
    ): Promise<AreaPosition> {
        try {
            // If the parent ranges address does not contain ":" the Coord-parsing is neither necessary nor possible
            if (!parentRange.address.toString().includes(':')) {
                return new AreaPosition({
                    address: rangeToMerge.address.toString(),
                    startCoords: { y: 0, x: 0 },
                });
            }
            // absolute starting Coords of the parent Range
            const parentRangeStartAddress =
                this.addressConverter.getAddressFromString(
                    parentRange.address.substring(
                        parentRange.address.indexOf('!') + 1,
                        parentRange.address.lastIndexOf(':')
                    )
                );
            const parentRangeStartCoords =
                this.addressConverter.getCoordsFromAddress(
                    parentRangeStartAddress.column,
                    Number.parseInt(parentRangeStartAddress.row.toString())
                );

            // absolute starting Coords of the subrange
            const rangeToMergeOriginalStartAddressString = rangeToMerge.address
                .substring(rangeToMerge.address.lastIndexOf('!') + 1)
                .split(':')[0];
            const rangeToMergeOriginalStartAddress =
                this.addressConverter.getAddressFromString(
                    rangeToMergeOriginalStartAddressString
                );
            const rangeToMergeOriginalStartCoords =
                this.addressConverter.getCoordsFromAddress(
                    rangeToMergeOriginalStartAddress.column,
                    Number.parseInt(
                        rangeToMergeOriginalStartAddress.row.toString()
                    )
                );

            // relative Coords
            const rangeToMergeRelativeStartCoords = {
                y: rangeToMergeOriginalStartCoords.y - parentRangeStartCoords.y,
                x: rangeToMergeOriginalStartCoords.x - parentRangeStartCoords.x,
            };

            return new AreaPosition({
                startCoords: {
                    y: rangeToMergeRelativeStartCoords.y,
                    x: rangeToMergeRelativeStartCoords.x,
                },
                address: rangeToMerge.address.toString(),
            });
        } catch (error) {
            throw error;
        }
    }
}
