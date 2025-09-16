import React, {JSX,memo, useContext, useState} from 'react';
import * as API from '@omniflow/omni-webapi';
import classNames from 'clsx';

import {Icon} from '../../../omni/icon';
import {Tile} from '../../../omni/tile';
import Button from '../../../components/buttons/Button';
import {mediaHierarchyTemplateApi} from '../../../lib/api';
import useNotification, {
    NotificationType,
} from '../../../components/notification/useNotification';
import {AppContext} from '../../../taskpane/contexts/AppContext';
import Dialog, {DialogType} from '../../../components/dialogs/Dialog';
import routes from '../routes';
import {UrlAndQueryParamKey} from '../../../enums/url-and-query-param-key.enum';
import {TBorder} from '../../../components/template-list/template-list.types';
import moment from 'moment/moment';
import { MediaHierarchyTemplateInfoDTO, UserDetailsDTO } from '@omniflow/omni-webapi';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum'

export type TMediaHierarchyTemplateProps = {
    template: API.MediaHierarchyTemplateInfoDTO;
    border?: TBorder;
    onClick?: (id: string) => void;
    onEdit?: (
        x: MediaHierarchyTemplateInfoDTO,
        event?: React.MouseEvent<HTMLButtonElement>,
        isRestricted?: boolean
    ) => void;
    onCopyAfter?: (template: API.MediaHierarchyTemplateDetailsDTO) => void;
    onDeleteAfter?: (id: string) => void;
    onRestore?: (template: API.MediaHierarchyTemplateDetailsDTO) => void;
    isArchived?: boolean;
    currentUser: UserDetailsDTO;
};

const MediaHierarchyTemplate = (
    {
        template,
        border = '',
        onClick = (): void => undefined,
        onEdit = (): void => undefined,
        onCopyAfter = (): void => undefined,
        onDeleteAfter = (): void => undefined,
        onRestore = (): void => undefined,
        isArchived = false,
        currentUser,
    }: TMediaHierarchyTemplateProps): JSX.Element => {
    const pushNotification = useNotification();
    const { flowchartTemplateDefinition, setUnsavedChanges, currentMediaHierarchyDetails } = useContext(AppContext);
    const {OmniClientId} = flowchartTemplateDefinition;
    const [isCopying, setIsCopying] = useState<boolean>(false);
    const [showDeleteDialog, setShowDeleteDialog] = useState<boolean>(false);
    const [isDeleting, setIsDeleting] = useState<boolean>(false);
    const { logEvent } = useEventLogger(Module.MEDIAHIERARCHY);

    const dateFormat = 'D MMM, YYYY - k:mm Z';

    // const isRestricted = (currentUser: UserDetailsDTO): boolean => {
    //     return (currentUser.Id !== template.CreatedByUser.Id)
    // }

    const copyTemplate = async (event) => {
        event.stopPropagation();
        setIsCopying(true);
        logEvent({ action: Action.DUPLICATE });
        try {
            const {data} =
                await mediaHierarchyTemplateApi.mediaHierarchyTemplateGet(
                    template.Id
                );

            const mediaHierarchyTemplate =
                data as API.MediaHierarchyTemplateCreateDTO;
            const {Name} = mediaHierarchyTemplate;
            const copyData =
                await mediaHierarchyTemplateApi.mediaHierarchyTemplateCreate({
                    ...mediaHierarchyTemplate,
                    Name: `${Name}_Copy`,
                    OmniClientId: OmniClientId,
                    IsDefault:false,
                });

            pushNotification(
                `Media Hierarchy Component "${template.Name}" was successfully copied!`,
                NotificationType.SUCCESS
            );
            setIsCopying(false);
            onCopyAfter(copyData.data);
        } catch (e) {
            pushNotification(
                `Media Hierarchy Component "${template.Name}" could not be copied!`,
                NotificationType.DANGER
            );
            setIsCopying(false);
            console.error(e);
        }
    };

    const showDelete = (event) => {
        event.stopPropagation();
        setShowDeleteDialog(true);
    };

    const cancelDelete = () => {
        logEvent({ action: Action.CANCEL });
        setShowDeleteDialog(false);
    };

    const deleteTemplate = async (id: string) => {
        setIsDeleting(true);

        try {
            
            await mediaHierarchyTemplateApi.mediaHierarchyTemplateDelete(id);

            pushNotification(
                `Media Hierarchy Component "${template.Name}" was successfully archived!`,
                NotificationType.SUCCESS
            );
            setIsDeleting(false);
            setShowDeleteDialog(false);
            onDeleteAfter(id);

            if (id === currentMediaHierarchyDetails?.Id) {
                setUnsavedChanges((prevState) => ({ ...prevState, mediaHierarchyDelete: true }));
            }
        } catch (e) {
            pushNotification(
                `Media Hierarchy Component "${template.Name}" could not be archived!`,
                NotificationType.DANGER
            );
            setIsDeleting(false);
            setShowDeleteDialog(false);
            console.error(e);
        }
      
    };
    
    const renderEditBtn = (
        template: MediaHierarchyTemplateInfoDTO,
        isRestricted: boolean = false) => {
        if (isRestricted) {
            return (
                <div></div>
            )
        }
        return (
            <Button
                className="button icon medium mr-4"
                onClick={() => onEdit(template)}
                tooltip="Edit"
                to={routes.update}
                vars={{
                    [UrlAndQueryParamKey.MEDIA_HIERARCHY_TEMPLATE_ID]:
                    template.Id,
                }}
            >
                <Icon icon-id="omni:interactive:edit" onClick={() => logEvent({ action: Action.EDIT })}></Icon>
            </Button>

        )
    }

    const renderDeleteBtn = (
        template: MediaHierarchyTemplateInfoDTO,
        isRestricted: boolean = false) => {

        if (template.IsDefault) {
            return (
                <Button
                    className="icon medium mr-4"
                    tooltip="Default template"
                    onClick={(e) => e.stopPropagation()}
                >
                    <Icon icon-id="omni:informative:info"></Icon>
                </Button>
            )
        }


        if (!template.IsDefault && !isRestricted) {
            return (
                <Button
                    className="icon medium mr-4"
                    tooltip="Archive"
                    onClick={showDelete}>
                    <Icon icon-id="omni:interactive:archive"onClick={() => logEvent({ action: Action.DELETE })} ></Icon>
                </Button>
            );
        }

        return <></>;
   
    }
    const renderRestoreBtn = (
        template: MediaHierarchyTemplateInfoDTO,
        isRestricted: boolean = false) => {
        if (isRestricted) {
            return (
                <div></div>
            )
        }
        return (
            <Button
                className="icon medium mr-4"
                tooltip="Restore"
                onClick={() => onRestore(template)}
            >
                <Icon icon-id="omni:interactive:unarchive"></Icon>
            </Button>
        )
    }

    const renderCopyBtn = (
        template: MediaHierarchyTemplateInfoDTO,
        isRestricted: boolean = false) => {

        if (isRestricted) {
            return (
                < Button
                    className="icon medium mr-4"
                    tooltip="Only the user who created this component may edit or archive it. You may duplicate it and modify the copy."
                    onClick={copyTemplate}
                    loading={isCopying}
                    onlyLoading
                    disabled={isCopying}>
                    <Icon icon-id="omni:interactive:copy"></Icon>
                </Button>
            )
        }
        return (
            <Button
                className="icon medium mr-4"
                tooltip="Duplicate"
                onClick={copyTemplate}
                loading={isCopying}
                onlyLoading
                disabled={isCopying}>
                <Icon icon-id="omni:interactive:copy"></Icon>
            </Button>
        )
    }

    const isRestricted = currentUser.Id !== template.CreatedByUser.Id

    return (
        <>
           
            <Tile
                onClick={() => onClick(template.Id)}
                className={classNames('item', border)}>
                <div className="d-flex w-100">
                    <div className="d-flex w-50 title is-size-4">{template.Name}</div>
                    <div className="d-flex ms-auto">
                        {!isArchived &&
                        <div className="actions">
                            {renderEditBtn(template, isRestricted)}
                            {renderCopyBtn(template, isRestricted)}
                            {renderDeleteBtn(template, isRestricted)}
                        </div>
                        }
                        {isArchived &&
                            <div className="actions">
                                {renderRestoreBtn(template, isRestricted)}
                            </div>
                        }

                    </div>
                </div>
                <div className="d-flex w-100 is-justify-content-space-between ">

                    <div>

                        <div className="is-size-7 ms-auto">
                            Created by:{' '}
                            {template?.CreatedByUser?.DisplayName || '-'}
                        </div>

                        <div className="is-size-7 ms-auto">
                            Version:{' '}
                            {template?.Version || '-'}
                        </div>

                    </div>

                    <div>

                        <div className="is-size-7 ms-auto">
                            Created on:{' '}
                            {moment(template.CreatedDate).format(
                                dateFormat
                            )}
                        </div>

                        <div className="is-size-7 ms-auto">
                            Modified on:{' '}
                            {moment(template.ModifiedDate).format(
                                dateFormat
                            )}
                        </div>
                    </div>
                </div>
            </Tile>
            {showDeleteDialog && (
                <Dialog
                    type={DialogType.DANGER}
                    title="Archive Media Hierarchy Component?"
                    icon="interactive:archive"
                    okLoading={isDeleting}
                    okDisabled={isDeleting}
                    okText='Ok'
                    onOk={() => deleteTemplate(template.Id)}
                    onCancel={cancelDelete}
                    showDialog={showDeleteDialog}
                >
                    Are you sure you want to archive the Media Hierarchy Component
                    "{template.Name}"?
                </Dialog>
            )}
        </>
    );
};

export default memo(MediaHierarchyTemplate);
