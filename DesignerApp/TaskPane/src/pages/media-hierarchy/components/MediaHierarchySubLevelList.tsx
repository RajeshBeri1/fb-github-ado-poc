import React, { JSX,memo, useContext } from 'react';

import { Toolbar } from '../../../omni/toolbar';
import Button from '../../../components/buttons/Button';
import { Tile } from '../../../omni/tile';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import useMediaHierarchySubLevels from '../hooks/useMediaHierarchySubLevels';
import MediaHierarchySubLevel from './MediaHierarchySubLevel';
import useAddMediaHierarchySubLevel from '../hooks/useAddMediaHierarchySubLevel';
import useMediaHierarchyLevels from '../hooks/useMediaHierarchyLevels';
import { find } from 'lodash';
import { AppContext } from '../../../taskpane/contexts/AppContext';

export type TMediaHierarchySubLevelListProps = {
    clientId: string;
    levelId: number;
    levelSettingId: number;
    columns: TColumn[];
    inflightOverlayColumns: TColumn[];
    metricColumns: TColumn[];
};

const MediaHierarchySubLevelList = ({
    clientId,
    levelId,
    levelSettingId,
    columns,
    inflightOverlayColumns,
    metricColumns,
}: TMediaHierarchySubLevelListProps): JSX.Element => {
    const subLevels = useMediaHierarchySubLevels(levelId, levelSettingId);
    const addSubLevel = useAddMediaHierarchySubLevel();
    const levels = useMediaHierarchyLevels();
    const appContext = useContext(AppContext);
    const DefaultMetric = appContext.defaultMetrics.NonBriefed;
    const DefaultMetricMediaBrief = appContext.defaultMetrics.Briefed;
    const levelData = find(levels, { Id: levelId });
    const isValidColumnForBriefedctcMetric = appContext.mediaBriefValidMetricColumns.includes(levelData.ColumnName);
    const filteredMetricColumns = levelData.TableId == DefaultMetric.MetricTableId ? metricColumns.filter(mc => (isValidColumnForBriefedctcMetric && mc.Name.toLowerCase() == DefaultMetricMediaBrief.MetricColumnName) || DefaultMetricMediaBrief.MetricTableId != mc.TableId || !appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : levelData.TableId == DefaultMetricMediaBrief.MetricTableId ? metricColumns.filter(mc => levelData.TableId == mc.TableId && appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : metricColumns;
    const filteredInflightOverlayColumns = levelData.TableId == DefaultMetric.MetricTableId ? inflightOverlayColumns.filter(mc => DefaultMetricMediaBrief.MetricTableId != mc.TableId || !appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : levelData.TableId == DefaultMetricMediaBrief.MetricTableId ? inflightOverlayColumns.filter(mc => levelData.TableId == mc.TableId && appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : inflightOverlayColumns;
    const filteredColumns = levelData.TableId == DefaultMetric.MetricTableId ? columns.filter(mc => DefaultMetricMediaBrief.MetricTableId != mc.TableId || !appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : levelData.TableId == DefaultMetricMediaBrief.MetricTableId ? columns.filter(mc => levelData.TableId == mc.TableId && appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : columns;

    return (
        <Tile>
            <Toolbar slot="header">
                Sub Level
                <div slot="end" className="toolbar-divider"></div>
                <div slot="end">
                    <Button
                        className="secondary"
                        onClick={() => addSubLevel(levelId, levelSettingId)}>
                        Add sub level
                    </Button>
                </div>
            </Toolbar>
            <table
                className="table is-fullwidth is-shadowless table-layout-fixed"
                style={{ padding: 0 }}>
                <thead>
                    <tr className="is-shadowless">
                        <th className="is-size-6" style={{ padding: 0 }} colSpan={2}>
                            Sub Level
                        </th>
                        <th className="is-size-6" style={{ padding: 0 }} colSpan={6}>
                            Column Name
                        </th>
                        <th className="is-size-6" style={{ padding: 0 }} colSpan={2}>
                            Actions
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {subLevels?.map((subLevel) => (
                        <MediaHierarchySubLevel
                            key={subLevel.Id}
                            clientId={clientId}
                            levelId={levelId}
                            levelSettingId={levelSettingId}
                            subLevel={subLevel}
                            columns={filteredColumns}
                            inflightOverlayColumns={filteredInflightOverlayColumns}
                            metricColumns={filteredMetricColumns}
                        />
                    ))}
                </tbody>
            </table>
        </Tile>
    );
};

export default memo(MediaHierarchySubLevelList);
