// filterFlatLevelData.ts
// Utility to filter flatLevelData by splitLevelIds and ancestor/descendant relationships

import { buildLevelParentMap, getAllAncestors, getAllDescendants } from '../business/engine/helpers/hierarchy-map';
import { isEmpty } from 'lodash';

export function filterFlatLevelData(flatLevelData: any[], splitLevelIds: string[]): { filteredFlatLevelData: any[], ancestorSet: Set<string> } {
    let filteredFlatLevelData: any[] = [];
    let ancestorSet: Set<string> = new Set();
    if (!isEmpty(splitLevelIds)) {
        splitLevelIds.forEach(splitLevelId => {
            const multi = buildLevelParentMap(flatLevelData);
            const ancestors = getAllAncestors(multi, splitLevelId) || [];
            const descendants = getAllDescendants(multi, splitLevelId) || [];
            ancestors.forEach(a => ancestorSet.add(a));
            descendants.forEach(d => ancestorSet.add(d));
            ancestorSet.add(splitLevelId);
        });
        filteredFlatLevelData = (flatLevelData || []).filter(f => {
            if (!f) return false;
            if (ancestorSet.has(f.levelId) && f.parentId && ancestorSet.has(f.parentId)) return true;
            if (ancestorSet.has(f.levelId) && !f.parentId) return true;
            return false;
        });
    }
    return { filteredFlatLevelData, ancestorSet };
}
