import { Block } from '../../models/block';

class SpacerBlock extends Block {
    getStyles = () => ({
        ...this.styles,
        rowHeight: 25,
        columnWidth: 20,
    });
}

export default SpacerBlock;
