import { useCallback } from 'react';
import { find, findIndex } from 'lodash';
import { useSetMediaHierarchyTemplateDraft } from '../states/MediaHierarchyState';

const reorderByKey = (obj: any, sourceKey: string, destinationKey: string) => {
    const keys = Object.keys(obj);

    // 1. Basic validation: Check if both keys exist
    if (!keys.includes(sourceKey) || !keys.includes(destinationKey)) {
        console.error("Source or destination key does not exist in the object.");
        return obj;
    }

    const sourceIndex = keys.indexOf(sourceKey);
    const destIndex = keys.indexOf(destinationKey);

    // 2. No operation needed if source and destination are the same key
    if (sourceIndex === destIndex) {
        return obj;
    }

    // 3. Remove the source key from its current position
    keys.splice(sourceIndex, 1);

    // 4. Insert the source key at the destination index
    //    If moving down, after removal, destIndex decreases by 1
    const insertAt = sourceIndex < destIndex ? destIndex : destIndex;
    keys.splice(insertAt, 0, sourceKey);

    // 5. Reconstruct the object with the new key order
    const reordered: { [key: string]: any } = {};
    for (const key of keys) {
        reordered[key] = obj[key];
    }

    // 6. Mutate original object to maintain reference
    Object.keys(obj).forEach(key => delete obj[key]);
    Object.assign(obj, reordered);

    return obj;
};


const useOrderMediaHierarchyLevelSummaryDisplayNames = (): any => {
    const setDraft = useSetMediaHierarchyTemplateDraft();

    return useCallback(
        (param: any, levelId: number,
            levelSettingId: number,
            summaryId: number | number[]) => {
            if (!param.destination) {
                return;
            }
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
                            const summarySettingsIdIndex = findIndex(
                                draft.Levels[levelIdIndex].Settings[levelSettingIdIndex].SubTotalSummary,
                                { Id: summaryId }
                            );

                            if (summarySettingsIdIndex >= 0) {
                                const displayNames = {
                                    ...find(draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary, { Id: summaryId }).DisplayNames};
                              
                                if (Object.keys(displayNames).length) {
                                    const srcIdx = param.source.index;
                                    const desIdx = param.destination.index;
                                    if (
                                        srcIdx < 0 ||
                                        srcIdx >= Object.keys(displayNames).length ||
                                        desIdx < 0 ||
                                        desIdx >= Object.keys(displayNames).length
                                    ) {
                                        console.error("Source or destination index is out of bounds");
                                        return;
                                    }
                                    const srcKey = Object.entries(displayNames)[srcIdx]?.[0];
                                    const desKey = Object.entries(displayNames)[desIdx]?.[0];

                                    if (!srcKey || !desKey) {
                                        console.error("Source or destination key is invalid");
                                        return;
                                    }
                                    reorderByKey(displayNames, srcKey, desKey);
                        
                                    draft.Levels[levelIdIndex].Settings[
                                        levelSettingIdIndex
                                    ].SubTotalSummary[summarySettingsIdIndex].DisplayNames = displayNames;

                                }
                                
                            }
                        };
                    }
                }
            });
        },
        [setDraft]
    );
};

export default useOrderMediaHierarchyLevelSummaryDisplayNames;