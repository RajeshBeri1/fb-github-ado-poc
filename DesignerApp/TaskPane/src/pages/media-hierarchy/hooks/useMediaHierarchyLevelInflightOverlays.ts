import { find, findIndex } from 'lodash';

import {
    MediaHierarchyInflightOverlayWithId,
    useMediaHierarchyTemplateState,
} from '../states/MediaHierarchyState';

const useMediaHierarchyLevelInflightOverlays = (
    levelId: number,
    levelSettingId: number
): MediaHierarchyInflightOverlayWithId[] | null => {
    const { Definition: state } = useMediaHierarchyTemplateState();

    const levelIdIndex = findIndex(state.Levels, { Id: levelId });
    if (levelIdIndex < 0) return null;

    const currentLevelSetting = find(state.Levels[levelIdIndex].Settings, {
        Id: levelSettingId,
    });
    if (!currentLevelSetting) return null;

    // Ensure that tracked values trigger an update (patch for arrays)
    let tracked = [];
    if (Array.isArray(currentLevelSetting.InflightOverlays)) {
        currentLevelSetting.InflightOverlays.forEach((inflightOverlay) => {
            tracked.push({ ...inflightOverlay });
        });
        tracked = [];
    }

    return currentLevelSetting.InflightOverlays || null;
};

export default useMediaHierarchyLevelInflightOverlays;
