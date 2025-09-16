import { Font, FontWeight } from '@omniflow/omni-webapi';
import { RangeUnderlineStyle } from '@omniflow/omni-webapi/dist/models/range-underline-style';
import React, { KeyboardEvent, useState } from 'react';
import { useForm, useWatch } from 'react-hook-form';
import { FontFamilyType } from '../../enums/font-family.enum';
import { ColorConfig } from './color-config';
import { OmniDropDownInput } from '../../omni/dropdown';
import { Option } from 'omni-ui';
interface IInputProps
{
    font: Font,
    formKey?: string ;
    showInherited?: boolean;
    onChange?: (attr, e) => void;

}

export const FontConfig: React.FC<IInputProps> = React.memo(({
    font,
    formKey,
    showInherited = true,
    onChange,
}) =>
{
    const [fontConfig, setFont] = useState(font);
    
  /*  const [fontConfig, setFont] = useState(() => {
        if (font?.Background?.Blue === -1 && font?.Background?.Red === -1 && font?.Background?.Green === -1) {
            return {
                ...font, Background: {
                    Red: 255, Green: 255, Blue: 255
                }
}
        }
        return font;
    });   */
    
    const {
        register,
        control,
        getValues,
        setValue,
        watch,
        handleSubmit,
        formState: { isDirty } } =
        useForm({
            mode: 'onChange',
            reValidateMode: 'onChange',
            defaultValues: fontConfig,
        });
    const size = new Array(30).fill(0).map((y, i) => i + 4);

    const handleFamilyChange = (e) => {
        let value = e.detail.value;
        if (value === 'Inherited from theme' ) {
            value = null;
        }
        onChange(`${formKey}.Family`, value);
        setValue(`Family`, value);
    };
    const optionFamily = (): Option[] => {
        const inheritedOption: Option[] = showInherited ? [{ id: '', value: 'Inherited from theme' }] : [];
        return [...inheritedOption, ...Object.keys(FontFamilyType).map((t) => (
            {
                id: t, value: t, style: { fontFamily: t }
                
            }
           
        )
        )];
    }
    
    const handleWeightChange = (e) => {
        let value = e.detail.value;
        if (value === 'Inherited from theme') {
            value = null;
        }
        onChange(`${formKey}.Weight`, value);
        setValue(`Weight`, value);
    };
    const optionWeight = (): Option[] => {
        const inheritedOption: Option[] = showInherited ? [{ id: '', value: 'Inherited from theme' }] : [];
        return [...inheritedOption, ...Object.keys(FontWeight).map((t) => (
            {
                id: t, value: t, style: { fontWeight: FontWeight.BoldItalic === t || FontWeight.Bold === t ? 'bold' : 'normal', fontStyle: FontWeight.BoldItalic === t || FontWeight.Italic === t ? 'italic' : 'normal' }
            }))];
    }
 

    const handleSizeChange = (e) => {
        onChange(`${formKey}.Size`, e.detail.value ? parseInt(e.detail.value) : e.detail.value);
        setValue(`Size`, e.detail.value ? parseInt(e.detail.value) : e.detail.value);
    };
    const optionSize = (): Option[] => {
        const inheritedOption: Option[] = showInherited ? [{ id: '', value: 'Inherited from theme' }] : [];
        return [...inheritedOption, ...size.map((value) => (
            { id: value.toString(), value: value.toString() }
        ))];
    }

    const handleUnderlineChange = (e) => {
        let value = e.detail.value;
        if (value === 'Inherited from theme') {
            value = null;
        }
        onChange(`${formKey}.Underline`, value);
        setValue(`Underline`, value);
    };
    const optionUnderline = (): Option[] => {
        const inheritedOption: Option[] = showInherited ? [{ id: '', value: 'Inherited from theme' }] : [];
        return [...inheritedOption, ...Object.keys(RangeUnderlineStyle).map((t) => (
            { id: t, value: t }
        ))];
    }
  
   
    return (
        <>
            <div className="d-flex w-100">
                <label className="w-50">
                    <OmniDropDownInput label="Font family" className="w-100"
                        options={optionFamily()} value={[getValues("Family") || 'Inherited from theme']} onValueChange={handleFamilyChange} >
                    </OmniDropDownInput>
                </label>
                <label className="w-50">
                    <OmniDropDownInput label="Font weight" className="w-100" placeholder='Inherited from theme'
                        options={optionWeight()} value={[getValues("Weight") || 'Inherited from theme']} onValueChange={handleWeightChange} >
                    </OmniDropDownInput>
                </label>
            </div>
            <div className="d-flex w-100">
                <label className="w-50">
                    <OmniDropDownInput label="Font size" className="w-100" placeholder='Inherited from theme'
                        options={optionSize()} value={[getValues("Size") || 'Inherited from theme']} onValueChange={handleSizeChange} >
                    </OmniDropDownInput>
                </label>
                <label className="w-50">
                    <OmniDropDownInput label="Font underline" className="w-100" placeholder='Inherited from theme'
                        options={optionUnderline()} value={[getValues("Underline") || 'Inherited from theme']} onValueChange={handleUnderlineChange} >
                    </OmniDropDownInput>
                </label>
            </div>
            <div className="d-flex w-100">
                <label className="w-50">
                    <div>
                        <small className='text-xs text-opactity'>Font color (primary)</small>
                    </div>
                    <ColorConfig color={fontConfig.Color} showRgbLabel={true} formKey={formKey} onChange={(key, color) => onChange(`${formKey}.Color`, color)} />
                </label>
                <label className="w-50">
                    <div>
                        <small className='text-xs text-opactity'>Background Color (primary)</small>
                    </div>
                    <ColorConfig color={fontConfig.Background} showRgbLabel={true} formKey={formKey} onChange={(key, color) => onChange(`${formKey}.Background`, color)} />
                </label>
            </div>
            {/*
            <div className="d-flex w-100">
                <label className="w-50">
                    <div>
                        <small>Font color (alternate)</small>
                    </div>
                    <ColorConfig color={fontConfig.ColorAlternate} showRgbLabel={true} formKey={formKey} onChange={(key, color) => onChange(`${formKey}.ColorAlternate`, color)} />
                </label>
                <label className="w-50">
                    <div>
                        <small>Background Color (alternate)</small>
                    </div>
                    <ColorConfig color={fontConfig.BackgroundAlternate} showRgbLabel={true} formKey={formKey} onChange={(key, color) => onChange(`${formKey}.BackgroundAlternate`, color)} />
                </label>
            </div>
            */}
        </>
    );
});


