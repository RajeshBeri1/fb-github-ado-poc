import { Block } from '../../models/block';

class TotalBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default TotalBlock;
