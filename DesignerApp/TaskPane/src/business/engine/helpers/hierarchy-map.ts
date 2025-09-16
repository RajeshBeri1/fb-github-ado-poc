// Parent-child map using levelId and parentId
export type LevelParentMap = {
	// childLevelId -> Set of direct parentLevelIds
	directParents: Record<string, Set<string>>;
	// parentLevelId -> Set of direct childLevelIds
	directChildren: Record<string, Set<string>>;
};

/**
 * Build a map of childLevelId -> set of direct parentLevelIds for nested Levels structures.
 * Keys are levelId and parentId.
 * @param levels - array of level objects with levelId and parentId
 */
export function buildLevelParentMap(levels: any): LevelParentMap {
    const directParents: Record<string, Set<string>> = {};
    const directChildren: Record<string, Set<string>> = {};

    if (!Array.isArray(levels)) return { directParents, directChildren };

    levels.forEach((level: any) => {
        const childId = level.levelId;
        const parentId = level.parentId;
        if (!childId) return;
        if (!directParents[childId]) directParents[childId] = new Set<string>();
        if (!directChildren[childId]) directChildren[childId] = new Set<string>();
        if (parentId) {
            directParents[childId].add(parentId);
            if (!directChildren[parentId]) directChildren[parentId] = new Set<string>();
            directChildren[parentId].add(childId);
        }
    });

    return { directParents, directChildren };
}

// ...existing code...

/**
 * Get all unique ancestors (parents at any depth) for a given levelId.
 */
export function getAllAncestors(map: LevelParentMap, levelId: string): string[] {
    const visited = new Set<string>();
    const queue: string[] = [];

    const direct = map.directParents[levelId];
    if (direct && direct.size > 0) {
        for (const parent of direct) {
            queue.push(parent);
            visited.add(parent);
        }
    }

    while (queue.length) {
        const current = queue.shift()!;
        const parentsOfCurrent = map.directParents[current];
        if (parentsOfCurrent && parentsOfCurrent.size > 0) {
            for (const parent of parentsOfCurrent) {
                if (!visited.has(parent)) {
                    visited.add(parent);
                    queue.push(parent);
                }
            }
        }
    }

    return Array.from(visited);
}

/**
 * Get all unique descendants (children at any depth) for a given levelId.
 */
export function getAllDescendants(map: LevelParentMap, levelId: string): string[] {
    const visited = new Set<string>();
    const queue: string[] = [];

    const direct = map.directChildren[levelId];
    if (direct && direct.size > 0) {
        for (const child of direct) {
            queue.push(child);
            visited.add(child);
        }
    }

    while (queue.length) {
        const current = queue.shift()!;
        const childrenOfCurrent = map.directChildren[current];
        if (childrenOfCurrent && childrenOfCurrent.size > 0) {
            for (const child of childrenOfCurrent) {
                if (!visited.has(child)) {
                    visited.add(child);
                    queue.push(child);
                }
            }
        }
    }

    return Array.from(visited);
}

/**
 * Get all parents and children (at any depth) for a given levelId.
 */
export function getAllParentsAndChildren(map: LevelParentMap, levelId: string): { parents: string[], children: string[] } {
    return {
        parents: getAllAncestors(map, levelId),
        children: getAllDescendants(map, levelId)
    };
}

// ...existing code...