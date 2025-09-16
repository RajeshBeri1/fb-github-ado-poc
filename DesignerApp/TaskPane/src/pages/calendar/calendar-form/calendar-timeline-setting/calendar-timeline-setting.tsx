import React, { useState } from 'react';
import { RegisterOptions, UseFormRegisterReturn } from 'react-hook-form';
//import { CalendarReportingTimeFrame } from '@omniflow/omni-webapi';
import { OmniIcon as OmniIcon } from '../../../../components/form/icon';
import { Input } from '../../../../components/form/input';
import '../../calendar-form/calendar-form.css';
import '../calendar-rows/calendar-rows.css';
import { OmniDropDownInput } from '../../../../omni/dropdown';
type CalendarDisplaySettingProps = {
    setValue: any;
    getValues: any;
    register: (
        name: string,
        options?: RegisterOptions
    ) => UseFormRegisterReturn;
};

enum CalendarReportingTimeFrameLocal {
    CurrentYear = "CurrentYear",
    LastYear = "LastYear",
    CustomiseYear = "CustomiseYear",
}

export const CalendarTimelineSetting: React.FC<
    CalendarDisplaySettingProps
> = ({ register, setValue, getValues }) => {
    /*const [currentSelection, setCurrentSelection] = useState(
        getValues('Definition.Configuration.IsReportingTimeFrame')
            ? displayBy.TYPE
            : displayBy.DATE
    );*/
    const [timeFrame, setTimeFrame] = useState<CalendarReportingTimeFrameLocal>(getValues('Definition.Configuration.IsReportingTimeFrame') ? getValues('Definition.Configuration.ReportingTimeFrame') : CalendarReportingTimeFrameLocal.CustomiseYear);
    const handleTypeChange = (timeFrame: CalendarReportingTimeFrameLocal) => {
        if (timeFrame != CalendarReportingTimeFrameLocal.CustomiseYear) {
            setValue('Definition.Configuration.ReportingTimeFrame', timeFrame, {
                shouldDirty: true,
            });
        }

        setTimeFrame(timeFrame);
        setValue(
            'Definition.Configuration.IsReportingTimeFrame',
            timeFrame !== CalendarReportingTimeFrameLocal.CustomiseYear,
            { shouldDirty: true }
        );
    };

    /* const handleSelectionChange = (selection: displayBy) => {
           setValue(
               'Definition.Configuration.IsReportingTimeFrame',
               selection === displayBy.TYPE,
               { shouldDirty: true }
           );
           setCurrentSelection(selection);
       };*/

    /*const renderRadioBtn = (value: displayBy) => {
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
    const calendarReportingTimeFrameOptions = [
        {
            value: 'Current Year',
            id: CalendarReportingTimeFrameLocal.CurrentYear,
        },
        {
            value: 'Last Year',
            id: CalendarReportingTimeFrameLocal.LastYear,
        },
        {
            value: 'Custom',
            id: CalendarReportingTimeFrameLocal.CustomiseYear,
        },
    ];

    const renderDateInput = (label: string, configName: string) => {
        return (
           
                <Input
                    labelStyle=""
                    label={label}
                    type="date"
                    register={register} 
                    registerKey={`Definition.Configuration.${configName}`}
                    registerOptions={{
                        valueAsDate: true,
                        disabled: timeFrame !== CalendarReportingTimeFrameLocal.CustomiseYear,
                    }}
                />
           
        );
    };

    return (
        <div className="d-flex flex-column">
            <div className="d-flex mb-2 mt-2">
                <OmniDropDownInput
                    label="Reporting Time Frame"
                    className="w-100"
                    options={calendarReportingTimeFrameOptions}
                    onValueChange={(e:CustomEvent) => 
                        handleTypeChange(
                            e.detail?.id as CalendarReportingTimeFrameLocal
                        )
                    }

                    value={[{
                        value: calendarReportingTimeFrameOptions.find((x) => timeFrame === x.id)?.value,
                        id: timeFrame
                    }]}
                    hidefooter
                >
                       
                </OmniDropDownInput>
            </div>

            <div className="d-flex mt-4 calendar-date">
                
               {renderDateInput('Start Date', 'CustomStartDate')}
               {renderDateInput('End Date', 'CustomEndDate')}
               
            </div>
        </div>
    );
};
