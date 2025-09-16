import React, { JSX } from 'react';

interface IModalProps {
    rowCount: number;
}

export default function Skeleton(props: IModalProps): JSX.Element {
    const style = {
        width: 'auto',
        height: '3rem',
        marginBottom: '.25rem',
    } as React.CSSProperties;
    return (
        <div className="box is-blue">
            {Array(props.rowCount)
                .fill(true)
                .map((_, i) => (
                    <div className="table skeleton" key={i} style={style}></div>
                ))}
        </div>
    );
}
