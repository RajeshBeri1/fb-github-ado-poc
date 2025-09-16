import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

import { ThemeTemplateList } from './components/theme-template-list/theme-template-list';
import { Page } from '../../enums/page.enum';
import { SUB_PATHS } from '../routes';
import { ThemeForm } from './components/theme-form/theme-form';

const ThemeIndex = () => {
    const params = useParams() as any;
    return (
        <Routes>
            <Route path={SUB_PATHS.LIST} element={
                <ThemeTemplateList params={params} /> }>
            </Route>
            <Route path={SUB_PATHS.CREATE} element={<ThemeForm params={params} />}>
                
            </Route>
            <Route path={SUB_PATHS.UPDATE} element={
                <ThemeForm isUpdating={true} params={params} /> }>
            </Route>
            <Route path="*" element={
                <Navigate to={`${Page.List}`} /> }>
            </Route>
        </Routes>
    );
};

export default ThemeIndex;
