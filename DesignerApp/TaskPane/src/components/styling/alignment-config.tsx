import { Alignment, HorizontalAlignment, VerticalAlignment } from '@omniflow/omni-webapi';
import React, { KeyboardEvent, useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import './alignment-config.css';
import NonRequiredTextAlignment from '../../enums/textAlignment.json'
import { Exclude } from 'class-transformer';
import { includes } from 'lodash';
import { OmniDropDownInput } from '../../omni/dropdown';
import { Option, SelectableItem } from 'omni-ui'
interface IInputProps
{
    alignment: Alignment,
    formKey?: string;
    showInherited?: boolean;
    onChange?: (attr, e) => void;
    onKeyUp?: (event: KeyboardEvent<HTMLInputElement>) => void;
}

export const AlignmentConfig: React.FC<IInputProps> = React.memo(({
    alignment,
    formKey,
    showInherited = true,
    onChange,
}) => {

    const {
        register,
        formState: { isDirty }, getValues, setValue } =
        useForm({
            mode: 'onChange',
            reValidateMode: 'onChange',
            defaultValues: alignment,
        });

    const handleHorizontalChange = (e) => {
        onChange(`${formKey}.Horizontal`, e.detail.value);
        setValue(`Horizontal`, e.detail.value);
    };

    const optionHorizontal = (): Option[] => {
        const inheritedOption: Option[] = showInherited ? [{ id: '', value: 'Inherited from theme' }] : [];
        return [...inheritedOption, ...Object.keys(HorizontalAlignment).map((option) => (
            { id: option, value: option }
        )).filter(val => !NonRequiredTextAlignment.includes(val.id.toString()))];
    };
    const selectedHorizontalValue = !getValues("Horizontal") && showInherited ? [optionHorizontal().find((al: SelectableItem) => al.value == 'Inherited from theme')] : [optionHorizontal().find((al: SelectableItem) => al.value ==getValues("Horizontal"))]; 

    const handleVerticalChange = (e) => { 
        onChange(`${formKey}.Vertical`, e.detail.value);
        setValue(`Vertical`, e.detail.value);
    };
    const optionVertical = () => {
        const inheritedOption: Option[] = showInherited ? [{ id: '', value: 'Inherited from theme' }] : [];
        return [...inheritedOption, ...Object.keys(VerticalAlignment).map((option) => (
            { id: option, value: option }
        )).filter(x => x.id !== 'Justify').filter(y => y.id !== 'Distributed')];
    };
    const selectedVerticalValue = !getValues("Vertical") && showInherited ? [optionVertical().find((al: SelectableItem) => al.value == 'Inherited from theme')] : [optionVertical().find((al: SelectableItem) => al.value == getValues("Vertical"))];


    return (
        <>
            <div className="d-flex w-100">
                <label className="w-50">
                <OmniDropDownInput label="Horizontal" className="w-100" placeholder='Inherited from theme'
                    options={optionHorizontal()} value={selectedHorizontalValue} onValueChange={handleHorizontalChange} >
                    </OmniDropDownInput>
                </label>
                <label className="w-50">
                <OmniDropDownInput label="Vertical" className="w-100" placeholder='Inherited from theme'
                    options={optionVertical()} value={selectedVerticalValue} onValueChange={handleVerticalChange} >
                    </OmniDropDownInput>
                </label>
                </div>
        </>
    );
});
