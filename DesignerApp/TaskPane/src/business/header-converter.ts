import {
    HeaderDataRow,
    HeaderDefinition,
    HeaderDetailsRow,
    HeaderRow,
    ValueSeparator,
} from '@omniflow/omni-webapi';
import { dataApi } from '../lib/api';
import { Header, HeaderDetails } from '../models/header/header';

export const getRowValues = async (
    clientId: string,
    definition: HeaderDefinition
) => {
    const apiData = undefined; //await dataApi.dataGetHeaderData({
    //     OmniClientId: clientId,
    //     HeaderDefinition: definition,
    //     RunRestrictions: [],
    // });
    const rowValues = _parseNestedValues(apiData?.Rows);
    const rowDetailValues = _parseNestedValues(apiData?.Details?.Rows);
    return { rowValues, rowDetailValues };
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

export const getHeaderBlock = (value: string | Date, length: number) => {
    return [
        new Header({
            value,
            length,
        }),
    ];
};

export const getHeaderDetailsBlock = (value: string, length: number) => {
    return new HeaderDetails({
        value,
        length,
    });
};

export const findAndFormatValues = (
    row: HeaderRow | HeaderDetailsRow,
    rowValues: HeaderDataRow[]
) => {
    const values = rowValues.find(
        (value) =>
            value.ColumnName === row.ColumnName && value.TableId === row.TableId
    );
    return values.ValuesJson.join(mapSeparator(row.ValueSeparator));
};
