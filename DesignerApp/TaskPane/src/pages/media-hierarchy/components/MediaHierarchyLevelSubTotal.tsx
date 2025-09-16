import React, { JSX, ChangeEvent, memo, useState, useEffect } from 'react';
import * as API from '@omniflow/omni-webapi';
import { filter, findIndex } from 'lodash';

import Button from '../../../components/buttons/Button';
import { Icon } from '../../../omni/icon';
import { MediaHierarchySubTotalWithId } from '../states/MediaHierarchyState';
import useRemoveMediaHierarchyLevelSubTotal from '../hooks/useRemoveMediaHierarchyLevelSubTotal';
import useUpdateMediaHierarchyLevelSubTotal from '../hooks/useUpdateMediaHierarchyLevelSubTotal';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { StylingConfig } from '../../../components/styling/styling-config';
import Tools from '../../../business/tools';
import { SelectWithCommonFields } from '../../../components/form/select-with-common-fields';
import '../../../pages/common-styles.css';
import { OmniDropDownInput } from '../../../omni/dropdown';
const FlightRange = Object.values(API.FlightRange).filter(x => x !== 'None' && x !== API.FlightRange.Annually && x !== API.FlightRange.AnnuallyBroadcast);

export type TMediaHierarchyLevelSubTotalProps = {
    levelId: number;
    levelSettingId: number;
    subTotal: MediaHierarchySubTotalWithId;
    metricColumns: TColumn[];
};

const MediaHierarchyLevelSubTotal = ({
    levelId,
    levelSettingId,
    subTotal,
    metricColumns,
}: TMediaHierarchyLevelSubTotalProps): JSX.Element => {
    const removeSubTotal = useRemoveMediaHierarchyLevelSubTotal();
    const updateSubTotal = useUpdateMediaHierarchyLevelSubTotal();
    const [showStyling, setShowStyling] = useState(false);

    const updateMetric = (event: { target: { value: string } }) => {
        const i = parseInt(event.target.value, 10);
        if (i < 0)
            updateSubTotal(levelId, levelSettingId, subTotal.Id, {
                ColumnName: null,
                TableId: null,
            });

        const { Name: ColumnName, TableId } = metricColumns[i];
        updateSubTotal(levelId, levelSettingId, subTotal.Id, {
            ColumnName,
            TableId,
        });
    };
    const filteredFlightRange = filter(
        FlightRange,
        (v) => v !== API.FlightRange.None && v !== API.FlightRange.Annually && v !== API.FlightRange.AnnuallyBroadcast
    );

    const flightRangeOptions = [
        { id: '-1', value: 'Flight Range', disabled: true },
        ...filteredFlightRange.map((value) => ({
            id: value,
            value: value,
        })),
    ];

    const flightRangeValue = (() => {
        if (!subTotal.FlightRange) return [flightRangeOptions[0]];
        const opt = flightRangeOptions.find(option => option.id === subTotal.FlightRange);
        return opt ? [opt] : [flightRangeOptions[0]];
    })();

    const updateFlightRange = (event: CustomEvent) => {
        const selected = event.detail;
        if (!selected || selected.id === '-1') {
            updateSubTotal(levelId, levelSettingId, subTotal.Id, { FlightRange: null });
            return;
        }
        updateSubTotal(levelId, levelSettingId, subTotal.Id, { FlightRange: selected.id });
    };

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
        updateSubTotal(levelId, levelSettingId, subTotal.Id, {
            Styling: updatedSubTotal.Styling,
            TitleStyling: updatedSubTotal.TitleStyling,
        });
    };

    const metricColumnValue = findIndex(metricColumns, {
        Name: subTotal.ColumnName,
        TableId: subTotal.TableId,
    });

   
    // update the flightrange if it is valid for subtotal
    useEffect(() => {
        const value = subTotal.FlightRange != API.FlightRange.None && subTotal.FlightRange != API.FlightRange.Annually && subTotal.FlightRange != API.FlightRange.AnnuallyBroadcast ? subTotal.FlightRange : API.FlightRange.Weekly;
        updateSubTotal(levelId, levelSettingId, subTotal.Id, {
            FlightRange: value,
        });
    },[])
    return (
        <>
            <tr>
                <td className="is-size-6" style={{ padding: ' 0 30px 0 16px' }}>
                    <span className="text-sm-gray">Sub Total {subTotal.Order + 1}</span>
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
                    <OmniDropDownInput
                        className="w-100 text-capitalize dropdown-md"
                        style={{ maxWidth: 100 }}
                        options={flightRangeOptions}
                        value={flightRangeValue}
                        onValueChange={updateFlightRange}
                        placeholder="Flight Range"
                        hidefooter
                        searchindropdown={flightRangeOptions.length > 5}
                    />
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
                            removeSubTotal(levelId, levelSettingId, subTotal.Id)
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

export default memo(MediaHierarchyLevelSubTotal);
