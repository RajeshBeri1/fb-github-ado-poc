import ToolsService from '../../business/tools';
import { Block } from '../block';

export class Month extends Block {
    getLabel = () => '';
    getRenderOptions = (index?: number) => {
        //console.log(index);
        return ToolsService.mergeDeep(this.renderBaseOptions, {
            columnWidth: 25,
            fill: {
                color: index % 2 === 0 ? '#d0d0d0' : '#ffffff',
                // pattern: Excel.FillPattern.gray16,
            },
        });
    };
}
