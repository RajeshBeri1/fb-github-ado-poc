import { themeRulesStandardCreator } from '@fluentui/react';
import React, { useEffect } from 'react';
import { useState } from 'react';
import { TestComponent } from '../../enums/test-component.enum';
import { TestRow } from '../models/test-row';
import TestsTableRow from './test-table-row';

type Props = {
    rows: TestRow[];
    component: TestComponent;
    onSelectionChange: (selectedRows: TestRow[]) => void;
};

const TestTable = (props: Props) => {
    const [rows, setRows] = useState<TestRow[]>([]);
    const [selectAll, setSelectAll] = useState<boolean>();

    useEffect(() => {
        setRows(props.rows);
    }, [props.rows]);

    useEffect(() => {
        setSelectAll(false);
    }, [props.component]);

    function handleRowChange(item: TestRow): void {
        const updatedRows = rows.map((row) => {
            if (row.id === item.id) {
                row.isSelected = item.isSelected;
            }

            return row;
        });

        setRows(updatedRows);
        props.onSelectionChange(updatedRows);
    }

    function onSelectAll(event: any) {
        const newRows = rows.map((obj) => {
            return { ...obj, isSelected: !selectAll };
        });

        setSelectAll(!selectAll);

        setRows(newRows);
        props.onSelectionChange(newRows);
    }

    return (
        <table className="table" id="testCaseTable">
            <thead>
                <tr>
                    <td>
                        <input
                            type="checkbox"
                            checked={selectAll}
                            onChange={onSelectAll}></input>
                    </td>
                    <td>ID</td>
                    <td>Info</td>
                    <td>Outcome</td>
                </tr>
            </thead>
            <tbody>
                {rows.map((item) => {
                    return (
                        <TestsTableRow
                            item={item}
                            key={item.id}
                            onChange={handleRowChange}
                        />
                    );
                })}
            </tbody>
        </table>
    );
};

export default TestTable;
