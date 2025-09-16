import {
    MediaHierarchyLevelWithId,
    useMediaHierarchyTemplateState,
} from '../states/MediaHierarchyState';

const useMediaHierarchyLevels = (): MediaHierarchyLevelWithId[] => {
    const { Definition: state } = useMediaHierarchyTemplateState();

    if (!Array.isArray(state.Levels)) return null;

    // Ensure that tracked values trigger an update (patch for arrays)
    let tracked = [];
    state.Levels.forEach((level) => {
        tracked.push({ ...level });
    });
    tracked = [];

    return state.Levels || [];
};

export default useMediaHierarchyLevels;
