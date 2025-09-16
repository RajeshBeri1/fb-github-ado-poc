import React, { JSX, useContext } from 'react';
import { generatePath } from 'react-router-dom';
import { Link as ReactLink, LinkProps } from 'react-router-dom';

import { AppContext } from '../taskpane/contexts/AppContext';

export type TLinkProps = Omit<LinkProps, 'to'> & {
    to: string;
    vars?: { [key: string]: string };
};

const Link = ({ to, vars, ...restProps }: TLinkProps): JSX.Element => {
    const contextProps = useContext(AppContext);
    
    let parsedTo = '';
    try {
        parsedTo = generatePath(to, { ...contextProps, ...vars });
    } catch (e) {
        console.error('Error generating path:', e);
    }

    return <ReactLink to={parsedTo} {...restProps} />;
};

export default Link;
