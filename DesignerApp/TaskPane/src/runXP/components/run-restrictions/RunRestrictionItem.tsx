import React, { JSX, memo, useState, useEffect } from 'react';
import { RunRestriction } from '@omniflow/omni-webapi';
import { findIndex, isEmpty } from 'lodash';
import { plainToClass } from 'class-transformer';
import { Tile } from '../../../omni/tile';
import { Icon } from '../../../omni/icon';
import Button from '../../../components/buttons/Button';
import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import RunRestrictionSettingList from './RunRestrictionSettingList';
import { SelectWithCommonFields } from '../../../components/form/select-with-common-fields';
import { SelectWithCommonFieldsTop } from '../../../components/form/select-with-common-fields-Top';
import { OmniDropDownInput } from "../../../omni/dropdown";

export type RunRestrictionItemProps = {
    tableId: string;
    restriction: RunRestriction;
    columns: TColumn[];
    clientId: string;
    levelId: number,
    deleteRestriction: any
};

const RunRestrictionItem = ({
    tableId,
    restriction,
    columns,
    clientId,
    levelId,
    deleteRestriction
}: RunRestrictionItemProps): JSX.Element => {
    const [showSettings, setShowSettings] = useState<boolean>(false);
    const [selectedColumn, setColumn] = useState<any>();
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const toggleSettings = () => {
        setShowSettings(!showSettings);
    };
    const selectColumn = (column) => {
       // const column = index > -1 ? columns[index] : null;
        setColumn(column);
        restriction.ColumnName = column?.Name;
        restriction.TableId = column?.id;

    };
    const columnValue = findIndex(columns, {
        Name: selectedColumn?.Name || restriction?.ColumnName,
        TableId: selectedColumn?.TableId || restriction?.TableId,
    });

    const columnsOptions = columns.map((column, index) => {
        return {
            value: `${column.DisplayName} (${column.TableName})`,
            id: column.TableId,
            Name: column.Name,
            TableId: column.TableId,
            ValueJson:'',
        };
    })
    
    useEffect(() => {
        //if (!isEmpty(restriction)) {
        //    let ValueJson = "";
        //    try {
        //        ValueJson = JSON.parse(restriction?.ValueJson);
        //    }
        //    catch (e) {
        //        console.log(e);
        //    }
        if (columnsOptions.length) {
            const column = columnsOptions.find((c) => c.Name === restriction?.ColumnName && c.TableId === restriction.TableId);
            if (column) {
                column.ValueJson = restriction?.ValueJson;
            }
            
            setColumn(column);
        }
           

    }, [restriction, restriction?.ValueJson]);

    return (
        <div className=" mt-2">
           
            <div className="d-flex w-100 align-items-center">
                <div className="is-text w-20 p-1 mr-2">
                    <Icon icon-id="omni:interactive:reorder" ></Icon>
                </div>
                <div className="w-25">
                    <span>Restriction {levelId + 1}</span>
                </div>
                <div className="w-75">
                    <OmniDropDownInput
                        hidefooter
                        showxicon
                        typeahead
                        searchindropdown
                        value={[selectedColumn]}
                        selectall
                        options={columnsOptions ?? []}
                        onValueChange={(e: CustomEvent) => selectColumn(e.detail)}
                        placeholder="Column"
                       className="w-100"
                    />
                </div>
                <div className="w-25 d-flex">
                    <div className="mx-auto">
                        <Button className="icon" onClick={toggleSettings}>
                            <Icon
                                icon-id={`omni:interactive:${showSettings ? 'left' : 'edit'
                                    }`}></Icon>
                        </Button>
                        <Button
                            className="icon"
                            tooltip="Delete"
                            onClick={() => deleteRestriction(restriction)}>
                            <Icon icon-id="omni:interactive:remove"></Icon>
                        </Button>
                    </div>
                </div>
            </div>
            {showSettings && (
                <RunRestrictionSettingList
                    column={selectedColumn}
                    clientId={clientId}
                    levelId={levelId}
                    restriction={restriction}
                />
            )}
        </div>
    );
};

export default memo(RunRestrictionItem);
