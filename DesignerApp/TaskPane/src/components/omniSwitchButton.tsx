import { AppContext } from '../taskpane/contexts/AppContext';
import { Switch } from "../omni/switch";
import React, { useCallback } from 'react';
import { useState, useContext } from 'react';
import useNotification, {
    NotificationType,
} from '../components/notification/useNotification';
import { Icon } from '../omni/icon';
import { Tooltip } from '../omni/tooltip';
import '../components/component.css';


export const OmniSwitchButton: React.FC = () => {
    const appContext = useContext(AppContext);
    const pushNotification = useNotification();
    const toggleSwitch = useCallback((event) => {
        appContext.setIsTrackFormatting(event.target.checked);
        if (event.target.checked===true) {
            pushNotification('(Track formatting) is ON. This might override Theme styles and Component styles.', NotificationType.WARNING, 4000);
            appContext.setUnsavedChanges((prevState) => ({ ...prevState, trackFormat: true }));
        }
    }, [appContext.setIsTrackFormatting])
    return (
        <>
            <div>
                <Switch onChange={(e) => toggleSwitch(e)} checked={appContext.isTrackFormatting} className="font-semi-bold" >
                    Track Formatting <span><Tooltip><Icon icon-id='omni:informative:error' className='is-size-6 add-position'></Icon> <div slot="content">Any formatting in excel would be tracked and applied when re-rendered (This will overwrite the configuration level changes)</div></Tooltip></span>
            </Switch>
                
            </div>
        </>
    )
}