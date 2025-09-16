import React, { memo, ChangeEvent, useState, useRef, JSX } from 'react';
import * as API from '@omniflow/omni-webapi';

import Button from '../../../components/buttons/Button';
import { Icon } from '../../../omni/icon';
import { MediaHierarchyLevelSettingWithId } from '../states/MediaHierarchyState';
import useUpdateMediaHierarchyLevelSetting from '../hooks/useUpdateMediaHierarchyLevelSetting';
import MediaHierarchyLevelSubTotalList from './MediaHierarchyLevelSubTotalList';
import MediaHierarchyLevelInflightOverlayList from './MediaHierarchyLevelInflightOverlayList';
import MediaHierarchySubLevelList from './MediaHierarchySubLevelList';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { findIndex } from 'lodash';
import { StylingConfig } from '../../../components/styling/styling-config';
import Tools from '../../../business/tools';
import CustomIcon from '../../../components/custom-icon';
import Dialog from '../../../components/dialogs/Dialog';
import { SelectWithCommonFields } from '../../../components/form/select-with-common-fields';
import { Tooltip } from '../../../omni/tooltip';
import MediaHierarchySubTotalSummaryList from './MediaHierarchySubTotalSummaryList';

import useOnClickOutside from '../../../hooks/useOnClickOutside';
import { ref } from 'omni-ui';
import { OmniCheckBoxInput } from '../../../omni/checkbox';
import { OmniDropDownInput } from '../../../omni/dropdown';
import "../../../pages/common-styles"

const FlightRange = Object.values(API.FlightRange).filter(x => x !== 'None' && x !== API.FlightRange.Annually && x !== API.FlightRange.AnnuallyBroadcast);

export type TMediaHierarchyLevelSettingProps = {
    clientId: string;
    levelId: number;
    setting: MediaHierarchyLevelSettingWithId;
    columns: TColumn[];
    inflightOverlayColumns: TColumn[];
    metricColumns: TColumn[];
    isDragging?: boolean;
    provided: any;
    sortType?: string;
    level,
};

const MediaHierarchyLevelSetting = ({
    clientId,
    levelId,
    setting,
    columns,
    inflightOverlayColumns,
    metricColumns,
    isDragging,
    provided,
    sortType,
    level,
}: TMediaHierarchyLevelSettingProps): JSX.Element => {
    const updateSetting = useUpdateMediaHierarchyLevelSetting();
    const [showSubTotals, setShowSubTotals] = useState<boolean>(false);
    const [showSubTotalsSummary, setShowSubTotalsSummary] = useState<boolean>(false);
    const [showInflightOverlays, setShowInflightOverlays] = useState<boolean>(false);
    const [showStyling, setShowStyling] = useState(false);
    const [showInflightOverStyling, setShowInflightOverStyling] =
        useState(false);
    const [showSubSettings, setShowSubSettings] = useState<boolean>(false);
    const [showSortModal, setShowSortModal] = useState<boolean>(false);
    const ref = useRef(null);
    useOnClickOutside(ref, () => setShowSortModal(false));
    
    const updateEnabled = (event) => {
        const value = event.target.checked;

        setShowSubSettings(false);
        updateSetting(levelId, setting.Id, { Enabled: value, SubLevels: null });
    };

    const inflightOverlayColumnsNew = [
        //{
        //DisplayName: "None",
        //IsCommon: true,
        //IsCurrency: false,
        //IsMetric: false,
        //Name: "None",
        //TableId: '00000000-0000-0000-0000-000000000001',// '00000000-0000-0000-0000-000000000001'
        //TableName: null

        //},
        {
        DisplayName: "Custom Flight Range",
        IsCommon: true,
        IsCurrency: false,
        IsMetric: false,
        Name: "CustomFlightRange",
        TableId: '00000000-0000-0000-0000-000000000002',// '00000000-0000-0000-0000-000000000002'
        TableName: null

        },
        ...inflightOverlayColumns];

    const metricColumnsNew = [{
        DisplayName: "None",
        IsCommon: true,
        IsCurrency: false,
        IsMetric: true,
        Name: "None",
        TableId: '00000000-0000-0000-0000-000000000001',
        TableName: null

    }, ...metricColumns];

    const updateInflightOverlay = (event: ChangeEvent<HTMLSelectElement>) => {
        const i = parseInt(event.target.value, 10);
        if (i < 0)
            updateSetting(levelId, setting.Id, {
                InflightOverlayColumnName: null,
                InflightOverlayTableId: null,
            });

        const {
            Name: InflightOverlayColumnName,
            TableId: InflightOverlayTableId,
        } = inflightOverlayColumnsNew[i];
        updateSetting(levelId, setting.Id, {
            InflightOverlayColumnName,
            InflightOverlayTableId,
        });
    };

    const updateMetric = (event: { target: { value: string } }) => {
        const i = parseInt(event.target.value, 10);
        if (i < 0) {
            updateSetting(levelId, setting.Id, {
                MetricColumnName: null,
                MetricTableId: null,
            });
            return;
        }
        const { Name: MetricColumnName, TableId: MetricTableId } = metricColumnsNew[i];
        updateSetting(levelId, setting.Id, { MetricColumnName, MetricTableId });
    };

    const updateFlightRange = (e: CustomEvent) => {
        const selected = e.detail?.value;
        // selected can be an array (multi-select) or a single object
        const value = Array.isArray(selected) && selected.length > 0
            ? selected[0].value
            : (selected?.value ?? selected);

        if (!value || !FlightRange.includes(value)) {
            updateSetting(levelId, setting.Id, { FlightRange: null });
        } else {
            updateSetting(levelId, setting.Id, { FlightRange: value });
        }
    };

    const toggleSubTotals = () => {
        setShowSubTotals(!showSubTotals);
    };
    const toggleSubTotalsSummary = () => {
        setShowSubTotalsSummary(!showSubTotalsSummary);
    };

    const toggleInflightOverlays = () => {
        setShowInflightOverlays(!showInflightOverlays);
    };

    const toggleStyling = () => {
        setShowStyling(!showStyling);
    };
    const toggleInflightOverStyling = () => {
        setShowInflightOverStyling(!showInflightOverStyling);
    };

    const toggleSubLevels = () => {
        setShowSubSettings(!showSubSettings);
    };

    const onChangeStyling = (...args) => {
        const settingCopy = Tools.deepCopy(setting);
        const updatedSetting = Tools.setProperty(
            settingCopy,
            args[0],
            args[1] === '' ? null : args[1]
        );
        updateSetting(levelId, setting.Id, { Styling: updatedSetting.Styling });
    };
    const onChangeInflightOverlayStyling = (...args) => {
        const settingCopy = Tools.deepCopy(setting);
        const updatedSetting = Tools.setProperty(
            settingCopy,
            args[0],
            args[1] === '' ? null : args[1]
        );
        updateSetting(levelId, setting.Id, {
            InflightOverlayStyling: updatedSetting.InflightOverlayStyling,
        });
    };

    const inflightOverlayColumnValue = findIndex(inflightOverlayColumnsNew, {
        Name: setting.InflightOverlayColumnName,
        TableId: setting.InflightOverlayTableId,
    });

    const metricColumnValue = findIndex(metricColumnsNew, {
        Name: setting.MetricColumnName,
        TableId: setting.MetricTableId,
    });
    const flightRangeOptions = [
        { label: '', value: 'Flight Range', disabled: true },
        ...FlightRange.map(value => ({
            label: value,
            value: value
        }))
    ];


    return (
        <>
            <tr className={`is-shadowless ${isDragging} ? 'isDragging' : ${undefined}`}
                ref={provided.innerRef}
                {...provided.draggableProps}>
                <td style={{ padding: '0 5px 0 0' }} colSpan={0}>
                    {< Icon
                        {...provided.dragHandleProps}
                        icon-id="omni:interactive:reorder" className="custom-width-height"></Icon>
                    }

                </td>
                <td style={{ padding: '0px 8px 0 3px' }} colSpan={0}>
                    <OmniCheckBoxInput
                        checked={setting.Enabled}
                        onChange={(e: CustomEvent) => updateEnabled(e)}
                    />
                </td>
                <td
                    className="is-size-6 "
                    style={{ padding: 0 }} colSpan={4}>
                    <Tooltip>
                        <div className="text-truncate">{setting.Name}</div>
                        <div slot="content">
                            {setting.Name}
                        </div>
                    </Tooltip>
                </td>
    <td className="is-size-6" style={{ padding: '2px' }} colSpan={3}>
                    <SelectWithCommonFields
                        isLoading={false}
                        placeHolder={'Metric'}
                        columnValue={metricColumnValue}
                        columns={metricColumnsNew}
                        onChange={updateMetric}
                        isMetricSelection={true}
                    />
                </td>
                <td className="is-size-6" style={{ padding: '2px' }} colSpan={3}>
                    <OmniDropDownInput
                        value={
                            setting.FlightRange
                                ? flightRangeOptions.filter(opt => opt.value === setting.FlightRange)
                                : []
                        }
                        options={flightRangeOptions}
                        onValueChange={updateFlightRange}
                        disabled={!setting.Enabled}
                        placeholder="Flight Range"
                        className="text-capitalize w-100 dropdown-md"
                        hidefooter
                    />
                </td>
                <td className="is-size-6" style={{ padding: '2px' }} colSpan={3}>
                    <Button
                        className="icon"
                        tooltip="Inflight overlay"
                        disabled={!setting.Enabled}
                        onClick={toggleInflightOverlays}>
                        <Icon className="custom-width-height" icon-id={`omni:object:activation`} style={{ fill: setting.Enabled && setting?.InflightOverlays?.length > 0 ? '#2cc4ad' : undefined }}></Icon>
                    </Button>
                    {setting.Enabled && showInflightOverlays && (
                        <Dialog
                            title={`Add Inflight Overlays for Channel "${setting.Name}"`}
                            icon="interactive:add"
                            cancelButton={false}
                            okText="Done"
                            onOk={(e) => {
                                e.preventDefault();
                                toggleInflightOverlays();
                            }}
                            showDialog={setting.Enabled && showInflightOverlays }
                        >
                            <MediaHierarchyLevelInflightOverlayList
                                levelId={levelId}
                                levelSettingId={setting.Id}
                                flightRange={setting.FlightRange}
                                inflightOverlayColumns={inflightOverlayColumnsNew}
                            />
                        </Dialog>
                    )}
                    <Button
                        className="icon"
                        tooltip="Subtotal"
                        disabled={!setting.Enabled}
                        onClick={toggleSubTotals}>
                        <CustomIcon
                            icon="sigma"
                            color={
                                setting.Enabled &&
                                    setting?.SubTotals?.length > 0
                                    ? '#2cc4ad'
                                    : undefined
                            }
                        />
                    </Button>
                    {setting.Enabled && showSubTotals && (
                        <Dialog
                            title={`Add Sub Total for Channel "${setting.Name}"`}
                            icon="interactive:add"
                            cancelButton={false}
                            okText="Done"
                            onOk={(e) => {
                                e.preventDefault();
                                toggleSubTotals();
                            }}
                            showDialog={setting.Enabled && showSubTotals }
                        >
                            <MediaHierarchyLevelSubTotalList
                                levelId={levelId}
                                levelSettingId={setting.Id}
                                flightRange={setting.FlightRange}
                                metricColumns={metricColumnsNew}
                                metricColumn={metricColumnsNew[metricColumnValue]}
                            />
                        </Dialog>
                    )}
                    <Button
                        className="icon"
                        tooltip="Format style"
                        disabled={!setting.Enabled}
                        onClick={toggleStyling}>
                        <Icon className="custom-width-height" icon-id={`omni:informative:theme`}></Icon>
                    </Button>
                        <div id="moreActions" className={`dropdown dropdownMoreActions ${showSortModal ? 'is-active' : ''}`}>
                            <div className="dropdown-trigger">
                                <Button className="icon" tooltip="More actions" aria-haspopup="true" aria-controls="dropdown-menu" onClick={() => setShowSortModal(!showSortModal)}>
                                <Icon className="custom-width-height" icon-id="omni:interactive:actions" ></Icon>
                                </Button>
                            </div>
                        </div>
                        <div ref={ref} slot="dropdown-menu" className={`dropdown-menu  ${showSortModal ? 'is-block more-menu-position' : ''}`} id="dropdown-menu" role="menu">
                            <div slot="dropdown-content" className="dropdown-content ">
                                <a className={`dropdown-item is-flex is-align-items-center ${!setting.Enabled}?"disabled":""` }  onClick={toggleSubLevels} ><div className="w-5">Edit</div></a>
                                <a className={`dropdown-item is-flex is-align-items-center ${!setting.Enabled}?"disabled":""`} onClick={toggleSubTotalsSummary}><div className="w-5">Show Sub-total Summary</div></a>
                            </div>
                    </div>

                    {setting.Enabled && showSubTotalsSummary && (
                        <Dialog
                            title={`Add Sub Total Summary for Channel "${setting.Name}"`}
                            icon="interactive:add"
                            cancelButton={false}
                            okText="Done"
                            onOk={(e) => {
                                e.preventDefault();
                                toggleSubTotalsSummary();
                            }}
                            showDialog={setting.Enabled && showSubTotalsSummary }
                        >
                            <MediaHierarchySubTotalSummaryList
                                levelId={levelId}
                                levelSettingId={setting.Id}
                                flightRange={setting.FlightRange}
                                metricColumns={metricColumnsNew}
                                metricColumn={metricColumnsNew[metricColumnValue]}
                                columns={columns}
                                clientId={clientId}
                                level={level}
                                levelSettingName={setting.Name}
                            />
                        </Dialog>
                    )}
                    { /*<Button
                        className="is-text"
                        tooltip="edit"
                        disabled={!setting.Enabled}
                        onClick={toggleSubLevels}>
                        <Icon icon-id={`omni:interactive:edit`}></Icon>
                    </Button>*/}
                </td>
            </tr>
            {showInflightOverStyling && (
                <tr>
                    <td colSpan={8}>
                        <StylingConfig
                            styling={setting.InflightOverlayStyling}
                            formKey={'InflightOverlayStyling'}
                            onChange={onChangeInflightOverlayStyling}
                        />
                    </td>
                </tr>
            )}
            {showStyling && (
                <tr>
                    <td colSpan={15}>
                        <StylingConfig
                            styling={setting.Styling}
                            formKey={'Styling'}
                            onChange={onChangeStyling}
                        />
                    </td>
                </tr>
            )}
            {setting.Enabled && showSubSettings && (
                <tr className="is-shadowless">
                    <td colSpan={16}>
                        <MediaHierarchySubLevelList
                            clientId={clientId}
                            levelId={levelId}
                            levelSettingId={setting.Id}
                            columns={columns}
                            inflightOverlayColumns={inflightOverlayColumnsNew}
                            metricColumns={metricColumnsNew}
                        />
                    </td>
                </tr>
            )}
        </>
    );
};

export default memo(MediaHierarchyLevelSetting);
