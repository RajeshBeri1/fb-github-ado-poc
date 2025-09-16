import { find, orderBy } from 'lodash';

import {
    MediaHierarchyLevelSettingWithId,
    useMediaHierarchyTemplateState,
} from '../states/MediaHierarchyState';

const useMediaHierarchyLevelSettings = (
    levelId: number
): MediaHierarchyLevelSettingWithId[] | null => {
    const { Definition: state } = useMediaHierarchyTemplateState();

    const currentLevel = find(state.Levels, { Id: levelId });
    if (!currentLevel || !Array.isArray(currentLevel.Settings)) return null;

    // Ensure that tracked values trigger an update (patch for arrays)
    let tracked = [];
    currentLevel.Settings.forEach((setting) => {
        tracked.push({ ...setting });
    });
    tracked = [];

    return orderBy(currentLevel.Settings || [],['Order'],['asc']);
};

export default useMediaHierarchyLevelSettings;
