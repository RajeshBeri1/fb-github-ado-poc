import { TColumn } from '../../../lib/utils/getColumnsFromDataDictonary';
import { ColumnType } from '@omniflow/omni-webapi';
import { NumberFormat } from '../../../enums/number-format.enum';

export const findFieldUsageInfo = (
    tableId: string,
    columnName: string,
    dataColumns: TColumn[]
): TColumn => {
    let column = dataColumns.find(
        (c) => c.Name === columnName // && c.TableId === tableId caused undeterministic failures to find a column.
        // Find by column alone should be ok because columns with the same name have the same characteristics across tables
    );

    if (column) {
        return column;
    }

    return {
        TableId: tableId,
        TableName: '',
        Name: columnName,
        Type: ColumnType.String,
        IsMetric: false,
        IsCommon: false,
        IsCurrency: false,
    };
};

export const findNumberFormat = (
    tableId: string,
    columnName: string,
    dataColumns: TColumn[]
): NumberFormat => {
    const fieldUsageInfo = findFieldUsageInfo(tableId, columnName, dataColumns);

    if (fieldUsageInfo) {
        if (fieldUsageInfo.IsMetric)
            if (fieldUsageInfo.IsCurrency) {
                return NumberFormat.Currency;
            } else {
                return NumberFormat.General;
            }
    }

    return NumberFormat.General;
};

export const findCurrencyCode = (currencyCode?: string): string => {
    // In the future for currencies other than USD we have to determine the currency
    // from the data (e.g. campaign.local_currency_name)
    return currencyCode??'USD';
};

export const findUnit = (divisor: number): string => {
    switch (divisor) {
        case 1000: {
            return 'K';
        }
        case 1000000: {
            return 'M';
        }
        case 1000000000: {
            return 'B';
        }
        case 1000000000000: {
            return 'T';
        }
        default: {
            return '';
        }
    }
};


export const formatNumberWithMaxLength = (
    value: number,
    numberFormat: NumberFormat,
    columnLength: number = 1,
    weekly: boolean = false,
    currencyCode?: string,
    precisedValue: boolean = false,
    isDecimal: boolean = false,
    applyToCurrencyMetric:boolean = true,
    applyToNonCurrencyMetric: boolean = true,

): string => {
    // 0 column length is likely an upstream error.
    // Setting to 1 to ensure calculation for worst case
    if (columnLength === 0) columnLength = 1;

    const charactersPerColumn = precisedValue ? 5 : 20; // 4 digits, 5 characters with punctuation
    // maxLength + 1 to account for the leading '
    const maxlength = columnLength * charactersPerColumn + 1;

    // Applied decimals only for currency values
    if (isDecimal) {
        if (numberFormat === NumberFormat.Currency && applyToCurrencyMetric) {
            // If currencyCode exists and applyToCurrencyMetric is true, use 2 decimal places
            return formatNumber(value, numberFormat, 2, currencyCode);
        }

        if (!(numberFormat === NumberFormat.Currency) && applyToNonCurrencyMetric) {
            // If no currencyCode and applyToNonCurrencyMetric is true, use 2 decimal places
            return formatNumber(value, numberFormat, 2);
        }

        // Default case: use 0 decimal places
        return formatNumber(value, numberFormat, 0, currencyCode || undefined);
    }
    
    if (!precisedValue) {
        return formatNumber(value, numberFormat, 0, currencyCode);
    }

    const formattedValue = formatNumber(value, numberFormat, 0, currencyCode);

    if (formatNumber(value, numberFormat, 0, currencyCode).length <= maxlength) {
        return formatNumber(value, numberFormat, 0, currencyCode);
    }

    // Try to shrink number by dividing by increasing multiples of 1000 until
    // the number fits or we run out of units
    let divisor = 1000;
    let decimals = 0;
    for (let i = 1; i <= 4; i++) {
        const inputValue = value / divisor;
        if (inputValue < 1) {
            decimals = 1;
        }
        const outputValue =
            formatNumber(inputValue, numberFormat, decimals, currencyCode) +
            findUnit(divisor);

        if (outputValue.length <= maxlength) {
            return outputValue;
        }

        divisor = divisor * 1000;
    }

    // if the number could not be shrunk enough, someone should see the actual number
    return formattedValue;
};


export const formatCurrencyToPreciseValue = (
    amount: string,
) => {
    if (!amount.includes('K') && !amount.includes('M') && !amount.includes('B') && !amount.includes('T') && amount.includes('$')) {
        amount = amount.replace('$', '').replace("'","").split(',').join('');
        return formatNumberWithMaxLength(parseInt(amount), NumberFormat.Currency, 1, false, 'USD');
    }
    return amount;
}
export const formatNumberFromDataType = (
    value: number,
    tableId: string,
    columnName: string,
    dataColumns: TColumn[],
    currencyCode?: string,
    isDecimal: boolean = false,
    applyToCurrencyMetric: boolean = true,
    applyToNonCurrencyMetric: boolean = true,
): string => {
    const numberFormat = findNumberFormat(tableId, columnName, dataColumns);
    const isCurrency = numberFormat === NumberFormat.Currency;

    if (isDecimal) {
        if (isCurrency && applyToCurrencyMetric) {
            return formatNumber(value, numberFormat, 2, currencyCode);
        }
        if (!isCurrency && applyToNonCurrencyMetric) {
            return formatNumber(value, numberFormat, 2);
        }
        return formatNumber(value, numberFormat, 0, currencyCode);
    }

    return formatNumber(value, numberFormat, 0, currencyCode);
};

export const formatNumber = (
    value: number,
    numberFormat: NumberFormat,
    decimals: number = 0,
    currencyCode?: string,
): string => {
    let formatter = new Intl.NumberFormat('en-US', {
        minimumFractionDigits: decimals,
        maximumFractionDigits: decimals,
    });

    switch (numberFormat) {
        case NumberFormat.General: {
            formatter = new Intl.NumberFormat('en-US', {
                minimumFractionDigits: decimals,
                maximumFractionDigits: decimals,
            });
            break;
        }
        case NumberFormat.Currency: {
            const metricCurrencyCode = findCurrencyCode(clenseCurrency(currencyCode));
            const currencyLanguage = Intl.NumberFormat(metricCurrencyCode)?.resolvedOptions()?.locale;
            formatter = new Intl.NumberFormat(currencyLanguage, {
                style: 'currency',
                currency: metricCurrencyCode,
                minimumFractionDigits: decimals,
                maximumFractionDigits: decimals,
            });
            break;
        }
        case NumberFormat.None: {
            return "'" + value.toString();
        }
    }

    const formattedNumber = formatter.format(Number(value)).toString();

    return formattedNumber;
};
const clenseCurrency = (currencyCode): string => {
    if (currencyCode == "US Dollar (USD)") {
        return "USD";
    }
    return currencyCode;
}

