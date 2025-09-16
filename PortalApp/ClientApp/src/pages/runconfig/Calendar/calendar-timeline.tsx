import React, { useState } from 'react';
import { DropDownInput } from '../../../omni-ui-components/dropdown';
import  './calendar-timeline.scss';
type CalendarDisplaySettingProps = {
    calendarDefinition: any;
};
enum CalendarReportingTimeFrameLocal {
    CurrentYear = "CurrentYear",
    LastYear = "LastYear",
    CustomiseYear = "CustomiseYear",
}

export const CalendarTimelineSetting: React.FC<
    CalendarDisplaySettingProps
> = ({ calendarDefinition }) => {
    const [timeFrame, setTimeFrame] = useState<CalendarReportingTimeFrameLocal>(calendarDefinition?.Definition?.Configuration?.IsReportingTimeFrame ? calendarDefinition.Definition?.Configuration?.ReportingTimeFrame : CalendarReportingTimeFrameLocal.CustomiseYear);

    const handleYearChange = (selection: CalendarReportingTimeFrameLocal) => {
        if (selection != CalendarReportingTimeFrameLocal.CustomiseYear) {
            calendarDefinition.Definition.Configuration.ReportingTimeFrame = selection;
        }

        setTimeFrame(selection);
        calendarDefinition.Definition.Configuration.IsReportingTimeFrame = selection !== CalendarReportingTimeFrameLocal.CustomiseYear;

    };
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
      
        return (
            <input
                className="w-50"
                name={label}
                type="date"               
            />
        );
    };

    return (
        <div className="d-flex flex-column mt-4">
            <DropDownInput label="Reporting time frame"
                options={calendarTimeFrameOptions}
                hidefooter
            />
            <div className="d-flex mt-4 calendar-date input-container">
                {renderDateInput('Custom Start Date', 'CustomStartDate')}
                {renderDateInput('Custom End Date', 'CustomEndDate')}
            </div>
        </div>
    );
};
