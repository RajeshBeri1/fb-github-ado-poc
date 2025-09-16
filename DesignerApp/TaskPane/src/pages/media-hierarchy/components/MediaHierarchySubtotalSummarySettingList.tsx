import React, { JSX, memo } from 'react';

import {findIndex} from 'lodash';

import { MediaHierarchySubTotalSummarySettingWithId, MediaHierarchySubTotalSummaryWithId } from '../states/MediaHierarchyState';

import '../../../pages/common-styles.css';

import useMediaHierarchyLevelSubTotalsSummarySetting from '../hooks/useMediaHierarchyLevelSubTotalSummarySetting';
import MediaHierarchySubtotalSummarySettings from './MediaHierarchySubtotalSummarySettings';

export type TMediaHierarchySubTotalSummarySettingListProps = {
    levelId: number;
    levelSettingId: number;
    summary: MediaHierarchySubTotalSummaryWithId,
    addSummarySetting: any,
    columns: any,
    clientId: string,
    level: any,
};

const MediaHierarchySubTotalSummarySettingList = ({
    levelId,
    levelSettingId,
    summary,
    addSummarySetting,
    columns,
    clientId,
    level,
}: TMediaHierarchySubTotalSummarySettingListProps): JSX.Element => {
    const subTotalSummarySettings = useMediaHierarchyLevelSubTotalsSummarySetting(levelId, levelSettingId, summary.Id);
    return (
        <>
            <table className="table table-layout-fixed  is-fullwidth is-shadowless subtotal-summary" style={{ padding: '0' }}>
                <thead>
                    <tr className="is-shadowless">
                        <th className="is-size-6 w-30" style={{ padding: 0 }}>
                            Column
                        </th>
                        <th className="is-size-6 w-30" >
                            Value
                        </th>
                        <th className="is-size-6 w-12 text-center" style={{ padding: 0 }}>
                            Action
                        </th>
                    </tr>
                </thead>
                {subTotalSummarySettings && subTotalSummarySettings.map((summarySetting: MediaHierarchySubTotalSummarySettingWithId, index) => {
                    const columnValue = findIndex(columns, {
                        Name: summarySetting.ColumnName,
                        TableId: summarySetting.TableId,
                    });
                    return (
                        <MediaHierarchySubtotalSummarySettings
                            key={summarySetting.Id}
                            levelId={levelId}
                            levelSettingId={levelSettingId}
                            summarySetting={summarySetting}
                            summaryId={summary.Id}
                            addSummarySetting={addSummarySetting}
                            columns={columns}
                            columnValue={columnValue}
                            clientId={clientId}
                            level={level}
                            summary={summary}
                            disableRemove={subTotalSummarySettings.length === 1}
                        ></MediaHierarchySubtotalSummarySettings>
                    )
                }
                )}
            </table>

        </>
    );
};

export default memo(MediaHierarchySubTotalSummarySettingList);



