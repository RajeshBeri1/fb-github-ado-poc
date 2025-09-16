import { find, findIndex } from 'lodash';

import {
    MediaHierarchySubTotalSummarySettingWithId,
    useMediaHierarchyTemplateState,
} from '../states/MediaHierarchyState';

const useMediaHierarchyLevelSubTotalsSummarySetting = (
    levelId: number,
    levelSettingId: number,
    summaryId: number
): MediaHierarchySubTotalSummarySettingWithId[] | null => {
    const { Definition: state } = useMediaHierarchyTemplateState();

    const levelIdIndex = findIndex(state.Levels, { Id: levelId });
    if (levelIdIndex < 0) return null;

    const currentLevelSetting = find(state.Levels[levelIdIndex].Settings, {
        Id: levelSettingId,
    });
    if (!currentLevelSetting) return null;


    const currentSubTotalSummary = find(currentLevelSetting.SubTotalSummary, {
        Id: summaryId,
    });

    if (!currentSubTotalSummary) return null;

    // Ensure that tracked values trigger an update (patch for arrays)
    let tracked = [];
    if (Array.isArray(currentSubTotalSummary.Settings)) {
        currentSubTotalSummary.Settings.forEach((setting) => {
            tracked.push({ ...setting });
        });
        tracked = [];
    }

    return currentSubTotalSummary.Settings || null;
};

export default useMediaHierarchyLevelSubTotalsSummarySetting;
