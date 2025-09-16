export interface ITestConfig {
    testsDirectory: string;
    tests: string[];
}

export class TestConfig implements ITestConfig {
    testsDirectory: string;
    tests: string[];

    constructor(data: ITestConfig | TestConfig) {
        this.testsDirectory = data.testsDirectory;
        this.tests = data.tests;
    }
}
