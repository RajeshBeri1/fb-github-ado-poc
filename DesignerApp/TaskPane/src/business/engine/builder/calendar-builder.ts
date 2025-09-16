import { CalendarDefinition, CalendarType } from '@omniflow/omni-webapi';

import { BuildEngine } from '../models/build-engine';
import { Block } from '../models/block';
import GenericBlock from '../blocks/calendar/generic-block';
import { CalendarHelper } from '../helpers/calendar-helper';
import SpacerBlock from '../blocks/calendar/spacer-block';
import { StyleMapper } from '../models/styles';
import CustomBlock from '../blocks/calendar/custom-block';
import { sum } from 'lodash';


export interface ICalendarBuilderProps {
    clientId: string;
    CalendarDefinition?: CalendarDefinition;
    CalendarHelper?: CalendarHelper;
    BriefedctcColumnSelected?: boolean;
    IsNormalCalendarRow?: boolean;
    NormalCalendarEnabled?: boolean;
    CalendarFullLengthForNormalView?: number;
}

export class CalendarBuilder extends BuildEngine {
    clientId: string;
    CalendarDefinition: CalendarDefinition;
    CalendarHelper: CalendarHelper;
    BriefedctcColumnSelected: boolean;
    calendarColumnLength: number = 0;
    calendarRowLength: number = 0;
    calendarFullLength: number = 0;
    IsNormalCalendarRow: boolean;
    NormalCalendarEnabled?: boolean;
    CalendarFullLengthForNormalView?: number = 0;
    StandardWeekText = "Week";
    BroadcastWeekText = "week_text";
    BriefedCTCText = "Briefed CTC";
    AddLastDayOfTheMonth = false;
    constructor({
        clientId,
        CalendarDefinition,
        CalendarHelper,
        BriefedctcColumnSelected,
        IsNormalCalendarRow,
        NormalCalendarEnabled,
        CalendarFullLengthForNormalView,
    }: ICalendarBuilderProps) {
        super();

        this.clientId = clientId;
        this.CalendarDefinition = CalendarDefinition;
        this.CalendarHelper = CalendarHelper;
        this.BriefedctcColumnSelected = BriefedctcColumnSelected;
        this.IsNormalCalendarRow = IsNormalCalendarRow;
        this.NormalCalendarEnabled = NormalCalendarEnabled;
        this.CalendarFullLengthForNormalView = CalendarFullLengthForNormalView;
    }

    build = async (): Promise<Block[][]> => {
        let rows: Block[][] = [];

        if (this.CalendarDefinition && this.CalendarHelper) {
            this.AddLastDayOfTheMonth = this.CalendarDefinition?.Configuration?.Type == CalendarType.Standard && this.CalendarDefinition?.Configuration?.IsNormalizedEnabled;
            rows = await this.buildCalendar();
        }

        return rows;
    };

    buildCalendar = async (): Promise<Block[][]> => {
        const rows: Block[][] = [];
        if (!this.IsNormalCalendarRow) {
            let spacerRow = [];
            spacerRow.push(new SpacerBlock());
            rows.push(spacerRow);
        }
        this.CalendarDefinition.Configuration.Rows.forEach(
            ({ ColumnName, StandardCalenderRowType, Styling }, index) => {
                let colName = ColumnName;
                if (
                    this.CalendarDefinition.Configuration.Type ===
                    CalendarType.Standard
                ) {
                    colName = StandardCalenderRowType;
                }

                let blockData = [];
                let weekdata = [];
                this.CalendarHelper.data.forEach((dataEntry) => {
                    dataEntry.forEach((week) => {
                        if (week.ColumnName === colName) {
                            if (
                                blockData.length &&
                                blockData[blockData.length - 1].value ===
                                week.JsonValue
                            ) {
                                // Update length
                                blockData[blockData.length - 1].length++;
                            } else {
                                // Create new object to render
                                blockData.push({
                                    value: week.JsonValue,
                                    length: 1,
                                });
                            }
                        }

                        if ((this.AddLastDayOfTheMonth && (week.ColumnName == this.StandardWeekText)) || (this.NormalCalendarEnabled && (this.CalendarDefinition.Configuration.Type ===
                            CalendarType.Standard ? (week.ColumnName == this.StandardWeekText) : colName != this.BroadcastWeekText && week.ColumnName == this.BroadcastWeekText))) {

                            if (
                                weekdata.length &&
                                weekdata[weekdata.length - 1].value ===
                                week.JsonValue
                            ) {
                                // Update length
                                weekdata[weekdata.length - 1].length++;
                            } else {
                                // Create new object to render
                                weekdata.push({
                                    value: week.JsonValue,
                                    length: 1,
                                });
                            }
                        }
                    });
                });

                const row: Block[] = [];

                // add briefed ctc column if channel is selected in media hirearchy
                if (this.BriefedctcColumnSelected) {
                    if (this.CalendarDefinition.Configuration.Rows.length - 1 == index) {
                        row.push(new CustomBlock(this.BriefedCTCText));
                    } else {
                        row.push(new SpacerBlock());
                    }
                }
                if (!this.NormalCalendarEnabled && !this.AddLastDayOfTheMonth) {
                    blockData.forEach(({ value, length }) => {
                        const block = new GenericBlock({
                            value: this.CalendarHelper.formatValue(value, colName),
                            columnLength: length,
                            autofitColumns: true,
                        });
                        block.getDirectStyles = () =>
                            StyleMapper.mapStyleToExcelStyle(Styling, false);
                        row.push(block);
                    });
                }
                else {
                    let currentLength = 0;
                    blockData.forEach(({ value, length }, index) => {
                        const isStandardCalendar = this.CalendarDefinition.Configuration.Type ===
                            CalendarType.Standard;
                        if (isStandardCalendar || colName != this.BroadcastWeekText) {
                            let additionLength = 0;
                            const startIndex = index == 0 ? 0 : sum(blockData.filter((val, bIndex) => bIndex < index).flatMap(c => c.length));
                            let weekBlockData = weekdata.filter((wValue, vIndex) => vIndex >= startIndex && vIndex <= startIndex + length);

                            weekBlockData.forEach(({ value: Vvalue }, wIndex) => {

                                if (wIndex < weekBlockData.length - 1) {
                                    let nextValue = parseInt(this.CalendarHelper.formatValue(weekBlockData[wIndex + 1].value, isStandardCalendar ? this.StandardWeekText : this.BroadcastWeekText));
                                    let currentValue = parseInt(this.CalendarHelper.formatValue(weekBlockData[wIndex].value, isStandardCalendar ? this.StandardWeekText : this.BroadcastWeekText));

                                    if (isStandardCalendar && colName == this.StandardWeekText && index < blockData.length - 1) {
                                        nextValue = parseInt(this.CalendarHelper.formatValue(blockData[index + 1].value, this.StandardWeekText));
                                        currentValue = parseInt(this.CalendarHelper.formatValue(blockData[index].value, this.StandardWeekText));

                                        const lastDate = (currentValue + 7) - nextValue;

                                        if (nextValue < currentValue && lastDate != currentValue && (this.CalendarFullLengthForNormalView == 0 || currentLength <= this.CalendarFullLengthForNormalView)) {
                                            additionLength += 1;
                                            currentLength += 1;
                                            value = lastDate;
                                        }
                                    } else {

                                        const lastDate = (currentValue + 7) - nextValue;
                                        if (nextValue < currentValue && lastDate != currentValue && (this.CalendarFullLengthForNormalView == 0 || currentLength <= this.CalendarFullLengthForNormalView)) {
                                            additionLength += 1;
                                            currentLength += 1;
                                        }
                                    }
                                }
                            });
                            currentLength += length;
                            if (index == blockData.length - 1 && this.CalendarFullLengthForNormalView != 0 && isStandardCalendar && currentLength > this.CalendarFullLengthForNormalView) {
                                const differenceInLength = Number(this.CalendarFullLengthForNormalView) - currentLength;
                                additionLength += differenceInLength;
                            }
                            const block = new GenericBlock({
                                value: this.CalendarHelper.formatValue(value, colName),
                                columnLength: length + additionLength,
                                autofitColumns: true,
                            });
                            block.getDirectStyles = () =>
                                StyleMapper.mapStyleToExcelStyle(Styling, false);
                            row.push(block);
                        } else {
                            const block = new GenericBlock({
                                value: this.CalendarHelper.formatValue(value, colName),
                                columnLength: length,
                                autofitColumns: true,
                            });
                            block.getDirectStyles = () =>
                                StyleMapper.mapStyleToExcelStyle(Styling, false);
                            row.push(block);
                            if (index < blockData.length - 1) {
                                let nextValue = parseInt(blockData[index + 1].value);
                                let currentValue = parseInt(blockData[index].value);
                                if (!isStandardCalendar) {
                                    nextValue = parseInt(this.CalendarHelper.formatValue(blockData[index + 1].value, this.BroadcastWeekText));
                                    currentValue = parseInt(this.CalendarHelper.formatValue(blockData[index].value, this.BroadcastWeekText));
                                }
                                const lastDate = (currentValue + 7) - nextValue;
                                if (nextValue < currentValue && lastDate != currentValue) {
                                    if (this.IsNormalCalendarRow) {
                                        const extraBlock = new GenericBlock({
                                            value: !isStandardCalendar ? lastDate.toString() : this.CalendarHelper.formatValue(lastDate.toString(), colName),
                                            columnLength: length,
                                            autofitColumns: true,
                                        });
                                        extraBlock.getDirectStyles = () =>
                                            StyleMapper.mapStyleToExcelStyle(Styling, false);
                                        row.push(extraBlock);
                                    } else {
                                        const extraBlock = new GenericBlock({
                                            value: '',
                                            columnLength: length,
                                            autofitColumns: true,
                                        });
                                        extraBlock.getDirectStyles = () =>
                                            StyleMapper.mapStyleToExcelStyle(Styling, false);
                                        row.push(extraBlock);
                                    }
                                }
                            }
                        }

                    });
                }
                rows.push(row);
            }
        );

        this.calendarColumnLength = this.getColumnLength(rows);
        this.calendarRowLength = this.getRowLength(rows);
        this.calendarFullLength = this.getFullColumnLength(rows);

        return rows;
    };
}
