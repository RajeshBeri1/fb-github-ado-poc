import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

import { HeaderList } from './header-list';
import { Page } from '../../enums/page.enum';
import { SUB_PATHS, routes } from '../routes';
import { HeaderForm } from './header-form';

const HeaderIndex = () => {
    const params = useParams() as any;
    return (
        <Routes>
            <Route path={SUB_PATHS.LIST} element={
                <HeaderList params={params} /> }>
            </Route>
            <Route path={SUB_PATHS.CREATE} element={
                <HeaderForm params={params} /> }>
            </Route>
            <Route path={SUB_PATHS.UPDATE} element={
                <HeaderForm isUpdating={true} params={params} />}>
            </Route>
            <Route path="*" element={
                <Navigate to={`${Page.List}`} /> }>
            </Route>
        </Routes>
    );
};

export default HeaderIndex;
