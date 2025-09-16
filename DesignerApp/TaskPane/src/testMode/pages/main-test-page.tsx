import React, { useEffect } from 'react';
import { TestOption } from '../../enums/test-option.enum';
import { Toolbar } from '../../omni/toolbar';
import { useState } from 'react';
import TestAutomator from '../business/test-automator';
import { ITestConfig } from '../models/test-config';
import { ITestCase } from '../models/test-case';
import Link from '../../components/Link';
import TestView from '../components/test-view';

export const MainTestPage = (props: any) => {
    const option = { optionName: TestOption.RunTests };
    const [state, setState] = useState(option);

    const [testConfig, setTestConfig] = React.useState<
        ITestConfig | undefined
    >();
    const [testCases, setTestCases] = React.useState<ITestCase[] | undefined>(
        []
    );
    const [testAutomator, setTestAutomator] = React.useState(
        new TestAutomator()
    );

    React.useEffect(() => {
        testAutomator.load().then(() => {
            setTestConfig(testAutomator.TestConfig);
            setTestCases(testAutomator.Tests);
        });
    }, []);

    return (
        <div>
            <Toolbar>
                <h3 slot="start" className="title">
                    Secret test mode
                </h3>
                <div className="toolbar-divider"></div>
                <Link to="/App">Back to normal mode</Link>
            </Toolbar>
            {testCases.length > 0 ? (
                <TestView
                    testCases={testCases}
                    testAutomator={testAutomator}></TestView>
            ) : null}
        </div>
    );
};
