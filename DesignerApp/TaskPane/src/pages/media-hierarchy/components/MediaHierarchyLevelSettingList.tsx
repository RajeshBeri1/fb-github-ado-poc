import React, { JSX, ChangeEvent, memo, useCallback, useContext, useState, useRef } from 'react';
import { find, sumBy, findIndex } from 'lodash';
import MediaHierarchyLevelSetting from './MediaHierarchyLevelSetting';
import useMediaHierarchyLevelSettings from '../hooks/useMediaHierarchyLevelSettings';
import useUpdateMediaHierarchyLevelSetting from '../hooks/useUpdateMediaHierarchyLevelSetting';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import useMediaHierarchyLevels from '../hooks/useMediaHierarchyLevels';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';
import useOrderMediaHierarchyLevelSettings from '../hooks/useOrderMediaHierarchyLevelSettings';
//import { alignCenter } from 'omni-ui/dist/icon/iconset-editor';
import { Icon } from '../../../omni/icon';
import { Tooltip } from '../../../omni/tooltip';
import '../../../pages/common-styles.css';
import useSortMediaHierarchyLevelSettings from '../hooks/useSortMediaHierarchyLevelSettings';
import useOnClickOutside from '../../../hooks/useOnClickOutside';
import { OmniCheckBoxInput } from '../../../omni/checkbox';
import Dialog from '../../../components/dialogs/Dialog';
import { SelectWithCommonFields } from '../../../components/form/select-with-common-fields';
import CustomIcon from '../../../components/custom-icon';
import { FlightRange as FlightRanges } from '@omniflow/omni-webapi';
import MediaHierarchySubTotalList from './MediaHierarchySubTotalList';
const FlightRange = Object.values(FlightRanges).filter(x => x !== 'None' && x !== FlightRanges.Annually && x !== FlightRanges.AnnuallyBroadcast);
export type TMediaHierarchyLevelSettingListProps = {
    clientId: string;
    levelId: number;
    columns: TColumn[];
    inflightOverlayColumns: TColumn[];
    metricColumns: TColumn[];
    level;
};

const SORT_TYPE = {
    ASC: 'asc',
    DESC: 'desc',
    CUSTOM: 'custom'
};

const MediaHierarchyLevelSettingList = ({
    clientId,
    levelId,
    columns,
    inflightOverlayColumns,
    metricColumns,
    level,
}: TMediaHierarchyLevelSettingListProps): JSX.Element => {
    const settings = useMediaHierarchyLevelSettings(levelId);
    const updateSetting = useUpdateMediaHierarchyLevelSetting();
    const countEnabledSettings = sumBy(settings, (s) => (s.Enabled ? 1 : 0));
    const changeOrder = useOrderMediaHierarchyLevelSettings();
    const sortOrder = useSortMediaHierarchyLevelSettings();
    const updateEnabled = useCallback(
        (event) => {
            const value = event.target.checked;

            settings.forEach((setting) => {
                updateSetting(levelId, setting.Id, {
                    Enabled: value,
                    SubLevels: null,
                });
            });
        },
        []
    );

    const [sortType, setSortType] = useState<string>('');
    const levels = useMediaHierarchyLevels();
    const appContext = useContext(AppContext);
    const DefaultMetric = appContext.defaultMetrics.NonBriefed;
    const DefaultMetricMediaBrief = appContext.defaultMetrics.Briefed;
    const levelData = find(levels, { Id: levelId });
    const isValidColumnForBriefedctcMetric = appContext.mediaBriefValidMetricColumns.includes(levelData.ColumnName);
    const filteredMetricColumns = levelData.TableId == DefaultMetric.MetricTableId ? metricColumns.filter(mc => DefaultMetricMediaBrief.MetricTableId != mc.TableId ||
        (isValidColumnForBriefedctcMetric && mc.Name.toLowerCase() == DefaultMetricMediaBrief.MetricColumnName) ||
        !appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : levelData.TableId == DefaultMetricMediaBrief.MetricTableId ? metricColumns.filter(mc => levelData.TableId == mc.TableId && appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : metricColumns;
    const filteredInflightOverlayColumns = levelData.TableId == DefaultMetric.MetricTableId ? inflightOverlayColumns.filter(mc => DefaultMetricMediaBrief.MetricTableId != mc.TableId || !appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : levelData.TableId == DefaultMetricMediaBrief.MetricTableId ? inflightOverlayColumns.filter(mc => levelData.TableId == mc.TableId && appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : inflightOverlayColumns;
    const [showSortModal, setShowSortModal] = useState<boolean>(false);
    const [showUpdateMetricModal, setShowUpdateMetricModal] = useState<boolean>(false);
    const [updatedSettingMetric, setUpdatedSettingMetric] = useState<TColumn>(null);
    const [updatedSettingFlightRange, setUpdatedSettingFlightRange] = useState<FlightRanges>(null);
    const [showBulkSubTotalModal, setShowBulkSubTotalModal] = useState<boolean>(false);

    const ref = useRef(null);
    const [showCheckIcon, setShowCheckIcon] = useState<string>('custom');

    const sort = (event, sortType: string) => {
        setSortType(sortType);
        switch (sortType) {
            case 'asc':
            case 'desc':
                sortOrder(levelId, sortType);
                setShowCheckIcon(event.target.id);
                break;
            default:
                setShowCheckIcon(event.target.id);
                break;
        }

    };

    const isCustomSelected = showCheckIcon === "custom";
    const isAscSelected = showCheckIcon === "asc";
    const isDescSelected = showCheckIcon === "desc";
    useOnClickOutside(ref, () => setShowSortModal(false));

    const updateSettingsMetricAndFlightRange = () => {
        // Only update if at least one value is set
        if (updatedSettingMetric == null && updatedSettingFlightRange == null) {
            setShowUpdateMetricModal(false);
            return;
        }

        const getPayload = () => {
            const payload: any = {};
            if (updatedSettingMetric) {
                payload.MetricColumnName = updatedSettingMetric.Name;
                payload.MetricTableId = updatedSettingMetric.TableId;
            }
            if (updatedSettingFlightRange) {
                payload.FlightRange = updatedSettingFlightRange;
            }
            return payload;
        };

        const payload = getPayload();
        if (Object.keys(payload).length > 0) {
            settings.forEach((setting) => {
                updateSetting(levelId, setting.Id, payload);
            });
        }

        setShowUpdateMetricModal(false);
        setUpdatedSettingMetric(null);
        setUpdatedSettingFlightRange(null);
    };

    const updateMetric = (event: ChangeEvent<HTMLSelectElement>) => {
        const i = Number(event.target.value);
        if (!isNaN(i) && metricColumns[i]) {
            setUpdatedSettingMetric(metricColumns[i]);
        } else {
            setUpdatedSettingMetric(null);
        }
    };

    const updateFlightRange = (event: ChangeEvent<HTMLSelectElement>) => {
        const value = event.target.value as FlightRanges;
        if (FlightRange.includes(value as any)) {
            setUpdatedSettingFlightRange(value);
        } else {
            setUpdatedSettingFlightRange(null);
        }
    };

    const metricColumnValue = findIndex(metricColumns, {
        Name: updatedSettingMetric?.Name,
        TableId: updatedSettingMetric?.TableId,
    });

    return (
        <>
            <div className="overflow-auto">
                <div className="is-relative">
                    <div id="moreActions" className={`dropdown dropdownMoreActions is-flex is-justify-content-end ${showSortModal ? 'is-active' : ''}`}>
                        <div className="dropdown-trigger">
                            <Tooltip>
                                <button className="icon" aria-haspopup="true" aria-controls="dropdown-menu" onClick={() => setShowSortModal(!showSortModal)}>
                                    <Icon icon-id="omni:interactive:filter" ></Icon>
                                </button>
                                <div slot="content">Sort settings</div>
                            </Tooltip>

                        </div>
                        <div className="dropdown-trigger">
                            <Tooltip>
                                <button className="icon" aria-haspopup="true" aria-controls="dropdown-menu" onClick={() => setShowUpdateMetricModal(!showUpdateMetricModal)}>
                                    <Icon icon-id="omni:informative:settings" ></Icon>
                                </button>
                                <div slot="content">Update Metrics And Flight Range</div>
                            </Tooltip>

                        </div>
                        <div className="dropdown-trigger">
                            <Tooltip>
                                <button className="icon" aria-haspopup="true" aria-controls="dropdown-menu" onClick={() => setShowBulkSubTotalModal(!showBulkSubTotalModal)}>
                                    <CustomIcon
                                        icon="sigma"
                                    />
                                </button>
                                <div slot="content">Add Bulk Sub Totals</div>
                            </Tooltip>

                        </div>
                    </div>
                    <div ref={ref as React.RefObject<HTMLDivElement>} slot="dropdown-menu" className={`dropdown-menu  ${showSortModal ? 'is-block fliter-menu-position' : ''}`} id="dropdown-menu" role="menu">
                        <div slot="dropdown-content" className="dropdown-content ">
                            <a className="dropdown-item is-flex is-align-items-center" onClick={(e) => sort(e, '')}><div className="w-5"><Icon className={`fill-color ${isCustomSelected ? '' : 'is-hidden'}`} icon-id="omni:informative:check"></Icon></div><span id="custom" className="is-size-6 p-1">Custom Order</span></a>
                            <a className="dropdown-item is-flex is-align-items-center" onClick={(e) => sort(e, SORT_TYPE.ASC)}><div className="w-5"><Icon className={`fill-color ${isAscSelected ? '' : 'is-hidden'}`} icon-id="omni:informative:check"></Icon></div><span id="asc" className="is-size-6 p-1">Name (a-z)</span></a>
                            <a className="dropdown-item is-flex is-align-items-center" onClick={(e) => sort(e, SORT_TYPE.DESC)}><div className="w-5"><Icon className={`fill-color ${isDescSelected ? '' : 'is-hidden'}`} icon-id="omni:informative:check"></Icon></div><span id='desc' className="is-size-6 p-1">Name (z-a)</span></a>

                        </div>
                    </div>
                    {showUpdateMetricModal && metricColumns?.length > 0 && (
                        <Dialog
                            title={`Bulk Update Metric And Flight Range`}
                            icon="interactive:add"
                            cancelButton={false}
                            okText="Done"
                            onOk={(e) => {
                                e.preventDefault();
                                updateSettingsMetricAndFlightRange();
                            }}
                            showDialog={showUpdateMetricModal && metricColumns?.length > 0}
                        >
                            <div className="columns">
                                <div className="column is-full">
                                    <label>Metric</label>
                                    <SelectWithCommonFields
                                        isLoading={false}
                                        placeHolder={'Metric'}
                                        columnValue={metricColumnValue}
                                        style={{ minWidth: 150 }}
                                        columns={metricColumns}
                                        onChange={updateMetric}
                                        isMetricSelection={true}
                                    />
                                </div>
                            </div>
                            <div className="columns">
                                <div className="column is-full">
                                    <label>Flight Range </label>
                                    <select
                                        className="select input text-capitalize no-shadow-select"
                                        value={updatedSettingFlightRange || '-1'}
                                        onChange={updateFlightRange}
                                        style={{ minWidth: 100 }}>
                                        <option disabled value="-1">
                                            Flight Range
                                        </option>
                                        {FlightRange.map((value, i) => (
                                            <option
                                                key={i}
                                                className="form-control text-capitalize"
                                                value={value}>
                                                {value}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                            </div>
                        </Dialog>
                    )}
                    <MediaHierarchySubTotalList
                        levelId={levelId}
                        flightRange={FlightRanges.FlightTotal}
                        metricColumns={filteredMetricColumns}
                        metricColumn={DefaultMetric}
                        showModal={showBulkSubTotalModal}
                        hideModal={() => setShowBulkSubTotalModal(false)}
                        />
                </div>
                <table
                    className="table table-layout-fixed is-fullwidth is-shadowless"
                    style={{ padding: 0 }}>
                    <thead>
                        <tr>
                            <th style={{ padding: '0' }} colSpan={0}></th>
                            <th style={{ padding: '0 8px 0 3px' }} colSpan={0}>
                                <OmniCheckBoxInput
                                    checked={
                                        countEnabledSettings === settings.length
                                    }
                                    onChange={(e: CustomEvent) => {
                                        return updateEnabled(e);
                                    }}
                                />
                            </th>
                            <th className="is-size-6" style={{ padding: '2px' }} colSpan={4}>
                                {countEnabledSettings === settings.length ? 'Unselect All' : 'Select All'}
                            </th>
                            <th className="is-size-6" style={{ padding: '2px' }} colSpan={3}>
                                Metric
                            </th>
                            <th className="is-size-6" style={{ padding: '2px' }} colSpan={3}>
                                Flight Range
                            </th>
                            <th className="is-size-6 text-center" style={{ padding: '2px' }} colSpan={3}>
                                Actions
                            </th>
                        </tr>
                    </thead>
                    <DragDropContext onDragEnd={(param) => changeOrder(param, levelId)}>
                        <Droppable
                            style={{ transform: 'none' }}
                            droppableId="droppable-rt-rows-settings">
                            {(provided) => (
                                <tbody ref={provided.innerRef}
                                    {...provided.droppableProps}>
                                    {settings.map((setting, index) => (
                                        <Draggable
                                            draggableId={`${setting.Id}`}
                                            key={setting.Id}
                                            index={index}
                                            isDragDisabled={sortType != ''}>
                                            {(provided, snapshot) => (
                                                <MediaHierarchyLevelSetting
                                                    key={setting.Id}
                                                    clientId={clientId}
                                                    levelId={levelId}
                                                    setting={setting}
                                                    columns={columns}
                                                    inflightOverlayColumns={filteredInflightOverlayColumns}
                                                    metricColumns={filteredMetricColumns}
                                                    isDragging={snapshot?.isDragging}
                                                    provided={provided}
                                                    sortType={sortType}
                                                    level={level}
                                                />
                                            )}
                                        </Draggable>
                                    ))}
                                    {provided.placeholder}

                                </tbody>
                            )}
                        </Droppable>
                    </DragDropContext>
                </table>

            </div>
        </>
    );
};

export default memo(MediaHierarchyLevelSettingList);
