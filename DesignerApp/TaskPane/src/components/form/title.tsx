import React from 'react';

export const Title: React.FC<{ text: string }> = ({ text }) => {
    return <p className="is-size-3">{text}</p>;
};
