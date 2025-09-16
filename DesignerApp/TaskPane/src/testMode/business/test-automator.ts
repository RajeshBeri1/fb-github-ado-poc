import { AxiosResponse } from 'axios';
import axios from 'axios';
import { ITestCase, TestCase } from '../models/test-case';
import { ITestConfig, TestConfig } from '../models/test-config';
import { TestRenderer } from './test-renderer';
import { ExpectedResult, IExpectedResult } from '../models/expected-result';
import saveAs from 'file-saver';
import JSZip from 'jszip';
import { TestOutcome } from '../../enums/test-outcome.enum';
import { TestExtractor } from './test-extractor';
import { TestComparer } from './test-comparer';
import { AddressConverter } from '../../business/address-converter';

export default class TestAutomator {
    private testRenderer = new TestRenderer({});
    private extractor = new TestExtractor();
    private testComparer = new TestComparer();
    private ac = new AddressConverter();

    private testConfig: ITestConfig;
    private tests: ITestCase[] = [];
    private results: IExpectedResult[] = [];

    constructor() {}

    public get TestConfig(): ITestConfig {
        return this.testConfig;
    }

    public get Tests(): ITestCase[] {
        return this.tests;
    }

    public set Tests(tests: ITestCase[]) {
        this.tests = tests;
    }

    /**
     * Load all tests
     */
    public async load() {
        await this.loadTestConfig();
        await this.loadTests();
    }

    /**
     * Run cleanup for testMode
     */
    public async cleanup(): Promise<void> {
        throw new Error('Not implemented yet.');
    }

    /**
     * Render the selected test cases
     * @param testCases
     */
    public async renderTests(testCases: TestCase[]): Promise<void> {
        if (testCases && testCases.length > 0) {
            for (const test of this.tests) {
                test.outcome = TestOutcome.Unknown;
            }

            await this.testRenderer.renderTests(testCases, this.results);
        }
    }
    /**
     * Compare actual and expected result
     * @param  {TestCase[]} testCases
     */
    public async compareTests(testCases: TestCase[]): Promise<void> {
        for (const test of testCases) {
            await this.testComparer.compareResult(test, test.renderAreas);
            let infoBoxRangeString;
            let start = this.ac.getAddressFromCoords(
                test.renderAreas.infoBox.from.x + 1,
                test.renderAreas.infoBox.from.y + 1
            );
            let end = this.ac.getAddressFromCoords(
                test.renderAreas.infoBox.to.x + 1,
                test.renderAreas.infoBox.to.y + 1
            );
            infoBoxRangeString = `${test.component.toString()}!${start.text}:${
                end.text
            }`;

            await this.testComparer.setInfoBoxColorAndOutcome(
                test.outcome,
                infoBoxRangeString
            );
        }
    }
    /**
     * Extracting results to download them. Using the renderRangeString and the test-extractor
     */
    public async extractResults() {
        const casesToDownload = this.tests.filter((x) => x.renderRangeString);
        let results = new Array<IExpectedResult>();
        for (const oneCase of casesToDownload) {
            const result = await this.extractor.extractExpected(
                oneCase.renderRangeString,
                oneCase
            );
            results.push(result);
        }
        this.downloadResults(results);
    }

    /**
     * Download test results
     */
    async downloadResults(results: IExpectedResult[]): Promise<void> {
        const zip = new JSZip();
        const folder = zip.folder('results');

        for await (const key of Object.keys(results)) {
            const result = results[key];
            const blob = new Blob([JSON.stringify(result)], {
                type: 'text/json;charset=utf-8',
            });
            const name = `${result.component}${result.testId}.json`;

            folder.file(name, blob);
        }

        zip.generateAsync({ type: 'blob' }).then((content) => {
            saveAs(content, `results.zip`);
        });
    }

    /**
     * Load test config
     */
    private async loadTestConfig() {
        const res: AxiosResponse<ITestConfig> | void = await axios
            .get<ITestConfig>('/assets/testConfig.json')
            .catch((err) => console.error(err));

        if (res && res.data) {
            this.testConfig = new TestConfig(res.data);
        }
    }

    /**
     * Load all test
     */
    private async loadTests() {
        if (this.testConfig && this.testConfig.tests.length > 0) {
            for await (const test of this.testConfig.tests) {
                const testRes: AxiosResponse<ITestCase> | void = await axios
                    .get<ITestCase>(
                        '/assets/tests/'.concat(test).concat('.json')
                    )
                    .catch((err) => console.error(err));

                if (testRes && testRes.data) {
                    this.tests.push(new TestCase(testRes.data));

                    const resultRes: AxiosResponse<IExpectedResult> | void =
                        await axios
                            .get<IExpectedResult>(
                                '/assets/results/'.concat(test).concat('.json')
                            )
                            .catch((err) => console.error(err));

                    if (resultRes && resultRes.data) {
                        this.results.push(new ExpectedResult(resultRes.data));
                    }
                }
            }
        }
    }
}
