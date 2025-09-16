import { CalendarType } from '@omniflow/omni-webapi';

/**
 * Get readable string from calendar type enum.
 * @param type CalendarType
 * @returns string
 */
export const CalendarTypeMap = (type: CalendarType): string => {
    switch (type) {
        case CalendarType.Broadcast:
            return 'Broadcast';
        case CalendarType.Client:
            return 'Custom';
        case CalendarType.Standard:
            return 'Standard';
        default:
            return String();
    }
};
