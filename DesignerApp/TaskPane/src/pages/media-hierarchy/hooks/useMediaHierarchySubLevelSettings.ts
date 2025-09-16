import { find, findIndex } from 'lodash';

import {
    MediaHierarchySubLevelSettingWithId,
    useMediaHierarchyTemplateState,
} from '../states/MediaHierarchyState';

const useMediaHierarchySubLevelSettings = (
    levelId: number,
    levelSettingId: number,
    subLevelId: number
): MediaHierarchySubLevelSettingWithId[] | null => {
    const { Definition: state } = useMediaHierarchyTemplateState();

    const levelIdIndex = findIndex(state.Levels, { Id: levelId });
    if (levelIdIndex < 0) return null;

    const levelSettingIdIndex = findIndex(state.Levels[levelIdIndex].Settings, {
        Id: levelSettingId,
    });
    if (levelSettingIdIndex < 0) return null;

    const currentSubLevel = find(
        state.Levels[levelIdIndex].Settings[levelSettingIdIndex].SubLevels,
        { Id: subLevelId }
    );
    if (!currentSubLevel || !Array.isArray(currentSubLevel.Settings))
        return null;

    // Ensure that tracked values trigger an update (patch for arrays)
    let tracked = [];
    currentSubLevel.Settings.forEach((setting) => {
        tracked.push({ ...setting });
    });
    tracked = [];

    return currentSubLevel.Settings || [];
};

export default useMediaHierarchySubLevelSettings;
