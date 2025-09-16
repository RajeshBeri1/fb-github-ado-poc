import React from 'react';
import {
    Control,
    RegisterOptions,
    UseFormRegisterReturn,
} from 'react-hook-form';
import { CalendarOverlayTemplateDetailsDTO } from '@omniflow/omni-webapi';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';

import { AppContext } from '../../../../taskpane/contexts/AppContext';
import useSections from './useSections';
import { changeOrder, removeRow } from '../../../../lib/utils/row-helpers';
import { Title } from '../../../../components/form/title';
import { CalendarOverlaySectionContent } from './calendar-overlay-section-content';
import '../../../../pages/common-styles.css';
interface ICalendarOverlaySectionsProps {
    title?: string;
    control: Control<CalendarOverlayTemplateDetailsDTO>;
    register: (
        name: string,
        options?: RegisterOptions
    ) => UseFormRegisterReturn;
}

export const CalendarOverlaySections: React.FC<
    ICalendarOverlaySectionsProps
> = ({ control, register, title = 'Calendar Overlay Sections' }) => {
    const { sections, addSection, onChange } = useSections(control, title);
    const appContext = React.useContext(AppContext);
    const handleRemoveRow = (index) => {
        removeRow(index, sections, onChange);
    };

    const handleUpdateSection = (index, updatedSection) => {
        onChange(
            sections.map((section, i) => {
                if (i === index) return { ...updatedSection, Rows: appContext.currentCalendarOverlayTemplateDetails?.Definition?.OverlaySections[i]?.Rows || [] };
                return { ...section, Rows: appContext.currentCalendarOverlayTemplateDetails?.Definition?.OverlaySections[i]?.Rows || [] };
            })
        );
    };

    return (
        <>
            <div className="d-flex mb-3 justify-content-between">
                <p className="component-title">{title} </p>
                <button
                    className="secondary small"
                    onClick={(e) => addSection(e)}>
                    Add section
                </button>
            </div>

            <table className="table border-gray py-3 px-2">
                <DragDropContext
                    onDragEnd={(param) =>
                        changeOrder(param, sections, onChange, appContext.currentCalendarOverlayTemplateDetails?.Definition?.OverlaySections)
                    }>
                    <Droppable
                        style={{ transform: 'none' }}
                        droppableId="droppable-overlay-sections">
                        {(provided) => (
                            <tbody
                                ref={provided.innerRef}
                                {...provided.droppableProps}>
                                {sections.map((section, index) => (
                                    <Draggable
                                        draggableId={section.id}
                                        key={section.id}
                                        index={index}>
                                        {(provided, snapshot) => (
                                            <CalendarOverlaySectionContent
                                                section={section}
                                                index={index}
                                                updateSections={
                                                    handleUpdateSection
                                                }
                                                removeSection={handleRemoveRow}
                                                isDragging={
                                                    snapshot?.isDragging
                                                }
                                                control={control}
                                                provided={provided}
                                                register={register}
                                            />
                                        )}
                                    </Draggable>
                                ))}
                                {provided.placeholder}
                            </tbody>
                        )}
                    </Droppable>
                </DragDropContext>
            </table>
        </>
    );
};
