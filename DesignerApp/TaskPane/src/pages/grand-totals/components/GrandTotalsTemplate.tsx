import React, { JSX, memo, useContext, useState } from 'react';
import * as API from '@omniflow/omni-webapi';
import moment from 'moment';
import {
    GrandTotalTemplateInfoDTO,
    MediaHierarchyTemplateInfoDTO,
    UserDetailsDTO,
} from '@omniflow/omni-webapi';

import { Icon } from '../../../omni/icon';
import { Tile } from '../../../omni/tile';
import Button from '../../../components/buttons/Button';
import { grandTotalTemplateApi } from '../../../lib/api';
import useNotification, {
    NotificationType,
} from '../../../components/notification/useNotification';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import Dialog, { DialogType } from '../../../components/dialogs/Dialog';
import routes from '../routes';
import { UrlAndQueryParamKey } from '../../../enums/url-and-query-param-key.enum';
import classNames from 'clsx';
import { TBorder } from '../../../components/template-list/template-list.types';
import useEventLogger from '../../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../../enums/event.enum'

export type TGrandTotalsTemplateProps = {
    template: API.GrandTotalTemplateInfoDTO;
    border?: TBorder;
    onClick?: (id: string) => void;
    onEdit?: (
        x: GrandTotalTemplateInfoDTO,
        event?: React.MouseEvent<HTMLButtonElement>,
        isRestricted?: boolean
    ) => void;
    onCopyAfter?: (template: API.GrandTotalTemplateDetailsDTO) => void;
    onDeleteAfter?: (id: string) => void;
    onRestore?: (template: API.GrandTotalTemplateDetailsDTO) => void;
    isArchived?: boolean;
    currentUser?: UserDetailsDTO;
};
const { logEvent } = useEventLogger(Module.GRANDTOTALS);
const GrandTotalsTemplate = ({
    template,
    border = '',
    onClick = (): void => undefined,
    onEdit = (): void => undefined,
    onCopyAfter = (): void => undefined,
    onDeleteAfter = (): void => undefined,
    onRestore = (): void => undefined,
    isArchived = false,
    currentUser,
}: TGrandTotalsTemplateProps): JSX.Element => {
    const pushNotification = useNotification();
    const { flowchartTemplateDefinition, setUnsavedChanges, currentGrandTotalsDetails, setCurrentGrandTotalsDetails } = useContext(AppContext);
    const { OmniClientId } = flowchartTemplateDefinition;
    const [isCopying, setIsCopying] = useState<boolean>(false);
    const [showDeleteDialog, setShowDeleteDialog] = useState<boolean>(false);
    const [isDeleting, setIsDeleting] = useState<boolean>(false);

    const dateFormat = 'D MMM, YYYY - k:mm Z';
    
    const copyTemplate = async (event) => {
        event.stopPropagation();
        setIsCopying(true);
        logEvent({ action: Action.DUPLICATE });
        try {
            const { data } = await grandTotalTemplateApi.grandTotalTemplateGet(
                template.Id
            );

            const grandTotalsTemplate = data as API.GrandTotalTemplateCreateDTO;
            const { Name } = grandTotalsTemplate;
            const copyData =
                await grandTotalTemplateApi.grandTotalTemplateCreate({
                    ...grandTotalsTemplate,
                    Name: `${Name}_Copy`,
                    OmniClientId: OmniClientId,
                    IsDefault:false,
                });

            pushNotification(
                `Grand Totals Component "${template.Name}" was successfully copied!`,
                NotificationType.SUCCESS
            );
            setIsCopying(false);
            onCopyAfter(copyData.data);
        } catch (e) {
            pushNotification(
                `Grand Totals Component "${template.Name}" could not be copied!`,
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
        logEvent({ action: Action.CANCELDELETE });
        setShowDeleteDialog(false);
    };

    const deleteTemplate = async (id: string) => {
        setIsDeleting(true);
        logEvent({ action: Action.DELETE });
        try {
            
            await grandTotalTemplateApi.grandTotalTemplateDelete(id);

            pushNotification(
                `Grand Totals Component "${template.Name}" was successfully archived!`,
                NotificationType.SUCCESS
            );
            setIsDeleting(false);
            setShowDeleteDialog(false);
            onDeleteAfter(id);
            
            if (id === currentGrandTotalsDetails?.Id) {
                setUnsavedChanges((prevState) => ({ ...prevState, grandTotalsDelete: true })); 
            }
          
        } catch (e) {
            pushNotification(
                `Grand Totals Component "${template.Name}" could not be archived!`,
                NotificationType.DANGER
            );
            setIsDeleting(false);
            setShowDeleteDialog(false);
            console.error(e);
        }
       
        
    };
    const renderEditBtn = (
        template: MediaHierarchyTemplateInfoDTO,
        isRestricted: boolean = false
    ) => {
        if (isRestricted) {
            return <div></div>;
        }
        return (
            <Button
                className="button icon medium mr-4"
                onClick={() => onEdit(template)}
                to={routes.update}
                tooltip="Edit"
                vars={{
                    [UrlAndQueryParamKey.GRAND_TOTALS_TEMPLATE_ID]: template.Id,
                }}>
                <Icon icon-id="omni:interactive:edit" ></Icon>
            </Button>
        );
    };

    const renderDeleteBtn = (
        template: MediaHierarchyTemplateInfoDTO,
        isRestricted: boolean = false
    ) => {
        if (template.IsDefault) {
            return (
                <Button
                    className="icon medium mr-4"
                    onClick={(e) => e.stopPropagation()}
                    tooltip="Default template"
                >
                    <Icon icon-id="omni:informative:info"></Icon>
                </Button>
            )
        }


        if (!template.IsDefault && !isRestricted) {
            return (
                <Button className="icon medium mr-4" onClick={showDelete} tooltip="Archive">
                    <Icon icon-id="omni:interactive:archive" ></Icon>
                </Button>
            );
        }

        return <></>;
        
    };

    const renderRestoreBtn = (
        template: MediaHierarchyTemplateInfoDTO,
        isRestricted: boolean = false
    ) => {
        if (isRestricted) {
            return <div></div>;
        }
        return (
            <Button
                className="icon medium mr-4"
                onClick={() => onRestore(template)}
                tooltip="Restore">
                <Icon icon-id="omni:interactive:unarchive"></Icon>
            </Button>
        );
    }

    const renderCopyBtn = (
        template: MediaHierarchyTemplateInfoDTO,
        isRestricted: boolean = false
    ) => {
        if (isRestricted) {
            return (
                <Button
                    tooltip="Only the user who created this component may edit or delete it. You may duplicate it and modify the copy."
                    className="icon medium mr-4"
                    onClick={copyTemplate}
                    loading={isCopying}
                    onlyLoading
                    disabled={isCopying}>
                    <Icon icon-id="omni:interactive:copy" ></Icon>
                </Button>
            );
        }
        return (
            <Button
                tooltip="Duplicate"
                className="icon medium mr-4"
                onClick={copyTemplate}
                loading={isCopying}
                onlyLoading
                disabled={isCopying}>
                <Icon icon-id="omni:interactive:copy"></Icon>
            </Button>
        );
    };

    const isRestricted = currentUser.Id !== template.CreatedByUser.Id;

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
                    title="Archive Grand Totals Component?"
                    icon="interactive:archive"
                    okLoading={isDeleting}
                    okDisabled={isDeleting}
                    okText='Ok'
                    onOk={() => deleteTemplate(template.Id)}
                    onCancel={cancelDelete}
                    showDialog={showDeleteDialog}
                >
                    Are you sure you want to archive the Grand Totals Component "
                    {template.Name}"?
                </Dialog>
            )}
        </>
    );
};

export default GrandTotalsTemplate;
