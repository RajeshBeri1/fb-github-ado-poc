import {
    CalendarOverlayRow,
    CalendarOverlayTemplateDetailsDTO,
} from '@omniflow/omni-webapi';
import { uniqueId } from 'lodash';
import { useState, useEffect } from 'react';
import { Control, useFieldArray } from 'react-hook-form';
import moment from 'moment';

import { TRows } from './calendar-overlay-section-row.type';
import { DefaultStyling } from '../../../../business/engine/models/styles';
import useEventLogger from '../../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../../enums/event.enum'

const useRows = (
    control: Control<CalendarOverlayTemplateDetailsDTO>,
    sectionId: number,
    title?: string
) => {
    const { logEvent } = useEventLogger(Module.CALENDAROVERLAY);
    const { fields: dbRows, replace } = useFieldArray({
        control,
        name: `Definition.OverlaySections.${sectionId}.Rows`,
    });
    const idPrefix = title.replace(/ /g, '_');

    const [rows, setRows] = useState(
        (dbRows as CalendarOverlayRow[])?.map((row) => ({
            ...row,
            StartDate: moment
                .utc(row.StartDate ?? moment.utc())
                .format('YYYY-MM-DD'),
            EndDate: moment
                .utc(row.EndDate ?? moment.utc())
                .format('YYYY-MM-DD'),
            id: uniqueId(idPrefix),
        }))
    );

    useEffect(() => {
        replace(rows as TRows);
    }, [rows]);

    const onChange = (newState: TRows) => {
        setRows(newState);
    };

    const addRow = (e, startDate?: Date) => {
        e.preventDefault();
        logEvent({ action: Action.ADDROW, subModule: SubModule.ADDCALENDAROVERLAYROW })
        const start = moment.utc(startDate ?? moment.utc()).toDate();
        const end =
            start > moment.utc().toDate() ? start : moment.utc().toDate();
        const updatedRows = [
            ...rows,
            {
                Text: null,
                Order: rows.length,
                id: uniqueId(idPrefix),
                StartDate: moment.utc(start).format('YYYY-MM-DD'),
                EndDate: moment.utc(end).format('YYYY-MM-DD'),
                Styling: DefaultStyling(),
            },
        ];

        setRows(updatedRows);
        // hacky! additional replace needed to ensure that dates are rendered correctly
        replace(updatedRows as TRows);
    };

    return { rows, onChange, addRow };
};

export default useRows;
