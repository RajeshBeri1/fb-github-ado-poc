import React, { useEffect, useState } from 'react';
import { Control, useWatch } from 'react-hook-form';
import {
    CalendarTemplateDetailsDTO,
    CalendarType,
    ColumnType,
    DataDictionaryColumnDetailsDTO,
    StandardCalenderRowType,
} from '@omniflow/omni-webapi';

import { Title } from '../../../../components/form/title';
import { CalendarRowsHeader } from './calendar-rows-header';
import { CalendarRows } from './calendar-rows';
import { dataDictionaryApi } from '../../../../lib/api';
import useRows from './useRows';
import { TRows } from './calendar-rows.type';
import calendarFields from '../../../../../assets/datadictionary/calendarFields.json'

import './calendar-rows.css';

type CalendarRowsSettingProps = {
    clientId: string;
    control: Control<CalendarTemplateDetailsDTO>;
};

export const CalendarRowsSetting: React.FC<CalendarRowsSettingProps> = ({
    clientId,
    control,
}) => {
    const { rows, addRow, onChange } = useRows(control, 'Calendar Rows');

    const [types, setTypes] = useState([]);
    const typesBlacklist = ['client_id'];
    const currentCalendarType = useWatch({
        control,
        name: 'Definition.Configuration.Type',
    });
    const [usingStandardType, setUsingStandardType] = useState(false);

    useEffect(() => {
        let isSubscribed = true;
        if (types.length) {
            // reset rows if calendar row types were already loaded once
            onChange([]);
        }

        const loadDataDictionaryCalendarCols = async () => {
            let result = null;
            if (currentCalendarType === CalendarType.Broadcast) {
                result =
                    await dataDictionaryApi.dataDictionaryGetBroadcastCalendarColumns(
                        clientId
                    );
            } else {
                result =
                    await dataDictionaryApi.dataDictionaryGetClientCalendarColumns(
                        clientId
                    );
            }
            
            if (result?.data && isSubscribed) {
                result.data = (currentCalendarType === CalendarType.Broadcast && result.data != null) ? result?.data.filter(x => calendarFields.broadcastFields.includes(x.Name.toLowerCase())) : result?.data.filter(x => calendarFields.customClientFields.includes(x.Name.toLowerCase()));
                const filteredTypes = result.data.filter(
                    (data: DataDictionaryColumnDetailsDTO) =>
                        data.Type === ColumnType.String &&
                        !typesBlacklist.includes(data.Name)
                ) as DataDictionaryColumnDetailsDTO[];

                setTypes(filteredTypes);
            }
        };

        if (
            !currentCalendarType ||
            currentCalendarType === CalendarType.Standard
        ) {
            setTypes(
                Object.values(StandardCalenderRowType).map((type) => ({
                    DisplayName: type,
                    Name: type,
                    Type: ColumnType.String,
                }))
            );
            setUsingStandardType(true);
        } else {
            setUsingStandardType(false);
            loadDataDictionaryCalendarCols().catch(console.error);
        }

        return () => {
            isSubscribed = false;
        };
    }, [clientId, currentCalendarType]);

    const handleChange = (newState: TRows) => {
        onChange(newState);
    };

    return (
        <>
            <div className="d-flex my-3 justify-content-between">
                <p className="component-title">Calendar Row Selection</p>
                <button
                    disabled={rows.length >= types.length}
                    className="secondary small"
                    onClick={(e) => addRow(e)}>
                    Add row
                </button>
            </div>
            <table className="table calendar-rows">
                <CalendarRowsHeader headers={['', 'Row Type', 'Actions']} />
                <CalendarRows
                    rows={rows}
                    types={types}
                    usingStandardType={usingStandardType}
                    changeRows={handleChange}
                />
            </table>
        </>
    );
};
