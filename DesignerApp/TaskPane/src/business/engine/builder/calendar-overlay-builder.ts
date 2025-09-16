import {
    CalendarOverlayDefinition,
    CalendarDefinition,
    MediaHierarchyDefinition,
    CalendarOverlayRow,
    CalendarType,
    FlightRange
} from '@omniflow/omni-webapi';
import { cloneDeep, findIndex, isEqual, uniqWith } from 'lodash';
import moment from 'moment';

import { BuildEngine } from '../models/build-engine';
import { Block } from '../models/block';
import CalendarOverlayLeftMenuLevelBlock from '../blocks/calendar-overlay/calendar-overlay-left-menu-level-block';
import CalendarOverlayEntryBlock from '../blocks/calendar-overlay/calendar-overlay-entry-block';
import FlightBarWallBarBlock from '../blocks/media-hierarchy/flight-bar-wall-bar-block';
import { CalendarHelper, normalizeCalendarDateRange } from '../helpers/calendar-helper';
import { rearrangeSectionsAndRows } from '../helpers/calendar-overlay-helper';
import { StyleMapper } from '../models/styles';
import { NumberFormat } from '../../../enums/number-format.enum';
import { getExtraWeekForSpace } from '../helpers/week-count-helper';

export interface ICalendarOverlayBuilderProps {
    clientId: string;
    CalendarOverlayDefinition: CalendarOverlayDefinition;
    CalendarDefinition?: CalendarDefinition;
    CalendarHelper?: CalendarHelper;
    MediaHierarchyDefinition?: MediaHierarchyDefinition;
    titleColumns?: number;
    BriefedctcColumnSelected?: boolean;
}

export class CalendarOverlayBuilder extends BuildEngine {
    clientId: string;
    CalendarOverlayDefinition: CalendarOverlayDefinition;
    CalendarDefinition: CalendarDefinition;
    CalendarHelper: CalendarHelper;
    BriefedctcColumnSelected?: boolean;

    overlayFlightBarWallColumnLength: number = 0;
    overlayFlightBarWallRowLength: number = 0;

    overlayLeftMenuColumnLength: number = 0;
    overlayLeftMenuRowLength: number = 0;

    titleColumns: number = 1;

    constructor({
        clientId,
        CalendarOverlayDefinition,
        CalendarDefinition,
        CalendarHelper,
        titleColumns,
        BriefedctcColumnSelected,
    }: ICalendarOverlayBuilderProps) {
        super();

        this.clientId = clientId;
        // Clone deep needed here, since the definition is later altered!
        this.CalendarOverlayDefinition = cloneDeep(CalendarOverlayDefinition);
        this.CalendarDefinition = CalendarDefinition;
        this.CalendarHelper = CalendarHelper;
        this.BriefedctcColumnSelected = BriefedctcColumnSelected;

        if (titleColumns > 1) {
            this.titleColumns = titleColumns;
        }
    }

    build = async (): Promise<Block[][]> => {
        let overlayBlocks: Block[][] = [];
        if (
            this.CalendarOverlayDefinition &&
            this.CalendarDefinition &&
            this.CalendarHelper
        ) {
            // First prepare data
            this.CalendarOverlayDefinition.OverlaySections =
                rearrangeSectionsAndRows(
                    this.CalendarOverlayDefinition.OverlaySections
                );

            const leftCalendarBlocks = await this.buildLeftCalendarOverlay();
            const flightCalendarBlocks = await this.buildRightCalendarOverlay();
            overlayBlocks = this.merge([
                leftCalendarBlocks,
                flightCalendarBlocks,
            ],0);
        }
        return overlayBlocks;
    };

    buildRightCalendarOverlay = async (): Promise<Block[][]> => {
        const rows: Block[][] = [];
        // Count calendar weeks and get start and end date of calendar
        const startDate = this.CalendarHelper.dataFrom;
        const endDate = this.CalendarHelper.dataTo;  
        const isNormalCalendarEnabled = (this.CalendarDefinition?.Configuration?.Type == CalendarType.Broadcast || this.CalendarDefinition?.Configuration?.Type == CalendarType.Standard) && this.CalendarDefinition?.Configuration?.IsNormalizedEnabled;
        const extraWeeks = isNormalCalendarEnabled ? getExtraWeekForSpace(startDate, endDate) : 0;
        const countWeeks = (this.CalendarHelper.data?.length || 0) + extraWeeks;
        let actualStartDate = this.CalendarDefinition?.Configuration?.CustomStartDate ?? this.CalendarHelper.dataFrom;
        let actualEndDate = this.CalendarDefinition?.Configuration?.CustomEndDate ?? this.CalendarHelper.dataTo;
        const isStandardAndUseCustomStartDay = this.CalendarDefinition?.Configuration?.Type == CalendarType.Standard && this.CalendarDefinition?.Configuration?.UseCustomStartDay;

        if (isNormalCalendarEnabled) {
            const startDay = !isStandardAndUseCustomStartDay && !isNormalCalendarEnabled ? 0 : 1;

            const { normalizedStartDate, normalizedEndDate } = normalizeCalendarDateRange(
                actualStartDate,
                actualEndDate,
                isNormalCalendarEnabled,
                startDay
            );

            actualStartDate = normalizedStartDate;
            actualEndDate = normalizedEndDate;
        }

        if (countWeeks && startDate && endDate) {
            this.CalendarOverlayDefinition.OverlaySections.forEach(
                (section) => {
                    const columns = [];

                    const column: Block = new CalendarOverlayEntryBlock({
                        columnLength: countWeeks,
                        merge: false,
                    });
                    columns.push(column);

                    const flightBars: (CalendarOverlayRow & {
                        StartDate: Date;
                        EndDate: Date;
                        Text: string;
                    })[] = [];

                    // Merge rows
                    uniqWith(section.Rows, isEqual).sort((s, d) => moment(d.StartDate).diff(s.StartDate, "milliseconds")).forEach((row) => {
                        const { StartDate, EndDate, Text } = row;
                        const flightBarIndex = findIndex(
                            flightBars,
                            ({ EndDate, StartDate }) => {
                                return (new Date(EndDate) <
                                    new Date(row.StartDate) || (moment(EndDate).isSame(moment.utc(row.EndDate).toDate()) && moment(StartDate).isSame(moment.utc(row.StartDate).toDate())))
                                    ? true
                                    : false;
                            }
                        );

                        if (flightBarIndex === -1) {
                            flightBars.push({
                                ...row,
                                StartDate: moment.utc(StartDate).toDate(),
                                EndDate: moment.utc(EndDate).toDate(),
                                Text: Text,
                            });
                        } else {
                            flightBars[flightBarIndex].Text += `, ${Text}`;
                        }
                    });

                    // Build and push flight bar columns
                    const flightBarColumns = this.buildPeriodicBlocks({
                        startDate,
                        endDate,
                        columnStart: -countWeeks,
                        columnLength: countWeeks,
                        periodicData: flightBars.map((flightBar) => ({
                            startDate: flightBar.StartDate,
                            endDate: flightBar.EndDate,
                            data: flightBar,
                        })),
                        calendarType: this.CalendarHelper.calendarType,
                        calendarStartDate: this.CalendarDefinition?.Configuration.CustomStartDate ?? startDate,
                        isFilterRequired: false,
                        isFlightRangeSelected: false,
                        isLastLevel: false,
                        isNormalCalendarEnabled: isNormalCalendarEnabled,
                        flightRange: FlightRange.None,
                        extraWeeks,
                        normalizedStartDate: actualStartDate,
                        referencedCalendarDefinition: this.CalendarDefinition,
                        blockCb: ({
                            columnStart,
                            columnLength,
                            data: blockData,
                        }) => {
                            const block = blockData?.Text
                                ? new FlightBarWallBarBlock({
                                      columnStart,
                                      columnLength,
                                      value: blockData?.Text,
                                  })
                                : null;
                            block.getDirectStyles = () =>
                                StyleMapper.mapStyleToExcelStyle(
                                    blockData.Styling,
                                    false
                                );
                            return block;
                        },
                    });
                    if (this.BriefedctcColumnSelected) {
                        columns.push(...[new FlightBarWallBarBlock({
                            columnStart:0,
                            columnLength:1,
                            value: ' ',
                        })]);
                    }
                    columns.push(...flightBarColumns);
                    rows.push(columns);
                }
            );
        }

        this.overlayFlightBarWallColumnLength = this.getColumnLength(rows);
        this.overlayFlightBarWallRowLength = this.getRowLength(rows);

        return rows;
    };

    buildLeftCalendarOverlay = async (): Promise<Block[][]> => {
        const sections: Block[][] = [];       
        let lastOrder = -1;
        this.CalendarOverlayDefinition.OverlaySections.forEach(
            (section, index) => {
                const row = [];

                if (lastOrder !== section.Order) {
                    const menuBlockProps = {
                        value: section.Text,
                        columnStart: 0,
                        columnLength: this.titleColumns,
                        rowLength:
                            this.CalendarOverlayDefinition.OverlaySections.filter(
                                (s) => s.Order === section.Order
                            ).length,
                    };

                    lastOrder = section.Order;
                    const block = new CalendarOverlayLeftMenuLevelBlock(
                        menuBlockProps
                    );
                    block.getDirectStyles = () =>
                        StyleMapper.mapStyleToExcelStyle(
                            section.Styling,
                            false
                        );

                    row.push(block);
                    sections.push(row);
                }
            }
        );

        this.overlayLeftMenuColumnLength = this.getColumnLength(sections);
        this.overlayLeftMenuRowLength = this.getRowLength(sections);

        return sections;
    };
    
}
