import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

import { Page } from '../../enums/page.enum';
import { SUB_PATHS } from '../routes';
import { FooterForm } from './footer-form';
import { FooterList } from './footer-list';

const FooterIndex = () => {
    const params = useParams() as any;
    return (
        <Routes>
            <Route path={SUB_PATHS.LIST} element={<FooterList params={params} /> }>
            </Route>
            <Route path={SUB_PATHS.CREATE} element={<FooterForm params={params} /> }>
            </Route>
            <Route path={SUB_PATHS.UPDATE} element={<FooterForm isUpdating={true} params={params} />}> 
            </Route>
            <Route path="*" element={
                <Navigate to={`${Page.List}`} /> }>
            </Route>
        </Routes>
    );
};

export default FooterIndex;
