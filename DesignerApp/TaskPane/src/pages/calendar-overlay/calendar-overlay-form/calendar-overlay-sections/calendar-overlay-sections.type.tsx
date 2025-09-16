import { CalendarOverlaySection } from '@omniflow/omni-webapi';

export type TSections = ({
    id: string;
} & CalendarOverlaySection)[];

export type TSection = {
    id: string;
} & CalendarOverlaySection;
