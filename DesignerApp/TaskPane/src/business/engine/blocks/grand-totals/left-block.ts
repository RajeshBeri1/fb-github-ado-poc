import { Block } from '../../models/block';

class LeftBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default LeftBlock;
