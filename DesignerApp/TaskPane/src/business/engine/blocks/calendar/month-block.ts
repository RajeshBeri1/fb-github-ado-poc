import { Block } from '../../models/block';

class MonthBlock extends Block {
    getStyles = (columnIndex) => ({
        ...this.styles,
        rowHeight: 25,
        columnWidth: 15,
    });
}

export default MonthBlock;
