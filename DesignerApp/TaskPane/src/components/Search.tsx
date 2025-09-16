import React, {useState, ChangeEvent, JSX} from 'react';
import {Icon} from '../omni/icon';
import { Tooltip } from '../omni/tooltip';
import '../pages/common-styles.css';
import { SearchInput } from '../omni/search'
export type TSearchProps = {
    onSearch?: (value: string) => void;
};

const Search = ({
                    onSearch = (): void => undefined,
                }: TSearchProps): JSX.Element => {
    const [isSearchVisible, setIsSearchVisible] = useState<boolean>(false);

    const showSearch = () => {
        setIsSearchVisible(true);
    };

    const closeSearch = () => {
        setIsSearchVisible(false);
        onSearch('');
    };

    const handleSearch = (event) => {
        const {value} = event.target;
        onSearch(value);
    };


    return (
        <>
           
                <Tooltip>
                <SearchInput
                    className="input-width-120px"
                    onChange={(e: CustomEvent) => handleSearch(e)}
                            placeholder="Search"
                            isOpen={false}
                        />
                    <div slot="content">
                        Search
                    </div>
                </Tooltip>
            
        </>
    );
};

export default Search;
