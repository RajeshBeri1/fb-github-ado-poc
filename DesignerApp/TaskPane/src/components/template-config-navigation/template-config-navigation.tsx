import React, { useState } from 'react';
import { Page } from '../../enums/page.enum';
import { useNavigate, useResolvedPath, useLocation } from 'react-router-dom';
import { OmniDropDownInput } from '../../omni/dropdown';
import './../../components/template-config-navigation/template-config-navigation.css';

const TemplateConfigNavigation = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [currentPathUrl, setCurrentPathUrl] = useState(
        location.pathname.split('/')[3]
    );
    const [currentPath, setCurrentPath] = useState([{ id: location.pathname.split('/')[3], value: location.pathname.split('/')[3] }]

    );
    const resolvedPath = useResolvedPath('');
    let path = resolvedPath.pathname;
    React.useEffect(() => {
        const segments = location.pathname.split('/');
        setCurrentPath([{ id: decodeURI(segments[3]), value: decodeURI(segments[3]) }]);
    }, [location]);

    const setPageURL = (event) => {

        setCurrentPath([{ id: path, value: path }]);
        const page = event.detail.value;
        navigate(`${path}/${page}`);
    };
    const pageOptions = [
        {
            id: `${Page.Header}`,
            value: `${Page.Header}`
        },
        {
            id: `${Page.Calendar}`,
            value: `${Page.Calendar}`
        },
        {
            id: `${Page.CalendarOverlay}`,
            value: `${Page.CalendarOverlay}`
        },
        {
            id: `${Page.MediaHierarchy}`,
            value: `${Page.MediaHierarchy}`
        },
        {
            id: `${Page.GrandTotals}`,
            value: `${Page.GrandTotals}`
        },
        {
            id: `${Page.RightHandTotals}`,
            value: `${Page.RightHandTotals}`
        },
        {
            id: `${Page.Theme}`,
            value: `${Page.Theme}`
        },
        {
            id: `${Page.Footer}`,
            value: `${Page.Footer}`
        },
    ]
    return (
        <div className="w-125px">
            <OmniDropDownInput className="w-100" variant="label" onValueChange={(x: CustomEvent) => setPageURL(x)} options={pageOptions} value={currentPath} />
        </div>
      
    );
};

export default TemplateConfigNavigation;
