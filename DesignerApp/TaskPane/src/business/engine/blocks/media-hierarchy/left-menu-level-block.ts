import { Block } from '../../models/block';

class LeftMenuLevelBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default LeftMenuLevelBlock;
