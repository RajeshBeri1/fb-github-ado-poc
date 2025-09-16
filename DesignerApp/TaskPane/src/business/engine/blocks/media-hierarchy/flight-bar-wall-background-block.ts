import { Block } from '../../models/block';

class FlightBarWallBackgroundBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}
 
export default FlightBarWallBackgroundBlock;
