import { useState, useEffect } from 'react';
import { Control, useFieldArray } from 'react-hook-form';
import {
    HeaderDetailsRow,
    HeaderTemplateDetailsDTO,
    ValueSeparator,
} from '@omniflow/omni-webapi';
import { uniqueId } from 'lodash';

import { DefaultStyling } from '../../../../../business/engine/models/styles';
import { TDetailRows } from '../shared/header-rows.type';
 
const useDetailRows = (
    control: Control<HeaderTemplateDetailsDTO>,
    title?: string
) => {
    const { fields: dbRows, replace } = useFieldArray<HeaderTemplateDetailsDTO>(
        {
            control,
            name: 'Definition.Configuration.Details.Rows',
        }
    );
    const idPrefix = title.replace(/ /g, '_');

    const [rows, setRows] = useState(
        (dbRows as HeaderDetailsRow[])?.map((row) => ({
            ...row,
            id: uniqueId(idPrefix),
        }))
    );

    useEffect(() => {
        replace(rows);
    }, [rows]);

    const onChange = (newState: TDetailRows) => {
        setRows(newState);
    };

    const addRow = (e) => {
        setRows([
            ...rows,
            {
                ColumnName: null,
                Order: rows.length,
                TableId: null,
                Text: null,
                ValueSeparator: ValueSeparator.Space,
                id: uniqueId(idPrefix),
                Styling: DefaultStyling(),
            },
        ]);
    };

    return { rows, onChange, addRow };
};

export default useDetailRows;
