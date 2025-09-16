import * as React from 'react';

export interface IFooterConfiguratorProps {
    handleChange?: (x: any) => void;
}

export interface IFooterConfiguratorState {}

export const FooterConfigurator: React.FC<IFooterConfiguratorProps> = ({ handleChange: handleChange }) => {
    const change = (x: any) => {
        handleChange(x.target.value);
    };
    return (
        <div>
            <h1>Footer Configurator</h1>
        </div>
    );
};
