import React, { JSX, ChangeEvent, memo, useState, useEffect , useRef} from 'react';
import { findIndex, find, isEmpty } from 'lodash';

import Button from '../../../components/buttons/Button';
import { Icon } from '../../../omni/icon';
import { MediaHierarchySubTotalSummarySettingWithId, MediaHierarchySubTotalSummaryWithId } from '../states/MediaHierarchyState';
import useRemoveMediaHierarchyLevelSubTotalSummarySettings from '../summaryHook/useRemoveMediaHierarchyLevelSubTotalSummarySettings';
import useUpdateMediaHierarchyLevelSubTotalSummarySettings from '../summaryHook/useUpdateMediaHierarchyLevelSubTotalSummarySettings';

import Tools from '../../../business/tools';
import { SelectWithCommonFields } from '../../../components/form/select-with-common-fields';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import '../../../pages/common-styles.css';
import { CalendarHelper } from '../../../business/engine/helpers/calendar-helper';
import { dataApi } from '../../../lib/api';
import useMediaHierarchyLevels from '../hooks/useMediaHierarchyLevels';
import { OmniDropDownInput } from '../../../omni/dropdown';
import { LevelDataDTO } from '@omniflow/omni-webapi';

export type TMediaHierarchySubTotalSummarySettingProps = {
    levelId: number;
    levelSettingId: number;
    summarySetting: MediaHierarchySubTotalSummarySettingWithId;
    summaryId: number,
    addSummarySetting: any,
    columns: any,
    columnValue: number,
    clientId: string,
    level: any,
    summary: MediaHierarchySubTotalSummaryWithId,
    disableRemove: boolean;
};

const MediaHierarchySubTotalSummarySettings = ({
    levelId,
    levelSettingId,
    summarySetting,
    summaryId,
    addSummarySetting,
    columns,
    columnValue,
    clientId,
    level,
    summary,
    disableRemove
}: TMediaHierarchySubTotalSummarySettingProps): JSX.Element => {
    const removeSubTotalSummarySettings = useRemoveMediaHierarchyLevelSubTotalSummarySettings();
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const isSubscribed = useRef(false);
    const loadedChannels = useRef([]);
    const appContext = React.useContext(AppContext);
    const levels = useMediaHierarchyLevels();
    const [settingValues, setSettingValues] = useState([]);
    const updateSummarySetting = useUpdateMediaHierarchyLevelSubTotalSummarySettings();
    const levelIndex = levels.findIndex((l) => l.Id === level.Id);
    const [stateDataLoaded, setStateDataLoaded] = useState(false);
    const [selectedSettingsValue, setSelectedSettingsValue] = useState([]);
    const [completeOptionsList, setCompleteOptionsList] = useState([])
    const [currentFilterValue, setCurrentFilterValue] = useState<string>('');
    const loadChannels = async () => {
        const { ColumnName, TableId } = summarySetting;

        if (ColumnName && TableId && !isLoading) {
            if (isSubscribed.current) {
                setIsLoading(true);
            }

            let data = find(loadedChannels.current, {
                ColumnName,
                TableId,
            });
            if (data) ({ data } = data);
            // create a new instance of the calendar helper class to use its methods and properties
            
            const calendarData = appContext.currentCalendarTemplateDetails?.Definition?.Configuration ? new CalendarHelper(clientId, appContext.currentCalendarTemplateDetails?.Definition?.Configuration) :
                appContext.flowchartTemplateDefinition?.Definition?.CalendarDefinition?.Definition?.Configuration ? new CalendarHelper(clientId, appContext.flowchartTemplateDefinition?.Definition?.CalendarDefinition?.Definition?.Configuration) : { calendarFrom: null, calendarTo: null };

            if (!data || currentFilterValue != JSON.stringify(getPreviousLevelsData())) {
                const levelData = find(levels, { Id: levelId });
                const selectedValue = find(levelData.Settings, { Id: levelSettingId });
                //const selectedValue = levelData.Settings[subLevelIndex];
                appContext.setLevelLoading(true);
                ({ data } = await dataApi.dataGetDistinctData({
                    ParentColumnName: levelData.ColumnName,
                    ParentTableId: levelData.TableId, 
                    ParentSelectedValue: selectedValue.Name,
                    OmniClientId: clientId,
                    ColumnName,
                    TableId,
                    StartDate: calendarData.calendarFrom,
                    EndDate: calendarData.calendarTo,
                    Levels: getPreviousLevelsData(),
                }));              

                appContext.setLevelLoading(false);
                setCurrentFilterValue(JSON.stringify(getPreviousLevelsData()));
                data = !level.Settings?.length ? data.filter((d) => !!d).sort() : data;
                let isChannelExists = findIndex(loadedChannels.current, {
                    ColumnName,
                    TableId,
                });
                if (isChannelExists > -1) {
                    loadedChannels.current[isChannelExists].data = data;
                } else {
                    loadedChannels.current.push({ ColumnName, TableId, data });
                }
            }

            if (data && isSubscribed.current) {
                setSettingValues(data);
                const list = data.map(
                    (settingValue) => ({
                        id: settingValue,
                        value: settingValue,
                        name: settingValue,
                    })
                )
                setCompleteOptionsList(list);
                if (summarySetting?.Values && !stateDataLoaded) {
                    setSelectedSettingsValue(list.filter((x) => {
                        return summarySetting.Values.split(';').includes(x.id)
                    }))
                }
                setIsLoading(false);
                setStateDataLoaded(true);
            }
        }
    };
    useEffect(() => {
        isSubscribed.current = true;
        if (!isLoading && isSubscribed.current)           
                loadChannels().catch(console.error);   
        return () => {
            isSubscribed.current = false;
        };
    }, [summarySetting.ColumnName]);

    const getPreviousLevelsData = () => {
        const previousLevelsData: LevelDataDTO[] = [];

        levels.forEach((l, index) => {
            if (index < levelIndex) {
                const { ColumnName, TableId, Settings } = l;
                const SelectedValues = Settings.filter((s) => s.Enabled).map(c => c.Name);
                if (SelectedValues.length > 0) {
                    previousLevelsData.push({ ColumnName, TableId, SelectedValues });
                }
            }
        });
        return previousLevelsData;
    }
    

    const subTotalSummarySettingColumnValue = findIndex(columns, {
        Name: summarySetting.ColumnName,
        TableId: summarySetting.TableId,
    });

    const updateSettingColumn = (event: ChangeEvent<HTMLSelectElement>) => {
        const i = parseInt(event.target.value, 10);
        if (i < 0)
            updateSummarySetting(levelId, levelSettingId, summaryId, summarySetting.Id, {
                ColumnName: null,
                TableId: null,
            });

        const { Name: ColumnName, TableId } = columns[i];
        updateSummarySetting(levelId, levelSettingId, summaryId, summarySetting.Id, {
            ColumnName,
            TableId,
        });
        setSelectedSettingsValue([]);
    };


    const updateSettingValue = (event) => {
        const values = event.detail;
        
        if (isEmpty(values)) {
            updateSummarySetting(levelId, levelSettingId, summaryId, summarySetting.Id, {
                ColumnName: null,
                TableId: null,
            })
        };

        const value = values.map((data) => data.value).join(";");
        updateSummarySetting(levelId, levelSettingId, summaryId, summarySetting.Id, {
            ColumnName: summarySetting.ColumnName,
            TableId: summarySetting.TableId,
            Values: value
        });
      
        setSelectedSettingsValue(values)
    };

    return (
        <>
            <tbody key={Tools.uuid()} style={{ zIndex: '0 !important' }} className="summary-tbody">
                <tr className="is-shadowless">
                    <td  style={{ padding: '0 4px' }}>
                        <SelectWithCommonFields
                            isLoading={isLoading}
                            placeHolder={'select column'}
                            columnValue={subTotalSummarySettingColumnValue}
                            columns={columns}
                            onChange={updateSettingColumn}
                            isMetricSelection={false}
                            includedMetrics={[]}
                        />
                    </td>
                    <td style={{ padding: '0 4px' }}>
                        <OmniDropDownInput
                            className="w-100 summary-dropdown dropdown-md"
                            onValueChange={(e)=>updateSettingValue(e)}
                            multiselect
                            options={completeOptionsList}
                            value={selectedSettingsValue}
                            selectall
                            disabled={isLoading}
                            hidefooter
                            placeholder={'select value'}
                            searchindropdown
                            typeahead
                            required></OmniDropDownInput>
                       
                    </td>
                    <td className="text-center w-12" style={{ padding: '0 4px' }}>
                        <Button
                            className="icon"
                            tooltip="Add column"
                            onClick={() => addSummarySetting(summaryId)}>
                            <Icon icon-id={`omni:interactive:plus`}></Icon>
                        </Button>
                        {!disableRemove && 
                            <Button
                                className="icon"
                                tooltip="Delete"
                                onClick={() =>
                                    removeSubTotalSummarySettings(levelId, levelSettingId, summarySetting.Id, summaryId)
                                }>
                                <Icon icon-id="omni:interactive:minus"></Icon>
                            </Button>
                            }
                       
                    </td>
                </tr>
            </tbody>

        </>
    );
};

export default memo(MediaHierarchySubTotalSummarySettings);



