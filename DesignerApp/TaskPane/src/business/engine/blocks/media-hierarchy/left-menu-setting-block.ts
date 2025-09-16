import { Block } from '../../models/block';

class LeftMenuSettingBlock extends Block {
    getStyles = () => ({
        ...this.styles,
    });
}
 
export default LeftMenuSettingBlock;
