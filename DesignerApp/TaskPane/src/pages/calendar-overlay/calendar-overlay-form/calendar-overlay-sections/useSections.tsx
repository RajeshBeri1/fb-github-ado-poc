import { useState, useEffect, useContext } from 'react';
import { Control, useFieldArray } from 'react-hook-form';
import {
    CalendarOverlaySection,
    CalendarOverlayTemplateDetailsDTO,
} from '@omniflow/omni-webapi';
import { uniqueId } from 'lodash';
import { AppContext } from '../../../../taskpane/contexts/AppContext';
import { DefaultStyling } from '../../../../business/engine/models/styles';
import { TSections } from './calendar-overlay-sections.type';
import useEventLogger from '../../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../../enums/event.enum'

const useSections = (
    control: Control<CalendarOverlayTemplateDetailsDTO>,
    title?: string
) => {
    const { fields: dbRows, replace } =
        useFieldArray<CalendarOverlayTemplateDetailsDTO>({
            control,
            name: 'Definition.OverlaySections',
        });
    const idPrefix = title.replace(/ /g, '_');
    const appContext = useContext(AppContext);
    const { logEvent } = useEventLogger(Module.CALENDAROVERLAY);
    const [sections, setSections] = useState(
        (dbRows as CalendarOverlaySection[])?.map((section, i) => ({
            ...section,
            id: uniqueId(idPrefix),

        }))
    );


    useEffect(() => {
        replace(sections);
    }, [sections]);

    const onChange = (newState: TSections) => {
        setSections(newState);
    };

    const addSection = (e) => {
        e.preventDefault();
        logEvent({ action: Action.ADDSECTION, subModule: SubModule.ADDCALENDAROVERLAYSECTION })
        setSections([
            ...sections.map((section, i) =>
                ({ ...section, Rows: appContext.currentCalendarOverlayTemplateDetails?.Definition?.OverlaySections[i]?.Rows || [] })
            ),
            {
                Text: null,
                Order: sections.length,
                id: uniqueId(idPrefix),
                Rows: [],
                Styling: DefaultStyling(),
            },

        ]);
    };

    return { sections, onChange, addSection };
};

export default useSections;
