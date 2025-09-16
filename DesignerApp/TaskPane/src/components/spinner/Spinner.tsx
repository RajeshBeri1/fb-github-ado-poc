import React from 'react';
import { createComponent } from '@lit-labs/react';
import { OmniLoadingIndicatorElement } from 'omni-ui';

import { SpinnerStyles } from './styles';

export const OmniLoadingIndicator = createComponent(
    React,
    'omni-loading-indicator',
    OmniLoadingIndicatorElement
);

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
            <OmniLoadingIndicator />
        </SpinnerStyles>
    );
};

export default Spinner;
