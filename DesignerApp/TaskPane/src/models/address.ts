import { Column } from '../enums/column.enum';

export class Address {
    column: string | null = null;
    row: string | number | null = null;
    text: string | null = null;
}
