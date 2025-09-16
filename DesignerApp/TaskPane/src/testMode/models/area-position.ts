import { Coords } from '../../models/coords';

export interface IAreaPosition {
    address: string;
    startCoords: Coords;
}

export class AreaPosition implements IAreaPosition {
    address: string;
    startCoords: Coords;

    constructor(area: IAreaPosition | AreaPosition) {
        this.address = area.address;
        this.startCoords = area.startCoords;
    }
}
