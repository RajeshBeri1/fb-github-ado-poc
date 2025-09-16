import React from 'react';
import { Icon as OmniIconElement } from '../../omni-ui-components/icon';

type Props = {
    title: string;
    content?: string;
    icon?: string;
    type: 'is-info' | 'is-success' | 'is-warning' | 'is-danger';
    okBtn?: string;
    cancelBtn?: string;
    ok: (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => any;
    cancel: (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => any;
};
const ConfirmDialog = (props: Props) => {
    return (
        <div>
            <div className={'modal is-active'}>
                <div className="modal-background"></div>
                <div className="modal-content">
                    <article className={'message ' + props.type}>
                        <div className="message-header">
                            <OmniIconElement icon-id={'omni:' + props.icon}></OmniIconElement>
                            {props.title}
                        </div>
                        <div className="message-body">{props.content}</div>
                        <div className="message-footer">
                            <div className="field is-grouped is-grouped-right">
                                <div className="control">
                                    <button
                                        className="button is-text is-info"
                                        onClick={(e) => props.ok(e)}>
                                        {props.okBtn ? props.okBtn : 'Yes'}
                                    </button>
                                </div>
                                <div className="control">
                                    <button
                                        className="button is-text is-info"
                                        onClick={(e) => props.cancel(e)}>
                                        {props.cancelBtn
                                            ? props.cancelBtn
                                            : 'No'}
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
export default ConfirmDialog;
