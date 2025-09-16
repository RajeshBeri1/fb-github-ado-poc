import {
    CalendarConfiguration,
    CalendarOverlayTemplateDetailsDTO,
    CalendarReportingTimeFrame,
} from '@omniflow/omni-webapi';
import React, { useContext } from 'react';
import {
    Control,
    RegisterOptions,
    UseFormRegisterReturn,
} from 'react-hook-form';
import moment from 'moment';

import useRows from './useRows';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';
import { changeOrder, removeRow } from '../../../../lib/utils/row-helpers';
import { Title } from '../../../../components/form/title';
import { CalendarOverlaySectionRowContent } from './calendar-overlay-section-row-content';
import { TRow } from './calendar-overlay-section-row.type';
import { AppContext } from '../../../../taskpane/contexts/AppContext';

interface ICalendarOverlaySectionRowsProps {
    title?: string;
    control: Control<CalendarOverlayTemplateDetailsDTO>;
    sectionId: number;
    register: (
        name: string,
        options?: RegisterOptions
    ) => UseFormRegisterReturn;
}

export const CalendarOverlaySectionRows: React.FC<
    ICalendarOverlaySectionRowsProps
> = ({ control, register, sectionId, title = 'Section Rows' }) => {
    const { rows, addRow, onChange } = useRows(control, sectionId, title);

    const { currentCalendarTemplateDetails, flowchartTemplateDefinition } =
        useContext(AppContext);

    const getCurrentCalendarStart = () => {
        const calendarConfig =
            currentCalendarTemplateDetails?.Definition?.Configuration;
        const flowchartCalendarConfig =
            flowchartTemplateDefinition?.Definition?.CalendarDefinition
                ?.Definition?.Configuration;

        if (calendarConfig) {
            return extractStartDate(calendarConfig);
        }

        return extractStartDate(flowchartCalendarConfig);
    };

    const extractStartDate = (
        calendarConfig?: CalendarConfiguration | undefined
    ) => {
        let start = moment.utc().toDate();
        const today = moment.utc().toDate();

        if (calendarConfig?.CustomStartDate) {
            start = calendarConfig?.CustomStartDate;
        }

        if (calendarConfig?.IsReportingTimeFrame) {
            if (
                calendarConfig?.ReportingTimeFrame ===
                CalendarReportingTimeFrame.CurrentYear
            ) {
                start = moment.utc(today).startOf('year').toDate();
            } else {
                start = moment
                    .utc(today)
                    .startOf('year')
                    .subtract(1, 'year')
                    .toDate();
            }
        }
        return start;
    };

    const handleRemoveRow = (index) => {
        removeRow(index, rows, onChange);
    };

    const handleUpdateRow = (index, updatedRow) => {
        onChange(
            rows.map((row, i) => {
                if (i === index) return updatedRow;
                return row;
            })
        );
    };

    return (
        <>
            <div className="d-flex mb-3 justify-content-between is-align-items-end">
                <p className="component-title">{title}</p>
                <button
                    className="secondary small"
                    onClick={(e) => addRow(e, getCurrentCalendarStart())}>
                    Add row
                </button>
            </div>

            <table className="table overlay-table table-padding p-3">
                <DragDropContext
                    onDragEnd={(param) => changeOrder(param, rows, onChange)}>
                    <Droppable
                        style={{ transform: 'none' }}
                        droppableId="droppable-overlay-section-rows">
                        {(provided) => (
                            <tbody
                                ref={provided.innerRef}
                                {...provided.droppableProps}>
                                {rows.map((row, index) => (
                                    <Draggable
                                        draggableId={row.id}
                                        key={row.id}
                                        index={index}>
                                        {(provided, snapshot) => (
                                            <CalendarOverlaySectionRowContent
                                                row={row as TRow}
                                                index={index}
                                                updateRows={handleUpdateRow}
                                                removeRow={handleRemoveRow}
                                                isDragging={
                                                    snapshot?.isDragging
                                                }
                                                provided={provided}
                                                register={register}
                                                sectionId={sectionId}
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
