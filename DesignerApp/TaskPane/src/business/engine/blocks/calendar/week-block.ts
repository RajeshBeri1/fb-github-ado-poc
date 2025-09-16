import { Block } from '../../models/block';

class WeekBlock extends Block {
    getStyles = () => ({
        ...this.styles,
        rowHeight: 25,
        columnWidth: 15,
    });
}

export default WeekBlock;
