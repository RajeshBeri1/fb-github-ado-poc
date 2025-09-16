import { Styling, ColumnType, LegendTheme, LegendThemeSetting } from '@omniflow/omni-webapi';
import React, { KeyboardEvent, useState, useContext, useCallback, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { AppContext } from '../../../../taskpane/contexts/AppContext';
import { OmniDropDownInput } from '../../../../omni/dropdown';
import { filter } from 'lodash';
import { dataApi } from '../../../../lib/api';
import LegendSettingList from './LegendSettingList';
import Button from '../../../../components/buttons/Button';
import { Icon } from '../../../../omni/icon';
import { DefaultStyling } from '../../../../business/engine/models/styles';
import { plainToClass } from 'class-transformer';
import { OmniCheckBoxInput } from '../../../../omni/checkbox';
import { OmniDropdown } from 'omni-ui'
import { CalendarHelper } from '../../../../business/engine/helpers/calendar-helper';
export enum EditSection {
    Alignment = 'Alignment',
    Font = 'Font',
    Border = 'Border',
    Fill = 'Fill',
    HeaderRowStyling = 'HeaderRowStyling',
}

interface IInputProps {
    formKey?: string;
    showInherited?,
    onChange?: (attr, e) => void;
    legend: LegendTheme
}

export const LegendDefault: React.FC<IInputProps> = React.memo(({
    formKey,
    onChange,
    showInherited = true,
    legend,
}) => {

    const [legendState, setLegend] = useState(legend);
    const [showSettings, setShowSettings] = useState<boolean>(false);
    const [settings, setSettings] = useState<LegendThemeSetting[]>(legend?.Settings ?? []);
    const [isDataLoading, setDataLoading] = useState<boolean>(false);
    const legendRef = React.useRef<OmniDropdown>(null);
    const toggleSettings = () => {
        setShowSettings(!showSettings);
    };
    const {
        register,
        setValue,
        formState: { isDirty }, getValues } =
        useForm({
            mode: 'onChange',
            reValidateMode: 'onChange',
            defaultValues: {
                LegendTheme: legendState
            },
        });
    const { columns, flowchartTemplateDefinition, currentCalendarTemplateDetails } = useContext(AppContext);

    const capitalizeFirstWord = (phrase) => {
        return phrase.replace(/^\w/, c => c.toUpperCase());
    }

    const filteredColumns = filter(
        columns,
        ({ Type, IsMetric }) => (Type === ColumnType.String || Type === ColumnType.Date) && IsMetric != true
    ).map((c, index) => {
        return { value: `${c.DisplayName} (${capitalizeFirstWord(c.TableName)})`, id: `${c.TableId}_${c.Name}` }
    });

    // create a new instance of the calendar helper class to use its methods and properties
    const calendarData = currentCalendarTemplateDetails?.Definition?.Configuration ? new CalendarHelper(flowchartTemplateDefinition.OmniClientId, currentCalendarTemplateDetails?.Definition?.Configuration) : { calendarFrom: null, calendarTo: null };

    const loadSettings = async () => {
        const columnName = getValues('LegendTheme.ColumnName');
        const tableId = getValues('LegendTheme.TableId');
        if (columnName && tableId) {
            setDataLoading(true);
            try {
                let { data } = await dataApi.dataGetDistinctData({
                    ParentColumnName: null,
                    ParentTableId: null,
                    ParentSelectedValue: null,
                    OmniClientId: flowchartTemplateDefinition.OmniClientId,
                    ColumnName: getValues('LegendTheme.ColumnName'),
                    TableId: getValues('LegendTheme.TableId'),
                    StartDate: calendarData.calendarFrom,
                    EndDate: calendarData.calendarTo,
                    Levels: [],
                });
                data = data.sort();
                let currentSettings = new Array<LegendThemeSetting>();
                data.forEach(col => {
                    let foundIndex = getValues('LegendTheme.Settings') && getValues('LegendTheme.Settings').length ? getValues('LegendTheme.Settings').findIndex(s => s.Name == col) : -1;
                    if (foundIndex > -1 && legend?.Settings && legend?.Settings.length) {
                        currentSettings.push(legend?.Settings[foundIndex]);
                    } else {
                        const setting = plainToClass(LegendThemeSetting, {
                            Name: col,
                            Enabled: true,
                            Styling: DefaultStyling(),
                            InflightOverlayStyling: DefaultStyling(),
                            SubTotalStyling: DefaultStyling(),
                        });
                        currentSettings.push(setting);
                    }
                });
                setSettings(currentSettings);
                onChange(formKey + '.Settings', currentSettings);
                setValue('LegendTheme.Settings', currentSettings);
                setDataLoading(false);
            } catch (e) {
                setDataLoading(false);
            }

        } else {
            setSettings([]);
            onChange(formKey + '.Settings', []);
            setValue('LegendTheme.Settings', []);
        }
    }
    useEffect(() => {
        loadSettings().catch(console.error);
    }, [getValues('LegendTheme.TableId'), getValues('LegendTheme.ColumnName')])

    const handleColumnChange = (event) => {
        event.preventDefault();
        const value = event.detail?.id ? event.detail.id : null;
        const tableId = value?.split('_')[0];
        const columnName = value?.split('_')[1];
        onChange(formKey + '.TableId', tableId);
        onChange(formKey + '.ColumnName', columnName);
        setValue('LegendTheme.TableId', tableId);
        setValue('LegendTheme.ColumnName', columnName);
        loadSettings().catch(console.error);
    }
    const columnSelected = getValues('LegendTheme.TableId') && getValues('LegendTheme.ColumnName') ? columns.find(c => c.Name == getValues('LegendTheme.ColumnName') && c.TableId == getValues('LegendTheme.TableId')) : null;
    const legendValue = getValues('LegendTheme.TableId') && getValues('LegendTheme.ColumnName') ? [filteredColumns.find(fc => fc.value == `${columnSelected.DisplayName} (${capitalizeFirstWord(columnSelected.TableName)})`)] : [];
    const onUpdated = (attr, e) => {
        onChange(attr, e);
        setValue('LegendTheme.Settings', e);
    };
    const handleDisplayChange = (event) => {
        event.preventDefault();
        const checked=event.currentTarget.checked;
        onChange(formKey + '.DisplayVertically', checked);
        setValue("LegendTheme.DisplayVertically", checked);
    }
    const deselectColumn = () => {
        legendRef.current?.reset();
        //onChange(formKey + '.TableId', undefined);
        //onChange(formKey + '.ColumnName', undefined);
        //setValue('LegendTheme.TableId', undefined);
        //setValue('LegendTheme.ColumnName', undefined);
    }
    return (

        <>
            <div className="d-flex w-100 align-items-center">
                <div className="is-align-items-center mb-3">
                    <OmniCheckBoxInput checked={getValues("LegendTheme.DisplayVertically")} onClick={(e) => handleDisplayChange(e)} className="display-vertically"> <label className="mb-0 text-core-dark">Display Vertically</label></OmniCheckBoxInput>
                </div>
            </div>
            <div className="d-flex w-100 align-items-center">
                <OmniDropDownInput ref={legendRef} label="Column Name" className="w-95" disabled={isDataLoading} searchindropdown typeahead
                    options={filteredColumns} value={legendValue} onValueChange={handleColumnChange} >
                </OmniDropDownInput>
                <Button
                    className="icon"
                    tooltip={showSettings ? 'Back' : 'Edit'}
                    disabled={!settings?.length}
                    onClick={toggleSettings}>
                    <Icon
                        icon-id={`omni:interactive:${showSettings ? 'left' : 'edit'
                            }`}></Icon>
                </Button>
                <Button
                    className="icon"
                    tooltip={'Delete'}
                    disabled={!getValues('LegendTheme.TableId')}
                    onClick={deselectColumn}>
                    <Icon
                        icon-id={`omni:interactive:delete`}></Icon>
                </Button>
            </div>
            <div>
                {showSettings && (
                    <LegendSettingList
                        settings={legend.Settings}
                        formKey={'Definition.LegendTheme.Settings'}
                        onChange={onUpdated}
                    />
                )}
            </div>
        </>
    );
});