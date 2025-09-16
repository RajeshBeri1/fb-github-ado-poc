/*
 * Modal Alerts UX Guidelines
 *
 * For all alerts, both notifications and messages, the user should not be able to click
 * outside the alert message to close the message.
 *
 * All messages should require the user to click the call-to-action or X to close the message
 *
 * - Larger messages with X would be user-controlled but only clicking on X would clear it
 * - Messages with user actions (buttons) would have two ways to dismiss. Users will have to
 *   click on call-to-actions to close out the message. These messages will not have an X.
 *
 */
import { css, html, DocumentElementClipMixin, OmniElement, OmniIconElement, OmniStyleElement } from 'omni-ui';
import React from 'react';

interface IModalProps {
    foo: React.Component;
}

interface IModalState {
    isModalOpen: boolean;
}
const ModalWrapper = ({ header, body, ...rest }) => {
    const [isModalOpen, toggleModal] = React.useState(true);

    const renderControls = (controls) => {
        if (controls) {
            return (
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
                        <button className="button is-outlined is-success">Submit</button>
                    </div>
                </div>
            );
        }
        return null;
    };

    return (
        <div>
            {/* <button onClick={() => toggleModal(true)}>toggle</button> */}
            <div className={'modal ' + (isModalOpen ? 'is-active' : '')}>
                <div className="modal-background"></div>
                <div className="modal-content">
                    <article className="message is-success">
                        <div className="message-header">{header}</div>
                        <div className="message-body">{body}</div>
                        <div className="message-footer">{renderControls(rest.controls)}</div>
                    </article>
                </div>
            </div>
        </div>
    );
};

export default ModalWrapper;
