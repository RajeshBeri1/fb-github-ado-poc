import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

import { Page } from '../../enums/page.enum';
import { SUB_PATHS } from '../routes';
import { CalendarForm } from './calendar-form';
import { CalendarList } from './calendar-list';

const CalendarIndex = () => {
    const params = useParams() as any;
    return (
        <Routes>
            <Route path={SUB_PATHS.LIST} element={<CalendarList params={params} />}>
              
            </Route>
            <Route path={SUB_PATHS.CREATE} element={<CalendarForm params={params} />}>
               
            </Route>
            <Route path={SUB_PATHS.UPDATE} element={<CalendarForm isUpdating={true} params={params} />}>
                
            </Route>
            <Route path="*" element={<Navigate to={`${Page.List}`} /> }>
              
            </Route>
        </Routes>
    );
};

export default CalendarIndex;
