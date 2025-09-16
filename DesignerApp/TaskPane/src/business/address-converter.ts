import { Column, ColumnMapZero, CoordsMap } from '../enums/column.enum';
import { Address } from '../models/address';
import { Coords } from '../models/coords';
import { Range } from '../models/range';

export class AddressConverter {
    private base = 26;

    /**
     * This class helps to convert excel addresses to system coordinates or grid and vice versa.
     * Converts x, y coord to Excel address.
     * @param x x coord. e.g. 704
     * @param y y coord. e.g. 2
     * @returns Adress object e.g. {column: 'AAB', row: 2}
     */
    getAddressFromCoords(x: number, y: number): Address {
        let result = '';

        while (x > 0) {
            const rest = (x - 1) % this.base;
            const addr = ColumnMapZero.get(rest);
            x = Math.trunc((x - rest) / this.base);
            result = addr?.toString() + result;
        }

        return {
            column: result,
            row: y,
            text: result && y ? `${result}${y}` : null,
        };
    }

    /**
     * Converts Excel address to x, y coords.
     * @param column Excel column e.g. 'AK'
     * @param row Excel row eg. 3
     * @returns Coords object e.g. {x: 36, y: 1}
     */
    getCoordsFromAddress(column: string, row: string | number): Coords {
        let result = 0;
        column.split('').map((x, i: number) => {
            const exp = column.length - i - 1;
            const value =
                CoordsMap.get(x.toUpperCase() as unknown as Column) || 0;
            result += value * Math.pow(this.base, exp);
        });
        return {
            x: result,
            y: parseInt(row.toString(), 10),
        };
    }

    /**
     * Get excel address from string e.g.
     * @param address e.g. 'B41'
     * @returns address e.g. {row: '41', column: 'B'}
     */
    getAddressFromString(address: string): Address {
        try {
            const [column, row] = address.toUpperCase().match(/[A-Z]+|[0-9]+/g);

            return {
                row: row,
                column: column,
                text: column && row ? `${column}${row}` : null,
            };
        } catch (e) {
            return null;
        }
    }

    /**
     * Get range from string
     * range must be in form of e.g.  "AA10:GSS1".
     * Check and warn if end values are greater than start values
     * @param range Range
     * @returns range with start and end.
     */
    getRangeFromString(range: string): Range {
        try {
            const [columnStart, rowStart, columnEnd, rowEnd] =
                range.match(/[A-Z]+|[0-9]+/g);
            if (
                this.getCoordsFromAddress(columnStart, 0).x >
                this.getCoordsFromAddress(columnEnd, 0).x
            ) {
                console.warn(
                    `The start address (${columnStart}) is greater then end addres (${columnEnd})`
                );
            }
            return {
                start: {
                    column: columnStart,
                    row: parseInt(rowStart, 10),
                    text:
                        columnStart && rowStart
                            ? `${columnStart}${rowStart}`
                            : null,
                },
                end: {
                    column: columnEnd,
                    row: parseInt(rowEnd, 10),
                    text: columnEnd && rowEnd ? `${columnEnd}${rowEnd}` : null,
                },
            };
        } catch (e) {
            return null;
        }
    }
}

window['AddressConverter'] = AddressConverter;
window['CoordsMap'] = CoordsMap;
