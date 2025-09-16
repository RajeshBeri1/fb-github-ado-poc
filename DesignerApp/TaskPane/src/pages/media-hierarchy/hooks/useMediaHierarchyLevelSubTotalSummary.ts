import { find, findIndex } from 'lodash';

import {
    MediaHierarchySubTotalSummaryWithId,
    useMediaHierarchyTemplateState,
} from '../states/MediaHierarchyState';

const useMediaHierarchyLevelSubTotalsSummary = (
    levelId: number,
    levelSettingId: number
): MediaHierarchySubTotalSummaryWithId[] | null => {
    const { Definition: state } = useMediaHierarchyTemplateState();

    const levelIdIndex = findIndex(state.Levels, { Id: levelId });
    if (levelIdIndex < 0) return null;

    const currentLevelSetting = find(state.Levels[levelIdIndex].Settings, {
        Id: levelSettingId,
    });
    if (!currentLevelSetting) return null;

    // Ensure that tracked values trigger an update (patch for arrays)
    let tracked = [];
    if (Array.isArray(currentLevelSetting.SubTotalSummary)) {
        currentLevelSetting.SubTotalSummary.forEach((subTotal) => {
            tracked.push({ ...subTotal });
        });
        tracked = [];
    }

    return currentLevelSetting.SubTotalSummary || null;
};

export default useMediaHierarchyLevelSubTotalsSummary;
