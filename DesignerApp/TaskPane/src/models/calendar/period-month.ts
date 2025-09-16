import ToolsService from '../../business/tools';
import { Block } from '../block';

export class PeriodMonth extends Block {
    getLabel = () => 'P' + this.value;

    getRenderOptions = () => {
        return ToolsService.mergeDeep(this.renderBaseOptions, {
            fill: {
                color: '#f0f0f0',
            },
        });
    };
}
