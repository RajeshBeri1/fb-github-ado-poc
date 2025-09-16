import React, { FormEvent, useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './runconfig.scss';
import { DropDownInput, Icon, Tile } from '../../omni-ui-components';
import { Switch } from '../../../src/omni-ui-components/switch';
import { Tooltip } from '../../omni-ui-components/tooltip';
import { Tab } from '../../omni-ui-components/tab';
import { Accordion } from '../../omni-ui-components/accordion';
import Spinner, { SpinnerType } from '../../omni-ui-components/spinner/Spinner';
import { CalendarTimelineSetting } from './Calendar/calendar-timeline';
const RunConfig: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const params = new URLSearchParams(location.search);
    const runConfigId = params.get('runConfigId');
    const [activeTab, setActiveTab] = useState('filters');
    const [trackFormatting, setTrackFormatting] = useState(true);
    const [reportName, setReportName] = useState('My Report');
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const [showRunConfig, setShowRunConfig] = useState(true);
    const [activeButton, setActiveButton] = useState('save');
    const [isLoading, setIsLoading] = useState(true);
    const [calendarDefinition, setCalendarDefinition] = useState({});
    const tabContents = [
        "", // Content 1 for Filters tab
        ""  // Content 2 for Components tab
    ];

    const handleClose = () => {
        setShowRunConfig(false);
    };

    const handleBack = () => {
        navigate(-1);
    };

    const toggleSwitch = (event: any) => {
        setTrackFormatting(event.target.checked);
    };

    const handleRender = () => {
        console.log('Rendering...');
        setActiveButton('render');
    };

    const handleSaveReportVersion = () => {
        console.log('Saving report version...');
        setActiveButton('save');
    };

    const handleSaveDownloadVersion = () => {
        console.log('Saving download version...');
    };

    const dropdownOptions = [
        { label: 'Save as new report version', value: 'Save as new report version' },
        { label: 'Download report in XLSX format', value: 'Download report in XLSX format' }
    ];

    const handleDropdownChange = (e: Event) => {
        const target = e.target as HTMLElement & { value: string };
        const selectedOption = target.value;

        setTimeout(() => {
            if (target.hasAttribute('value')) {
                target.removeAttribute('value');
            }
        }, 100);

        if (selectedOption === 'Save as new report version') {
            handleSaveReportVersion();
        } else if (selectedOption === 'Download report in XLSX format') {
            handleSaveDownloadVersion();
        }
    };

    useEffect(() => {
        const timer = setTimeout(() => {
            setIsLoading(false);
        }, 1500);

        return () => clearTimeout(timer);
    }, []);

    const setTab = (selectedIndex: number) => {
        setActiveTab(selectedIndex === 0 ? 'filters' : 'components');
    }
    /* calendar section */

    return (
        <div className="run-config-page">
            {isLoading ? (
                <Spinner type={SpinnerType.STANDARD} />
            ) : (
                <>
                    {/* Header section */}
                    <div className="header-section">
                        <button
                            onClick={handleBack}>
                            <Icon
                                className="icon-back"
                                icon-id="omni:interactive:back">
                            </Icon>
                            <span className="ml-2 font-light ">Flowchart Templates</span>
                        </button>
                        {/* dropdown menu */}
                        <DropDownInput
                            label=""
                            options={dropdownOptions}
                            onValueChange={handleDropdownChange}
                            variant="label"
                            placeholder="Save Download Version"
                            className="custom-dropdown"
                        />
                    </div>
                    <div className="content-section">
                        {/* Spreadsheet Container */}
                        <div className="spreadsheet-container">
                            <table className="spreadsheet-table">
                                <tbody>
                                    {Array.from({ length: 20 }, (_, rowIndex) => (
                                        <tr key={rowIndex}>
                                            {Array.from({ length: 10 }, (_, colIndex) => (
                                                <td key={colIndex}></td>
                                            ))}
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                        {/* Run Configuration Container */}
                        <div className="run-config-container">
                            <Tile className="mb-1 runxp-main-tile">
                                <div className="is-flex is-align-items-center py-2">
                                    <p className="component-title mr-3">
                                        Run Configuration
                                    </p>
                                    <Icon
                                        className="close-icon"
                                        icon-id="omni:interactive:close"
                                        onClick={handleClose}>
                                    </Icon>
                                </div>
                                <div className="border-line"></div>

                                {/* Tab Container with integrated content */}
                                <div className="tab-header">
                                    <div className="tab-wrapper">
                                        <Tab
                                            tabs={[
                                                { label: 'Filters', isActive: activeTab === 'filters', isDisabled: false },
                                                { label: 'Components', isActive: activeTab === 'components', isDisabled: false }
                                            ]}
                                            contents={tabContents}
                                            onTabChange={(e: any) => {
                                                setTab(e.detail.selectedIndex);
                                            }}
                                        />
                                        {/* Switch positioned next to tabs header */}
                                        <div className="switch-container">
                                            <Switch
                                                onChange={toggleSwitch}
                                                checked={trackFormatting}
                                                className="font-semi-bold">
                                                Track Formatting
                                                <span>
                                                    <Tooltip>
                                                        <Icon
                                                            icon-id='omni:informative:error'
                                                            className='is-size-6 add-position'>
                                                        </Icon>
                                                        <div slot="content">
                                                            Any formatting in excel would be tracked and applied when re-rendered
                                                            (This will overwrite the configuration level changes)
                                                        </div>
                                                    </Tooltip>
                                                </span>
                                            </Switch>
                                        </div>
                                        {/* Add Filter button below tab header */}
                                        {activeTab === 'filters' && (
                                            <div className="filter-section-container">
                                                <span className="filter-info-text">Add filters to see more precise flowchart</span>
                                                <button className="button btn-xp add-filter-button">
                                                    Add filter
                                                </button>
                                            </div>
                                        )}
                                        {activeTab === 'components' && (
                                            <div className="component-section-container mt-5">
                                                <Accordion
                                                    header="Calendar"
                                                    variant="highlighted"
                                                    closed-icon="rightIcon"
                                                    opened-icon="downIcon">
                                                    <div>
                                                        <CalendarTimelineSetting calendarDefinition={calendarDefinition} />
                                                    </div>
                                                </Accordion>
                                                <Accordion className="mt-2"
                                                    header="Media Hierarchy"
                                                    closed-icon="rightIcon"
                                                    variant="highlighted"
                                                    opened-icon="downIcon">
                                                    <p> Lorem Ipsum is simply dummy text of the printing
                                                        of Lorem Ipsum.
                                                    </p>
                                                </Accordion>
                                            </div>
                                        )}
                                    </div>
                                </div>

                                {/* Action Buttons at the bottom */}
                                <div className="button-container">
                                    <button
                                        className={`button btn-xp ${activeButton === 'render' ? 'active-button' : 'text-button'}`}
                                        onClick={handleRender}>
                                        Render
                                    </button>
                                    <button
                                        className={`button btn-xp ${activeButton === 'save' ? 'active-button' : 'text-button'}`}
                                        onClick={handleSaveReportVersion}>
                                        Save Report Version
                                    </button>
                                </div>
                            </Tile>
                        </div>
                    </div >
                </>
            )}
        </div >
    );
};

export default RunConfig;
