import React, { JSX } from 'react';
//import { FlowchartTemplateApi } from '@omniflow/omni-webapi';
//import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';
//import RequestInterceptor from '../../business/request-interceptor';

import ModalDialogIcon from '../modal/modalDialog-icon';
// import {
//     FlowChartTemplateDelete
// } from '@omniflow/omni-webapi';
//import useOpenDocument from '../../hooks/useOpenDocument';
import { Tooltip } from '../../omni-ui-components/tooltip';

type Props = {
    templateGuid: string;
};

// eslint-disable-next-line @typescript-eslint/explicit-module-boundary-types
const DeleteFlowchartTemplateDialog = (props: Props) => {
    //const { openDocument, openDocumentbyReportId } = useOpenDocument();
    //ToDo!! set proper type

    //const templateApi = new FlowchartTemplateApi(
    //    WEBAPI_CONFIGURATION,
    //    '',
    //    RequestInterceptor
    //);

    const DeleteFlowchart = () => {
        // templateApi
        //     .flowchartTemplateDelete(
        //         plainToClass(FlowchartTemplateDeleteDTO, {
        //             id: this.props.templateGuid,
        //         })
        // );
    };

    const modalHeader: JSX.Element = <p>New template</p>;
    const modalBody: JSX.Element = (
        <div>
            <div>
                Please select the name and the brand details for your new
                template.
            </div>
            <br />
            <div>
                {/* Error should be displayed in case user enters the same template name that has already been used before */}
                <div className="dataCard">
                    <label htmlFor="brand-name" className="label">
                        Brand name
                    </label>
                    <br />
                    <input
                        id="brand-name"
                        type="text"
                        placeholder="Enter brand name"
                        className="input text"
                    />
                </div>
                <div className="dataCard">
                    <label htmlFor="template-name" className="label">
                        Template name
                    </label>
                    <br />
                    <input
                        id="template-name"
                        type="text"
                        placeholder="Enter template name"
                        className="input text"
                    />
                </div>
            </div>
        </div>
    );

    return (
        <Tooltip>
            <ModalDialogIcon
                body={modalBody}
                header={modalHeader}
                openOmniIconName="omni:interactive:delete"
                submitButtonTitle="Delete template"
                callback={DeleteFlowchart}
            />
            <div slot="content">delete</div>
        </Tooltip>
    );
};

export default DeleteFlowchartTemplateDialog;
