import { Block } from '../block';

export class Week extends Block {
    getLabel = () => '' + this.value;
    getRenderOptions = () => {
        return { ...this.renderBaseOptions, ...{ fill: { color: '#00f0f0' } } };
    };
}
