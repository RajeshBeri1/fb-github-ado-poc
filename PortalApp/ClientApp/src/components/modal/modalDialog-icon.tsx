/* eslint-disable @typescript-eslint/no-unused-vars */
import React, { JSX } from 'react';
import {
    css,
    html,
    DocumentElementClipMixin,
    OmniElement,
    OmniIconElement,
    OmniStyleElement,
} from 'omni-ui';

import { Icon as OmniIcon } from '../../omni-ui-components/icon';

interface IModalState {
    isModalOpen: boolean;
}

interface IModalProps {
    body: JSX.Element;
    header: JSX.Element;
    openOmniIconName: string;
    submitButtonTitle: string;
    callback: (title: string) => {};
}

const ModalDialogIcon: (value: any) => JSX.Element = (props: IModalProps) => {
    const [isModalOpen, toggleModal] = React.useState(false);

    return (
        <div>
            <a
                href="#"
                className="button is-text is-alt-1"
                onClick={() => toggleModal(true)}
                slot="invoker">
                <span className="is-sr-only">toggle Nav</span>
                <OmniIcon icon-id={props.openOmniIconName}></OmniIcon>
            </a>

            <div className={'modal ' + (isModalOpen ? 'is-active' : '')}>
                <div className="modal-background"></div>
                <div className="modal-content">
                    <article className="message is-success">
                        <div className="message-header">{props.header}</div>
                        <div className="message-body">{props.body}</div>
                        <div className="message-footer">
                            <div className="field is-grouped is-grouped-right">
                                <div className="control">
                                    <button
                                        className="button is-text is-success"
                                        onClick={() => {
                                            toggleModal(false);
                                        }}>
                                        Cancel
                                    </button>
                                </div>
                                <div className="control">
                                    <button
                                        onClick={() => {
                                            props.callback('');
                                            toggleModal(false);
                                        }}
                                        className="button is-outlined is-success">
                                        {props.submitButtonTitle}
                                    </button>
                                </div>
                            </div>
                        </div>
                    </article>
                </div>
            </div>
        </div>
    );
};

export default ModalDialogIcon;
