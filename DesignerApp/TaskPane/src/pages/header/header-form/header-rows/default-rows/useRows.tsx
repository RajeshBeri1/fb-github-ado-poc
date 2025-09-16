import { useState, useEffect } from 'react';
import { Control, useFieldArray } from 'react-hook-form';
import {
    HeaderRow,
    HeaderTemplateDetailsDTO,
    ValueSeparator,
} from '@omniflow/omni-webapi';
import { uniqueId } from 'lodash';
import moment from 'moment';

import { DefaultStyling } from '../../../../../business/engine/models/styles';
import { TRows } from '../shared/header-rows.type';

const useRows = (
    control: Control<HeaderTemplateDetailsDTO>,
    title?: string
) => {
    const { fields: dbRows, replace } = useFieldArray<HeaderTemplateDetailsDTO>(
        {
            control,
            name: 'Definition.Configuration.Rows',
        }
    );
    const idPrefix = title.replace(/ /g, '_');

    const [rows, setRows] = useState(
        (dbRows as unknown as HeaderRow[])?.map((row) => ({
            ...row,
            Date: moment.utc(row?.Date ?? moment.utc()).format('YYYY-MM-DD'),
            id: uniqueId(idPrefix),
        }))
    );

    useEffect(() => {
        replace(rows as TRows);
    }, [rows]);

    const onChange = (newState: TRows) => {
        setRows(newState);
    };

    const addRow = (e) => {
        e.preventDefault();
        const updatedRows = [
            ...rows,
            {
                ColumnName: null,
                Date: moment.utc().format('YYYY-MM-DD'),
                DateFormat: null,
                Text: '',
                Type: null,
                Order: rows.length,
                TableId: null,
                ValueSeparator: ValueSeparator.Space,
                id: uniqueId(idPrefix),
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
