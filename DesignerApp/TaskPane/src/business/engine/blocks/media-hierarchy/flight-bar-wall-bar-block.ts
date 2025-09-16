import { Block } from '../../models/block';

class FlightBarWallBarBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default FlightBarWallBarBlock;
