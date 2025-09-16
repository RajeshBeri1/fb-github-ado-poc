import React, { JSX, memo, useEffect, useState, ChangeEvent, useContext } from 'react';
import { Settings } from '../../../models/settings';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { dataApi } from '../../../lib/api';
import RunRestrictionSetting from './RunRestrictionSetting';
import { plainToClass } from 'class-transformer';
import { RunRestriction } from '@omniflow/omni-webapi';
import { OmniCheckBoxInput } from '../../../omni/checkbox';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import { CalendarHelper } from '../../../business/engine/helpers/calendar-helper';

export type RunRestrictionSettingListProps = {
    column: any;
    clientId: string;
    levelId: number,
    restriction: RunRestriction,
};

const RunRestrictionSettingList = ({
    column,
    clientId,
    levelId,
    restriction
}: RunRestrictionSettingListProps): JSX.Element => {
    const [settings, setSettings] = useState([] as Settings[]);
    const appContext = useContext(AppContext);

    useEffect(() => {
        let isSubscribed = true;

        // create a new instance of the calendar helper class to use its methods and properties
        const calendarData = appContext.currentCalendarTemplateDetails?.Definition?.Configuration ? new CalendarHelper(clientId, appContext.currentCalendarTemplateDetails?.Definition?.Configuration) : { calendarFrom: null, calendarTo: null };

        const loadChannels = async () => {
            const data = await dataApi.dataGetDistinctData({
                OmniClientId: clientId,
                ColumnName: column.Name,
                TableId: column.TableId,
                StartDate: calendarData.calendarFrom,
                EndDate: calendarData.calendarTo,

            });
            setSettings(data.data.map((value) => plainToClass(Settings, { Enabled: Boolean(column?.ValueJson?.split('$')?.includes(value)), Name: value })));
            //updateValue();
        };
        loadChannels().catch(console.error);

        return () => {
            isSubscribed = false;
        };
    }, [column]);

    useEffect(() => {
        updateValue();
    }, [settings]);

    const updateValue = () => {
        const selectedValues = settings.filter(c => c.Enabled).map(c => c.Name).join("$");
        restriction.ValueJson = selectedValues;
    }
    const updateValueJson = (checked: boolean, subLevelId: number) => {
        const value = checked;
        const setting = settings[subLevelId];
        setting.Enabled = value;
        const settingsData = [...settings];
        settingsData[subLevelId] = setting;
        setSettings(settingsData);
        updateValue();
    };
    const channelChecked = () => {
        let checkCounter = 0;
        settings.forEach((setting) => {
            if (setting.Enabled) {
                checkCounter++;
            }
        })

        return checkCounter != 0 ? checkCounter === settings?.length : false;

    };
    const handleChannelChange = (val) => {
        setSettings(settings.map((setting) => {
            setting.Enabled = Boolean(val?.target?.checked);
            return setting;
        }));
    }
    return (
        <>
            {settings.length > 0 && (
                <div className="overflow-auto">
                    <table className="table is-shadowless" style={{ padding: 0 }}>
                        <thead>
                            <tr className="d-flex flex-row w-100">
                                <th className="d-flex is-align-items-center p-3" style={{ padding: '10px' }}>
                                    <OmniCheckBoxInput checked={Boolean(channelChecked())} onClick={(e) => handleChannelChange(e)} />
                                </th>
                                <th className="d-flex is-align-items-center p-1 is-size-6 w-100" style={{ padding: '10px' }}>
                                    {Boolean(channelChecked()) ? 'Unselect All' : 'Select All'}
                                </th>
                                <th style={{ padding: '10px' }}></th>
                            </tr>
                        </thead>
                        <tbody className="d-flex flex-column">
                            {settings.map((setting, index) => (
                                <RunRestrictionSetting
                                    key={index}
                                    levelId={levelId}
                                    setting={setting}
                                    onChange={updateValueJson}
                                    subLevelId={index}
                                />
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </>
    );
};

export default memo(RunRestrictionSettingList);
