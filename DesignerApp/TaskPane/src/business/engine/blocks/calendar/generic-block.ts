import { Block } from '../../models/block';

class GenericBlock extends Block {
    getStyles = () => ({
        ...this.styles,
        rowHeight: 25,
        columnWidth: 33,
    });
}

export default GenericBlock;
