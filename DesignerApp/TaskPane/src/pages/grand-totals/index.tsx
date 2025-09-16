import React from 'react';
import { Navigate, Route, Routes } from 'react-router';
import { SUB_PATHS } from './routes'
import { GrandTotalsTemplateProvider } from './states/GrandTotalsState';
import GrandTotalsList from './list';
import GrandTotalsCreateOrUpdate from './create-or-update';
import { Page } from '../../enums/page.enum';

const GrandTotalsIndex = () => (
    <Routes>
        <Route path={SUB_PATHS.LIST} element={
            <GrandTotalsList /> }>
        </Route>
        <Route path={SUB_PATHS.CREATE} element={
            <GrandTotalsTemplateProvider>
                <GrandTotalsCreateOrUpdate />
            </GrandTotalsTemplateProvider> }>
        </Route>
        <Route path={SUB_PATHS.UPDATE} element={
            <GrandTotalsTemplateProvider>
                <GrandTotalsCreateOrUpdate />
            </GrandTotalsTemplateProvider> }>
        </Route>
        <Route path="*" element={
            <Navigate to={`${Page.List}`} /> }>
        </Route>
    </Routes>
);

export default GrandTotalsIndex;
