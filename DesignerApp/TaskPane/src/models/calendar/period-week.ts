import ToolsService from '../../business/tools';
import { Block } from '../block';

export class PeriodWeek extends Block {
    getLabel = () => '' + this.value.periodOfMonth + 'x' + this.value.counter;

    getRenderOptions = (index?: number) => {
        return ToolsService.mergeDeep(this.renderBaseOptions, {
            columnWidth: 25,
            fill: {
                color: '#f0f0f0',
            },
        });
    };
}
