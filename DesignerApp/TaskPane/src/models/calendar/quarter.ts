import ToolsService from '../../business/tools';
import { Block } from '../block';

export class Quarter extends Block {
    getLabel = () => 'Q' + this.value;
    getRenderOptions = () => {
        return ToolsService.mergeDeep(this.renderBaseOptions, {
            columnWidth: 15,
            fill: {
                color: '#9f9f9f',
            },
        });
    };
}
