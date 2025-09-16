import React, { useState } from 'react';
import { TestOutcome } from '../../enums/test-outcome.enum';
import { Icon } from '../../omni/icon';
import { ITestCase, TestCase } from '../models/test-case';
import { TestRow } from '../models/test-row';

type Props = {
    item: TestRow;
    onChange: (item: TestRow) => void;
};

const TestsTableRow = (props: Props) => {
    const [item, setItem] = useState(props.item);

    const [icon, setIcon] = useState<string>('');
    const [iconClass, setIconClass] = useState<string>('');

    React.useEffect(() => {
        handleItemChange();
        setItem(props.item);
    }, [props.item]);

    function handleItemChange() {
        const outcome = item.testCase.outcome;

        let value = 'danger';
        let icon = 'error';

        switch (outcome) {
            case TestOutcome.Fail:
                value = 'danger';
                icon = 'error';
                break;
            case TestOutcome.Pass:
                value = 'success';
                icon = 'check';
                break;
            case TestOutcome.Review:
                value = 'warning';
                icon = 'help';
                break;
            case TestOutcome.Unknown:
                value = 'info';
                icon = 'notStarted';
                break;
            default:
                break;
        }

        setIcon(`omni:informative:${icon}`);
        setIconClass(`is-size-1 is-${value}`);
    }

    return (
        <tr id="checkboxRow" key={item.id}>
            <td>
                <input
                    type="checkbox"
                    checked={item.isSelected}
                    name="thisRow"
                    onChange={(e) => {
                        item.isSelected = e.target.checked;
                        setItem(item);
                        props.onChange(item);
                    }}
                />
            </td>
            <td>{item.id}</td>
            <td>
                <abbr
                    style={{ textDecoration: 'none' }}
                    title={item.testCase.description}>
                    {item.testCase.description.substring(0, 50)}
                </abbr>
            </td>
            <td>
                <Icon className={iconClass} icon-id={icon}></Icon>
            </td>
        </tr>
    );
};

export default TestsTableRow;
