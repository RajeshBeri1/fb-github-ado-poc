import { Block } from '../../models/block';

class FlightBarWallInflightOverlayBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default FlightBarWallInflightOverlayBlock;
