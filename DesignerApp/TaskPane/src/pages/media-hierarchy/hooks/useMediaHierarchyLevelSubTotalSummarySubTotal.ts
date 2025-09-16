import { find, findIndex } from 'lodash';

import {
    MediaHierarchySubTotalSummarySettingWithId,
    MediaHierarchySummarySubTotalWithId,
    useMediaHierarchyTemplateState,
} from '../states/MediaHierarchyState';

const useMediaHierarchyLevelSubTotalsSummarySubTotal = (
    levelId: number,
    levelSettingId: number,
    summaryId: number
): MediaHierarchySummarySubTotalWithId[] | null => {
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
    if (Array.isArray(currentSubTotalSummary.SubTotals)) {
        currentSubTotalSummary.SubTotals.forEach((subTotal) => {
            tracked.push({ ...subTotal });
        });
        tracked = [];
    }

    return currentSubTotalSummary.SubTotals || null;
};

export default useMediaHierarchyLevelSubTotalsSummarySubTotal;
