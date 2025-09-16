import React from 'react';
import { Navigate, Route, Routes } from 'react-router';

import { SUB_PATHS } from './routes';
import { MediaHierarchyTemplateProvider } from './states/MediaHierarchyState';
import MediaHierarchyList from './list';
import MediaHierarchyCreateOrUpdate from './create-or-update';
import { Page } from '../../enums/page.enum';

const MediaHierarchyIndex = () => (
    <Routes>
        <Route path={SUB_PATHS.LIST} element={
            <MediaHierarchyList /> }>
        </Route>
        <Route path={SUB_PATHS.CREATE} element={
            <MediaHierarchyTemplateProvider>
                <MediaHierarchyCreateOrUpdate />
            </MediaHierarchyTemplateProvider> }>
        </Route>
        <Route path={SUB_PATHS.UPDATE} element={
            <MediaHierarchyTemplateProvider>
                <MediaHierarchyCreateOrUpdate />
            </MediaHierarchyTemplateProvider> }>
        </Route>
        <Route path="*" element={
            <Navigate to={`${Page.List}`} /> }>
        </Route>
    </Routes>
);

export default MediaHierarchyIndex;
