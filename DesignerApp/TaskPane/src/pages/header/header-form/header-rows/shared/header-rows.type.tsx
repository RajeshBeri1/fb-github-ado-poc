import { HeaderDetailsRow, HeaderLogo, HeaderRow } from '@omniflow/omni-webapi';

export type TRows = (HeaderRow & {
    id: string;
    Date: string;
})[];

export type TRow = HeaderRow & {
    id: string;
    Date: string;
};

export type TDetailRows = ({
    id: string;
} & HeaderDetailsRow)[];

export type TDetailRow = {
    id: string;
} & HeaderDetailsRow;

export type TLogoRow = {
    id: string;
    ScaleWidth?: number;
    ScaleHeight?: number;
} & HeaderLogo;

export type TLogoRows = ({
    id: string;
} & HeaderLogo)[];
