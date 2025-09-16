import { Block } from '../block';

class LeftMenuSetting extends Block {
    getLabel = () => '';
    getRenderOptions = () => {
        return {
            ...this.renderBaseOptions,
            ...{
                rowHeight: 15,
                font: {
                    bold: true,
                    underline: Excel.RangeUnderlineStyle.single,
                },
                fill: {
                    color: '#ffffff',
                },
                borders: {},
                horizontalAlignment: Excel.HorizontalAlignment.left,
                verticalAlignment: Excel.VerticalAlignment.center,
            },
        };
    };
}

export default LeftMenuSetting;
