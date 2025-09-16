import { red } from '@mui/material/colors';
import { borderBottom } from '@mui/system';
import React from 'react';

type Props = {};
const styles = {
    borderBottom: '1px solid black',
    margin: '1rem 0',
};

export default function Divider({}: Props) {
    return <div className="divider vertical" style={{ ...styles }}></div>;
}
