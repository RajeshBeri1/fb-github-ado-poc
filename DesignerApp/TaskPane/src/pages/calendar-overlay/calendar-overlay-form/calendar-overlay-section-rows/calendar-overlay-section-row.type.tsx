import { CalendarOverlayRow } from '@omniflow/omni-webapi';

export type TRows = (CalendarOverlayRow & {
    id: string;
    StartDate: string;
    EndDate: string;
})[];

export type TRow = CalendarOverlayRow & {
    id: string;
    StartDate: string;
    EndDate: string;
};
