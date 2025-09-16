import * as React from 'react';
import { useContext, useEffect, useState } from 'react';
import { TestComponent } from '../../enums/test-component.enum';
import TestAutomator from '../business/test-automator';
import { TestCase } from '../models/test-case';
import { TestRow } from '../models/test-row';
import { Toolbar } from '../../omni/toolbar';
import { Button } from '@mui/material';
import TestTable from './test-table';

type Props = {
    testCases: TestCase[];
    testAutomator: TestAutomator;
};

const TestView = ({ testAutomator, testCases }: Props) => {
    const originalCases = JSON.parse(JSON.stringify(testCases));
    // set to TestComponent.All if multi-worksheet testing is wanted
    const [component, setComponent] = useState<TestComponent>(
        TestComponent.Header
    );

    const [rows, setRows] = useState<TestRow[]>([]);

    useEffect(() => {
        let newRows = originalCases.map((obj) => new TestRow(obj));
        setRows(newRows);
    }, []);

    useEffect(() => {
        const rows = testCases.map((x) => new TestRow(x));

        // comment out if multi-worksheet testing is wanted
        let newMatchingRows = rows.filter(
            (x) => x.testCase.component === component
        );

        // // comment in if multi-worksheed testing is wanted
        // let newMatchingRows = rows.filter(
        //     (x) =>
        //         x.testCase.component === component ||
        //         component === TestComponent.All
        // );

        setRows(newMatchingRows);
    }, [component]);

    function handleComponentSelect(e) {
        setComponent(e.target.value);
    }

    function handleSelectionChange(rows: TestRow[]) {
        setRows(rows);
    }

    async function runTests() {
        for (const testcase of testCases) {
            testcase.renderRangeString = undefined;
            testcase.renderAreas = undefined;
        }

        const cases = rows.filter((x) => x.isSelected).map((x) => x.testCase);
        await testAutomator.renderTests(cases);
        await testAutomator.compareTests(cases);

        setRows(rows.map((x) => new TestRow(x.testCase)));

        await unselectMasterCheckbox();
    }

    async function unselectMasterCheckbox() {
        const currentComponenet = component;
        // comment out if multi-worksheet testing is wanted
        if (currentComponenet !== TestComponent.Header) {
            setComponent(TestComponent.Header);
            setComponent(currentComponenet);
        } else {
            setComponent(TestComponent.Calendar);
            setComponent(currentComponenet);
        }

        // // comment in if multi-worksheet testing is wanted
        // if (currentComponenet !== TestComponent.All) {
        //     setComponent(TestComponent.All);
        //     setComponent(currentComponenet);
        // } else {
        //     setComponent(TestComponent.Calendar);
        //     setComponent(currentComponenet);
        // }
    }

    async function downloadResults() {
        testAutomator.extractResults();
    }

    return (
        <div>
            <br />
            <Toolbar>
                <h3 slot="start" className="title">
                    Choose Component
                </h3>
                <div className="toolbar-divider"></div>
                <select
                    className="select input"
                    onChange={handleComponentSelect}
                    // set to TestComponent.All if multi-worksheet testing is wanted
                    defaultValue={TestComponent.Header}>
                    {Object.keys(TestComponent).map((key, i) => (
                        <option
                            key={i}
                            className="form-control"
                            value={TestComponent[key]}>
                            {key}
                        </option>
                    ))}
                </select>
                <div className="toolbar-divider"></div>
                <Button onClick={runTests}>Run </Button>
                <div className="toolbar-divider"></div>
                <Button onClick={downloadResults}>Download Results</Button>
            </Toolbar>

            <br />
            <br />
            <h1>Available test cases:</h1>
            <br />
            <div>
                <TestTable
                    rows={rows}
                    onSelectionChange={handleSelectionChange}
                    component={component}
                />
            </div>
        </div>
    );
};
export default TestView;
