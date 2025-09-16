import React, { JSX, ChangeEvent, memo, useState, useEffect } from 'react';
import * as API from '@omniflow/omni-webapi';
import { filter, findIndex } from 'lodash';

import Button from '../../../components/buttons/Button';
import { Icon } from '../../../omni/icon';
import { MediaHierarchySubTotalWithId } from '../states/MediaHierarchyState';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { StylingConfig } from '../../../components/styling/styling-config';
import Tools from '../../../business/tools';
import { SelectWithCommonFields } from '../../../components/form/select-with-common-fields';
import '../../../pages/common-styles.css';
const FlightRange = Object.values(API.FlightRange).filter(x => x !== 'None' && x !== API.FlightRange.Annually && x !== API.FlightRange.AnnuallyBroadcast);

export type TMediaHierarchySubTotalProps = {
    metricColumns: TColumn[];
    index: number;
    update: (index: number, data: Partial<MediaHierarchySubTotalWithId>) => void;
    subTotal: MediaHierarchySubTotalWithId;
    remove: (index:number) => void;
};

const MediaHierarchySubTotal = ({
    metricColumns,
    index,
    update: updateSubTotal,
    subTotal,
    remove,
}: TMediaHierarchySubTotalProps): JSX.Element => {
    const [showStyling, setShowStyling] = useState(false);

    const updateMetric = (event: ChangeEvent<HTMLSelectElement>) => {
        const i = parseInt(event.target.value, 10);
        if (i < 0)
            updateSubTotal(index, {
                ColumnName: null,
                TableId: null,
            });

        const { Name: ColumnName, TableId } = metricColumns[i];
        updateSubTotal(index, {
            ColumnName,
            TableId,
        });
    };

    const updateFlightRange = (event: ChangeEvent<HTMLSelectElement>) => {
        const value = event.target.value as API.FlightRange;
        if (!FlightRange.includes(value as any))
            updateSubTotal(index, {
                FlightRange: null,
            });

        updateSubTotal(index, {
            FlightRange: value,
        });
    };

    const removeSubTotal = () => {
        remove(index);
    }

    const toggleStyling = () => {
        setShowStyling(!showStyling);
    };

    const onChangeStyling = (...args) => {
        const subTotalCopy = Tools.deepCopy(subTotal);
        const updatedSubTotal = Tools.setProperty(
            subTotalCopy,
            args[0],
            args[1] === '' ? null : args[1]
        );
        updateSubTotal(index, {
            Styling: updatedSubTotal.Styling,
            TitleStyling: updatedSubTotal.TitleStyling,
        });
    };

    const metricColumnValue = findIndex(metricColumns, {
        Name: subTotal.ColumnName,
        TableId: subTotal.TableId,
    });

    const filteredFlightRange = filter(
        FlightRange,
        (v) => v !== API.FlightRange.None && v !== API.FlightRange.Annually && v !== API.FlightRange.AnnuallyBroadcast
    );

    // update the flightrange if it is valid for subtotal
    useEffect(() => {
        const value = subTotal.FlightRange != API.FlightRange.None && subTotal.FlightRange != API.FlightRange.Annually && subTotal.FlightRange != API.FlightRange.AnnuallyBroadcast ? subTotal.FlightRange : API.FlightRange.Weekly;
        updateSubTotal(index, {
            FlightRange: value,
        });
    }, [])
    return (
        <>
            <tr>
                <td className="is-size-6" style={{ padding: ' 0 30px 0 16px' }}>
                    <span className="text-sm-gray">Sub Total {index+1}</span>
                </td>
                <td className="is-size-6" style={{ padding: '0 4px' }}>
                    <SelectWithCommonFields
                        isLoading={false}
                        columnValue={metricColumnValue}
                        style={{ maxWidth: 100 }}
                        columns={metricColumns}
                        onChange={updateMetric}
                        isMetricSelection={true}
                    />
                </td>
                <td className="is-size-6" style={{ padding: '0 4px' }}>
                    <select
                        className="select input text-capitalize no-shadow-select"
                        value={subTotal.FlightRange || '-1'}
                        onChange={updateFlightRange}
                        style={{ maxWidth: 100 }}>
                        <option disabled value="-1">
                            Flight Range
                        </option>
                        {filteredFlightRange.map((value, i) => (
                            <option
                                key={i}
                                className="form-control text-capitalize"
                                value={value}>
                                {value}
                            </option>
                        ))}
                    </select>
                </td>
                <td style={{ padding: 0 }} className="text-center">
                    <Button
                        className="icon"
                        tooltip="Format style"
                        onClick={toggleStyling}>
                        <Icon icon-id={`omni:informative:theme`} className="custom-width-height"></Icon>
                    </Button>
                    <Button
                        className="icon"
                        tooltip="Delete"
                        onClick={() =>
                            removeSubTotal()
                        }>
                        <Icon icon-id="omni:interactive:remove" className="custom-width-height"></Icon>
                    </Button>
                </td>
            </tr>
            {showStyling && (
                <>
                    <thead>
                        <tr>
                            <th colSpan={4} className="p-0">Title</th>
                        </tr>
                    </thead>
                    <tr>
                        <td colSpan={4} className="p-0">
                            <StylingConfig
                                styling={subTotal.TitleStyling}
                                formKey={'TitleStyling'}
                                onChange={onChangeStyling}
                            />
                        </td>
                    </tr>
                    <thead>
                        <tr>
                            <th colSpan={4} className="p-0">Sub totals</th>
                        </tr>
                    </thead>
                    <tr>
                        <td colSpan={4} className="p-0">
                            <StylingConfig
                                styling={subTotal.Styling}
                                formKey={'Styling'}
                                onChange={onChangeStyling}
                            />
                        </td>
                    </tr>
                </>
            )}
        </>
    );
};

export default memo(MediaHierarchySubTotal);
