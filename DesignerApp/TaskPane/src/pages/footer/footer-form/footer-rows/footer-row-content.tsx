import React, { useState } from 'react';
import { FooterRow } from '@omniflow/omni-webapi';

import { Input } from '../../../../components/form/input';
import { TValueOf } from '../../../../interfaces/valueOf.type';
import { Icon } from '../../../../omni/icon';
import { ClickableIcon } from '../../../../components/utils/clickable-icon';
import { TRow } from './footer-rows.type';
import { StylingConfig } from '../../../../components/styling/styling-config';
import Tools from '../../../../business/tools';
import { Tooltip } from '../../../../omni/tooltip';
import '../../../../pages/common-styles.css'
import { OmniTextareaInput } from '../../../../omni/textarea'
interface IFooterRowContentProps {
    row: TRow;
    index: number;
    updateRows: (index: number, updatedRow: TRow) => void;
    removeRow: (index: number) => void;
    isDragging?: boolean;
    provided: any;
}

export const FooterRowContent: React.FC<IFooterRowContentProps> = ({
    row,
    index,
    isDragging = false,
    provided,
    updateRows,
    removeRow,
}) => {
    const handleRowChange = (
        index: number,
        key: keyof FooterRow,
        value: TValueOf<FooterRow>
    ) => {
        const validValue = value ?? null;
        let overwrite = { [key]: validValue };
        updateRows(index, { ...row, ...overwrite });
    };

    const [showStyling, toggleStyling] = useState(false);

    const onChangeStyling = (...args) => {
        row = Tools.setProperty(row, args[0], args[1] === '' ? null : args[1]);
        updateRows(index, {
            ...row,
        });
    };

    return (
        <>
            <tr
                className={isDragging ? 'isDragging' : undefined}
                {...provided.draggableProps}
                ref={provided.innerRef}>
                <td className="smallPadding">
                    <Icon
                        {...provided.dragHandleProps}
                        icon-id="omni:interactive:reorder" className="custom-width-height"></Icon>
                </td>
                <td className="smallPadding">
                    <OmniTextareaInput
                        required
                        onChange={(e:CustomEvent) =>
                            handleRowChange(index, 'Text', e.detail)
                        }
                        value={row.Text}
                        rows={1}
                        hidefooter
                    />
                </td>
                <td className="smallPadding">
                    <div className="d-flex is-justify-content-center">
                        <div className="mr-5">
                            <Tooltip>
                                <ClickableIcon
                                    className="custom-width-height"
                                    iconId="omni:informative:theme"
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
                    <td colSpan={5} className="p-2">
                        <StylingConfig
                            styling={row.Styling}
                            formKey={'Styling'}
                            onChange={onChangeStyling}
                        />
                    </td>
                </tr>
            )}
        </>
    );
};
