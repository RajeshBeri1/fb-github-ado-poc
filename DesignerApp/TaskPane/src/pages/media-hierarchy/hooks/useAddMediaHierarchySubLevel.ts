import { useCallback } from 'react';
import { findIndex } from 'lodash';

import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';
import { DefaultStyling } from '../../../business/engine/models/styles';

const useAddMediaHierarchySubLevel = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (
            levelId: number,
            levelSettingId: number,
            columnName: string | null | (string | null)[] = null,
            reset: boolean = false
        ) => {
            setDraft(({ Definition: draft }) => {
                if (levelId && levelSettingId) {
                    const levelIdIndex = findIndex(draft.Levels, {
                        Id: levelId,
                    });

                    if (levelIdIndex >= 0) {
                        const levelSettingIdIndex = findIndex(
                            draft.Levels[levelIdIndex].Settings,
                            { Id: levelSettingId }
                        );

                        if (levelSettingIdIndex >= 0) {
                            const columnNames = Array.isArray(columnName)
                                ? columnName
                                : [columnName];

                            if (
                                reset ||
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubLevels === null
                            ) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubLevels = [];
                            }

                            let nextId =
                                Math.max(
                                    0,
                                    ...draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ]?.SubLevels?.map((o) => o.Id)
                                ) + 1;

                            columnNames.forEach((ColumnName) => {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubLevels.push({
                                    Id: nextId++,
                                    ColumnName,
                                    Order: draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubLevels.length,
                                    Settings: [],
                                    TableId: null,
                                    Styling: DefaultStyling(),
                                });
                            });

                            if (
                                !draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubLevels.length
                            ) {
                                draft.Levels[levelIdIndex].Settings[
                                    levelSettingIdIndex
                                ].SubLevels = null;
                            }
                        }
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useAddMediaHierarchySubLevel;
