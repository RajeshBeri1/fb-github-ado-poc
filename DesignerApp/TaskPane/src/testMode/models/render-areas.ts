import { Coords } from '../../models/coords';

export interface IRenderAreas {
    infoBox: { from: Coords; to: Coords };
    actualResult: { from: Coords; to: Coords };
    expectedResult: { from: Coords; to: Coords };
    componentSize: { columns: number; rows: number; startAdjustment: number };
}

export class RenderAreas implements IRenderAreas {
    infoBox: { from: Coords; to: Coords };
    actualResult: { from: Coords; to: Coords };
    expectedResult: { from: Coords; to: Coords };
    componentSize: { columns: number; rows: number; startAdjustment: number };

    constructor(value: RenderAreas | IRenderAreas) {
        this.infoBox = value.infoBox;
        this.actualResult = value.actualResult;
        this.expectedResult = value.expectedResult;
        this.componentSize = value.componentSize;
    }
}
