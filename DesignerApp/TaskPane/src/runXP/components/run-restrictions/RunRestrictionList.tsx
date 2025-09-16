import React, { JSX,  memo } from 'react';
import { filter, isEmpty, values } from 'lodash';
import RunRestrictionItem from './RunRestrictionItem';
import { Tile } from '../../../omni/tile';
import { RunRestriction as RestrictionSetting } from '@omniflow/omni-webapi';
const RunRestrictionList = ({
    runRestrictions,
    columns,
    clientId,
    deleteRestriction
}): JSX.Element => {

    const nonMetricColumns = filter(columns, ({ IsMetric, IsCommon }) =>
        IsMetric === false
    );
    const processRestrictions = () => {
        let processedObj = {};
        if (!isEmpty(runRestrictions)) {
            runRestrictions.forEach((runRestriction) => {
                const key = `${runRestriction.ColumnName}_${runRestriction.TableId}`;
                if (processedObj[key]) {
                    processedObj[key].ValueJson = `${processedObj[key].ValueJson}$${runRestriction.ValueJson}`;
                    runRestriction.ValueJson = '';
                }
                else {
                    processedObj[key] = runRestriction;
                }

            })
        }
        return values(processedObj);

    }
    let sortedRunRestrictions: Array<any> = processRestrictions();
    return (
        <div className="mb-31px">
            
            <div className="d-flex w-100">
                <div className="w-6">&nbsp;</div>
                <div className="w-25 f-12">Restriction</div>
                <div className="w-75 f-12">Column Name</div>
                    <div className="w-25 d-flex">
                    <span className="mx-auto f-12">Actions</span>
                    </div>
                </div>
            
            {!isEmpty(sortedRunRestrictions) &&
                sortedRunRestrictions.map((restriction, index) => (
                    
                    <RunRestrictionItem
                        tableId={restriction?.TableId}
                        key={`${index}_${restriction.TableId}`}
                        restriction={restriction}
                        columns={nonMetricColumns}
                        clientId={clientId}
                        levelId={index}
                        deleteRestriction={deleteRestriction}
                        />
                    
                ))}
            
        </div>
    );
};

export default memo(RunRestrictionList);
