import * as React from 'react';

export interface IHierarchyConfiguratorProps {
    handleChange?: (x: any) => void;
}

export interface IHierarchyConfiguratorState {}
export const HierarchyConfigurator: React.FC<IHierarchyConfiguratorProps> = ({ handleChange: handleChange }) => {
    const change = (x: any) => {
        handleChange(x.target.value);
    };
    return (
        <div>
            <h1>Hierarchy Configurator</h1>
        </div>
    );
};
