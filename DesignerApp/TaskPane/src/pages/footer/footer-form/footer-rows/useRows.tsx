import { useState, useEffect } from 'react';
import { Control, useFieldArray } from 'react-hook-form';
import { FooterRow, FooterTemplateDetailsDTO } from '@omniflow/omni-webapi';
import { uniqueId } from 'lodash';

import { DefaultStyling } from '../../../../business/engine/models/styles';
import { TRows } from './footer-rows.type';

const useRows = (
    control: Control<FooterTemplateDetailsDTO>,
    title?: string
) => {
    const { fields: dbRows, replace } = useFieldArray<FooterTemplateDetailsDTO>(
        {
            control,
            name: 'Definition.Configuration.Rows',
        }
    );
    const idPrefix = title.replace(/ /g, '_');

    const [rows, setRows] = useState(
        (dbRows as FooterRow[])?.map((row) => ({
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

    const addRow = (e) => {
        e.preventDefault();
        setRows([
            ...rows,
            {
                Text: null,
                Order: rows.length,
                id: uniqueId(idPrefix),
                Styling: DefaultStyling(),
            },
        ]);
    };

    return { rows, onChange, addRow };
};

export default useRows;
