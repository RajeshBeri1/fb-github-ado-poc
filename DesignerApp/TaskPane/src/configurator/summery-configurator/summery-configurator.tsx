import * as React from 'react';

export interface ISummeryConfiguratorProps {
    handleChange?: (x: any) => void;
}

export interface ISummeryConfiguratorState {}
export const SummeryConfigurator: React.FC<ISummeryConfiguratorProps> = ({ handleChange: handleChange }) => {
    const change = (x: any) => {
        handleChange(x.target.value);
    };
    return (
        <div>
            <h1>Summery Configurator</h1>
        </div>
    );
};
