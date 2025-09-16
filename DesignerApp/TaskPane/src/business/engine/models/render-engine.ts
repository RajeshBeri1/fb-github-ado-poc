import { Block } from './block';
import { Styles } from './styles';

export type TGetValueDataReturn = {
    isEmpty: boolean;
    isSame: boolean;
    values: string[][];
};

export class RenderEngine {
    constructor() {}

    render = async (blockRows: Block[][]): Promise<void> => {
        throw new Error(
            `There is no render method defined for the ${this.constructor.name} class!`
        );
    };

    getStyles(block: Block): Styles[][] {
        return Array.from({ length: block.rowLength }).map((_y, rowIndex) =>
            Array.from({ length: block.columnLength }).map(
                (_x, columnIndex) => ({
                    ...block.getStyles(columnIndex, rowIndex),
                })
            )
        );
    }

    getValueData(block: Block): TGetValueDataReturn {
        const valueData = {
            isEmpty: true,
            isSame: true,
            values: [],
        };

        let lastValue = null;
        valueData.values = Array.from({ length: block.rowLength }).map(
            (_y, rowIndex) =>
                Array.from({ length: block.columnLength }).map(
                    (_x, columnIndex) => {
                        const value = `${block.getValue(
                            columnIndex,
                            rowIndex
                        )}`;

                        if (value !== '') valueData.isEmpty = false;
                        if (lastValue && lastValue !== value)
                            valueData.isSame = false;

                        lastValue = value;

                        return value;
                    }
                )
        );

        return valueData;
    }
}
