export enum Column {
    A = 'A',
    B = 'B',
    C = 'C',
    D = 'D',
    E = 'E',
    F = 'F',
    G = 'G',
    H = 'H',
    I = 'I',
    J = 'J',
    K = 'K',
    L = 'L',
    M = 'M',
    N = 'N',
    O = 'O',
    P = 'P',
    Q = 'Q',
    R = 'R',
    S = 'S',
    T = 'T',
    U = 'U',
    V = 'V',
    W = 'W',
    X = 'X',
    Y = 'Y',
    Z = 'Z',
}

export const ColumnMap: Map<number, Column> = new Map();
export const ColumnMapZero: Map<number, Column> = new Map();
export const CoordsMap: Map<Column, number> = new Map();
Object.entries(Column).map((x, y) => {
    ColumnMap.set(y + 1, x[1]);
    ColumnMapZero.set(y, x[1]);
    CoordsMap.set(x[1], y + 1);
});
