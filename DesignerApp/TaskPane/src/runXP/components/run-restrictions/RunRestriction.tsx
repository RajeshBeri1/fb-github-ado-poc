import React, { JSX, memo } from 'react';
import * as API from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';
import { RunRestriction } from '@omniflow/omni-webapi';

import { Tile } from '../../../omni/tile';
import { Toolbar } from '../../../omni/toolbar';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import RunRestrictionList from './RunRestrictionList';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum'
import { groupBy } from 'lodash';
import Button from '../../../components/buttons/Button';
import '../../components/runxp.css';

const { logEvent } = useEventLogger(Module.RUNRESTRICTION);

const Restriction = ({
    runResctrictions: runRestrictions,
    setRunResctrictions: setRunResctrictions,
    clientId: clientId,
}): JSX.Element => {
    const { columns, columnsLoaded } = React.useContext(AppContext);

    const addRestriction = () => {
        logEvent({ action: Action.ADDFILTER });
        setRunResctrictions([
            ...runRestrictions,
            plainToClass(API.RunRestriction, {
                ColumnName: 'restriction',
                TableId: '',
            }),
        ]);
    };
    const deleteRestriction = (restriction: RunRestriction) => {
        setRunResctrictions(runRestrictions.filter((item) => item?.ColumnName != restriction?.ColumnName || item?.TableId != restriction?.TableId));
    }
    return (
        columnsLoaded?
        <>
            <Toolbar slot="header" className="runxp-toolbar">
                <p className="component-title">
                    Global Filters
                </p>
                <div slot="end">
                        <Button className='secondary medium' onClick={addRestriction}>Add Filter</Button>
                </div>
                </Toolbar>   
            
                <RunRestrictionList
                    runRestrictions={runRestrictions}
                    columns={columns}
                    clientId={clientId}
                    deleteRestriction={deleteRestriction}
                />
                 
        </>:<></>
    );
};
export default memo(Restriction);
