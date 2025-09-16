import React, { JSX, ChangeEvent, memo, useState, useEffect } from 'react';
import * as API from '@omniflow/omni-webapi';
import { filter, findIndex } from 'lodash';

import Button from '../../../components/buttons/Button';
import { Icon } from '../../../omni/icon';
import { MediaHierarchySummarySubTotalWithId } from '../states/MediaHierarchyState';
import useRemoveMediaHierarchyLevelSubTotalSummarySubTotal from '../summaryHook/useRemoveMediaHierarchyLevelSubTotalSummarySubtotal';
import useUpdateMediaHierarchyLevelSubTotalSummarySubTotal from '../summaryHook/useUpdateMediaHierarchyLvelSubtotalSummarySubtotal';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { SelectWithCommonFields } from '../../../components/form/select-with-common-fields';
import '../../../pages/common-styles.css';
import { OmniDropDownInput } from "../../../omni/dropdown";
const FlightRange = Object.values(API.FlightRange).filter(x => x !== 'None' && x !== API.FlightRange.Annually && x !== API.FlightRange.AnnuallyBroadcast);

export type TMediaHierarchySubTotalSummarySubTotalProps = {
    levelId: number;
    levelSettingId: number;
    subtotal: MediaHierarchySummarySubTotalWithId;
    metricColumns: TColumn[];
    summaryId: number,
    addSummarySubtotals: any,
    disableRemove: boolean
};

const MediaHierarchySubTotalSummarySubTotal = ({
    levelId,
    levelSettingId,
    subtotal,
    metricColumns,
    summaryId,
    addSummarySubtotals,
    disableRemove
}: TMediaHierarchySubTotalSummarySubTotalProps): JSX.Element => {
    const removeSubTotalSummarySubTotal = useRemoveMediaHierarchyLevelSubTotalSummarySubTotal();
    const updateSubTotal = useUpdateMediaHierarchyLevelSubTotalSummarySubTotal();

    const updateMetric = (event: ChangeEvent<HTMLSelectElement>) => {
        const i = parseInt(event.target.value, 10);
        if (i < 0)
            updateSubTotal(levelId, levelSettingId, summaryId, subtotal.Id, {
                ColumnName: null,
                TableId: null,
            });

        const { Name: ColumnName, TableId } = metricColumns[i];
        updateSubTotal(levelId, levelSettingId, summaryId, subtotal.Id, {
            ColumnName,
            TableId,
        });
    };

    const updateFlightRange = (e: CustomEvent) => {
        const selected = e.detail?.value;
        // selected can be an array (multi-select) or a single object
        const value = Array.isArray(selected) && selected.length > 0
            ? selected[0].value
            : (selected?.value ?? selected);

        if (!value || !filteredFlightRange.includes(value)) {
            updateSubTotal(levelId, levelSettingId, summaryId, subtotal.Id, {
                FlightRange: null,
            });
        } else {
            updateSubTotal(levelId, levelSettingId, summaryId, subtotal.Id, {
                FlightRange: value,
            });
        }
    };

 

    const metricColumnValue = findIndex(metricColumns, {
        Name: subtotal.ColumnName,
        TableId: subtotal.TableId,
    });

    const filteredFlightRange = filter(
        FlightRange,
        (v) => v !== API.FlightRange.None && v !== API.FlightRange.Annually && v !== API.FlightRange.AnnuallyBroadcast
    );

    // update the flightrange if it is valid for subtotal
   /* useEffect(() => {
        const value = subtotal.FlightRange != API.FlightRange.None && subtotal.FlightRange != API.FlightRange.Annually && subtotal.FlightRange != API.FlightRange.AnnuallyBroadcast ? subtotal.FlightRange : API.FlightRange.Weekly;
        updateSubTotal(levelId, levelSettingId, subtotal.Id, {
            FlightRange: value,
        });
    }, [])*/
    const flightRangeOptions = [
        ...filteredFlightRange.map(value => ({
            label: value,
            value: value
        }))
    ];

    return (
        <>
            <tbody style={{ zIndex: '0' }} className="summary-tbody">
                <tr className="is-shadowless">

                <td  style={{ padding: '0 4px' }}>
                    <SelectWithCommonFields
                        isLoading={false}
                        placeHolder={'Subtotal Metric'}
                        columnValue={metricColumnValue}
                        columns={metricColumns}
                        onChange={updateMetric}
                        isMetricSelection={true}
                    />
                </td>
                <td  style={{ padding: '0 4px' }}>
                        <OmniDropDownInput
                            value={
                                subtotal.FlightRange
                                    ? flightRangeOptions.filter(opt => opt.value === subtotal.FlightRange)
                                    : []
                            }
                            options={flightRangeOptions}
                            onValueChange={updateFlightRange}
                            disabled={false}
                            placeholder="Flight Range"
                            className="text-capitalize w-100 dropdown-md"
                            hidefooter
                        />
                </td>
                <td style={{ padding: '0 4px' }} className=" text-center w-12">
                    <Button
                        className="icon"
                        tooltip="Add mertics"
                        onClick={() => addSummarySubtotals(summaryId)}
                    >
                        <Icon icon-id={`omni:interactive:plus`}></Icon>
                    </Button>
                    {!disableRemove &&
                        <Button
                            className="icon"
                            tooltip="Delete"
                            onClick={() =>
                                removeSubTotalSummarySubTotal(levelId, levelSettingId, subtotal.Id, summaryId)
                            }
                        >
                            <Icon icon-id="omni:interactive:minus"></Icon>
                        </Button>
                        }
                   
                </td>
            </tr>
            </tbody>
        </>
    );
};

export default memo(MediaHierarchySubTotalSummarySubTotal);



