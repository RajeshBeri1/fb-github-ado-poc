import ReactGA from 'react-ga4';

import { Category, Module, Action, SubModule } from '../enums/event.enum';

const useEventLogger = (module: Module) => {
    // log google analytics event

    const GetEventLabel = (
        action: Action,
        module: Module,
        _category = Category.LANDINGPAGE
    ): string => {
        return `${action} ${module}`;
    };

    const GetEventCategory = (
        module: Module,
        _SubModule: SubModule | null = null,
        _isModal = false,
        category = Category.LANDINGPAGE
    ): string => {
        return `${category} ${module}`;
    };

    const logEvent = (
        action: Action,
        _subModule: SubModule | null = null,
        _isModal = false,
        value: number | null = null,
        label: string | null = null
    ) => {
        ReactGA.event({
            category: GetEventCategory(module),

            action: action,

            label: label ?? GetEventLabel(action, module),

            value: value ?? 1,
        });
    };

    return { logEvent };
};

export default useEventLogger;
