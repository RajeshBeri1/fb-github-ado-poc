import React, { useContext, useEffect, useState, KeyboardEvent } from 'react';
import {
    RunConfigurationInfoDTO,
    RunConfigurationSearchDTO,
} from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';
import moment from 'moment';

import Dialog, { DialogType } from '../../components/dialogs/Dialog';
import { runConfigurationApi } from '../../lib/api';
import { AppContext } from '../../taskpane/contexts/AppContext';
import '../../components/dialogs/Dialog.css'
interface IRunXPDialog {
    onClose: () => void;
    onPublish: (id: string) => void;
    isPublishing: boolean;
}

export const RunXPDialog = React.memo(({
    onClose,
    onPublish,
    isPublishing,
}: IRunXPDialog) => {
    const { flowchartTemplateId } = useContext(AppContext);
    const [items, setItems] = useState<RunConfigurationInfoDTO[]>([]);
    const [selectedRunId, setSelectedRunId] = useState('');
    const dateFormat = 'D MMM, YYYY - k:mm Z';

    useEffect(() => {
        loadList().then();
    }, []);

    const loadList = async () => {
        runConfigurationApi
            .runConfigurationList(
                plainToClass(RunConfigurationSearchDTO, {
                    FlowchartTemplateId: flowchartTemplateId,
                    Start: 0,
                    Count: 0,
                })
            )
            .then((x) => {
                if (x.data.Items && x.data.TotalCount) {
                    setItems(x.data.Items);
                }
            });
    };

    const handleOk = () => {
        onPublish(selectedRunId);
    };

    const handleCancel = () => {
        onClose();
    };

    return (
        <Dialog
            title={'Publish Report'}
            icon="interactive:save"
            type={DialogType.INFO}
            okDisabled={isPublishing}
            okText='Ok'
            onOk={handleOk}
            onCancel={handleCancel}>
            <table className="table">
                <thead>
                    <tr>
                        <td className="smallPadding">Name</td>
                        <td className="smallPadding">Last Modified</td>
                    </tr>
                </thead>
                <tbody>
                    {/*<tr onClick={() => setSelectedRunId('')}>*/}
                    {/*    <td className="smallPadding">*/}
                    {/*        <input*/}
                    {/*            className="mr-5"*/}
                    {/*            type="radio"*/}
                    {/*            value={''}*/}
                    {/*            checked={selectedRunId === ''}*/}
                    {/*        />*/}
                    {/*        default*/}
                    {/*    </td>*/}
                    {/*    <td className="smallPadding"></td>*/}
                    {/*</tr>*/}
                    {items.map((item) => {
                        return (
                            item && (
                                <tr
                                    key={item.Id}
                                    onClick={() => setSelectedRunId(item.Id)}>
                                    <td className="smallPadding">
                                        <input
                                            className="mr-5"
                                            type="radio"
                                            value={item.Id}
                                            checked={selectedRunId === item.Id}
                                        />
                                        {item.Name}
                                    </td>
                                    <td className="smallPadding">
                                        {moment(item.ModifiedDate).format(
                                            dateFormat
                                        )}
                                    </td>
                                </tr>
                            )
                        );
                    })}
                </tbody>
            </table>
        </Dialog>
    );
});
