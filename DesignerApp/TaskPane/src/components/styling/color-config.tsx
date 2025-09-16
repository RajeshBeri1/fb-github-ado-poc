import React, { useEffect, useState } from 'react'
import
    {
        Color
    } from '@omniflow/omni-webapi';
import { useForm } from 'react-hook-form';
import './color-config.css';
import Tools from '../../business/tools';
import Button from '../buttons/Button';
import { Icon } from '../../omni/icon';
import '../../pages/common-styles.css'
import { debounce } from 'lodash';

export interface IColorConfigProps
{
    color: Color,
    formKey?: string,
    showRgbLabel?: boolean,
    onChange: (key: string, rgba: Color) => void,
   // defaultColor?: Color
}

export const ColorConfig: React.FC<IColorConfigProps> = ({ color, formKey, showRgbLabel = true, onChange }) => {

    //const [rgbColor, setRegColor] = useState(color || defaultColor);
    const [rgbColor, setRegColor] = useState(color);
    const [hexColor, setHexColor] = useState(null);
    const {
        register,
        setValue,
        formState: { isDirty } } =
        useForm({
            mode: 'onChange',
            reValidateMode: 'onChange',
            defaultValues: {
                Color: '#000'
            },
        });

    useEffect(() => {
        const hexColor = Tools.rgbToHex(rgbColor?.Red, rgbColor?.Green, rgbColor?.Blue);
        setValue('Color', hexColor);
        setHexColor(hexColor);
    }, [rgbColor]);

    const debounceOnChangeColor = debounce((value: string) => {
        const rgb = Tools.hexToRgb(value);
        const color = new Color()
        color.Alpha = 1;
        color.Red = rgb[0];
        color.Green = rgb[1];
        color.Blue = rgb[2];
        setRegColor(color);
        onChange(formKey, color);
    }, 300);

    const resetColor = () => {
        const color = new Color();
        color.Alpha = -1;
        color.Red =  -1;
        color.Green =  -1;
        color.Blue =  -1;
        setRegColor(color);
        onChange(formKey, color);
    }

    const isDefault = () => {
        return rgbColor?.Red === -1 && rgbColor?.Green === -1 && rgbColor?.Blue === -1;
    };

    return (
        <div className='d-flex align-items-center'>
            {formKey ? (
                <input
                className="input input-type-color mr-0 custom-input-prop"
                {...register('Color')}
                    type="color"
                onChange={(e) => debounceOnChangeColor(e.target.value)}
            />
            ): (
                <input 
                        className="input input-type-color mr-0 custom-input-prop"
                        type="color"
                onChange={(e) => debounceOnChangeColor(e.target.value)}
            />
            )}
            
            
            {showRgbLabel ? (
                <div className='w-100 rgb-box'>RGB({rgbColor?.Red} , {rgbColor?.Green}, {rgbColor?.Blue})</div>

            ) : <></>}

            {!isDefault() && <Button
                className="icon"
                tooltip="delete"
                onClick={() => resetColor()}>
                <Icon icon-id="omni:interactive:remove"></Icon>
            </Button>}
           
        </div>

    )
}