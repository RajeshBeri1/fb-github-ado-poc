import { ITestCase, TestCase } from './test-case';

export interface ITestRow {
    id: string;
    isSelected: boolean;
    testCase: TestCase;
}

export class TestRow implements ITestRow {
    id: string;
    isSelected: boolean;

    testCase: TestCase;

    constructor(testCase: TestCase | ITestCase) {
        this.testCase = testCase;
        this.id = `${testCase.component}${testCase.id}`;
        this.isSelected = false;
    }
}
