import React from 'react';
import { Navigate, Route, Routes } from 'react-router';

import { SUB_PATHS } from './routes';
import { RightHandTotalsTemplateProvider } from './states/RightHandTotalsState';
import RightHandTotalsList from './list';
import RightHandTotalsCreateOrUpdate from './create-or-update';
import { Page } from '../../enums/page.enum';

const RightHandTotalsIndex = () => (
    <Routes>
        <Route path={SUB_PATHS.LIST} element={
            <RightHandTotalsList /> }>
        </Route>
        <Route path={SUB_PATHS.CREATE} element={
            <RightHandTotalsTemplateProvider>
                <RightHandTotalsCreateOrUpdate />
            </RightHandTotalsTemplateProvider> }>
        </Route>
        <Route path={SUB_PATHS.UPDATE} element={
            <RightHandTotalsTemplateProvider>
                <RightHandTotalsCreateOrUpdate />
            </RightHandTotalsTemplateProvider> }>
        </Route>
        <Route path="*" element={
            <Navigate to={`${Page.List}`} /> }>
        </Route>
    </Routes>
);

export default RightHandTotalsIndex;
