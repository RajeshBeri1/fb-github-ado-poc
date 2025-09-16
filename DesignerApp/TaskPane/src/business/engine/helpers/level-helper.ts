import * as API from '@omniflow/omni-webapi';
import { FlowchartDataLevel } from '@omniflow/omni-webapi';
import { flatMapDeep } from 'lodash';
import Tools from "../../../business/tools";

export type TFlowchartDataLevelFlat = FlowchartDataLevel & {
    Depth: number;
    IsSubLevel: boolean;
    SettingIndex: number | null;
    SubLevelDepth: number | null;
    LevelDefinition: API.MediaHierarchyLevel;
    IsSplitRow: boolean | null;
    IsLastDepth: boolean | null;
    levelId: string;
    parentId: string | null;
};

export const flatLevels = (
    levels: FlowchartDataLevel[],
    MediaHierarchyDefinition: API.ReferencedMediaHierarchyDefinition
): TFlowchartDataLevelFlat[] => {
    const flatten = (level): TFlowchartDataLevelFlat[] => {
        const Depth = !level.Depth ? 1 : level.Depth + 1;
        const IsSubLevel = level.IsSubLevel ?? false;
        const SettingIndex = level.SettingIndex ?? null;
        const levelId = level.parentId ? `${level.ColumnName}_${level.Name}_${level.parentId}` : `${level.ColumnName}_${level.Name}`;
        const SubLevelDepth = IsSubLevel
            ? !level.SubLevelDepth
                ? 1
                : level.SubLevelDepth + 1
            : null;
        const LevelDefinition =
            level.LevelDefinition ??
            MediaHierarchyDefinition?.Definition?.Levels?.[Depth - 1];

        const subLevels = Array.isArray(level.SubLevels)
            ? level.SubLevels.map((l) => ({
                  ...l,
                  Depth,
                  IsSubLevel: true,
                  SettingIndex: level.SettingIndex ?? level.Order,
                  SubLevelDepth,
                  LevelDefinition,
                  levelId: `${l.ColumnName}_${l.Name}_${level.levelId ?? levelId}`,
                  parentId: level.levelId ?? levelId,
              }))
            : null;

        const childLevels = Array.isArray(level.Levels)
            ? level.Levels.map((l) => ({
                  ...l,
                  Depth,
                  IsSubLevel: false,
                  SettingIndex: null,
                  SubLevelDepth,
                  levelId: `${l.ColumnName}_${l.Name}_${level.levelId ?? levelId}`,
                  parentId: level.levelId ?? levelId,
              }))
            : null;

        return [
            {
                ...level,
                Depth,
                IsSubLevel,
                SettingIndex,
                SubLevelDepth,
                levelId,
                LevelDefinition: IsSubLevel
                    ? LevelDefinition?.Settings?.[SettingIndex]?.SubLevels?.[
                          SubLevelDepth - 1
                      ] ?? ({} as API.MediaHierarchyLevel)
                    : LevelDefinition,
            },
            flatMapDeep(subLevels, flatten),
            flatMapDeep(childLevels, flatten),
        ];
    };

    return flatMapDeep(levels, flatten);
};


// ...existing code...

/**
/**
 * Converts a composite key (e.g. "brand2:Applegate,channel:Paid Social,vehicle:Facebook / Instagram")
 * to a string like "vehicle_Facebook / Instagram_channel_Paid Social_brand2_Applegate"
 * and checks if this string exists in the ancestor set.
 * @param compositeKey - string like "brand2:Applegate,channel:Paid Social,vehicle:Facebook / Instagram"
 * @param ancestorSet - Set of keys like "vehicle_Facebook / Instagram_channel_Paid Social_brand2_Applegate"
 * @returns boolean
 */
export function checkCompositeKeyInAncestors(compositeKey: string, ancestorSet: Set<string>): boolean {
    if (!compositeKey) return false;
    // Split and reverse to match the requested order: vehicle, channel, brand2
    const parts = compositeKey.split(',').reverse();
    // Convert to "key_value" and join with "_"
    const keyString = parts
        .map(part => {
            const [key, value] = part.split(':');
            return `${key}_${value}`;
        })
        .join('_');
    // Check if this string exists in the ancestor set
    return ancestorSet.has(keyString);
}
