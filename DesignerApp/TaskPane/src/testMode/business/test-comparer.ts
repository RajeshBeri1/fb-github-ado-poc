import { TestCase } from '../models/test-case';
import { TestExtractor } from './test-extractor';
import { detailedDiff } from 'deep-object-diff';
import { ExpectedResult } from '../models/expected-result';
import { TestOutcome } from '../../enums/test-outcome.enum';
import {
    FAIL_COLOR,
    PASSED_COLOR,
    REVIEW_COLOR,
    UNKNOWN_COLOR,
} from '../constants/info-box';
import { RenderPosition } from '../models/render-position';
import { RenderAreas } from '../models/render-areas';

export interface IDiffResult {
    added: object;
    deleted: object;
    updated: object;
}

export class TestComparer {
    private testExtractor: TestExtractor = new TestExtractor();
    private renderPos: RenderPosition = new RenderPosition();

    constructor() {}

    /**
     * Compare actual and expected result
     * @param testCase
     * @param testRange
     * @param expectedRange
     */
    public async compareResult(
        testCase: TestCase,
        ranges: RenderAreas
    ): Promise<void> {
        const infoBoxRange = this.renderPos.getRange(
            ranges.infoBox.from,
            ranges.infoBox.to
        );
        const testRange = this.renderPos.getRange(
            ranges.actualResult.from,
            ranges.actualResult.to
        );
        const expectedRange = this.renderPos.getRange(
            ranges.expectedResult.from,
            ranges.expectedResult.to
        );

        if (testCase.outcome !== TestOutcome.Unknown) {
            await this.setInfoBoxColorAndOutcome(
                testCase.outcome,
                infoBoxRange
            );
            return;
        }

        let testResult: ExpectedResult =
            await this.testExtractor.extractExpected(testRange, testCase);
        let expectedResult: ExpectedResult =
            await this.testExtractor.extractExpected(expectedRange, testCase);

        const cellDiff: IDiffResult = detailedDiff(
            expectedResult.cells,
            testResult.cells
        ) as IDiffResult;

        const valueDiff: IDiffResult = detailedDiff(
            expectedResult.range.values,
            testResult.range.values
        ) as IDiffResult;

        const rowPropsDiff: IDiffResult = detailedDiff(
            expectedResult.rows,
            testResult.rows
        ) as IDiffResult;

        // // Comment in to get Information about why a testcase failed
        // console.log('comparer - actualResult:');
        // console.log(testResult);
        // console.log('comparer - expectedResult:');
        // console.log(expectedResult);
        // console.log('Testcase');
        // console.log(testCase);
        // console.log('cellDiff');
        // console.log(cellDiff);
        // console.log('valueDiff');
        // console.log(valueDiff);
        // console.log('rowPropsDiff');
        // console.log(rowPropsDiff);

        if (
            Object.keys(cellDiff.added).length === 0 &&
            Object.keys(cellDiff.updated).length === 0 &&
            Object.keys(cellDiff.deleted).length === 0 &&
            Object.keys(valueDiff.added).length === 0 &&
            Object.keys(valueDiff.updated).length === 0 &&
            Object.keys(valueDiff.deleted).length === 0 &&
            Object.keys(rowPropsDiff.added).length === 0 &&
            Object.keys(rowPropsDiff.updated).length === 0 &&
            Object.keys(rowPropsDiff.deleted).length === 0
        ) {
            testCase.outcome = TestOutcome.Pass;
        } else {
            testCase.outcome = TestOutcome.Fail;
        }
    }

    /**
     * Set the color and text of the infobox according to the test outcome
     * @param  {TestOutcome} outcome
     * @param  {string} range
     */
    public async setInfoBoxColorAndOutcome(
        outcome: TestOutcome,
        range: string
    ): Promise<void> {
        const infoBoxRange = range;
        let color;
        let outcomeText = outcome.toString();

        switch (outcome) {
            case TestOutcome.Fail:
                color = FAIL_COLOR;
                break;
            case TestOutcome.Pass:
                color = PASSED_COLOR;
                break;
            case TestOutcome.Review:
                color = REVIEW_COLOR;
                break;
            case TestOutcome.Unknown:
                color = UNKNOWN_COLOR;
                break;
            default:
                color = FAIL_COLOR;
                console.error('no valid value for test outcome.');
                break;
        }

        await Excel.run(async (context) => {
            const ws = context.workbook.worksheets.getActiveWorksheet();
            const range = ws.getRange(infoBoxRange);
            range.format.fill.color = color;
            // The cell that displays the outcome as text is always two rows below and one column right from the topleft cell of the infobox
            let outcomeCell = range.getCell(2, 1);
            outcomeCell.values = [[outcomeText]];
        });
    }
}
