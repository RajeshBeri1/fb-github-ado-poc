import React, { useState } from 'react';
import { CalendarReportingTimeFrame } from '@omniflow/omni-webapi';
import * as DateFns from 'date-fns';
import moment from 'moment';

import { AppContext } from '../../../../../taskpane/contexts/AppContext';
import { OmniDropDownInput } from '../../../../../omni/dropdown';
import '../../../../components/runxp.css';
type CalendarDisplaySettingProps = {
    calendarDefinition: any;
};
enum CalendarReportingTimeFrameLocal {
    CurrentYear = "CurrentYear",
    LastYear = "LastYear",
    CustomiseYear = "CustomiseYear",
}
enum displayBy {
    TYPE,
    DATE,
}

export const CalendarTimelineSetting: React.FC<
    CalendarDisplaySettingProps
> = ({ calendarDefinition }) => {
    const appContext = React.useContext(AppContext);
    const { setCurrentCalendarTemplateDetails } = appContext;
   /* const [currentSelection, setCurrentSelection] = useState(
        calendarDefinition.Definition?.Configuration?.IsReportingTimeFrame
            ? displayBy.TYPE
            : displayBy.DATE
    );*/

    const handleDateChange = (configName: string, timeFrame: string) => {
        configName === 'CustomStartDate'
            ? (calendarDefinition.Definition.Configuration.CustomStartDate =
                timeFrame)
            : (calendarDefinition.Definition.Configuration.CustomEndDate =
                timeFrame);
        setCurrentCalendarTemplateDetails(
            JSON.parse(JSON.stringify(calendarDefinition))
        );
    };

   /*const handleSelectionChange = (selection: displayBy) => {
        if (selection === 0) {
            calendarDefinition.Definition.Configuration.IsReportingTimeFrame =
                true;
            setCurrentSelection(displayBy.TYPE);
        } else {
            calendarDefinition.Definition.Configuration.IsReportingTimeFrame =
                false;
            setCurrentSelection(displayBy.DATE);
        }
        setCurrentCalendarTemplateDetails(
            JSON.parse(JSON.stringify(calendarDefinition))
        );
    };*/
    const [timeFrame, setTimeFrame] = useState<CalendarReportingTimeFrameLocal>(calendarDefinition.Definition?.Configuration?.IsReportingTimeFrame ? calendarDefinition.Definition?.Configuration?.ReportingTimeFrame : CalendarReportingTimeFrameLocal.CustomiseYear);
    
    const handleYearChange = (selection: CalendarReportingTimeFrameLocal) => {
        if (selection != CalendarReportingTimeFrameLocal.CustomiseYear) {
            calendarDefinition.Definition.Configuration.ReportingTimeFrame = selection;
        }

        setTimeFrame(selection);
        calendarDefinition.Definition.Configuration.IsReportingTimeFrame = selection !== CalendarReportingTimeFrameLocal.CustomiseYear ;
        
        setCurrentCalendarTemplateDetails(
            JSON.parse(JSON.stringify(calendarDefinition))
        );
    };

   /* const renderRadioBtn = (value: displayBy) => {
        return (
            <input
                className="mr-5"
                type="radio"
                value={value}
                checked={currentSelection === value}
                onChange={() => handleSelectionChange(value)}
            />
        );
    };*/

    const dateAsString = new Date().toString();
    const timezone = dateAsString.match(/\(([^\)]+)\)$/)[1];
    console.log("timezone", timezone);

    function getLocaleTimeString(system) {
        return !system.now ?
            new Date().toLocaleTimeString() :
            system.now().toLocaleTimeString("en-US", { timeZone: "UTC" });
    }
    const calendarTimeFrameOptions = [
        {
            value: "Current Year",
            id: CalendarReportingTimeFrameLocal.CurrentYear,
        },
        {
            value: "Last Year",
            id: CalendarReportingTimeFrameLocal.LastYear,
        },
        {
            value: 'Custom',
            id: CalendarReportingTimeFrameLocal.CustomiseYear,
        },
    ]

      
    
    const renderDateInput = (label: string, configName: string) => {
        let dateType: string;

        if (configName === 'CustomStartDate') {
            const startDate = new Date(
                calendarDefinition.Definition?.Configuration?.CustomStartDate ||
                DateFns.startOfYear(new Date())
            );
            calendarDefinition.Definition.Configuration.CustomStartDate = moment.utc(startDate).format('YYYY-MM-DD');
            //DateFns.format(startDate, 'yyyy-MM-dd');
            dateType =
                calendarDefinition.Definition.Configuration.CustomStartDate;
        } else {
            const endDate = new Date(
                calendarDefinition.Definition?.Configuration?.CustomEndDate ||
                DateFns.endOfYear(new Date())
            );
            calendarDefinition.Definition.Configuration.CustomEndDate = moment.utc(endDate).format('YYYY-MM-DD');
            //DateFns.format(endDate, 'yyyy-MM-dd');
            dateType =
                calendarDefinition.Definition.Configuration.CustomEndDate;
        }

        return (
            <input
                className="w-25 p-1 mx-2"
                name={label}
                type="date"
                onChange={(e) => handleDateChange(configName, e.target.value)}
                value={dateType}
                disabled={timeFrame !== CalendarReportingTimeFrameLocal.CustomiseYear}
            />
        );
    };

    return (
        <div className="d-flex flex-column">
            <div className="">
             
                <label className="w-100 mt-12px">
                    <div>
                        <div className="input-label">Reporting time frame</div>
                    </div>
                    <OmniDropDownInput
                        className="w-100"
                        onValueChange={(e: CustomEvent) =>
                            handleYearChange(
                                e.detail.id as CalendarReportingTimeFrameLocal
                            )
                        }
                        value={[{
                            id: calendarDefinition.Definition?.Configuration
                                ?.ReportingTimeFrame,
                            value: calendarDefinition.Definition?.Configuration?.ReportingTimeFrame
                        }] }
                        options={calendarTimeFrameOptions}
                        hidefooter
                    />
                      
                

                    {/*<select
                        disabled={currentSelection !== displayBy.TYPE}
                        onChange={(e) =>
                            handleYearChange(
                                e.target.value as CalendarReportingTimeFrame
                            )
                        }
                        className="select input"
                        value={
                            calendarDefinition.Definition?.Configuration
                                ?.ReportingTimeFrame
                        }>
                        <option value={CalendarReportingTimeFrame.CurrentYear}>
                            Current Year
                        </option>
                        <option value={CalendarReportingTimeFrame.LastYear}>
                            Last Year
                        </option>
                    </select>*/}
                </label>
            </div>

            <div className="d-flex mt-4 calendar-date">
                {renderDateInput('Custom Start Date', 'CustomStartDate')}
                {renderDateInput('Custom End Date', 'CustomEndDate')}
            </div>
        </div>
    );
};
