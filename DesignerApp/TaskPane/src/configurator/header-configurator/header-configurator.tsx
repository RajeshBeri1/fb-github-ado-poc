import * as React from 'react';
import { Input } from '../../components/form/input';
import { Title } from '../../components/form/title';

export interface IHeaderConfiguratorProps {
    handleChange?: (x: any) => void;
}

export interface IHeaderConfiguratorState {}

export const HeaderConfigurator: React.FC<IHeaderConfiguratorProps> = ({
    handleChange: handleChange,
}) => {
    const change = (input: string) => {
        handleChange(input);
    };

    return (
        <>
            <div className="d-flex">
                <Input
                    label="Row Margin"
                    labelStyle="w-50"
                    onChange={change}
                    type="number"
                />
                <Input
                    label="Column Margin"
                    labelStyle="w-50"
                    onChange={change}
                    type="number"
                />
            </div>
            <div className="d-flex">
                <Input
                    label="Total Rows"
                    labelStyle="w-50"
                    onChange={change}
                    type="number"
                />
                <Input
                    label="Margin Rows"
                    labelStyle="w-50"
                    onChange={change}
                    type="number"
                />
            </div>
            <Title text="Header rows" />
            <div className="d-flex">
                <label className="w-25">
                    <small>Header Row Type</small>
                    <select className="select input">
                        <option>Date</option>
                        <option>Value</option>
                    </select>
                </label>
                <label className="w-25">
                    <small>Include Summery Component</small>
                    <select className="select input">
                        <option>PepsiCo NA BEV Media Plan</option>
                        <option>PepsiCo FL Media Plan</option>
                        <option>JC Penny Social Brand</option>
                    </select>
                </label>
                <Input
                    labelStyle="w-50"
                    label="Header Row Text"
                    onChange={change}
                />
            </div>
        </>
    );
};
