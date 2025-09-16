import { Block } from '../block';

class LeftMenuLevel extends Block {
    getLabel = () => '';
    getRenderOptions = () => {
        return {
            ...this.renderBaseOptions,
            ...{
                rowHeight: 15,
                fill: {
                    color: '#bfbfbf',
                },
                horizontalAlignment: Excel.HorizontalAlignment.left,
                verticalAlignment: Excel.VerticalAlignment.center,
            },
        };
    };
}

export default LeftMenuLevel;
