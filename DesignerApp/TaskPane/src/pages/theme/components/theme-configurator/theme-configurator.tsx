import React, { useContext } from 'react';
import { ThemeTemplateDetailsDTO } from '@omniflow/omni-webapi';

import OnLeaveDialog from '../../../../components/dialogs/OnLeaveDialog';
import { Toolbar } from '../../../../omni/toolbar';
import { Icon } from '../../../../omni/icon';
import Link from '../../../../components/Link';
import Button from '../../../../components/buttons/Button';
import { AppContext } from '../../../../taskpane/contexts/AppContext';
import useRender from '../../../../hooks/useRender';

import './theme-configurator.css';
import useEventLogger from '../../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../../enums/event.enum'
export interface IThemeConfiguratorProps {
    themeTemplateDetails: ThemeTemplateDetailsDTO;
    onSave: (definition: ThemeTemplateDetailsDTO) => void;
    returnLink: string;
    isSaving: boolean;
}

export const ThemeConfigurator: React.FC<IThemeConfiguratorProps> = ({
    themeTemplateDetails,
    onSave,
    isSaving = false,
    returnLink,
}) => {
    const appContext = useContext(AppContext);
    const { isRendering } = useRender();
    const { logEvent } = useEventLogger(Module.THEME);
    return (
        <>
            <Toolbar slot="header">
                {themeTemplateDetails ? 'Edit' : 'Create'} theme templates
                <button
                    className={isRendering ? 'spinner' : ''}
                    // onClick={() =>
                    //     render(
                    //         appContext.currentThemeTemplateDetails.Definition
                    //     )
                    // }
                >
                    <Icon icon-id="omni:interactive:refresh"></Icon>
                </button>
                <div slot="end">
                    <Link to={returnLink}>
                        <button>
                            <Icon icon-id="omni:interactive:back"></Icon>
                        </button>
                    </Link>
                </div>
            </Toolbar>
            {/* <ThemeForm themeTemplateDetails={themeTemplateDetails} /> */}

            <div className="d-flex">
                <Link to={returnLink} className="ms-auto button is-text">
                    <button className="ms-auto button is-text" onClick={() => logEvent({ action: Action.CANCEL })} >Cancel</button>
                </Link>
                <Button
                    loading={isSaving}
                    onClick={() => {
                        onSave(appContext.currentThemeTemplateDetails);
                        logEvent({ action: Action.SAVE, subModule: SubModule.THEME })
                    }}>
                    {themeTemplateDetails ? 'Save' : 'Create'} theme template
                </Button>
            </div>

            <OnLeaveDialog when={!isSaving} />
        </>
    );
};
