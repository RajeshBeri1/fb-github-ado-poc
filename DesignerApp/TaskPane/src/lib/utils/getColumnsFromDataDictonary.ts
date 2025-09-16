import * as API from '@omniflow/omni-webapi';
import {
    ColumnType,
    DataDictionaryColumnDetailsDTO,
} from '@omniflow/omni-webapi';
import { filter, flatMap, sortBy } from 'lodash';
import { FieldInfo } from '../../models/fieldinfo';
import fieldInfo from '../../../assets/datadictionary/fieldinfo.json'

const defaultInclude: ColumnType[] = [
    ColumnType.LongInteger,
    ColumnType.Boolean,
    ColumnType.Decimal,
    ColumnType.String,
    ColumnType.Date,
];

const fieldUsageInfo = (fieldInfo as FieldInfo[]);

export type TColumn = DataDictionaryColumnDetailsDTO & {
    TableName: string;
    TableId: string;
    IsMetric: boolean;
    IsCurrency: boolean;
    IsCommon: boolean;

};

const getFieldInfo = (tableId: string, columnName: string, columnType: string): FieldInfo => {
    // console.debug(`Getting field info for column ${tableId}.${columnName}`);
    let fieldInfo = fieldUsageInfo.find((f) => f.TableId === tableId && f.ColumnName === columnName)
    if (fieldInfo) {
        //console.debug(`Field info found for column ${tableId}.${columnName}`);
        //fieldInfo.IsCommon = true;
        //console.debug(fieldInfo);
        return fieldInfo;

    }
    else {
        // console.debug(`Field info not found for column ${tableId}.${columnName}. Setting defaults`);
        fieldInfo = new FieldInfo();
        fieldInfo.ColumnName = columnName;
        fieldInfo.TableId = tableId;
        fieldInfo.ColumnType = columnType;
        fieldInfo.IsCommon = false;
        fieldInfo.IsCurrency = false;
        fieldInfo.IsMetric = columnType === ColumnType.Decimal ? true : false;
        return fieldInfo;
    }
}


const getColumnsFromDataDictionary = (
    dataDictionary: API.DataDictionaryDTO,
    include: ColumnType[] = defaultInclude
): TColumn[] =>
    sortBy(
        filter(
            flatMap(
                filter(dataDictionary.Tables, { Removed: false }),
                ({ Columns, Name: TableName, Id: TableId }) =>
                    Columns.map((column) => ({
                        ...column,
                        TableName,
                        TableId,
                        IsMetric: (getFieldInfo(TableId, column.Name, column.Type)).IsMetric,
                        IsCurrency: (getFieldInfo(TableId, column.Name,column.Type)).IsCurrency,
                        IsCommon: (getFieldInfo(TableId, column.Name, column.Type)).IsCommon
                    }))
            ),
            ({ Type }) => include.includes(Type)
        ),
        ['DisplayName']
    );

export default getColumnsFromDataDictionary;
