import { useNavigate } from 'react-router';
import { useCallback, useContext } from 'react';
import { generatePath } from 'react-router';

import { AppContext } from '../taskpane/contexts/AppContext';

const useGoto = (): any => {
    const navigate = useNavigate();
    const contextProps = useContext(AppContext);

    const goto = useCallback((to: string, vars?: { [key: string]: string }) => {
        try {
            const parsedTo = generatePath(to, { ...contextProps, ...vars });
            navigate(parsedTo);
        } catch (e) {
            console.error(e);
        }
    }, []);

    return goto;
};

export default useGoto;
