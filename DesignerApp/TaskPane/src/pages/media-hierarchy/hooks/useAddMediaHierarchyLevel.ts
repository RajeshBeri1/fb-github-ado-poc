import { useCallback } from 'react';
import { DefaultStyling } from '../../../business/engine/models/styles';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum'

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const useAddMediaHierarchyLevel = (): any => {
    const { logEvent } = useEventLogger(Module.MEDIAHIERARCHY);
    const setDraft = useSetMediaHierarchyTemplateDraft();
 
    return useCallback(
        (
            columnName: string | null | (string | null)[] = null,
            reset: boolean = false
        ) => {
            setDraft(({ Definition: draft }) => {
                const columnNames = Array.isArray(columnName)
                    ? columnName
                    : [columnName];

                if (reset) draft.Levels = [];

                let nextId =
                    Math.max(0, ...draft?.Levels?.map((o) => o.Id)) + 1;
                logEvent({ action: Action.ADDLEVEL, subModule: SubModule.ADDMEDIAHIERARCHYLEVEL });
                columnNames.forEach((ColumnName) => {
                    draft.Levels.push({
                        Id: nextId++,
                        ColumnName,
                        Order: draft.Levels.length,
                        Settings: [],
                        TableId: null,
                        Styling: DefaultStyling(),
                    });
                });
            });
        },
        [setDraft]
    );
};

export default useAddMediaHierarchyLevel;
