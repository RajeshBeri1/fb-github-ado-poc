import React, { useEffect } from 'react';

import { Title } from '../../../../components/form/title';
import { AppContext } from '../../../../taskpane/contexts/AppContext';
import { Tile } from '../../../../omni/tile';
import { CalendarTimelineSetting } from './calendar-timeline-setting/calendar-timeline-setting';
import { calendarTemplateApi } from '../../../../lib/api';
import '../../../components/runxp.css';
export const CalendarForm = () => {
    const {
        currentCalendarTemplateDetails,
        setCurrentCalendarTemplateDetails,
        flowchartTemplateDefinition,
        flowchartRunConfiguration, mode,
    } = React.useContext(AppContext);

    useEffect(() => {
        getCurrentCalendar();
    }, [flowchartRunConfiguration?.CalendarDefinition, mode, flowchartTemplateDefinition?.Definition?.CalendarDefinition]);

    const getCurrentCalendar = () => {
        // calendar already loaded?
        if (currentCalendarTemplateDetails?.Id) return;

        if (mode && flowchartRunConfiguration?.CalendarDefinition != null) {
            setCurrentCalendarTemplateDetails(flowchartRunConfiguration.CalendarDefinition);
        }
        else {
            const flowchartCalendarId =
                flowchartTemplateDefinition?.Definition?.CalendarDefinition
                    ?.TemplateId;
            if (flowchartCalendarId) {
                calendarTemplateApi
                    .calendarTemplateGet(flowchartCalendarId)
                    .then((x) => {
                        setCurrentCalendarTemplateDetails(x.data);
                    });
            }
        }
        // otherwise load and set by flowchart id
        
    };

    return (
        <div className="h-100">
            <div key="calendarConfigurator">
                {/*<div className="d-flex mb-3">
                    <Title text="General calendar settings" />
                </div>*/}
                <div className="d-flex">
                    <label className="w-100 mb-0  mt-12px">
                        <div className="">
                            <div className="input-label">Calendar type </div>
                            <div
                                className="input"
                                style={{ borderColor: 'transparent' }}>
                                {
                                    currentCalendarTemplateDetails.Definition
                                        ?.Configuration?.Type
                                }
                            </div>
                        </div>
                    </label>
                </div>
                {currentCalendarTemplateDetails?.Definition && (
                    <CalendarTimelineSetting
                        calendarDefinition={currentCalendarTemplateDetails}
                    />
                )}
            </div>
        </div>
    );
};
