import { Block } from '../../models/block';

class LegendBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default LegendBlock;
