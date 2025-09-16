import useEventLogger from '../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../enums/event.enum'

export const changeOrder = (param, rows, setRows, overlaySections = null) => {
    if (!param.destination) {
        return;
    }
    const isOverlaySection = param.source.droppableId === 'droppable-overlay-sections';
    const srcIdx = param.source.index;
    const desIdx = param.destination.index;
    const [srcItem, destItem] = isOverlaySection ? [overlaySections[srcIdx], overlaySections[desIdx]] : [rows[srcIdx], rows[desIdx]];
    let updatedRows = [...rows];
    updatedRows[desIdx] = srcItem;
    updatedRows[srcIdx] = destItem;
    // update order
    updatedRows = updatedRows.map((row, index) => ({
        ...row,
        Order: index,
    }));
    setRows(updatedRows);
};

export const removeRow = (index, rows, setRows) => {
    const { logEvent } = useEventLogger(Module.CALENDAROVERLAY);
    logEvent({ action: Action.DELETEROW, subModule: SubModule.REMOVECALENDAROVERLAYROW })
    const updatedRows = rows
        .filter((_, i) => i !== index)
        .map((row, i) => ({ ...row, Order: i }));
    setRows(updatedRows);
};