import React, { useContext, useState } from 'react';

import { Toolbar } from '../../omni/toolbar';
import RenderButton from '../../components/buttons/RenderButton';
import Search from '../../components/Search';
import Button from '../../components/buttons/Button';
import { Tile } from '../../omni/tile';
import routes from './routes';
import GrandTotalsTemplateList from './components/GrandTotalsTemplateList';
import { AppContext } from '../../taskpane/contexts/AppContext';
import { SaveFlowchartButton } from '../../components/buttons/SaveFlowchartButton';
import useEventLogger from '../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../enums/event.enum'
import { OmniSwitchButton } from '../../components/omniSwitchButton';

const { logEvent } = useEventLogger(Module.GRANDTOTALS);
const GrandTotalsList = () => {
    const [search, setSearch] = useState<string>();
    const { currentGrandTotalsDetails } = useContext(AppContext);

    const handleSearch = (value) => {
        logEvent({ action: Action.SEARCH });
        setSearch(value);
    };
    const [showArchiveTemplate, setShowArchiveTemplate] = useState<boolean>(false);
    const [showActiveTemplate, setShowActiveTemplate] = useState<boolean>(true);
    const showHideActiveTemplates = () => {
        setShowArchiveTemplate(false);
        setShowActiveTemplate(true);
    };
    const showHideArchiveTemplates = () => {
        setShowArchiveTemplate(true);
        setShowActiveTemplate(false);
    };

    return (
        <Tile className="fb-list-container">
            <Toolbar slot="header">
                <div slot="start">
                    <button className={`tab calc-margin-tab ${showActiveTemplate ? "active" : ''}`} onClick={showHideActiveTemplates}>
                        Active
                    </button>
                    <button className={`tab calc-margin-tab ${showArchiveTemplate ? "active" : ''}`} onClick={showHideArchiveTemplates}>
                        Archived
                    </button>
                    <RenderButton
                        definitionName="grandTotalsDefinition"
                        definition={currentGrandTotalsDetails?.Definition}
                        autoRender
                    />
                </div>
               
                <div slot="center-end">
                    <Search onSearch={handleSearch} />
                </div>
                <div slot="end" className="toolbar-divider"></div>
                <div slot="end">
                    <Button className="button secondary small" to={routes.create}>
                        New component
                    </Button>
                </div>
            </Toolbar>
            {showActiveTemplate &&
                <GrandTotalsTemplateList search={search} showActiveTemplate={showActiveTemplate} />
            }
            {showArchiveTemplate &&
                <GrandTotalsTemplateList search={search} showArchiveTemplate={showArchiveTemplate} />

            }
            <div className="d-flex mt-5 is-justify-content-space-between">
                <OmniSwitchButton></OmniSwitchButton>
                <SaveFlowchartButton />
            </div>
        </Tile>
    );
};

export default GrandTotalsList;
