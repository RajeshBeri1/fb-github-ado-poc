import React from 'react';
import Button from '../buttons/Button';
import Link from '../Link';
import {Icon} from '../../omni/icon';
import {Toolbar} from '../../omni/toolbar';
import {Tooltip} from '../../omni/tooltip';
import RenderButton from '../buttons/RenderButton';
import { TDefinition, TDefinitionName } from '../../interfaces/definition.type';
import '../../pages/calendar/calendar-form/calendar-rows/calendar-rows.css'
interface ICalendarToolbarProps {
    title: string;
    isUpdating: boolean;
    templateName: string;
    goBack: string;
    definitionName: TDefinitionName;
    definition: TDefinition;
    onEditTemplateName: () => void;
}

export const TemplateToolbar: React.FC<ICalendarToolbarProps> = (
    {
        title,
        isUpdating,
        templateName,
        goBack,
        definitionName,
        definition,
        onEditTemplateName,
    }) => {
    return (
        <Toolbar slot="header">
            <p className="component-title">
                {isUpdating ? 'Edit ' : 'Create '}
                {title}
            </p>
            
            <RenderButton
                definitionName={definitionName}
                definition={definition}
            />
            <>
                <div className="template-name-container">
                    <span className="template-name" title={templateName}>{templateName}</span>
                    <Button className='icon' tooltip="Edit" onClick={onEditTemplateName}>
                        <Icon icon-id="omni:interactive:edit"></Icon>
                    </Button>
                </div>
            </>
            <div slot="end">
                <Link to={goBack}>
                    <Button className='icon'  tooltip="Back">
                        <Icon className='is-size-2' icon-id="omni:interactive:back"></Icon>
                    </Button>
                </Link>
            </div>
        </Toolbar>
    );
};
