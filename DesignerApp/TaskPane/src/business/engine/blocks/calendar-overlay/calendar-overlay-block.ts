import { Block } from '../../models/block';

class CalendarOverlayBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default CalendarOverlayBlock;
