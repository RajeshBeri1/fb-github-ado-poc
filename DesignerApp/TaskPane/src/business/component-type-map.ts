import { Page } from '../enums/page.enum';

/**
 * Get readable string from component type enum.
 * @param type Page
 * @returns string
 */
export const ComponentTypeMap = (type: Page): string => {
    switch (type) {
        case Page.Header:
            return 'Header Component';
        case Page.Calendar:
            return 'Calendar Component';
        case Page.MediaHierarchy:
            return 'Hierarchy Component';
        default:
            return 'Unknown Component';
    }
};
