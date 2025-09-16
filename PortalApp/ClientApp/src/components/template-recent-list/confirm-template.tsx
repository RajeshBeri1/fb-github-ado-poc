import { JSX, useRef, useImperativeHandle, forwardRef } from 'react';
import Spinner, { SpinnerType } from '../../omni-ui-components/spinner/Spinner';
import { Modal } from '../../omni-ui-components/modal';

type ConfirmModalPropsType = {
    confirmShareTemplate: boolean;
    templateName: string;
    selectedClientName: any;
    selectedComponentOptions: any;
    setConfirmShareTemplate: (e: any) => void;
    shareTemplateMethod: () => void;
    isShareTemplateInProgress: boolean;
};
const ConfirmShareTemplateModal = forwardRef(({
    confirmShareTemplate,
    templateName,
    selectedClientName,
    selectedComponentOptions,
    setConfirmShareTemplate,
    shareTemplateMethod,
    isShareTemplateInProgress,
}: ConfirmModalPropsType, confirmModalRef): JSX.Element => {


    const modalChildRefConfirm = useRef<any>(null);
    useImperativeHandle(confirmModalRef, () => ({
        showModal: () => {
            if (modalChildRefConfirm.current) {
                modalChildRefConfirm.current.showModal();
            }

        },
        close: () => {
            if (modalChildRefConfirm.current) {
                modalChildRefConfirm.current.close();
            }

        },

    }));

    return (
        <Modal ref={modalChildRefConfirm} className="dialog-500px">
                <div slot="header" className="modal-header">
                    <h2 className="title>">Confirm details</h2>
                </div>
                <div className="">
                    <p className="text-msg mb-3">
                        Your are sharing template:{' '}
                        <span className="fb-bold text-name">
                            {templateName}
                        </span>{' '}
                        with client:{' '}
                        <span className="fb-bold text-name">
                            {selectedClientName.value}
                        </span>
                    </p>
                    <h2 className="fb-bold text-name mb-4">
                        Components shared:
                    </h2>
                    <div className="field">
                        {selectedComponentOptions.map((x: any) => {
                            return (
                                <>
                                    <label className="ml-0 component">
                                        {x.value}
                                    </label>
                                    <ul className="pl-5">
                                        <li className="component-name">
                                            {x.name}
                                        </li>
                                    </ul>
                                </>
                            );
                        })}
                    </div>

                    <div slot="footer" className="field is-grouped is-justify-content-end">
                        <div className="control">
                            <button
                                className="tertiary"
                                onClick={() => modalChildRefConfirm.current.close()}>
                                Cancel
                            </button>
                        </div>
                        <div className="control">
                            <button
                                className="button is-link"
                                disabled={isShareTemplateInProgress}
                                onClick={shareTemplateMethod}>
                                Share
                            </button>
                            {isShareTemplateInProgress && (
                                <Spinner type={SpinnerType.STANDARD} />
                            )}
                        </div>
                    </div>
                </div>
        </Modal>
    );
});
export default ConfirmShareTemplateModal;