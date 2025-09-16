import React, { useState } from 'react';
import {
    Control,
    RegisterOptions,
    UseFormRegisterReturn,
} from 'react-hook-form';
import {
    CalendarOverlaySection,
    CalendarOverlayTemplateDetailsDTO,
} from '@omniflow/omni-webapi';

import { Input } from '../../../../components/form/input';
import { TValueOf } from '../../../../interfaces/valueOf.type';
import { Icon } from '../../../../omni/icon';
import { ClickableIcon } from '../../../../components/utils/clickable-icon';
import { TSection } from './calendar-overlay-sections.type';
import { CalendarOverlaySectionRows } from '../calendar-overlay-section-rows/calendar-overlay-section-rows';
import { StylingConfig } from '../../../../components/styling/styling-config';
import Tools from '../../../../business/tools';
import { Tooltip } from '../../../../omni/tooltip';

interface ICalendarOverlaySectionContentProps {
    section: TSection;
    index: number;
    updateSections: (index: number, updatedSection: TSection) => void;
    removeSection: (index: number) => void;
    isDragging?: boolean;
    provided: any;
    control: any;
    register: (
        name: string,
        options?: RegisterOptions
    ) => UseFormRegisterReturn;
}

export const CalendarOverlaySectionContent: React.FC<
    ICalendarOverlaySectionContentProps
> = ({
    section,
    index,
    isDragging = false,
    provided,
    updateSections,
    removeSection,
    control,
    register,
}) => {
    const handleSectionChange = (
        index: number,
        key: keyof CalendarOverlaySection,
        value: TValueOf<CalendarOverlaySection>
    ) => {
        const validValue = value ?? null;
        let overwrite = { [key]: validValue };
        updateSections(index, { ...section, ...overwrite });
    };
    const [showStyling, toggleStyling] = useState(false);

    const onChangeStyling = (index, ...args) => {
        section = Tools.setProperty(
            section,
            args[0],
            args[1] === '' ? null : args[1]
        );
        updateSections(index, {
            ...section,
        });
    };
    return (
        <tr
            className={`no-shadow ${isDragging ? 'isDragging' : undefined}` }
            {...provided.draggableProps}
            ref={provided.innerRef} >
            <td className="extraSmallPadding p-0">
                <table className="table overlay-table no-brdr-space">
                    <tbody>
                        <tr className="no-shadow mb-2">
                            <td className="h-0">

                            </td>
                            <td className=" pt-0 pl-114 pb-2 h-0 text-sm">
                                Section Title
                            </td>
                            <td className="text-center pt-0 px-0 pb-2 h-0 text-sm">
                                Actions
                            </td>
                        </tr>
                        <tr className="overlay-table-row">
                            <td className="py-1 iconSpace w-0 px-5 h-44">
                                <Icon
                                    {...provided.dragHandleProps}
                                    icon-id="omni:interactive:reorder" className="custom-width-height"></Icon>
                            </td>
                            <td className="py-1 w-67 pl-3 h-44">
                                <Input
                                    required
                                    placeholder="Section Title"
                                    onChange={(input) =>
                                        handleSectionChange(
                                            index,
                                            'Text',
                                            input
                                        )
                                    }
                                    value={section.Text}
                                    breakLine
                                />
                            </td>
                            <td className="py-1 iconSpace h-44">
                                <div className="d-flex is-justify-content-center">
                                    <div className="p-1 mr-5">
                                        <Tooltip>
                                            <ClickableIcon
                                                className="custom-width-height"
                                                iconId="omni:informative:theme"
                                                onClick={() =>
                                                    toggleStyling(!showStyling)
                                                }
                                            />
                                            <div slot="content">Format style</div>
                                        </Tooltip>
                                        
                                    </div>
                                    <div className="p-1">
                                        <Tooltip>
                                        <ClickableIcon
                                            className="custom-width-height"
                                            iconId="omni:interactive:trash"
                                            onClick={() => removeSection(index)}
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
                                        styling={section.Styling}
                                        formKey={'Styling'}
                                        onChange={(x, y) =>
                                            onChangeStyling(index, x, y)
                                        }
                                    />
                                </td>
                            </tr>
                        )}
                        <tr className="overlay-table-row no-shadow ">
                            <td
                                className="p-0 overlay-table-data"
                                colSpan={4}>
                                <CalendarOverlaySectionRows
                                    control={
                                        control as unknown as Control<CalendarOverlayTemplateDetailsDTO>
                                    }
                                    register={register}
                                    sectionId={index}
                                />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    );
};
