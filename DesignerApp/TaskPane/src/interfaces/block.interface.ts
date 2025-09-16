import { RenderOptions } from '../models/render-option';

export interface IBlock {
    length: number;
    value: number | string;
    renderBaseOptions: RenderOptions;
    getLabel: () => string;
    getRenderOptions: (index?: number) => RenderOptions;
}
