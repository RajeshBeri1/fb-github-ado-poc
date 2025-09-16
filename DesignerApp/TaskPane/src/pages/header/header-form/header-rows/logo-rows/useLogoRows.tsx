import { useState, useEffect } from 'react';
import { Control, useFieldArray } from 'react-hook-form';
import { HeaderLogo, HeaderTemplateDetailsDTO } from '@omniflow/omni-webapi';
import { uniqueId } from 'lodash';

import { TLogoRows } from '../shared/header-rows.type';

const useLogoRows = (
    control: Control<HeaderTemplateDetailsDTO>,
    title?: string
) => {
    const { fields: dbRows, replace } = useFieldArray<HeaderTemplateDetailsDTO>(
        {
            control,
            name: 'Definition.Configuration.Logos',
        }
    );
    const idPrefix = title.replace(/ /g, '_');

    const [rows, setRows] = useState(
        (dbRows as HeaderLogo[])?.map((row) => ({
            ...row,
            id: uniqueId(idPrefix),
        }))
    );

    useEffect(() => {
        replace(rows);
    }, [rows]);

    const onChange = (newState: TLogoRows) => {
        setRows(newState);
    };

    const addRow = () => {
        setRows([
            ...rows,
            {
                Image: null,
                Order: rows.length,
                id: uniqueId(idPrefix),
                Alignment: null,
            },
        ]);
    };

    return { rows, onChange, addRow };
};

export default useLogoRows;
