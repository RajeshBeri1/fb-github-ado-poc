import ToolsService from '../../business/tools';
import { Block } from '../block';

export class Generic extends Block {
    getLabel = () => '' + this.value;
    getRenderOptions = () => {
        return ToolsService.mergeDeep(this.renderBaseOptions, {
            columnWidth: 25,
            fill: {
                color: '#ffffff',
            },
        });
    };
}
