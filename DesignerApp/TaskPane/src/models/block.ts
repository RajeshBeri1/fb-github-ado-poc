import { RenderOptions } from './render-option';
import { IBlock } from '../interfaces/block.interface';

export class Block implements IBlock {
    value: any;
    length: number;
    renderBaseOptions: RenderOptions = {
        // columnWidth: null,
        rowHeight: 25,
        borders: {
            bottom: {
                style: Excel.BorderLineStyle.continuous,
            },
            left: {
                style: Excel.BorderLineStyle.continuous,
            },
            right: {
                style: Excel.BorderLineStyle.continuous,
            },
            top: {
                style: Excel.BorderLineStyle.continuous,
            },
        },
        fill: {
            color: '#ff0000',
        },
        horizontalAlignment: Excel.HorizontalAlignment.center,
        font: {
            // color: "White",
            bold: true,
        },
    };

    constructor(props: { value?: any; length: number }) {
        this.value = props.value || 0;
        this.length = props.length || 0;
    }

    getLabel = () => '';
    getRenderOptions = (index?: number): RenderOptions => this.renderBaseOptions;
}
