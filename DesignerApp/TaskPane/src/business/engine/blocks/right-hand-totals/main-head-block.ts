import { Block } from '../../models/block';

class MainHeadBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default MainHeadBlock;
