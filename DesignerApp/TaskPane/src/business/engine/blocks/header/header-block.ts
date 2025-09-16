import { Block } from '../../models/block';

class HeaderBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}
 
export default HeaderBlock;
