import React, { JSX, memo, useCallback, useContext, useState } from 'react';
import * as API from '@omniflow/omni-webapi';
import Button from '../../../components/buttons/Button';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import useMediaHierarchyLevels from '../hooks/useMediaHierarchyLevels';
import { find } from 'lodash';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import MediaHierarchySubTotal from './MediaHierarchySubTotal';
import useAddMediaHierarchyLevelSubTotal from '../hooks/useAddMediaHierarchyLevelSubTotal';
import Dialog from '../../../components/dialogs/Dialog';
import { DefaultStyling } from '../../../business/engine/models/styles';
import useNotification, { NotificationType } from '../../../components/notification/useNotification';

export type TMediaHierarchySubTotalListProps = {
    showModal: boolean;
    levelId: number;
    flightRange?: API.FlightRange;
    metricColumns: TColumn[];
    metricColumn: TColumn;
    hideModal?: () => void;
};

const MediaHierarchySubTotalList = ({
    showModal,
    levelId,
    flightRange = API.FlightRange.FlightTotal,
    metricColumns,
    metricColumn,
    hideModal,
}: TMediaHierarchySubTotalListProps): JSX.Element => {
    const [subTotals, setSubTotals] = useState<API.MediaHierarchyLevelSubTotal[]>([]);
    const levels = useMediaHierarchyLevels();
    const addSubTotalToList = useAddMediaHierarchyLevelSubTotal();
    const appContext = useContext(AppContext);
    const DefaultMetric = appContext.defaultMetrics.NonBriefed;
    const DefaultMetricMediaBrief = appContext.defaultMetrics.Briefed;
    const levelData = find(levels, { Id: levelId });
    const isValidColumnForBriefedctcMetric = appContext.mediaBriefValidMetricColumns.includes(levelData.ColumnName);
    const filteredMetricColumns = levelData.TableId == DefaultMetric.MetricTableId ? metricColumns.filter(mc => (isValidColumnForBriefedctcMetric && mc.Name.toLowerCase() == DefaultMetricMediaBrief.MetricColumnName) || DefaultMetricMediaBrief.MetricTableId != mc.TableId || !appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : levelData.TableId == DefaultMetricMediaBrief.MetricTableId ? metricColumns.filter(mc => levelData.TableId == mc.TableId && appContext.mediaBriefValidColumns.includes(mc.Name.toLowerCase())) : metricColumns;
    const pushNotification = useNotification();

    const createDefaultSubTotal = useCallback((): API.MediaHierarchyLevelSubTotal => ({
        Id: 0,
        LevelId: levelId,
        FlightRange: flightRange,
        ColumnName: metricColumn.Name,
        TableId: metricColumn.TableId,
        IsBriefed: false,
        Styling: DefaultStyling(),
        TitleStyling: DefaultStyling()
    }), [levelId, flightRange, metricColumn]);

    const addSubTotal = useCallback(() => {
        setSubTotals(prev => [...prev, createDefaultSubTotal()]);
    }, [createDefaultSubTotal]);

    const update = (index: number, data: Partial<API.MediaHierarchyLevelSubTotal>) => {
        const updatedSubTotals = [...subTotals];
        updatedSubTotals[index] = {
            ...updatedSubTotals[index],
            ...data,
        };
        setSubTotals(updatedSubTotals);
    }

    const remove = (index: number) => {
        const updatedSubTotals = [...subTotals];
        updatedSubTotals.splice(index, 1);
        setSubTotals(updatedSubTotals);
    }

    const validate = useCallback((): { isExists: boolean, isAllRequiredFieldsSelected: boolean } => {
        try {
            const isExists = subTotals.length > 0;
            const isAllRequiredFieldsSelected = isExists &&
                subTotals.every(subTotal => subTotal.ColumnName && subTotal.FlightRange);

            return { isExists, isAllRequiredFieldsSelected };
        } catch (e) {
            console.error('Validation error:', e);
            return { isExists: false, isAllRequiredFieldsSelected: false };
        }
    }, [subTotals]);

    const done = () => {

        try {
            const { isExists, isAllRequiredFieldsSelected } = validate();
            if (isExists && isAllRequiredFieldsSelected) {
                const settings = levels.find(level => level.Id === levelId)?.Settings || [];
                const levelData = levels.find(level => level.Id === levelId);
                if (settings?.length === 0) {
                    hideModal();
                    return;
                }
                   

                settings.forEach(setting => {
                    const existingSubTotals = setting.SubTotals || [];

                    const newSubTotals = subTotals.filter(newSubTotal => {
                        return !existingSubTotals.some(existingSubTotal =>
                            existingSubTotal.ColumnName === newSubTotal.ColumnName &&
                            existingSubTotal.FlightRange === newSubTotal.FlightRange
                        );
                    });
                    if (newSubTotals?.length > 0) {
                        newSubTotals.forEach((newSubTotal) => {
                            addSubTotalToList(levelId, setting.Id, 1, {
                                FlightRange: newSubTotal.FlightRange,
                                ColumnName: newSubTotal.ColumnName,
                                TableId: newSubTotal.TableId,
                                Styling: newSubTotal.Styling,
                                TitleStyling: newSubTotal.TitleStyling,
                            }, false, levelData.TableId, appContext.defaultMetrics);
                        });
                    }
                });
                hideModal();
            }
            else if (isExists && !isAllRequiredFieldsSelected) {
                pushNotification("Please select all required values to add", NotificationType.DANGER, 2000);
            } else {
                hideModal();
            }

        } catch (e) {
            pushNotification("Error occured while adding sub totals. Please try again later.", NotificationType.DANGER, 2000);
            hideModal();
        }
    }
    return (
        <div>
            {showModal && metricColumns?.length > 0 && (
                <Dialog
                    title={`Bulk Update Sub Totals`}
                    icon="interactive:add"
                    cancelButton={false}
                    okText="Add"
                    onOk={(e) => {
                        e.preventDefault();
                        done();
                    }}
                    className="media-hierarchy-subtotal-list"
                    showDialog={showModal && metricColumns?.length > 0}
                >
                    <div slot="header" className="d-flex is-justify-content-space-between mb-4">
                        Sub Total     
                        <div slot="end">
                            <Button
                                className="is-outlined is-size-7 px-4"
                                onClick={() =>
                                    addSubTotal()
                                }>
                                Add sub total
                            </Button>
                        </div>
                    </div>
                    {subTotals && subTotals?.length > 0 && (
                        <table
                            className="table is-fullwidth is-shadowless"
                            style={{ padding: 0 }}>
                            <thead>
                                <tr className="no-shadow">
                                    <th style={{ padding: 0 }}></th>
                                    <th className="is-size-6" style={{ padding: 0 }}>
                                        Metric *
                                    </th>
                                    <th className="is-size-6" style={{ padding: 0 }}>
                                        Flight Range *
                                    </th>
                                    <th className="is-size-6 text-center" style={{ padding: 0 }}>
                                        Action
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                {subTotals?.map((subTotal, index) => (
                                    <MediaHierarchySubTotal
                                        key={`subtotal-${index}`}
                                        index={index}
                                        subTotal={subTotal}
                                        metricColumns={filteredMetricColumns}
                                        update={update}
                                        remove={remove}
                                    />
                                ))}
                            </tbody>
                        </table>
                    )}
                   
                </Dialog>
            )}
        </div>
    );
};

export default memo(MediaHierarchySubTotalList);

