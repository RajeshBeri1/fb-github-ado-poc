import React, { useEffect, useState } from 'react';
import { isEmpty } from 'lodash';
import { Icon } from '../../omni/icon';
import MediaHierarchyRunLevelList from './media-hierarchy/MediaHierarchyRunLevelList';
import { Tile } from '../../omni/tile';
import { CalendarForm } from './calendar/calendar-form';
import { AppContext } from '../../taskpane/contexts/AppContext';
const RunXPGroup = React.memo(() => {
    const {
        flowchartTemplateDefinition,
        flowchartRunConfiguration, mode, setCurrentCalendarTemplateDetails, currentCalendarTemplateDetails,
    } = React.useContext(AppContext);

    const [mediaHierarchyState, setMediaHierarchyState] =
        useState<boolean>(false);
    const [calendarState, setCalendarState] = useState<boolean>(false);

    useEffect(() => {
       /* setCalendarState(false);*/
        /*  setCalendarState(true);*/
        setCurrentCalendar();
    }, [flowchartRunConfiguration?.CalendarDefinition, mode, flowchartTemplateDefinition]

    )
    const setCurrentCalendar = () => {
        if (mode && flowchartRunConfiguration?.CalendarDefinition != null && isEmpty(currentCalendarTemplateDetails)) {
            setCurrentCalendarTemplateDetails(flowchartRunConfiguration.CalendarDefinition);
        }
    };
    const CategoryHandler = ({ title, onClickHandler }) => {
        return (
            <div
                className="is-size-4 has-text-centered"
                onClick={onClickHandler}>
                <div className="d-flex flex-row">
                    <Icon
                        icon-id="omni:interactive:down"
                        className="is-align-self-flex-end"></Icon>
                    <h2 className="ml-3 active-grp-title">{title}</h2>
                </div>
            </div>
        );
    };

    return (
        <div>
        <Tile className="active-group-tile my-2">
            <CategoryHandler
                title="Calendar"
                onClickHandler={() => setCalendarState(!calendarState)}
            />
            {calendarState && <CalendarForm />}
            </Tile>
        <Tile className="active-group-tile">
            <CategoryHandler
                title="Media Hierarchy"
                onClickHandler={() =>
                    setMediaHierarchyState(!mediaHierarchyState)
                }
            />
            {mediaHierarchyState && <MediaHierarchyRunLevelList />}
        </Tile>
            {/* <CategoryHandler title="Global Filter" onClickHandler={() => setGlobalFilter(!globalFilter)}/>
       {globalFilter &&  <GlobalFilterLevelList />} */}
        </div>
    );
});

export default RunXPGroup;
