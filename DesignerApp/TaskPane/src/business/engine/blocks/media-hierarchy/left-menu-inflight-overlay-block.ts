import { Block } from '../../models/block';

class LeftMenuInflightOverlayBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}

export default LeftMenuInflightOverlayBlock;
