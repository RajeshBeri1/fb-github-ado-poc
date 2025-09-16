import {
    CalendarOverlayRow,
    CalendarOverlaySection,
} from '@omniflow/omni-webapi';
import { cloneDeep } from 'lodash';
import * as DateFns from 'date-fns';
import moment from 'moment';

export const rearrangeSectionsAndRows = (
    originalSections: CalendarOverlaySection[]
) => {
    // copyObj structure is [[section1], [section2]] instead of [section1, section2]
    // every section row is then iterated and when overlapping intervals are found the
    // section is "split". This ensures the correct order of the
    // sections i.e. [[Section1a, Section1b], [section2]]
    const restructuredSections = cloneDeep(originalSections).map((section) => [
        section,
    ]);

    originalSections.forEach((_, index) => {
        const sectionToSplit = restructuredSections[index];
        // change row order based on startdate
        sectionToSplit[0].Rows.sort(function (rowA, rowB) {
            return (
                new Date(rowB.StartDate).getTime() -
                new Date(rowA.StartDate).getTime()
            );
        });
        const sortedRows = sectionToSplit[0].Rows;
        // save moved row indices in order to remove them later on
        const movedRowIndices = [];
        sortedRows.forEach((row, rowIndex) => {
            for (let j = rowIndex + 1; j < sortedRows.length; j++) {
                if (
                    movedRowIndices.includes(rowIndex) ||
                    movedRowIndices.includes(j)
                )
                    continue;

                const comparingRow = sortedRows[j];

                if (_overlappingIntervals(row, comparingRow)) {
                    // split occurs here
                    movedRowIndices.push(rowIndex);
                    // move row into a different or new section!
                    _moveRowEntry(sortedRows[rowIndex], sectionToSplit);
                }
            }
        });
      
        // the first section (section1a) still contains all overlapping rows!
        _removeRowEntries(sortedRows, movedRowIndices);
        sectionToSplit.forEach((section, index) => {
            if (index != 0) {
                // skip the first section
                const existingRows = sectionToSplit.filter((row, sectionIndex) => sectionIndex < index).flatMap(d => d.Rows);
                section.Rows = section.Rows.filter((row) => existingRows.findIndex(c => c == row) < 0);
            }
        });
    });

    return restructuredSections.flat();
};

const _removeRowEntries = (
    row: CalendarOverlayRow[],
    removeIndices: number[]
) => {
    for (var i = removeIndices.length - 1; i >= 0; i--) {
        row.splice(removeIndices[i], 1);
    }
};

const _moveRowEntry = (
    row: CalendarOverlayRow,
    sections: CalendarOverlaySection[]
) => {
    let insertedRow = false;

    // try to fit row with siblings
    sections.forEach((section, index) => {
        if (index > 0) {
            section.Rows.forEach((sectionRow, rowIndex) => {
                if (!_overlappingIntervals(sectionRow, row)) {
                    section.Rows.splice(rowIndex + 1, 0, row);
                    insertedRow = true;
                    return;
                }
            });
        }
        if (insertedRow) return;
    });
    // otherwise create a new section for it
    if (!insertedRow) {
        sections.push({
            ...sections[0],
            Rows: [row],
            Text: '',
        });
    }
};

const _overlappingIntervals = (
    rowOne: CalendarOverlayRow,
    rowTwo: CalendarOverlayRow
) => {
    try {
        const isOverlapping = DateFns.areIntervalsOverlapping(
            {
                start: moment.utc(rowOne.StartDate).toDate(),
                end: moment.utc(rowOne.EndDate).toDate(),
            },
            {
                start: moment.utc(rowTwo.StartDate).toDate(),
                end: moment.utc(rowTwo.EndDate).toDate(),
            }
        );
        return isOverlapping;
    } catch {
        return false;
    }
};
