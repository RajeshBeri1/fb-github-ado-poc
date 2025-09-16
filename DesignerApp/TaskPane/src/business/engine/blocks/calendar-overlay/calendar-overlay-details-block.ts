import { Block } from '../../models/block';

class CalendarOverlayDetailsBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default CalendarOverlayDetailsBlock;
