import { Block } from '../../models/block';

class LeftMenuSubLevelBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}
 
export default LeftMenuSubLevelBlock;
