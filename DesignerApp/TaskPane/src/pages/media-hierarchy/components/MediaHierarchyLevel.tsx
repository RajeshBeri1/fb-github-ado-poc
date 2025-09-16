import React, { ChangeEvent, memo, useEffect, useRef, useState, useContext, useMemo, useCallback, JSX } from 'react';
import { find, findIndex, orderBy, uniq, uniqBy } from 'lodash';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';

import { Tile } from '../../../omni/tile';
import { Icon } from '../../../omni/icon';
import Button from '../../../components/buttons/Button';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import MediaHierarchyLevelSettingList from './MediaHierarchyLevelSettingList';
import useRemoveMediaHierarchyLevel from '../hooks/useRemoveMediaHierarchyLevel';
import useUpdateMediaHierarchyLevel from '../hooks/useUpdateMediaHierarchyLevel';
import {
    MediaHierarchyDefinitionWithId,
    MediaHierarchyLevelWithId,
    MediaHierarchyLevelSettingWithId,
    MediaHierarchyInflightOverlayWithId
} from '../states/MediaHierarchyState';
import useAddMediaHierarchyLevelSetting from '../hooks/useAddMediaHierarchyLevelSetting';
import { dataApi } from '../../../lib/api';
import { StylingConfig } from '../../../components/styling/styling-config';
import Tools from '../../../business/tools';
import { SelectWithCommonFields } from '../../../components/form/select-with-common-fields';
import { SelectWithCommonFieldsTop } from '../../../components/form/select-with-common-fields-Top';
import '../../../pages/common-styles.css';
import useMediaHierarchyLevels from '../hooks/useMediaHierarchyLevels';
import { DataDTO, LevelDataDTO, MediaHierarchyInflightOverlay } from '@omniflow/omni-webapi';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import { CalendarHelper } from '../../../business/engine/helpers/calendar-helper';
import useNotification, { NotificationType } from '../../../components/notification/useNotification';

export type TMediaHierarchyLevelProps = {
    clientId: string;
    level: MediaHierarchyLevelWithId;
    columns: TColumn[];
    inflightOverlayColumns: TColumn[];
    metricColumns: TColumn[];
    isDragging?: boolean;
    provided: any;
    onSettingsLoaded?: (Definition: MediaHierarchyDefinitionWithId) => void;
};

const MediaHierarchyLevel = ({
    clientId,
    level,
    columns,
    inflightOverlayColumns,
    metricColumns,
    isDragging,
    provided,
    onSettingsLoaded = (): void => undefined,
}: TMediaHierarchyLevelProps): JSX.Element => {
    const isSubscribed = useRef(false);
    const loadedChannels = useRef([]);
    // Add cache and debounce
    const distinctDataCache = useRef({});
    const debounceTimer = useRef(null);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [currentDefinition, setCurrentDefinition] =
        useState<MediaHierarchyDefinitionWithId>(null);
    const [showSettings, setShowSettings] = useState<boolean>(false);
    const [showStyling, toggleStyling] = useState(false);
    const addSetting = useAddMediaHierarchyLevelSetting();
    const removeLevel = useRemoveMediaHierarchyLevel();
    const updateLevel = useUpdateMediaHierarchyLevel();
    const levels = useMediaHierarchyLevels();
    const appContext = useContext(AppContext);
    const pushNotification = useNotification();
    const [currentFilterValue, setCurrentFilterValue] = useState<string>('');

    // Memoized values
    const levelIndex = useMemo(() => levels.findIndex((l) => l.Id === level.Id), [levels, level.Id]);
    const columnValue = useMemo(() => findIndex(columns, { Name: level.ColumnName, TableId: level.TableId }), [columns, level.ColumnName, level.TableId]);

    // Memoized callbacks
    const getPreviousLevelsData = useCallback(() => {
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
    }, [levels, levelIndex]);

    const getPreviousInflightOverlaysData = useCallback((settings: string[], currentSetting: string) => {
        return levelIndex > 0
            ? {
                name: currentSetting,
                inflightOverlays: uniqBy(
                    levels[levelIndex - 1].Settings
                        .filter((s) => s.Enabled && settings.includes(s.Name) && s.InflightOverlays != null)
                        .flatMap(c => c.InflightOverlays)
                        .filter(c => c != null && c?.ColumnName != null)
                        .map(c => JSON.parse(JSON.stringify(c)) as MediaHierarchyInflightOverlay),
                    'ColumnName'
                )
            }
            : { name: currentSetting, inflightOverlays: [] };
    }, [levels, levelIndex]);

    const isPreviousInflightOverlaysExists = useCallback(() => {
        return levelIndex > 0
            ? levels[levelIndex - 1].Settings.filter((s) => s.Enabled && s.InflightOverlays != null)
                .flatMap(c => c.InflightOverlays).length > 0
            : false;
    }, [levels, levelIndex]);

    const setColumn = useCallback((event: ChangeEvent<HTMLSelectElement>) => {
        setShowSettings(false);
        const i = parseInt(event.target.value, 10);
        if (i < 0)
            updateLevel(level.Id, { ColumnName: null, TableId: null, Settings: [] });
        else {
            const { Name: ColumnName, TableId } = columns[i];
            updateLevel(level.Id, { ColumnName, TableId, Settings: [] });
        }
    }, [columns, level.Id, updateLevel]);

    const onChangeStyling = useCallback((...args) => {
        const levelCopy = Tools.deepCopy(level);
        const updatedLevel = Tools.setProperty(
            levelCopy,
            args[0],
            args[1] === '' ? null : args[1]
        );
        updateLevel(level.Id, {
            ColumnName: level.ColumnName,
            TableId: level.TableId,
            Styling: updatedLevel.Styling,
        });
    }, [level, updateLevel]);

    const toggleSettings = useCallback(() => setShowSettings((prev) => !prev), []);

    // loadChannels is not memoized because it depends on up-to-date state/refs
    // Helper to get a stable cache key
    const getStableFilterKey = useCallback(() => {
        const { ColumnName, TableId } = level;
        return JSON.stringify({ ColumnName, TableId, filter: getPreviousLevelsData() });
    }, [level, getPreviousLevelsData]);

    // Debounced loader
    const debouncedLoadChannels = useCallback(() => {
        if (debounceTimer.current) {
            clearTimeout(debounceTimer.current);
        }
        debounceTimer.current = setTimeout(() => {
            loadChannels().catch(console.error);
        }, 200);
    }, [level, getPreviousLevelsData]);

    const loadChannels = async () => {
        const { ColumnName, TableId } = level;
        if (!(ColumnName && TableId && !isLoading)) return;
        if (!isSubscribed.current) return;
        setIsLoading(true);
        const filterKey = getStableFilterKey();
        let data = distinctDataCache.current[filterKey];
        let previousLevelData: DataDTO[] = [];
        const calendarData = appContext.currentCalendarTemplateDetails?.Definition?.Configuration ? new CalendarHelper(clientId, appContext.currentCalendarTemplateDetails?.Definition?.Configuration) :
            appContext.flowchartTemplateDefinition?.Definition?.CalendarDefinition?.Definition?.Configuration ? new CalendarHelper(clientId, appContext.flowchartTemplateDefinition?.Definition?.CalendarDefinition?.Definition?.Configuration) : { calendarFrom: null, calendarTo: null };
        // Only call API if cache is empty
        if (!data) {
            appContext.setLevelLoading(true);
            try {
                ({ data } = await dataApi.dataGetDistinctData({
                    ParentColumnName: null,
                    ParentTableId: null,
                    ParentSelectedValue: null,
                    OmniClientId: clientId,
                    ColumnName,
                    TableId,
                    StartDate: calendarData.calendarFrom,
                    EndDate: calendarData.calendarTo,
                    Levels: getPreviousLevelsData(),
                }));
                distinctDataCache.current[filterKey] = data;
            } catch (e) {
                pushNotification("Error while getting data for media hierarchy level", NotificationType.DANGER);
            }
            if (getPreviousLevelsData().length > 0 && isPreviousInflightOverlaysExists()) {
                try {
                    ({ data: previousLevelData } = await dataApi.dataGetPreviousLevelDistinctData({
                        ParentColumnName: null,
                        ParentTableId: null,
                        ParentSelectedValue: null,
                        OmniClientId: clientId,
                        ColumnName,
                        TableId,
                        StartDate: calendarData.calendarFrom,
                        EndDate: calendarData.calendarTo,
                        Levels: getPreviousLevelsData(),
                    }));
                } catch (e) {
                    pushNotification("Error while getting data for inflight overlay mapping", NotificationType.DANGER);
                }
            } else {
                previousLevelData = [];
            }
            appContext.setLevelLoading(false);
            setCurrentFilterValue(JSON.stringify(getPreviousLevelsData()));
            data = !level.Settings?.length ? data.filter((d) => !!d).sort() : data;
            let isChannelExists = findIndex(loadedChannels.current, { ColumnName, TableId });
            if (isChannelExists > -1) {
                loadedChannels.current[isChannelExists].data = data;
            } else {
                loadedChannels.current.push({ ColumnName, TableId, data });
            }
        }
        // If data is present (from cache or just loaded), continue
        if (data && isSubscribed.current) {
            let initialSettings = [];
            let previousInflifghtOverlays: Array<{ name: string, inflightOverlays: MediaHierarchyInflightOverlay[] }> = [];
            data.forEach((Name, index) => {
                const foundSetting = find(level.Settings, { Name });
                const initialSetting = foundSetting
                    ? {
                        ...JSON.parse(JSON.stringify(foundSetting)),
                        Id: index + 1,
                        Order: foundSetting.Order,
                    }
                    : {};
                initialSettings.push(initialSetting);
                if (previousLevelData.length > 0) {
                    previousInflifghtOverlays.push(getPreviousInflightOverlaysData(previousLevelData.filter(c => c.Column1 == Name).flatMap(c => c.Column2), Name));
                }
            });
            initialSettings = orderBy(initialSettings, ['Order'], ['asc']);
            const mediaHierarchyDefinition = addSetting(
                level.Id,
                data,
                initialSettings,
                true,
                TableId,
                appContext.defaultMetrics,
                previousInflifghtOverlays
            );
            setCurrentDefinition(mediaHierarchyDefinition);
            setIsLoading(false);
        }
    };

    useEffect(() => {
        isSubscribed.current = true;
        const filterKey = getStableFilterKey();
        // Only call if cache is missing for this key
        if (!distinctDataCache.current[filterKey]) {
            debouncedLoadChannels();
        }
        return () => {
            isSubscribed.current = false;
            if (debounceTimer.current) clearTimeout(debounceTimer.current);
        };
    }, [clientId, level.ColumnName, level.TableId, JSON.stringify(getPreviousLevelsData())]);

    useEffect(() => {
        isSubscribed.current = true;
        if (!isLoading && currentDefinition && isSubscribed.current)
            onSettingsLoaded(currentDefinition);
        return () => {
            isSubscribed.current = false;
        };
    }, [isLoading, currentDefinition]);

    useEffect(() => {
        isSubscribed.current = true;
        const filterKey = getStableFilterKey();
        if (!isLoading && isSubscribed.current && !distinctDataCache.current[filterKey]) {
            debouncedLoadChannels();
        }
        return () => {
            isSubscribed.current = false;
            if (debounceTimer.current) clearTimeout(debounceTimer.current);
        };
    }, [levels, JSON.stringify(getPreviousLevelsData())]);

    return (
        <div
            className={isDragging ? 'isDragging' : undefined}
            ref={provided.innerRef}
            {...provided.draggableProps}>
            <div slot="subheader">&nbsp;</div>
            <div className="d-flex w-100 align-items-center">
                <div className="w-5">
                    <Icon
                        {...provided.dragHandleProps}
                        icon-id="omni:interactive:reorder"></Icon>
                </div>
                <div className="w-12">
                    <span className='text-md'>Level {level.Order + 1}</span>
                </div>
                <div className="w-60 mr-2">
                    <SelectWithCommonFieldsTop
                        isLoading={isLoading}
                        placeHolder={'Column'}
                        columnValue={columnValue}
                        columns={columns}
                        onChange={setColumn}
                        isMetricSelection={false}
                    />
                </div>
                <div className="w-23">
                    <div className=" d-flex is-justify-content-space-around">
                        <Button
                            className="icon"
                            tooltip="Format style"
                            disabled={!level.Settings.length}
                            onClick={() => toggleStyling(!showStyling)}>
                            <Icon icon-id={`omni:informative:theme`}></Icon>
                        </Button>
                        <Button
                            className="icon"
                            tooltip={showSettings ? 'Back' : 'Edit'}
                            disabled={!level.Settings.length}
                            onClick={toggleSettings}>
                            <Icon
                                icon-id={`omni:interactive:${showSettings ? 'left' : 'edit'}`}></Icon>
                        </Button>
                        <Button
                            className="icon"
                            tooltip="Delete"
                            onClick={() => removeLevel(level.Id)}>
                            <Icon icon-id="omni:interactive:trash"></Icon>
                        </Button>
                    </div>
                </div>
            </div>
            {showStyling && (
                <table className="table">
                    <tr>
                        <td colSpan={5}>
                            <StylingConfig
                                styling={level.Styling}
                                formKey={'Styling'}
                                onChange={onChangeStyling}
                            />
                        </td>
                    </tr>
                </table>
            )}
            {showSettings && (
                <MediaHierarchyLevelSettingList
                    clientId={clientId}
                    levelId={level.Id}
                    columns={columns}
                    inflightOverlayColumns={inflightOverlayColumns}
                    metricColumns={metricColumns}
                    level={level}
                />
            )}
        </div>
    );
};

export default memo(MediaHierarchyLevel);
