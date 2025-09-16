import React, { JSX, ChangeEvent, memo, useContext, useEffect, useRef, useState } from 'react';
import { find, findIndex } from 'lodash';

  import { MediaHierarchySubLevelWithId, useMediaHierarchyTemplateState } from '../states/MediaHierarchyState';
import Button from '../../../components/buttons/Button';
import { Icon } from '../../../omni/icon';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import useRemoveMediaHierarchySubLevel from '../hooks/useRemoveMediaHierarchySubLevel';
import useUpdateMediaHierarchySubLevel from '../hooks/useUpdateMediaHierarchySubLevel';
import useAddMediaHierarchySubLevelSetting from '../hooks/useAddMediaHierarchySubLevelSetting';
import { dataApi } from '../../../lib/api';
import MediaHierarchySubLevelSettingList from './MediaHierarchySubLevelSettingList';
import { StylingConfig } from '../../../components/styling/styling-config';
import Tools from '../../../business/tools';
import { SelectWithCommonFields } from '../../../components/form/select-with-common-fields';
import { SelectWithCommonFieldsTop } from '../../../components/form/select-with-common-fields-Top';
import useMediaHierarchyLevels from '../hooks/useMediaHierarchyLevels';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import { LevelDataDTO } from '@omniflow/omni-webapi';
import useMediaHierarchySubLevels from '../hooks/useMediaHierarchySubLevels';
import { CalendarHelper } from '../../../business/engine/helpers/calendar-helper';

export type TMediaHierarchySubLevelProps = {
    clientId: string;
    levelId: number;
    levelSettingId: number;
    subLevel: MediaHierarchySubLevelWithId;
    columns: TColumn[];
    inflightOverlayColumns: TColumn[];
    metricColumns: TColumn[];
};

const MediaHierarchySubLevel = ({
    clientId,
    levelId,
    levelSettingId,
    subLevel,
    columns,
    inflightOverlayColumns,
    metricColumns,
}: TMediaHierarchySubLevelProps): JSX.Element => {
    const loadedChannels = useRef([]);
    const isSubscribed = useRef(false);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [showSettings, setShowSettings] = useState<boolean>(false);
    const [showStyling, toggleStyling] = useState(false);
    const addSetting = useAddMediaHierarchySubLevelSetting();
    const removeSubLevel = useRemoveMediaHierarchySubLevel();
    const updateSubLevel = useUpdateMediaHierarchySubLevel();
    const appContext = useContext(AppContext);
    const levels = useMediaHierarchyLevels();
    const levelIndex = levels.findIndex((l) => l.Id === levelId);
    // find subevels index within the level
    const settingIndex = levels[levelIndex].Settings.findIndex(
        (s) => s.Id === levelSettingId
    );
    const subLevelIndex = levels[levelIndex].Settings[settingIndex].SubLevels.findIndex((sl) => sl.Id === subLevel.Id);
    const subLevels = levels[levelIndex].Settings[settingIndex].SubLevels;
    const state = useMediaHierarchyTemplateState();
    const [currentFilterValue, setCurrentFilterValue] = useState<string>('');

    useEffect(() => {
        isSubscribed.current = true;
        loadChannels().catch(console.error);
        return () => {
            isSubscribed.current = false;
        };
    }, [clientId, subLevel.ColumnName, subLevel.TableId]);

    const loadChannels = async () => {
        const { ColumnName, TableId } = subLevel;

        if (ColumnName && TableId) {
            if (isSubscribed) {
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

            if (!data|| currentFilterValue != JSON.stringify(getPreviousLevelsData())) {
                const levelData = find(levels, { Id: levelId });
                const subLevelIndex = levelSettingId - 1;
                const selectedValue = levelData.Settings[subLevelIndex];
                ({ data } = await dataApi.dataGetDistinctData({
                    ParentColumnName: levelData.ColumnName, //'channel',
                    ParentTableId: levelData.TableId, //'f0867476-5d91-4fbf-8ebf-c5308309f4ea',
                    ParentSelectedValue: selectedValue.Name, //'TV',
                    OmniClientId: clientId,
                    ColumnName,
                    TableId,
                    StartDate: calendarData.calendarFrom,
                    EndDate: calendarData.calendarTo,
                    Levels:getPreviousLevelsData(),
                }));
                setCurrentFilterValue(JSON.stringify(getPreviousLevelsData()));
                data = data.filter((d) => !!d).sort();
                loadedChannels.current.push({ ColumnName, TableId, data });
            }

            if (data && isSubscribed) {
                const initialSettings = [];
                data.forEach((Name, index) => {
                    const foundSetting = find(subLevel.Settings, {
                        Name,
                    });
                    const initialSetting = foundSetting
                        ? {
                            ...JSON.parse(JSON.stringify(foundSetting)),
                            Id: index + 1,
                            Order: index,
                        }
                        : {};

                    initialSettings.push(initialSetting);
                });

                addSetting(
                    levelId,
                    levelSettingId,
                    subLevel.Id,
                    data,
                    initialSettings,
                    true,
                    TableId,
                    appContext.defaultMetrics,

                );
                setIsLoading(false);
            }
        }
    };

    const toggleSettings = () => {
        setShowSettings(!showSettings);
    };

    const setColumn = (event: ChangeEvent<HTMLSelectElement>) => {
        setShowSettings(false);

        const i = parseInt(event.target.value, 10);
        if (i < 0)
            updateSubLevel(levelId, levelSettingId, subLevel.Id, {
                ColumnName: null,
                TableId: null,
                Settings: [],
            });

        const { Name: ColumnName, TableId } = columns[i];
        updateSubLevel(levelId, levelSettingId, subLevel.Id, {
            ColumnName,
            TableId,
            Settings: [],
        });
    };

    const columnValue = findIndex(columns, {
        Name: subLevel.ColumnName,
        TableId: subLevel.TableId,
    });

    const onChangeStyling = (...args) => {
        const copy = Tools.deepCopy(subLevel);
        const updatedSubLevel = Tools.setProperty(
            copy,
            args[0],
            args[1] === '' ? null : args[1]
        );
        updateSubLevel(levelId, levelSettingId, subLevel.Id, {
            Styling: updatedSubLevel.Styling,
        });
    };

    const getPreviousLevelsData = () => {
        const previousLevelsData: LevelDataDTO[] = [];

        // get previous levels data
        levels.forEach((l, index) => {
            if (index < levelIndex) {
                const { ColumnName, TableId, Settings } = l;
                const SelectedValues = Settings.filter((s) => s.Enabled).map(c => c.Name);
                if (SelectedValues.length > 0) {
                    previousLevelsData.push({ ColumnName, TableId, SelectedValues });
                }
            }
        });

        // get previous sublevels data
        levels[levelIndex].Settings[settingIndex].SubLevels.forEach((sl, index) => {
            if (index < subLevelIndex) {
                const { ColumnName, TableId, Settings } = sl;
                const SelectedValues = Settings.filter((s) => s.Enabled).map(c => c.Name);
                if (SelectedValues.length > 0) {
                    previousLevelsData.push({ ColumnName, TableId, SelectedValues });
                }
            }
        });
        return previousLevelsData;
    }

    // reload data when previous levels data changes
    useEffect(() => {
        isSubscribed.current = true;
        if (!isLoading && isSubscribed.current)
            if (currentFilterValue != JSON.stringify(getPreviousLevelsData())) {
                loadChannels().catch(console.error);
            }

        return () => {
            isSubscribed.current = false;
        };
    }, [levels, state, subLevels]);

    return (
        <>
            <tr className="is-shadowless">
                <td className="is-size-6" style={{ padding: 0 }} colSpan={2}>
                    <span>Sub Level {subLevel.Order + 1}</span>
                </td>
                <td style={{ padding: 0 }} colSpan={6}>
                    <SelectWithCommonFieldsTop
                        isLoading={isLoading}
                        placeHolder={'Sublevel Column'}
                        columnValue={columnValue}
                        columns={columns}
                        onChange={setColumn}
                        isMetricSelection={false}
                    />
                </td>
                <td style={{ padding: 0 }} colSpan={2}>
                    <Button
                        className="icon"
                        tooltip="Format style"
                        disabled={!subLevel.Settings.length}
                        onClick={() => toggleStyling(!showStyling)}>
                        <Icon icon-id={`omni:informative:theme`} className="custom-width-height"></Icon>
                    </Button>
                    <Button
                        className="icon"
                        disabled={!subLevel.Settings.length}
                        tooltip={showSettings ? 'Back' : 'Edit'}
                        onClick={toggleSettings}>
                        <Icon
                            icon-id={`omni:interactive:${
                                showSettings ? 'left' : 'edit'
                                }`} className="custom-width-height"></Icon>
                    </Button>
                    <Button
                        className="icon"
                        tooltip="Delete"
                        onClick={() =>
                            removeSubLevel(levelId, levelSettingId, subLevel.Id)
                        }>
                        <Icon icon-id="omni:interactive:remove" className="custom-width-height"></Icon>
                    </Button>
                </td>
            </tr>
            {showStyling && (
                <tr>
                    <td colSpan={11}>
                        <StylingConfig
                            styling={subLevel.Styling}
                            formKey={'Styling'}
                            onChange={onChangeStyling}
                        />
                    </td>
                </tr>
            )}
            {showSettings && (
                <tr className="is-shadowless">
                    <td colSpan={10} style={{ padding: 0 }}>
                        <MediaHierarchySubLevelSettingList
                            levelId={levelId}
                            levelSettingId={levelSettingId}
                            subLevelId={subLevel.Id}
                            inflightOverlayColumns={inflightOverlayColumns}
                            metricColumns={metricColumns}
                        />
                    </td>
                </tr>
            )}
        </>
    );
};

export default memo(MediaHierarchySubLevel);
