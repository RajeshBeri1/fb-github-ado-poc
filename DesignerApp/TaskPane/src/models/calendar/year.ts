import { Block } from '../block';

export class Year extends Block {
    getLabel = () => '' + this.value;
    getRenderOptions = () => {
        return { ...this.renderBaseOptions, ...{ fill: { color: '#00aaff' } } };
    };
}
