import React, { ChangeEvent, JSX, memo, useCallback, useContext, useState } from 'react';
import { find, sumBy } from 'lodash';
import { LegendThemeSetting } from '@omniflow/omni-webapi';
import { useForm } from 'react-hook-form';
import { OmniCheckBoxInput } from '../../../../omni/checkbox';
import { StylingConfig } from '../../../../components/styling/styling-config';
import Button from '../../../../components/buttons/Button';
import { Icon } from '../../../../omni/icon';
import Tools from '../../../../business/tools';

export type TLegendSettingListProps = {
    settings: LegendThemeSetting[];
    formKey?: string;
    onChange?: (attr, e) => void;
};

const LegendSettingList = ({
    settings,
    formKey,
    onChange,
}: TLegendSettingListProps): JSX.Element => {
    // for select all
    //const countEnabledSettings = sumBy(settings, (s) => (s.Enabled ? 1 : 0));
    const [settingsState, setSettings] = useState(settings);
    const {
        register,
        setValue,
        formState: { isDirty }, getValues } =
        useForm({
            mode: 'onChange',
            reValidateMode: 'onChange',
            defaultValues: {
                settings: settingsState
            },
        });

    //const updateEnabled = useCallback(
    //    (event) => {
    //        const value = event?.currentTarget?.checked;
    //        const updateAll = settings.map((setting) => {
    //            return { ...setting, Enabled: value }
    //        });
    //        onChange(formKey, updateAll);
    //        setValue('settings', settings.map((setting) => {
    //            return { ...setting, Enabled: value }
    //        }));
    //    },
    //    []
    //);
    const onSettingUpdated = (index: number, setting: LegendThemeSetting) => {
        onChange(formKey, settings.map((s, i) => {
            return i === index ? { ...s, ...setting } : s;
        }));
        setValue('settings', settings.map((s, i) => {
            return i === index ? { ...s, ...setting } : s;
        }));
    };

    const [showStyling, toggleStyling] = useState(-1);

    const updateEnabled = (index: number, event: any) => {
        onChange(formKey,
            settings.map((row, i) => {
                if (i === index) {
                    row = Tools.setProperty(
                        row,
                        'Enabled',
                        event?.currentTarget?.checked
                    );
                    return {
                        ...row,
                    };
                }
                return row;
            })
        );

        setValue('settings', settings.map((row, i) => {
            if (i === index) {
                row = Tools.setProperty(
                    row,
                    'Enabled',
                    event?.currentTarget?.checked
                );
                return {
                    ...row,
                };
            }
            return row;
        }));
    };

    const onChangeStyling = (index: number, ...args) => {
        onChange(formKey,
            settings.map((row, i) => {
                if (i === index) {
                    row = Tools.setProperty(
                        row,
                        args[0],
                        args[1] === '' ? null : args[1]
                    );
                    return {
                        ...row,
                    };
                }
                return row;
            })
        );

        setValue('settings', settings.map((row, i) => {
            if (i === index) {
                row = Tools.setProperty(
                    row,
                    args[0],
                    args[1] === '' ? null : args[1]
                );
                return {
                    ...row,
                };
            }
            return row;
        }));
    };

    const stylingConfigs = [
        { title: "Media Hierarchy Flightbars", key: "Styling" },
        { title: "Sub Totals", key: "SubTotalStyling" },
        { title: "Inflight Overlay", key: "InflightOverlayStyling" },
    ];

    const renderRows = () => {
        return settings.map((setting, index) => {
            return (
                <> <tr>
                    <td className="is-size-6 is-four-fifths" style={{ 'paddingLeft': '0px', width: '90%' }}>
                        <OmniCheckBoxInput checked={setting.Enabled} onClick={(e) => updateEnabled(index, e)}>
                            <label style={{ 'marginBottom': '0px' }}>{setting.Name}
                            </label></OmniCheckBoxInput>

                    </td>
                    <td className="is-size-6 text-center" style={{ padding: '2px', width: '10%' }}>
                        <Button
                            className="icon"
                            tooltip="Format style"
                            disabled={!setting.Enabled}
                            onClick={() =>
                                toggleStyling(
                                    showStyling ===
                                        index
                                        ? -1
                                        : index
                                )
                            }>
                            <Icon icon-id={`omni:informative:theme`}></Icon>
                        </Button>
                    </td>
                </tr>
                    {showStyling === index && stylingConfigs.map(({ title, key }) => (
                        <tr>
                            <td colSpan={8} style={{ 'paddingLeft': '0px' }}>
                                <header>
                                    <p className="text-md">{title}</p>
                                </header>
                                <StylingConfig
                                    styling={setting[key]}
                                    formKey={key}
                                    onChange={(x, y) => onChangeStyling(index, x, y)}
                                />
                            </td>
                        </tr>
                    ))}
                </>
            );
        });
    };
    return (
        <>
            <div className="overflow-auto">
                <table
                    className="table is-fullwidth is-shadowless"
                    style={{ padding: 0 }}>
                    <thead>
                        <tr>
                            {/* for select/unselect all  <th style={{ padding: '0 8px 0 3px' }}>*/}
                            {/*<input*/}
                            {/*    type="checkbox"*/}
                            {/*    checked={*/}
                            {/*        countEnabledSettings == settings.length*/}
                            {/*    }*/}
                            {/*    onChange={updateEnabled}*/}
                            {/*/>*/}
                            {/*<OmniCheckBoxInput checked={countEnabledSettings == settings.length} onClick={(e) => updateEnabled(e)}></OmniCheckBoxInput>*/}
                            {/*<label>{countEnabledSettings == settings.length ? ' Unselect All' : ' Select All'}</label>*/}
                            {/*</th>*/}
                            <th className="is-size-6 is-four-fifths" style={{ 'paddingLeft': '0px', width: '90%' }}>
                                Legend
                            </th>
                            <th className="is-size-6 text-center" style={{ width: '10%' }} >
                                Actions
                            </th>
                        </tr>
                    </thead>
                    <tbody style={{ 'zIndex': 0 }}>
                        {renderRows()}
                    </tbody>

                </table>

            </div>
        </>
    );
};

export default memo(LegendSettingList);
