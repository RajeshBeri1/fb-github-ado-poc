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
import {
    css,
    html,
    DocumentElementClipMixin,
    OmniElement,
    OmniIconElement,
    OmniStyleElement,
} from 'omni-ui';
import React, { JSX } from 'react';

interface IModalProps {
    foo: React.Component;
}

interface IModalState {
    isModalOpen: boolean;
}

interface IModalProps {
    body: JSX.Element;
    header: JSX.Element;
    openButtonTitle: string;
    submitButtonTitle: string;
    callback: (title: string) => {};
}

const ModalDialogButton: (value: any) => JSX.Element = (props: IModalProps) => {
    const [isModalOpen, toggleModal] = React.useState(false);

    return (
        <div>
            <button
                className="button is-primary"
                onClick={() => toggleModal(true)}>
                {props.openButtonTitle}
            </button>
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

export default ModalDialogButton;

// export class Modal extends React.Component<IModalProps, IModalState> {
//   constructor(props, context) {
//     super(props, context);
//     this.state = {
//       isModalOpen: true,
//     };
//   }

//   alertStyle;

//   toggleModal() {
//     this.setState({
//       isModalOpen: !this.state.isModalOpen,
//     });
//   }

//   render() {
//     return (
//         <div>
//             <button onClick={() => this.toggleModal()}>toggle</button>
//             <div className={"modal " + (this.state.isModalOpen ? "is-active" : "")}>
//             <div className="modal-background"></div>
//             <div className="modal-content">
//                 <article className="message is-success">
//                     <div className="message-header">
//                         <p>
//                         {/* <omni-icon icon-id="omni:informative:success"></omni-icon> */}
//                             Title of message
//                         </p>
//                     </div>
//                     <div className="message-body">
//                         Lorem ipsum dolor sit amet, consectetur adipiscing elit lorem ipsum dolor.
//                     </div>
//                     <div className="message-footer">
//                         <div className="field is-grouped is-grouped-right">
//                             <div className="control">
//                                 <button
//                                     className="button is-text is-success"
//                                     onClick={() => { this.toggleModal() }}
//                                 >
//                                     Cancel
//                                 </button>
//                             </div>
//                             <div className="control">
//                                 <button className="button is-outlined is-success">Submit</button>
//                             </div>
//                         </div>
//                     </div>
//                 </article>
//             </div>
//         </div>

//         </div>

//     //   <div>
//     //     <button onClick={() => this.toggleModal()}>toggle</button>
//     //     <div className={"modal " + (this.state.isModalOpen ? "is-active" : "")}>
//     //       <div className="modal-background ${this.alertStyle}"></div>
//     //       <div className="modal-content">
//     //         <article className="notification is-danger">
//     //           <button
//     //             className="delete"
//     //             aria-label="delete"
//     //             onClick={() => {
//     //               this.toggleModal();
//     //             }}
//     //           ></button>
//     //           Lorem ipsum dolor sit amet, consectetur adipiscing elit lorem ipsum dolor.
//     //         </article>
//     //       </div>
//     //     </div>
//     //   </div>
//     );
//     // return html`
//     //     <omni-style>

//     //         <!-- Notification Type Modal Alert -->
//     //         <div class="modal ${this.isAlertOpen ? 'is-active' : ''}">
//     //             <div
//     //                 class="modal-background ${this.alertStyle}"
//     //             ></div>
//     //             <div class="modal-content">
//     //                 <article class="notification is-danger">
//     //                     <omni-icon icon-id=omni:informative:error></omni-icon>
//     //                     <button
//     //                         class="delete"
//     //                         aria-label="delete"
//     //                         @click=${() => { this.isAlertOpen = false }}
//     //                     ></button>
//     //                     Lorem ipsum dolor sit amet, consectetur adipiscing elit lorem ipsum dolor.
//     //                 </article>
//     //             </div>
//     //         </div>

//     //         <!-- Message Type Modal Alert -->
//     //         <div class="modal ${this.isAlertOpen ? 'is-active' : ''}">
//     //             <div
//     //                 class="modal-background"
//     //             ></div>
//     //             <div class="modal-content">
//     //                 <article class="message is-success">
//     //                     <div class="message-header">
//     //                         <p>
//     //                             <omni-icon icon-id=omni:informative:success></omni-icon>
//     //                             Title of message
//     //                         </p>
//     //                     </div>
//     //                     <div class="message-body">
//     //                         Lorem ipsum dolor sit amet, consectetur adipiscing elit lorem ipsum dolor.
//     //                     </div>
//     //                     <div class="message-footer">
//     //                         <div class="field is-grouped is-grouped-right">
//     //                             <div class="control">
//     //                                 <button
//     //                                     class="button is-text is-success"
//     //                                     @click=${() => { this.isAlertOpen = false }}
//     //                                 >
//     //                                     Cancel
//     //                                 </button>
//     //                             </div>
//     //                             <div class="control">
//     //                                 <button class="button is-outlined is-success">Submit</button>
//     //                             </div>
//     //                         </div>
//     //                     </div>
//     //                 </article>
//     //             </div>
//     //         </div>
//     //     </omni-style>
//     // `;
//   }
// }
