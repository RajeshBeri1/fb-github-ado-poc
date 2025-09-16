import { Block } from '../../models/block';

class QuarterBlock extends Block {
    getValue = () => `Q${this.value}`;
    getStyles = () => ({
        ...this.styles,
        rowHeight: 25,
        columnWidth: 15,
    });
}

export default QuarterBlock;
