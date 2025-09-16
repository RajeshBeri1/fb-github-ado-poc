import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

import { CalendarOverlayTemplateList } from './calendar-overlay-template-list/calendar-overlay-template-list';
import { Page } from '../../enums/page.enum';
import { SUB_PATHS } from '../routes';
import { CalendarOverlayForm } from './calendar-overlay-form/calendar-overlay-form';

const CalendarOverlayIndex = () => {
    const params = useParams() as any;
    return (
        <Routes>
            <Route path={SUB_PATHS.LIST} element={<CalendarOverlayTemplateList params={params} /> }>
            </Route>
            <Route path={SUB_PATHS.CREATE} element={<CalendarOverlayForm params={params} />}>
            </Route>
            <Route path={SUB_PATHS.UPDATE} element={<CalendarOverlayForm isUpdating={true} params={params} />}>
            </Route>
            <Route path="*" element={<Navigate to={`${Page.List}`} /> }>
            </Route>
        </Routes>
    );
};

export default CalendarOverlayIndex;
