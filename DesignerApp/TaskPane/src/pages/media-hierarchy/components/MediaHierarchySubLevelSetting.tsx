import React, { JSX, memo, ChangeEvent, useState } from 'react';
import * as API from '@omniflow/omni-webapi';
import { findIndex } from 'lodash';

import Button from '../../../components/buttons/Button';
import { Icon } from '../../../omni/icon';
import { MediaHierarchySubLevelSettingWithId } from '../states/MediaHierarchyState';
import useUpdateMediaHierarchySubLevelSetting from '../hooks/useUpdateMediaHierarchySubLevelSetting';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import CustomIcon from '../../../components/custom-icon';
import { StylingConfig } from '../../../components/styling/styling-config';
import Tools from '../../../business/tools';
import { SelectWithCommonFields } from '../../../components/form/select-with-common-fields';
import { Tooltip } from '../../../omni/tooltip';
import { OmniCheckBoxInput } from '../../../omni/checkbox';
import { OmniDropDownInput } from '../../../omni/dropdown';
import "../../../pages/common-styles"

const FlightRange = Object.values(API.FlightRange).filter(x => x !== 'None' && x !== API.FlightRange.Annually && x !== API.FlightRange.AnnuallyBroadcast);

export type TMediaHierarchySubLevelSettingProps = {
    levelId: number;
    levelSettingId: number;
    subLevelId: number;
    setting: MediaHierarchySubLevelSettingWithId;
    inflightOverlayColumns: TColumn[];
    metricColumns: TColumn[];
};

const MediaHierarchySubLevelSetting = ({
    levelId,
    levelSettingId,
    subLevelId,
    setting,
    inflightOverlayColumns,
    metricColumns,
}: TMediaHierarchySubLevelSettingProps): JSX.Element => {
    const updateSetting = useUpdateMediaHierarchySubLevelSetting();
    const [showSubSettings, setShowSubSettings] = useState<boolean>(false);
    const [showStyling, toggleStyling] = useState(false);
    const [showInflightOverStyling, setShowInflightOverStyling] =
        useState(false);

    const toggleInflightOverStyling = () => {
        setShowInflightOverStyling(!showInflightOverStyling);
    };

    const updateEnabled = (event: ChangeEvent<HTMLInputElement>) => {
        const value = event.target.checked;

        setShowSubSettings(false);
        updateSetting(levelId, levelSettingId, subLevelId, setting.Id, {
            Enabled: value,
            SubLevels: null,
        });
    };

    const updateInflightOverlay = (event: { target: { value: string } }) => {
        const i = parseInt(event.target.value, 10);
        if (i < 0)
            updateSetting(levelId, levelSettingId, subLevelId, setting.Id, {
                InflightOverlayColumnName: null,
                InflightOverlayTableId: null,
            });

        const {
            Name: InflightOverlayColumnName,
            TableId: InflightOverlayTableId,
        } = inflightOverlayColumns[i];
        updateSetting(levelId, levelSettingId, subLevelId, setting.Id, {
            InflightOverlayColumnName,
            InflightOverlayTableId,
        });
    };

    const updateMetric = (event: { target: { value: string } }) => {
        const i = parseInt(event.target.value, 10);
        if (i < 0)
            updateSetting(levelId, levelSettingId, subLevelId, setting.Id, {
                MetricColumnName: null,
                MetricTableId: null,
            });

        const { Name: MetricColumnName, TableId: MetricTableId } =
            metricColumns[i];
        updateSetting(levelId, levelSettingId, subLevelId, setting.Id, {
            MetricColumnName,
            MetricTableId,
        });
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

    const inflightOverlayColumnValue = findIndex(inflightOverlayColumns, {
        Name: setting.InflightOverlayColumnName,
        TableId: setting.InflightOverlayTableId,
    });

    const metricColumnValue = findIndex(metricColumns, {
        Name: setting.MetricColumnName,
        TableId: setting.MetricTableId,
    });

    const onChangeStyling = (...args) => {
        const copy = Tools.deepCopy(setting);
        const updatedSubLevelSetting = Tools.setProperty(
            copy,
            args[0],
            args[1] === '' ? null : args[1]
        );
        updateSetting(levelId, levelSettingId, subLevelId, setting.Id, {
            Styling: updatedSubLevelSetting.Styling,
        });
    };
    const onChangeInflightOverlayStyling = (...args) => {
        const settingCopy = Tools.deepCopy(setting);
        const updatedSetting = Tools.setProperty(
            settingCopy,
            args[0],
            args[1] === '' ? null : args[1]
        );
        updateSetting(levelId, levelSettingId, subLevelId, setting.Id, {
            InflightOverlayStyling: updatedSetting.InflightOverlayStyling,
        });
    };
    const flightRangeOptions = [
        { label: '', value: 'Flight Range', disabled: true },
        ...FlightRange.map(value => ({
            label: value,
            value: value
        }))
    ];

    return (
        <>
            <tr className="is-shadowless">
                <td style={{ padding: '0 10px' }} colSpan={2}>
                    <OmniCheckBoxInput
                        checked={setting.Enabled}
                        onChange={(e) => updateEnabled(e)}
                    />
                </td>
                <td
                    className="is-size-6"
                    style={{ padding: 0, maxWidth: '100px' }} colSpan={4}>
                    <Tooltip>
                        <div className="text-truncate">{setting.Name}</div>
                        <div slot="content">
                            {setting.Name}
                        </div>
                    </Tooltip>
                </td>
                <td  style={{ padding: 0 }} colSpan={6}>
                    <div className="d-flex is-align-items-center">
                        <SelectWithCommonFields
                            isLoading={false}
                            placeHolder={'Inflight overlay'}
                            columnValue={inflightOverlayColumnValue}
                            style={{ maxWidth: 100 }}
                            columns={inflightOverlayColumns}
                            onChange={updateInflightOverlay}
                            isMetricSelection={false}
                            className="text-capitalize w-75 dropdown-lg"
                        />
                        <Button
                            className="icon"
                            tooltip="Format style inflight overlay"
                            disabled={!setting.Enabled}
                            onClick={toggleInflightOverStyling}>
                            <Icon icon-id={`omni:informative:theme`} className="custom-width-height"></Icon>
                        </Button>
                    </div>
                   
                </td>
                <td className="is-size-6" style={{ padding: 0 }} colSpan={4}>
                    <SelectWithCommonFields
                        isLoading={false}
                        placeHolder={'Metric'}
                        columnValue={metricColumnValue}
                        style={{ maxWidth: 100 }}
                        columns={metricColumns}
                        onChange={updateMetric}
                        isMetricSelection={true}
                    />
                </td>
                <td className="is-size-6" style={{ padding: 0 }} colSpan={4}>
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
                <td style={{ padding: 0 }} colSpan={2}>
                    <Button
                        className="icon"
                        tooltip="Format style"
                        disabled={!setting.Enabled}
                        onClick={() => toggleStyling(!showStyling)}>
                        <Icon icon-id={`omni:informative:theme`} className="custom-width-height"></Icon>
                    </Button>
                </td>
            </tr>
            {showInflightOverStyling && (
                <tr>
                    <td colSpan={21}>
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
                    <td colSpan={21}>
                        <StylingConfig
                            styling={setting.Styling}
                            formKey={'Styling'}
                            onChange={onChangeStyling}
                        />
                    </td>
                </tr>
            )}
        </>
    );
};

export default memo(MediaHierarchySubLevelSetting);
