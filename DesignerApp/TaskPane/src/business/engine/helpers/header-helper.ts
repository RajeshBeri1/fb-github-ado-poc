import {
    FlowchartDefinition,
    HeaderDataRow,
    HeaderDetailsRow,
    HeaderRow,
    ValueSeparator,
} from '@omniflow/omni-webapi';
import * as API from '@omniflow/omni-webapi';
import {dataApi} from '../../../lib/api';
import HeaderBlock from '../blocks/header/header-block';
import {TColumn} from '../../../lib/utils/getColumnsFromDataDictonary';
import {
    findFieldUsageInfo,
    formatNumberFromDataType,
} from './number-format-helper';

export type THeaderRowData = {
    rowValues: API.HeaderDataRow[];
    rowDetailValues: API.HeaderDataRow[];
};

// In-flight request cache to dedupe concurrent header requests.
const _pendingHeaderRequests: Map<string, Promise<THeaderRowData>> = new Map();

export const getRowValues = async (
    clientId: string,
    definition: FlowchartDefinition | undefined
): Promise<THeaderRowData> => {
    if (
        !definition?.MediaHierarchyDefinition ||
        !definition?.CalendarDefinition
    ) {
        return {rowValues: undefined, rowDetailValues: undefined};
    }

    // Key by clientId + serialized definition. Keep this deterministic.
    const key = `${clientId}|${JSON.stringify(definition || {})}`;

    // If an identical request is already in-flight, reuse its promise.
    if (_pendingHeaderRequests.has(key)) {
        return _pendingHeaderRequests.get(key) as Promise<THeaderRowData>;
    }

    // Create the in-flight request and store it.
    const p = (async (): Promise<THeaderRowData> => {
        // Helpful trace for debugging duplicate triggers
        // console.trace('dataGetHeaderData called', { clientId });
        const {data: apiData} = await dataApi.dataGetHeaderData({
            OmniClientId: clientId,
            FlowchartDefinition: definition,
            RunRestrictions: [],
        });
        const rowValues = _parseNestedValues(apiData?.Rows);
        const rowDetailValues = _parseNestedValues(apiData?.Details?.Rows);
        return {rowValues, rowDetailValues};
    })();

    _pendingHeaderRequests.set(key, p);
    // Ensure we clean up the cache entry when request settles.
    p.finally(() => _pendingHeaderRequests.delete(key));
    return p;
};

const _parseNestedValues = (values: HeaderDataRow[]) => {
    if (!values) {
        return [];
    }

    return values.map((row) => ({
        ...row,
        ValuesJson: row.ValuesJson.map((value) => JSON.parse(value)),
    }));
};

export const mapSeparator = (separator: ValueSeparator) => {
    switch (separator) {
        case ValueSeparator.Comma:
            return ', ';
        case ValueSeparator.Hyphen:
            return ' ‐ ';
        case ValueSeparator.Pipe:
            return ' | ';
        case ValueSeparator.Slash:
            return ' / ';
        case ValueSeparator.Underscore:
            return ' _ ';
        default:
            return ' ';
    }
};

export const getHeaderBlock = (
    value: string | Date,
    length: number,
    columnStart = 0
) => {
    return [
        new HeaderBlock({
            value,
            columnLength: length,
            columnStart,
        }),
    ];
};

type THeaderLogoBlockProps = {
    value: string | Date;
    length: number;
    logoRight: string;
    logoRightColumns: number;
    logoLeft: string;
    logoLeftColumns: number;
    logoCenter: string;
    logoCenterColumns: number;
    logoHeight: number;
};
export const getHeaderBlockWithLogo = ({
                                           value,
                                           length,
                                           logoRight,
                                           logoRightColumns,
                                           logoLeft,
                                           logoLeftColumns,
                                           logoCenter,
                                           logoCenterColumns,
                                           logoHeight = 2,
                                       }: THeaderLogoBlockProps) => {
    const headerBlock = [];
    if (logoLeft) {
        headerBlock.push(
            new HeaderBlock({
                value: logoLeft,
                columnLength: logoLeftColumns,
                containsImage: true,
                imageMaxHeight: logoHeight,
            })
        );
    }

    if (logoCenter) {
        const defaultWidth = 6;
        headerBlock.push(
            new HeaderBlock({
                value: logoCenter,
                containsImage: true,
                imageMaxHeight: logoHeight,
                columnLength: parseInt((!logoLeft ? Math.ceil((length + defaultWidth) / 2) : Math.ceil(length / 2)).toString()),
                columnStart: parseInt((!logoLeft ? logoLeftColumns + (length + defaultWidth) / 2 : length / 2).toString()),
            })
        );
    }

    if (!logoCenter) {
        // add the header row content
        headerBlock.push(
            new HeaderBlock({
                value,
                columnLength: length,
                columnStart: !logoLeft ? logoLeftColumns : 0,
            })
        );
    }

    if (logoRight) {
        headerBlock.push(
            new HeaderBlock({
                value: logoRight,
                columnLength: logoRightColumns,
                containsImage: true,
                imageMaxHeight: logoHeight,
            })
        );
    }
    return headerBlock;
};



export const findDataType = (
    tableId: string,
    columnName: string,
    dataColumns: TColumn[]
) => {
    const column = dataColumns.find(
        (c) => c.TableId === tableId && c.Name === columnName
    );

    if (column) {
        return column.Type;
    }

    return 'string';
};

export const findAndFormatValues = (
    row: HeaderRow | HeaderDetailsRow,
    rowValues: HeaderDataRow[],
    dataColumns: TColumn[]
) => {
    const values = rowValues.find(
        (value) =>
            value.ColumnName === row.ColumnName && value.TableId === row.TableId
    );

    if (values?.ValuesJson) {
        const fieldUsageInfo = findFieldUsageInfo(
            row.TableId,
            row.ColumnName,
            dataColumns
        );
        if (fieldUsageInfo.IsMetric) {
            // metrics should be summed
            if (values.ValuesJson && values.ValuesJson.length > 0) {
                // sometimes values.ValuesJson is empty
                const total = values.ValuesJson?.reduce((sum, x) => sum + x);
                return formatNumberFromDataType(
                    Number(total),
                    row.TableId,
                    row.ColumnName,
                    dataColumns
                );
            }
            return formatNumberFromDataType(
                Number(0),
                row.TableId,
                row.ColumnName,
                dataColumns);
        } else {
            return values.ValuesJson.join(mapSeparator(row.ValueSeparator));
        }
    }
    return '';
};
