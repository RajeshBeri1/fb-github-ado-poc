import { Block } from '../block';

class LeftMenuSubLevel extends Block {
    getLabel = () => '';
    getRenderOptions = () => {
        return {
            ...this.renderBaseOptions,
            ...{
                rowHeight: 15,
                fill: {
                    color: '#ffffff',
                },
                font: {},
                borders: {},
                horizontalAlignment: Excel.HorizontalAlignment.left,
                verticalAlignment: Excel.VerticalAlignment.center,
            },
        };
    };
}

export default LeftMenuSubLevel;
