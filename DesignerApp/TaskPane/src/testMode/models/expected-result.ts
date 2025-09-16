import { TestComponent } from '../../enums/test-component.enum';
import { AreaPosition } from './area-position';

export interface IExpectedResult {
    testId: string;
    component: TestComponent;
    areas: Excel.Interfaces.RangeData[];
    areaPositions: AreaPosition[];
    range: Excel.Interfaces.RangeData;
    rows: Excel.RowProperties[];
    columns: Excel.ColumnProperties[];
    cells: Excel.CellProperties[][];
}

export class ExpectedResult implements IExpectedResult {
    testId: string;
    component: TestComponent;

    areas: Excel.Interfaces.RangeData[];

    areaPositions: AreaPosition[];
    range: Excel.Interfaces.RangeData;
    rows: Excel.RowProperties[];
    columns: Excel.ColumnProperties[];
    cells: Excel.CellProperties[][];

    constructor(value: IExpectedResult | ExpectedResult) {
        this.testId = value.testId;
        this.component = value.component;

        this.areas = value.areas;
        this.areaPositions = value.areaPositions;
        this.range = value.range;
        this.rows = value.rows;
        this.columns = value.columns;
        this.cells = value.cells;
    }
}
