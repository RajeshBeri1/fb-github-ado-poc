import { Block } from '../../models/block';

class SubTotalBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default SubTotalBlock;
