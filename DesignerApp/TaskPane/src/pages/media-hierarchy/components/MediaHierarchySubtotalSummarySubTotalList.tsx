import React, { JSX, memo } from 'react';
import {  MediaHierarchySubTotalSummaryWithId, MediaHierarchySummarySubTotalWithId } from '../states/MediaHierarchyState';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import '../../../pages/common-styles.css';
import MediaHierarchySubTotalSummarySubTotal from './MediaHierarchySubTotalSummarySubTotal';
import useMediaHierarchyLevelSubTotalsSummarySubTotal from '../hooks/useMediaHierarchyLevelSubTotalSummarySubTotal';

export type TMediaHierarchySubTotalSummarySubTotalListProps = {
    levelId: number;
    levelSettingId: number;
    summary: MediaHierarchySubTotalSummaryWithId,
    addSummarySubTotal: any,
    columns: TColumn[],
    clientId: string,
    level: any,
};

const MediaHierarchySubTotalSummarySubTotalList = ({
    levelId,
    levelSettingId,
    summary,
    addSummarySubTotal,
    columns,
    clientId,
    level,
}: TMediaHierarchySubTotalSummarySubTotalListProps): JSX.Element => {
    const subTotalSummarySubTotals = useMediaHierarchyLevelSubTotalsSummarySubTotal(levelId, levelSettingId, summary.Id);
    return (
        <>
            <table className="table table-layout-fixed  is-fullwidth is-shadowless subtotal-summary" style={{ padding: '0px'}}>
                <thead>
                    <tr className="is-shadowless">
                        <th className="is-size-6 w-30" style={{ padding: 0 }}>
                            Column
                        </th>
                        <th className="is-size-6 w-30" style={{ padding: 0 }} >
                            Value
                        </th>
                        <th className="is-size-6 w-12 text-center" style={{ padding: 0 }}>
                            Action

                            {/*<div slot="end" >*/}
                            {/*    <Button className="is-outlined is-size-7 px-4" onClick={() => addSummarySubTotal(summary.Id)*/}
                            {/*    }>*/}
                            {/*        Add sub Setting*/}
                            {/*    </Button>*/}
                            {/*</div>*/}
                        </th>
                    </tr>
                </thead>
                {subTotalSummarySubTotals && subTotalSummarySubTotals.map((summarySubTotal: MediaHierarchySummarySubTotalWithId, index) => {
                    
                    return (
                        <MediaHierarchySubTotalSummarySubTotal
                            key={summarySubTotal.Id}
                            levelId={levelId}
                            levelSettingId={levelSettingId}
                            metricColumns={columns}
                            summaryId={summary.Id}
                            addSummarySubtotals={addSummarySubTotal}
                            subtotal={summarySubTotal}
                            disableRemove={subTotalSummarySubTotals.length===1}
                        />
                    )
                }
                )}
            </table>

        </>
    );
};

export default memo(MediaHierarchySubTotalSummarySubTotalList);



