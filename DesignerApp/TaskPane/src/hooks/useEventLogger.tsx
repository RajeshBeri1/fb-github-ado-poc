import ReactGA from 'react-ga4';
import { Module, Action, SubModule, Category } from '../enums/event.enum';
import { Page } from '../enums/page.enum';
const useEventLoger = (module: Module | Page) => {
    // formatting event label
    const GetEventLabel = (data: { action: Action, subModule?: SubModule }): string => {
        return data.subModule ? `${module} ${data.subModule}` : `${module}`;
    }

    // formatting event category
    const GetEventCategory = (data: { SubModule?: SubModule, isModal?: boolean }): string => {
        return `${Category.EXCELADDIN} ${module}`;
    }

    // log google analytics event
    const logEvent = (data: { action: Action, label?: string, subModule?: SubModule, isModal?: boolean, value?: number }) => {
        ReactGA.event({
            category: GetEventCategory({ isModal: data.isModal }),
            action: data.action,
            label: data.label ?? GetEventLabel({ action: data.action, subModule: data.subModule }),
            value: data.value ?? 1
        });
    };
    return { logEvent };
};

export default useEventLoger;