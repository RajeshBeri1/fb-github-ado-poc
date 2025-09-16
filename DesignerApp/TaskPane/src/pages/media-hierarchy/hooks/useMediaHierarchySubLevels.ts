import { find, findIndex } from 'lodash';

import {
    MediaHierarchySubLevelWithId,
    useMediaHierarchyTemplateState,
} from '../states/MediaHierarchyState';

const useMediaHierarchySubLevels = (
    levelId: number,
    levelSettingId: number
): MediaHierarchySubLevelWithId[] | null => {
    const { Definition: state } = useMediaHierarchyTemplateState();

    const levelIdIndex = findIndex(state.Levels, { Id: levelId });
    if (levelIdIndex < 0) return null;

    const currentLevelSetting = find(state.Levels[levelIdIndex].Settings, {
        Id: levelSettingId,
    });
    if (!currentLevelSetting) return null;

    // Ensure that tracked values trigger an update (patch for arrays)
    let tracked = [];
    if (Array.isArray(currentLevelSetting.SubLevels)) {
        currentLevelSetting.SubLevels.forEach((subLevel) => {
            tracked.push({ ...subLevel });
        });
        tracked = [];
    }

    return currentLevelSetting.SubLevels || null;
};

export default useMediaHierarchySubLevels;
