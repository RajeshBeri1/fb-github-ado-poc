import React, { useState } from 'react';
import { Input } from '../../../../components/form/input';
import { TValueOf } from '../../../../interfaces/valueOf.type';
import { Icon } from '../../../../omni/icon';
import { ClickableIcon } from '../../../../components/utils/clickable-icon';
import { TRow } from './calendar-overlay-section-row.type';
import { CalendarOverlayRow } from '@omniflow/omni-webapi';
import { RegisterOptions, UseFormRegisterReturn } from 'react-hook-form';
import { StylingConfig } from '../../../../components/styling/styling-config';
import Tools from '../../../../business/tools';
import {Tooltip} from "../../../../omni/tooltip";

interface ICalendarOverlaySectionRowContentProps {
    row: TRow;
    index: number;
    updateRows: (index: number, updatedSection: TRow) => void;
    removeRow: (index: number) => void;
    isDragging?: boolean;
    provided: any;
    sectionId: number;
    register: (
        name: string,
        options?: RegisterOptions
    ) => UseFormRegisterReturn;
}

export const CalendarOverlaySectionRowContent: React.FC<
    ICalendarOverlaySectionRowContentProps
> = ({
    row,
    index,
    isDragging = false,
    provided,
    updateRows,
    removeRow,
    register,
    sectionId,
}) => {
    const handleRowChange = (
        index: number,
        key: keyof CalendarOverlayRow,
        value: TValueOf<CalendarOverlayRow>
    ) => {
        const validValue = value ?? null;
        let overwrite = { [key]: validValue };
        updateRows(index, { ...row, ...overwrite });
    };

    const [showStyling, toggleStyling] = useState(false);

    const onChangeStyling = (index, ...args) => {
        row = Tools.setProperty(row, args[0], args[1] === '' ? null : args[1]);
        updateRows(index, {
            ...row,
        });
    };

    return (
        <>
            <tr className="no-shadow">
                <td className="p-0 h-0"></td>
                <td className="py-0 px-0 h-0 text-xs"> Row Text</td>
                <td className="py-0 px-3 h-0 text-xs"> Start Date</td>
                <td className="py-0 px-3 h-0 text-xs">End Date</td>
                <td className="py-0 px-3 h-0 text-xs">Actions</td>
            </tr>

            <tr
            className={isDragging ? 'isDragging' : undefined}
            {...provided.draggableProps}
            ref={provided.innerRef}>
            <td className="p-0 h-36">
                <Icon
                    {...provided.dragHandleProps}
                        icon-id="omni:interactive:reorder" hidden="true"></Icon>
            </td>
            <td className="py-1 pl-0 pr-2 h-36">
                <Input
                    required
                    label=""
                    labelStyle="w-100"
                    onChange={(input) => handleRowChange(index, 'Text', input)}
                    value={row.Text}
                    breakLine
                />
            </td>
                <td className="py-1 px-2 h-36">
                <Input
                    labelStyle="w-100"
                    label=""
                    type="date"
                    value={row.StartDate}
                    onChange={(input) => handleRowChange(index, 'StartDate', input)}
                    breakLine
                />
            </td>
                <td className="py-1 px-2 h-36">
                <Input
                    labelStyle="w-100"
                    label=""
                    type="date"
                    value={row.EndDate}
                    onChange={(input) => handleRowChange(index, 'EndDate', input)}
                    breakLine
                />
            </td>
                <td className="py-1 pl-2 pr-0 h-36">
                <div className="d-flex">
                    <div className="mr-5">
                        <Tooltip>
                            <ClickableIcon
                            className="custom-width-height"
                            iconId="omni:informative:theme"
                            slot="invoker"
                            onClick={() => toggleStyling(!showStyling)}
                        />
                            <div slot="content">Format style</div>
                        </Tooltip>
                    </div>
                    <div>
                        <Tooltip>
                            <ClickableIcon
                            className="custom-width-height"
                            iconId="omni:interactive:trash"
                            slot="invoker"
                            onClick={() => removeRow(index)}
                        />
                            <div slot="content">Delete</div>
                        </Tooltip>
                    </div>
                </div>
            </td>
        </tr>
        {showStyling && (
            <tr>
                <td colSpan={8}>
                    <StylingConfig
                        styling={row.Styling}
                        formKey={'Styling'}
                        onChange={(x,y) => onChangeStyling(index,x,y)}
                    />
                </td>
            </tr>
        )}
        </>

    );
};
