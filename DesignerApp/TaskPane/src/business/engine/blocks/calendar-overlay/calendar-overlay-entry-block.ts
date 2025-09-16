import { Block } from '../../models/block';

class CalendarOverlayEntryBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default CalendarOverlayEntryBlock;
