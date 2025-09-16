import { Block } from '../../models/block';

class HeadBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default HeadBlock;
