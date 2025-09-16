import React from 'react';
import { RegisterOptions, UseFormRegisterReturn } from 'react-hook-form';

import { Input } from '../../../../components/form/input';
import { Label } from '../../../../components/form/label';
import { Title } from '../../../../components/form/title';

interface IFooterGeneralSettings {
    register: (
        name: string,
        options?: RegisterOptions
    ) => UseFormRegisterReturn;
    getValues: any;
    setValue: any;
}

export const FooterGeneralSettings: React.FC<IFooterGeneralSettings> = ({
    register,
    getValues,
    setValue,
}) => {
    return (
        <>
            <div className="d-flex mb-3">
                <Title text="General footer settings" />
            </div>
            <div className="d-flex align-items-center justify-content-center">
                <div className="w-50">
                    <input
                        type="checkbox"
                        {...register('Definition.Configuration.EnableLegend')}
                        className="px-3"
                    />
                    <Label label="Enable Legend" />
                </div>
                <Input
                    disabled={
                        !getValues('Definition.Configuration.EnableLegend')
                    }
                    type="number"
                    register={register}
                    registerKey="Definition.Configuration.LegendColumnCount"
                    label="Legend Columns"
                    labelStyle="w-50"
                    min="1"
                    max="5"
                />
            </div>
        </>
    );
};
