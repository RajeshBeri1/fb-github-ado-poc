import { Border, BorderLineStyle } from '@omniflow/omni-webapi';
import React, { KeyboardEvent, useRef } from 'react';
import { useForm } from 'react-hook-form';
import { ColorConfig } from './color-config';
import { OmniDropDownInput } from '../../omni/dropdown';
import { Option, SelectableItem, OmniDropdown } from 'omni-ui'
interface IInputProps {
    border: Border,
    formKey?: string;
    showInherited?: boolean;
    onChange?: (attr, e) => void;
    onKeyUp?: (event: KeyboardEvent<HTMLInputElement>) => void;
}

export const BorderConfig: React.FC<IInputProps> = React.memo(({
    border,
    formKey,
    showInherited = true,
    onChange,
}) => {
    const {
        register,
        formState: { isDirty }, setValue, getValues } =
        useForm({
            mode: 'onChange',
            reValidateMode: 'onChange',
            defaultValues: border,
        });

    const leftBorder = useRef<OmniDropdown>(null);
    const rightBorder = useRef<OmniDropdown>(null);
    const topBorder = useRef<OmniDropdown>(null);
    const bottomBorder = useRef<OmniDropdown>(null);

    const updateAllBorderLineStyles = (value: any) => {
        onChange(`${formKey}`, {
            Top: {
                Style: value,
                Color: getValues("Top.Color")
            },
            Bottom: {
                Style: value,
                Color: getValues("Bottom.Color")
            },
            Right: {
                Style: value,
                Color: getValues("Right.Color")
            },
            Left: {
                Style: value,
                Color: getValues("Left.Color")
            }

        })

        setValue(`Top.Style`, value, { shouldDirty: true });
        setValue(`Right.Style`, value, { shouldDirty: true });
        setValue(`Bottom.Style`, value, { shouldDirty: true });
        setValue(`Left.Style`, value, { shouldDirty: true });
    }

    const updateAllBoarderColors = (value: any) => {
        setValue(`Top.Color`, value, { shouldDirty: true });
        setValue(`Right.Color`, value, { shouldDirty: true });
        setValue(`Bottom.Color`, value, { shouldDirty: true });
        setValue(`Left.Color`, value, { shouldDirty: true });
    }

    const handleBorderStyleChange = (e) => {
        updateAllBorderLineStyles(e.detail.value);
    }

    const optionBorderStyle = () => {
        return Object.keys(BorderLineStyle).map((option) => (
            { id: option, value: option }
        ));
    }

    const optionsStyle = (): Option[] => {
        const inheritedOption: Option[] = showInherited ? [{ id: '', value: 'Inherited from theme' }] : [];
        return [...inheritedOption, ...Object.keys(BorderLineStyle).map((option) => (
            { id: option, value: option }
        ))];
    };

    const handleTopStyleChange = (e) => {
        onChange(`${formKey}.Top.Style`, e.detail.value);
        setValue(`Top.Style`, e.detail.value);
    };

    const selectedTopValue = !getValues("Top.Style") && showInherited ? [optionsStyle().find((b: SelectableItem) => b.value == 'Inherited from theme')] : [optionsStyle().find((b: SelectableItem) => b.value == getValues("Top.Style"))];

    const handleRightStyleChange = (e) => {
        onChange(`${formKey}.Right.Style`, e.detail.value);
        setValue(`Right.Style`, e.detail.value);
    };
    const selectedRightValue = !getValues("Right.Style") && showInherited ? [optionsStyle().find((b: SelectableItem) => b.value == 'Inherited from theme')] : [optionsStyle().find((b: SelectableItem) => b.value == getValues("Right.Style"))];

    const handleBottomStyleChange = (e) => {
        onChange(`${formKey}.Bottom.Style`, e.detail.value);
        setValue(`Bottom.Style`, e.detail.value);

    };
    const selectedBottomValue = !getValues("Bottom.Style") && showInherited ? [optionsStyle().find((b: SelectableItem) => b.value == 'Inherited from theme')] : [optionsStyle().find((b: SelectableItem) => b.value == getValues("Bottom.Style"))];

    const handleLeftStyleChange = (e) => {
        onChange(`${formKey}.Left.Style`, e.detail.value);
        setValue(`Left.Style`, e.detail.value);
    };
    const selectedLeftValue = !getValues("Left.Style") && showInherited ? [optionsStyle().find((b: SelectableItem) => b.value == 'Inherited from theme')] : [optionsStyle().find((b: SelectableItem) => b.value == getValues("Left.Style"))];

    return (
        <>
            <div className='w-100 d-flex'>
                <label className='w-100 d-flex flex-wrap '>
                    <OmniDropDownInput placeholder='Select Border Style' className="w-100"
                        options={optionBorderStyle()} onValueChange={handleBorderStyleChange} >
                    </OmniDropDownInput>
                </label>
                {/* <small>All Colors</small>
                    <ColorConfig color={border.Top.Color} onChange={(emptyKey, e) => updateAllBoarderColors(e)} /> */}

            </div>


            <div className='w-100 d-flex'>

                <label className='w-100 d-flex flex-wrap '>
                    <OmniDropDownInput label="Top" ref={topBorder} className="w-100"
                        options={optionsStyle()} value={selectedTopValue} onValueChange={handleTopStyleChange} >
                    </OmniDropDownInput>
                </label>
                <label className='w-100'>
                    <small className='text-xs'>Top Color</small>
                    <ColorConfig color={border.Top.Color} formKey={`${formKey}.Top.Color`} onChange={(key, color) => onChange(`${key}`, color)} />
                </label>
            </div>
            <div className='w-100 d-flex'>
                <label className='w-100 d-flex flex-wrap '>
                    <OmniDropDownInput label="Right" ref={rightBorder} className="w-100"
                        options={optionsStyle()} value={selectedRightValue} onValueChange={handleRightStyleChange} >
                    </OmniDropDownInput>
                </label>
                <label className='w-100'>
                    <small className='text-xs text-opactity'>Right Color</small>
                    <ColorConfig color={border.Right.Color} formKey={`${formKey}.Right.Color`} onChange={(key, color) => onChange(`${key}`, color)} />
                </label>
            </div>


            <div className='w-100 d-flex'>
                <label className='w-100 d-flex flex-wrap '>
                    <OmniDropDownInput label="Bottom" ref={bottomBorder} className="w-100"
                        options={optionsStyle()} value={selectedBottomValue} onValueChange={handleBottomStyleChange} >
                    </OmniDropDownInput>
                </label>
                <label className='w-100'>
                    <small className='text-xs text-opactity'>Bottom Color</small>
                    <ColorConfig color={border.Bottom.Color} formKey={`${formKey}.Bottom.Color`} onChange={(key, color) => onChange(`${key}`, color)} />
                </label>
            </div>
            <div className='w-100 d-flex'>
                <label className='w-100 d-flex flex-wrap '>
                    <OmniDropDownInput label="Left" ref={leftBorder} className="w-100"
                        options={optionsStyle()} value={selectedLeftValue} onValueChange={handleLeftStyleChange} >
                    </OmniDropDownInput>
                </label>
                <label className='w-100'>
                    <small className='text-xs text-opactity'>Left Color</small>
                    <ColorConfig color={border.Left.Color} formKey={`${formKey}.Left.Color`} onChange={(key, color) => onChange(`${key}`, color)} />
                </label>
            </div>

        </>
    );
});
