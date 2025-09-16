import { Block } from '../../models/block';

class ValueBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default ValueBlock;
