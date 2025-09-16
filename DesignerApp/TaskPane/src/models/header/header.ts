import { Block } from '../block';

export class Header extends Block {
    getLabel = () => '' + this.value;
    getRenderOptions = () => {
        return {
            ...this.renderBaseOptions,
            ...{
                fill: {
                    color: '#ffffff',
                },
                borders: {},
            },
        };
    };
}

export class HeaderDetails extends Block {
    getLabel = () => '' + this.value;
    getRenderOptions = () => {
        return {
            ...this.renderBaseOptions,
            ...{
                fill: {
                    color: '#a6a6a6',
                },
            },
        };
    };
}
