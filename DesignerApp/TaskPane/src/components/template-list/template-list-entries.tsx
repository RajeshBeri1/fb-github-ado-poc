import React from 'react';
import './template-list-entries.css';
import {Tile} from '../../omni/tile';
import {Icon} from '../../omni/icon';
import Button from '../buttons/Button';
import {FlowchartDefinition, UserDetailsDTO} from '@omniflow/omni-webapi';
import {Tooltip} from '../../omni/tooltip';
import {TValueOf} from '../../interfaces/valueOf.type';
import {TemplateInfoDTO} from '../../interfaces/definition.type';
import useTemplateList from '../../hooks/useTemplateList';
import moment from "moment/moment";

export interface ITemplateListProps {
    onClick?: (x: TemplateInfoDTO) => void;
    onDelete?: (
        x: TemplateInfoDTO,
        event?: React.MouseEvent<HTMLButtonElement | HTMLAnchorElement>,
        isRestricted?: boolean,
    ) => void;
    onEdit?: (
        x: TemplateInfoDTO,
        event?: React.MouseEvent<HTMLButtonElement | HTMLAnchorElement>,
        isRestricted?: boolean,
    ) => void;
    onCopy?: (
        x: TemplateInfoDTO,
        event?: React.MouseEvent<HTMLButtonElement | HTMLAnchorElement>,
    ) => void;
    onRestore?: (
        x: TemplateInfoDTO,
        event?: React.MouseEvent<HTMLButtonElement | HTMLAnchorElement>,
        isRestricted?: boolean,
    ) => void;
    items?: TemplateInfoDTO[];
    arhiveItems?: TemplateInfoDTO[];
    isArchived?: boolean;
    templateToCopy?: string;
    currentTemplate: TemplateInfoDTO;
    currentUser: UserDetailsDTO;
    currentflowchartData?: TValueOf<FlowchartDefinition>;
}

export const TemplateListEntries: React.FC<ITemplateListProps> = (
    {
        items,
        arhiveItems,
        isArchived,
        onClick,
        onEdit,
        onCopy,
        onDelete,
        onRestore,
        templateToCopy,
        currentTemplate,
        currentUser,
        currentflowchartData,
    }) => {
    const dateFormat = 'D MMM, YYYY - k:mm Z';
    const { getListItemBorder } = useTemplateList();

    const renderEditBtn = (item: TemplateInfoDTO, isRestricted = false) => (
        <Button
            tooltip="Edit"
            onClick={(e) => onEdit(item, e, isRestricted)}
            className="icon medium mr-4">
            <Icon icon-id="omni:interactive:edit"></Icon>
        </Button>
    );

    const renderDeleteBtn = (item: TemplateInfoDTO, isRestricted = false) => (
        <Tooltip>
            <button
                slot="invoker"
                onClick={(e) => onDelete(item, e, isRestricted)}
                className="icon medium">
                <Icon icon-id="omni:interactive:archive"></Icon>
            </button>
            <div slot="content">Archive</div>
        </Tooltip>
    );
    const renderRestoreBtn = (item: TemplateInfoDTO,e,isRestricted = false) => {
        return (
            <Tooltip>
                <button
                    slot="invoker"
                    onClick={(e) => onRestore(item, e, isRestricted)}
                    className="icon medium mr-4">
                    <Icon icon-id="omni:interactive:unarchive"></Icon>
                </button>
                <div slot="content">Restore</div>
            </Tooltip>
        );
    }
    const renderDefaultTemplateInfoBtn = (item: TemplateInfoDTO) => {
        return (
            <Tooltip>
                <button slot="invoker" className="icon medium" onClick={(e) => e.stopPropagation()} >
                    <Icon icon-id="omni:informative:info" aria-hidden="true" ></Icon>
                </button>
                <div slot="content">Default Template</div>
            </Tooltip>
        );
    }
    const renderCopyBtn = (item: TemplateInfoDTO, isRestricted = false) => {
        if (!isRestricted) {
            return (
                <Button
                    loading={
                        templateToCopy &&
                        templateToCopy === item.Id
                    }
                    onClick={(e) => onCopy(item, e)}
                    tooltip="Duplicate"
                    className="icon medium mr-4">
                    {(!templateToCopy ||
                        templateToCopy !== item.Id) && (
                            <Icon icon-id="omni:interactive:copy" 
                            ></Icon>
                        )}
                </Button>
            )
        }

        return (
            <Button
                loading={
                    templateToCopy &&
                    templateToCopy === item.Id
                }
                onClick={(e) => onCopy(item, e)}
                tooltip="Only the user who created this template may edit or archive it. You may duplicate it and modify the copy."
                className="icon medium">
                {(!templateToCopy ||
                    templateToCopy !== item.Id) && (
                        <Icon icon-id="omni:interactive:copy" ></Icon>
                    )}
            </Button>)
    };

    const tooltipMessage = () => (
        <div slot="content">
            Action not possible. You are not the owner of that template.
        </div>
    );

    return (
        <>
            <div className="item-wrapper">
                {items.map((item, index) => {
                    const isRestricted = item.CreatedByUser.Id !== currentUser.Id;

                    return (
                        <Tile
                            key={index}
                            onClick={() => onClick(item)}
                            className={
                                'item ' +
                                getListItemBorder(
                                    item,
                                    currentTemplate,
                                    currentflowchartData
                                )
                            }>
                            <div className="d-flex w-100">
                                <div className="d-flex w-50 title is-size-4">{item.Name}</div>
                                <div className="d-flex ms-auto">
                                    {!isArchived && 
                                    <div className="actions">
                                        {isRestricted ? (
                                            <Tooltip>
                                                {tooltipMessage()}
                                            </Tooltip>
                                        ) : (
                                            renderEditBtn(item)
                                        )}
                                           {renderCopyBtn(item, isRestricted)}
                                           
                                            {!item.IsDefault ? (
                                                 isRestricted ? (
                                            <Tooltip>
                                                {tooltipMessage()}
                                            </Tooltip>
                                        ) : (
                                            renderDeleteBtn(item)
                                            )
                                            ) : (
                                                    renderDefaultTemplateInfoBtn(item)
                                            )}
                                    </div>
                                    }
                                    {isArchived  &&
                                        <div className="actions">
                                            {isRestricted ? (
                                                <Tooltip>
                                                    {tooltipMessage()}
                                                </Tooltip>
                                            ) : (
                                             renderRestoreBtn(item, isRestricted) 
                                            )}
                                        </div>
                                    }

                                </div>
                            </div>
                            <div className="d-flex w-100 is-justify-content-space-between ">

                                <div>
                                    <div className="is-size-7">
                                        Created by: {item.CreatedByUser.DisplayName}
                                    </div>
                                    <div className="is-size-7">
                                        Version: {item.Version}
                                    </div>
                                </div>
                                <div>
                                    <div className="is-size-7">
                                        Created on: {moment(item.CreatedDate).format(dateFormat)}

                                    </div>
                                    <div className="is-size-7">
                                        Modified on: {moment(item.ModifiedDate).format(dateFormat)}
                                    </div>

                                </div>

                            </div>
                        </Tile>
                    );
                })}
            </div>
          
        </>
    )
};
