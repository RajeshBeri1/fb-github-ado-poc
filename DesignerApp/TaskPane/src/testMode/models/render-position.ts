import { AddressConverter } from '../../business/address-converter';
import { Address } from '../../models/address';
import { Coords } from '../../models/coords';

export class RenderPosition {
    private coords = new Coords();
    private ac = new AddressConverter();

    constructor(x: number = 0, y: number = 0) {
        this.coords = { x, y };
    }

    /**
     * Get current address (not zero indexed)
     * @returns Address
     */
    public get Address(): Address {
        const { x, y } = this.coords;
        return this.ac.getAddressFromCoords(x + 1, y + 1);
    }

    /**
     * Get current column
     * @returns string
     *
     */
    public get Column(): string {
        return this.Address.column;
    }

    /**
     * Get current row
     * @returns number
     */
    public get Row(): number {
        return this.Address.row as number;
    }

    /**
     * Get current coords (zero indexed)
     * @returns Coords
     */
    public get Coords(): Coords {
        return this.coords;
    }

    /**
     * Get y coordinate
     * @returns number
     */
    public get Y(): number {
        return this.Coords.y;
    }

    /**
     * Get x coordinate
     * @returns number
     */
    public get X(): number {
        return this.Coords.x;
    }

    /**
     * Set coords (zero indexed)
     * @param x
     * @param y
     * @returns Coords
     */
    public setCoords(x: number = 0, y: number = 0): Coords {
        this.coords = { x, y };
        return this.coords;
    }

    /**
     * Add row to position
     * @param rows
     * @returns number
     */
    public addRows(rows: number): number {
        this.coords.y += rows;
        return this.Row;
    }

    /**
     * Get current cell (zero indexed)
     * @returns Coords
     */
    public get Cell(): Coords {
        return this.coords;
    }

    /**
     * Set cell (zero indexed)
     * @param x
     * @param y
     * @returns Coords
     */

    public setCell(x: number = 0, y: number = 0): Coords {
        this.coords = { x, y };
        return this.Cell;
    }

    /**
     *
     * @param from
     * @param to
     * @returns string (Excel Range String)
     */
    public getRange(from: Coords, to: Coords): string {
        const fromAddress = this.ac.getAddressFromCoords(from.x + 1, from.y + 1);
        const toAddress = this.ac.getAddressFromCoords(to.x + 1, to.y + 1);

        return `${fromAddress.column}${fromAddress.row}:${toAddress.column}${toAddress.row}`;
    }
}
