import React, { useMemo } from 'react';
import { Control } from 'react-hook-form';
import {
    HeaderLogoAlignment,
    HeaderTemplateDetailsDTO,
} from '@omniflow/omni-webapi';

import { Title } from '../../../../../components/form/title';
import useDetailRows from './useLogoRows';
import { removeRow } from '../../../../../lib/utils/row-helpers';
import { HeaderLogoRowContent } from './header-logo-row-content';
import useEventLogger from '../../../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../../../enums/event.enum'
interface IHeaderDetailRowsProps {
    title?: string;
    control: Control<HeaderTemplateDetailsDTO>;
}

export const HeaderLogoRows: React.FC<IHeaderDetailRowsProps> = ({
    control,
    title = 'Logos',
}) => {
    const { rows, addRow, onChange } = useDetailRows(control, title);
    const { logEvent } = useEventLogger(Module.HEADER);
    const availableAlignments = useMemo(() => {
        const usedAlignments = rows.map((row) => row.Alignment);
        return Object.values(HeaderLogoAlignment).filter(
            (alignment) => !usedAlignments.includes(alignment)
        );
    }, [rows]);

    const handleRemoveRow = (index) => {
        logEvent({ action: Action.DELETE, subModule: SubModule.LOGO });
        removeRow(index, rows, onChange);
    };

    const handleUpdateRow = (index, updatedRow) => {
        logEvent({ action: Action.UPDATE, subModule: SubModule.LOGO });
        onChange(
            rows.map((row, i) => {
                if (i === index) return updatedRow;
                return row;
            })
        );
    };

    return (
        <>
            <div className="d-flex my-3 justify-content-between">
                <Title text={title} />
                <button
                    className="secondary small"
                    onClick={() => { addRow(); logEvent({ action: Action.ADDROW, subModule: SubModule.LOGO }); }}
                    disabled={rows.length >= 3}>
                    Add logo
                </button>
            </div>
            <div className="w-100">
                <table className="table p-0">
                    <tbody className="static-body">
                        {rows.map((row, index) => (
                            <HeaderLogoRowContent
                                row={row}
                                index={index}
                                updateRows={handleUpdateRow}
                                removeRow={handleRemoveRow}
                                key={row.id}
                                availableAlignments={availableAlignments}
                            />
                        ))}
                    </tbody>
                </table>
            </div>
           
        </>
    );
};
