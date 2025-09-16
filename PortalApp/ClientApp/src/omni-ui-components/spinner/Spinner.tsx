import React from 'react';
import { createComponent } from '@lit/react';
import { OmniLoadingIndicator } from 'omni-ui/omni-loading-indicator.js';
//import { OmniLoadingIndicatorElement } from 'omni-ui/all';

import { SpinnerStyles } from './styles';

export const LoadingIndicator = createComponent({
    tagName: 'omni-loading-indicator',
    elementClass: OmniLoadingIndicator,
    react: React,
   
})

export enum SpinnerType {
    STANDARD = 'STANDARD',
    SMALL = 'SMALL',
    BUTTON = 'BUTTON',
}

export const spinnerSizes: Record<SpinnerType, number> = {
    [SpinnerType.STANDARD]: 80,
    [SpinnerType.SMALL]: 50,
    [SpinnerType.BUTTON]: 20,
};

export type TSpinnerProps = {
    type?: SpinnerType;
};

const Spinner = ({ type = SpinnerType.STANDARD }: TSpinnerProps) => {
    const size: number = spinnerSizes[type];

    return (
        <SpinnerStyles size={size}>
            <LoadingIndicator />
        </SpinnerStyles>
    );
};

export default Spinner;
