import React, { JSX, MouseEvent, PropsWithChildren, useRef, useEffect, useImperativeHandle, forwardRef } from 'react';
import { Icon } from '../../omni/icon';
import Button from '../buttons/Button';
import "../dialogs/Dialog.css";
import { OmniModalElement } from '../../omni/omni-modal';
export enum DialogType {
    'INFO' = 'info',
    'SUCCESS' = 'success',
    'WARNING' = 'warning',
    'DANGER' = 'danger',
}
export enum ButtonType {
    'PRIMARY' = 'primary',
    'OUTLINED' = 'secondary',
    'TEXT' = 'tertiary',
}

export type TDialogProps = PropsWithChildren<{
    title?: string;
    type?: DialogType;
    icon?: string;
    okButton?: boolean;
    okText?: string;
    okLoading?: boolean;
    okDisabled?: boolean;
    onOk?: (event?: MouseEvent<HTMLButtonElement>, fn?: () => {}) => void;
    cancelButton?: boolean;
    cancelText?: string;
    cancelLoading?: boolean;
    cancelDisabled?: boolean;
    onCancel?: (event?: MouseEvent<HTMLButtonElement>, fn?: () => {}) => void;
    checkbox?: string;
    buttonType?: ButtonType;
    isTemplateDialog?: boolean;
    isRequired?: boolean;
    className?: string;
    showDialog?: boolean;
}>;

const Dialog = ({
    title = '',
    type = DialogType.INFO,
    icon,
    okButton = true,
    okText = '',
    okLoading = false,
    okDisabled = false,
    onOk = (): void => undefined,
    cancelButton = true,
    cancelText = 'Cancel',
    cancelLoading = false,
    cancelDisabled = false,
    onCancel = (): void => undefined,
    children,
    checkbox,
    buttonType,
    isRequired = true,
    isTemplateDialog = false,
    className = '',
    showDialog = false,
}: TDialogProps): JSX.Element => {
    const modalChildRefNewComponent = useRef<any>(null);
    useEffect(() => {
        if (showDialog) {
            setTimeout(() => {
                modalChildRefNewComponent?.current?.showModal?.();
            }, 0)

        }
        else {
            modalChildRefNewComponent?.current?.close?.();
        }
    }, [showDialog])

    return (
        <OmniModalElement ref={modalChildRefNewComponent} className={className}>
                    <div slot="header">
                        {title}
                    </div>
                    <div className={`${isTemplateDialog ? 'is-template-modal' : ''} message-body`}>
                        {children}
                    </div>

                    {(okButton || cancelButton) && (
                        <div slot="footer"  className=" is-flex is-justify-content-space-between is-align-items-center">
                            {isRequired && < div className="reqd-text w-50">*  Required fields</div>}
                            <div className="field is-grouped is-grouped-right w-100">
                                {cancelButton && (
                                    <div className="control">
                                        <Button
                                            className={`tertiary`}
                                            loading={cancelLoading}
                                            disabled={cancelDisabled}
                                            onClick={onCancel}>
                                            {cancelText}
                                        </Button>
                                    </div>
                                )}
                                {okButton && okText !== 'Done' && (
                                    <div className="control">
                                        <Button
                                            className={`${buttonType ? buttonType : 'is-' + type} is-large save-version`}
                                            loading={okLoading}
                                            disabled={okDisabled}
                                            onClick={onOk}>
                                            {okText}
                                        </Button>
                                    </div>
                                )}
                                {okButton && okText === 'Done' && (
                                    <div className="control">
                                        <Button
                                            loading={okLoading}
                                            disabled={okDisabled}
                                            onClick={onOk}>
                                            {okText}
                                        </Button>
                                    </div>
                                )}
                            </div>
                        </div>
                    )}
        </OmniModalElement>
    );
    
};

export default Dialog;
