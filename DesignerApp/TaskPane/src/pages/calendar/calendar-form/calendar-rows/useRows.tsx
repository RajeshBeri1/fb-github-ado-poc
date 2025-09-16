import { useState, useEffect } from 'react';
import { Control, useFieldArray } from 'react-hook-form';
import { CalendarRow, CalendarTemplateDetailsDTO } from '@omniflow/omni-webapi';
import { uniqueId } from 'lodash';

import { DefaultStyling, CalendarStyling } from '../../../../business/engine/models/styles'; 
import { TRows } from './calendar-rows.type';
import useEventLogger from '../../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../../enums/event.enum'

/**
 * hook for template rows
 */
const useRows = (
    control: Control<CalendarTemplateDetailsDTO>,
    title?: string
) => {
    const { logEvent } = useEventLogger(Module.CALENDAR);
    const { fields: dbRows, replace } =
        useFieldArray<CalendarTemplateDetailsDTO>({
            control,
            name: 'Definition.Configuration.Rows',
        });
    const idPrefix = title.replace(/ /g, '_');

    const [rows, setRows] = useState(
        (dbRows as CalendarRow[])?.map((row) => ({
            ...row,
            id: uniqueId(idPrefix),
        }))
    );

    useEffect(() => {
        replace(rows);
    }, [rows]);

    const onChange = (newState: TRows) => {
        setRows(newState);
    };

    const addRow = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        logEvent({ action: Action.ADDROW, subModule: SubModule.ADDCALENDARROW })
        setRows([
            ...rows,
            {
                ColumnName: undefined,
                Style: undefined,
                Styling: {
                    ...DefaultStyling(), ...CalendarStyling()
                },
                Order: rows.length,
                id: uniqueId('calendar_rows'),
            },
        ]);
    };

    return { rows, onChange, addRow };
};

export default useRows;
