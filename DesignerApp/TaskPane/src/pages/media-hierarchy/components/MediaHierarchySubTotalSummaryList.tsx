import React, { JSX, memo, useContext, useState, useEffect } from 'react';
import * as API from '@omniflow/omni-webapi';


import Button from '../../../components/buttons/Button';

import useMediaHierarchyLevelSubTotalSummary from '../hooks/useMediaHierarchyLevelSubTotalSummary';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import useMediaHierarchyLevels from '../hooks/useMediaHierarchyLevels';
import { find } from 'lodash';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import { Icon } from '../../../omni/icon';
import '../../../pages/common-styles.css'
import useAddMediaHierarchyLevelSubTotalSummary from '../hooks/useAddMediaHierarchyLevelSubTotalSummary';
import useAddMediaHierarchyLevelSummarySubTotals from '../hooks/useAddMediaHierarchyLevelSummarySubTotals';
import { MediaHierarchySubTotalSummarySettingWithId, MediaHierarchySubTotalSummaryWithId, MediaHierarchySummarySubTotalWithId } from '../states/MediaHierarchyState';
import useAddMediaHierarchyLevelSummarySettings from '../hooks/useAddMediaHierarchyLevelSummarySettings';
import useRemoveMediaHierarchyLevelSubTotalSummary from '../summaryHook/useRemoveMediaHierarchyLevelSubTotalSummary';
import { StylingConfig } from '../../../components/styling/styling-config';

import MediaHierarchySubtotalSummarySubTotalList from './MediaHierarchySubtotalSummarySubTotalList';
import MediaHierarchySubtotalSummarySettingList from './MediaHierarchySubtotalSummarySettingList';
import MediaHierarchySubTotalSummaryDisplayNameList from './MediaHierarchySubtotalSummaryDisplayNameList';

import useUpdateMediaHierarchyLevelSubTotalSummary from '../summaryHook/useUpdateMediaHierarchyLevelSubtotalSummary';
import Tools from '../../../business/tools';

export type TMediaHierarchyLevelSubTotalSummaryProps = {
    levelId: number;
    levelSettingId: number;
    flightRange?: API.FlightRange;
    metricColumns: TColumn[];
    metricColumn: TColumn;
    columns: TColumn[];
    clientId: string;
    level,
    levelSettingName,
};

const MediaHierarchyLevelSubTotalSummaryList = ({
    levelId,
    levelSettingId,
    flightRange = API.FlightRange.FlightTotal,
    metricColumns,
    metricColumn,
    columns,
    clientId,
    level,
    levelSettingName,
}: TMediaHierarchyLevelSubTotalSummaryProps): JSX.Element => {
    const subTotalSummary = useMediaHierarchyLevelSubTotalSummary(levelId, levelSettingId);
    const addSubTotalSummary = useAddMediaHierarchyLevelSubTotalSummary();
    const addSummarySubTotal = useAddMediaHierarchyLevelSummarySubTotals();
    const addSummarySettings = useAddMediaHierarchyLevelSummarySettings();
    const updateSubTotalSummary = useUpdateMediaHierarchyLevelSubTotalSummary();
    const removeSubTotalSummary = useRemoveMediaHierarchyLevelSubTotalSummary()
    const levels = useMediaHierarchyLevels();
    const appContext = useContext(AppContext);

    const levelData = find(levels, { Id: levelId });
    const [showStyling, setShowStyling] = useState(false);
    const [showAliasDisplayNames, setShowAliasDisplayNames] = useState<boolean>(false);

    const toggleStyling = () => {
        setShowStyling(!showStyling);
    };
    const onChangeStyling = (key: any, value: any, subTotalSummary: MediaHierarchySubTotalSummaryWithId) => {
        const subTotalCopy = Tools.deepCopy(subTotalSummary);
        const updatedSubTotal = Tools.setProperty(
            subTotalCopy,
            key,
            value === '' ? null : value
        );
        updateSubTotalSummary(levelId, levelSettingId, subTotalSummary.Id, {
            Styling: updatedSubTotal.Styling,
            TitleStyling: updatedSubTotal.TitleStyling,
        })

    };

    const addSummarySubtotals = (summaryId) => addSummarySubTotal(levelId, levelSettingId, 1, {
        FlightRange: flightRange,
        Column: metricColumn.Name,
        TableId: metricColumn.TableId,
    }, false, levelData.TableId, appContext.defaultMetrics, summaryId)

    const addSummarySetting = (summaryId) => addSummarySettings(levelId, levelSettingId, 1, {
        Values: '',
        Column: metricColumn.Name,
        TableId: metricColumn.TableId,
    }, false, levelData.TableId, appContext.defaultMetrics, summaryId)

    const toggleShowAliasDisplayNames = () => {
        setShowAliasDisplayNames(!showAliasDisplayNames);
    };


    const table = (summary: MediaHierarchySubTotalSummaryWithId) => {

        return (

            <>
                <div style={{ border: '1px solid #ccc', padding: '8px' }}>
                    <div className="d-flex is-justify-content-space-between">
                        <div className="is-size-6" >
                            <span className="text-sm-gray">Sub Total  {summary.Order + 1} </span>
                        </div>
                        <div className="is-size-6 is-flex-end">

                            <p className="text-sm-gray" style={{ textAlign: 'center' }}>Actions </p>
                            <div className="is-size-6">
                                <Button
                                    className="icon"
                                    tooltip="Format style"
                                    onClick={toggleStyling}>
                                    <Icon icon-id={`omni:informative:theme`}></Icon>
                                </Button>
                                <Button
                                    className="icon"
                                    tooltip="Delete"
                                    onClick={() =>
                                        removeSubTotalSummary(levelId, levelSettingId, summary.Id)
                                    }>
                                    <Icon icon-id="omni:interactive:delete"></Icon>
                                </Button>
                            </div>
                        </div>
                    </div>


                    <table className="table is-fullwidth is-shadowless" style={{ padding: '0px' }}>
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
                                            styling={summary.TitleStyling}
                                            formKey={'TitleStyling'}
                                            onChange={(k, v) => onChangeStyling(k, v, summary)}
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
                                            styling={summary.Styling}
                                            formKey={'Styling'}
                                            onChange={(k, v) => onChangeStyling(k, v, summary)}
                                        />
                                    </td>
                                </tr>
                            </>
                        )}
                        <MediaHierarchySubtotalSummarySettingList
                            levelId={levelId} levelSettingId={levelSettingId}
                            summary={summary} addSummarySetting={addSummarySetting}
                            columns={columns} clientId={clientId} level={level} />

                    </table>
                    <hr className="my-0"></hr>
                    <div className="is-size-5">Metrics</div>
                    <MediaHierarchySubtotalSummarySubTotalList
                        levelId={levelId} levelSettingId={levelSettingId}
                        summary={summary} addSummarySubTotal={addSummarySubtotals}
                        columns={metricColumns} clientId={clientId} level={level} />

                    <div className="">
                        <div className="is-flex is-size-5 mb-2 is-align-items-center"><label className="ml-0 mr-2 my-0">Alias Names </label>
                            <Button
                                className="icon"
                                tooltip={`${showAliasDisplayNames ? 'Hide alias names' : 'Show alias names'}`}
                                onClick={toggleShowAliasDisplayNames}>
                                <Icon icon-id={`omni:interactive:${showAliasDisplayNames ? 'up' : 'down'
                                    }`}></Icon>
                            </Button>
                        </div>
                            
                        {showAliasDisplayNames &&
                        <MediaHierarchySubTotalSummaryDisplayNameList
                            levelId={levelId} levelSettingId={levelSettingId} columns={metricColumns} 
                            summary={summary} clientId={clientId} level={level} levelSettingName={levelSettingName}
                            />
                        }
                    </div>


                </div>

            </>
        )
    };
    return (
        <div>
            <div slot="header" className="d-flex is-justify-content-space-between mb-4" >
                Sub total summary

                <div slot="end" >
                    <Button className="secondary is-size-7 px-4" onClick={() => addSubTotalSummary(levelId, levelSettingId, 1, {
                        FlightRange: flightRange,
                        Column: metricColumn.Name,
                        TableId: metricColumn.TableId,
                    }, false, levelData.TableId, appContext.defaultMetrics)
                    }>
                        Add sub total summary
                    </Button>
                </div>



            </div>

            {subTotalSummary && subTotalSummary.map((item: MediaHierarchySubTotalSummaryWithId) =>
                table(item)
            )}

        </div>
    );
};

export default memo(MediaHierarchyLevelSubTotalSummaryList);
