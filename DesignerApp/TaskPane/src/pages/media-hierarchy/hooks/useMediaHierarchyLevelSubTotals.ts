import { find, findIndex } from 'lodash';

import {
    MediaHierarchySubTotalWithId,
    useMediaHierarchyTemplateState,
} from '../states/MediaHierarchyState';

const useMediaHierarchyLevelSubTotals = (
    levelId: number,
    levelSettingId: number
): MediaHierarchySubTotalWithId[] | null => {
    const { Definition: state } = useMediaHierarchyTemplateState();

    const levelIdIndex = findIndex(state.Levels, { Id: levelId });
    if (levelIdIndex < 0) return null;

    const currentLevelSetting = find(state.Levels[levelIdIndex].Settings, {
        Id: levelSettingId,
    });
    if (!currentLevelSetting) return null;

    // Ensure that tracked values trigger an update (patch for arrays)
    let tracked = [];
    if (Array.isArray(currentLevelSetting.SubTotals)) {
        currentLevelSetting.SubTotals.forEach((subTotal) => {
            tracked.push({ ...subTotal });
        });
        tracked = [];
    }

    return currentLevelSetting.SubTotals || null;
};

export default useMediaHierarchyLevelSubTotals;
