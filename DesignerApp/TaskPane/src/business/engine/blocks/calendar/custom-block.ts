import { Block } from '../../models/block';

class CustomBlock extends Block {
    constructor(value: string) {
        super();
        this.value = value;
    }
    getStyles = () => ({
        ...this.styles,
        rowHeight: 25,
        columnWidth: 60,
    });
}

export default CustomBlock;
