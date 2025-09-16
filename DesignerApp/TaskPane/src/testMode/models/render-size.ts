export interface IRenderSize {
    columns: number;
    rows: number;
    startAdjustment: number;
}

export class RenderSize implements IRenderSize {
    columns: number;
    rows: number;
    startAdjustment: number;

    constructor(size: RenderSize | IRenderSize) {
        this.columns = size.columns;
        this.rows = size.rows;
        this.startAdjustment = size.startAdjustment;
    }
}
