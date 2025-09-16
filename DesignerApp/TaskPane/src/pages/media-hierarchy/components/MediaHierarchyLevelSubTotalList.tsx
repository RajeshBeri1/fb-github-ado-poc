import React, { JSX, memo,useContext } from 'react';
import * as API from '@omniflow/omni-webapi';

import { Toolbar } from '../../../omni/toolbar';
import Button from '../../../components/buttons/Button';
import { Tile } from '../../../omni/tile';
import useMediaHierarchyLevelSubTotals from '../hooks/useMediaHierarchyLevelSubTotals';
import useAddMediaHierarchyLevelSubTotal from '../hooks/useAddMediaHierarchyLevelSubTotal';
import MediaHierarchyLevelSubTotal from './MediaHierarchyLevelSubTotal';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import useMediaHierarchyLevels from '../hooks/useMediaHierarchyLevels';
import { find } from 'lodash';
import { AppContext } from '../../../taskpane/contexts/AppContext';


export type TMediaHierarchyLevelSubTotalListProps = {
    levelId: number;
    levelSettingId: number;
    flightRange?: API.FlightRange;
    metricColumns: TColumn[];
    metricColumn: TColumn;
};

const MediaHierarchyLevelSubTotalList = ({
    levelId,
    levelSettingId,
    flightRange = API.FlightRange.FlightTotal,
    metricColumns,
    metricColumn,
}: TMediaHierarchyLevelSubTotalListProps): JSX.Element => {
    const subTotals = useMediaHierarchyLevelSubTotals(levelId, levelSettingId);
    const addSubTotal = useAddMediaHierarchyLevelSubTotal();
    const levels = useMediaHierarchyLevels();
    const appContext = useContext(AppContext);
    const DefaultMetric = appContext.defaultMetrics.NonBriefed;
    const DefaultMetricMediaBrief = appContext.defaultMetrics.Briefed;
    const levelData = find(levels, { Id: levelId });
    const isValidColumnForBriefedctcMetric = appContext.mediaBriefValidMetricColumns.includes(levelData.ColumnName);
    const filteredMetricColumns = levelData.TableId == DefaultMetric.MetricTableId ? metricColumns.filter(mc => (isValidColumnForBriefedctcMetric && mc.Name.toLowerCase() == DefaultMetricMediaBrief.MetricColumnName) || DefaultMetricMediaBrief.MetricTableId != mc.TableId || !appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : levelData.TableId == DefaultMetricMediaBrief.MetricTableId ? metricColumns.filter(mc => levelData.TableId == mc.TableId && appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : metricColumns;
    return (
        <div>
            <div slot="header" className="d-flex is-justify-content-space-between mb-4">
                Sub Totals
               
                <div slot="end">
                    <Button
                        className="is-outlined is-size-7 px-4"
                        onClick={() =>
                            addSubTotal(levelId, levelSettingId, 1, {
                                FlightRange: flightRange,
                                Column: metricColumn.Name,
                                TableId: metricColumn.TableId,
                            }, false, levelData.TableId, appContext.defaultMetrics)
                        }>
                        Add sub total
                    </Button>
                </div>
            </div>
            <table
                className="table table-layout-fixed is-fullwidth is-shadowless"
                style={{ padding: 0 }}>
                <thead>
                    <tr className="no-shadow">
                        <th style={{ padding: 0 }}></th>
                        <th className="is-size-6" style={{ padding: 0 }}>
                            Metric
                        </th>
                        <th className="is-size-6" style={{ padding: 0 }}>
                            Flight Range
                        </th>
                        <th className="is-size-6 text-center" style={{ padding: 0 }}>
                            Action
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {subTotals?.map((subTotal) => (
                        <MediaHierarchyLevelSubTotal
                            key={subTotal.Id}
                            levelId={levelId}
                            levelSettingId={levelSettingId}
                            subTotal={subTotal}
                            metricColumns={filteredMetricColumns}
                        />
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default memo(MediaHierarchyLevelSubTotalList);
