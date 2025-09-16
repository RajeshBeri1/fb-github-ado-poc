import { Block } from '../../models/block';

class FooterBlock extends Block {
    getStyles = () => ({
        ...this.styles,
        wrapText: false,
    });
}

export default FooterBlock;
