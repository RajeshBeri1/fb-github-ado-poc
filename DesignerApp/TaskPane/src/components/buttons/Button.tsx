import React, {JSX, MouseEvent, PropsWithChildren} from 'react';
import classNames from 'clsx';

import Link from '../Link';
import Spinner, {SpinnerType} from '../spinner/Spinner';
import {Tooltip} from "../../omni/tooltip";

export type TButtonProps = Partial<
    PropsWithChildren<{
        to?: string;
        tooltip?: string;
        vars?: { [key: string]: string };
        className?: string;
        disabled?: boolean;
        loading?: boolean;
        onlyLoading?: boolean;
        type?: 'submit' | 'reset' | 'button';
        isDisabled?: boolean;
        onClick?: (event: MouseEvent<HTMLButtonElement | HTMLAnchorElement>) => void;
    }>
>;

const Button = ({
                    to,
                    tooltip,
                    vars,
                    className,
                    loading = false,
                    onlyLoading = false,
                    children,
                    type = 'button',
                    isDisabled = false,
                    ...restProps
                }:
                    TButtonProps
    ):
        JSX.Element => {
        const finalClassName = classNames(' flex gap-1', className);

        return (
            <>
                {to ? tooltip ? (
                    <Tooltip>
                            <Link
                                to={to}
                                slot="invoker"
                                vars={vars}
                                className={finalClassName}
                                {...restProps}>
                                {(!loading || !onlyLoading) && children}
                                {loading && <Spinner type={SpinnerType.BUTTON}
                                />}
                            </Link>
                            <div slot="content">{tooltip}</div>
                        </Tooltip>
                    ) : (
                        <Link
                            to={to}
                            vars={vars}
                            className={finalClassName}
                            {...restProps}>
                            {(!loading || !onlyLoading) && children}
                            {loading && <Spinner type={SpinnerType.BUTTON}/>}
                        </Link>
                    )
                    : tooltip ? (
                        <Tooltip>
                            <button slot="invoker" type={type} className={className}
                                    disabled={isDisabled} {...restProps}>
                                {(!loading || !onlyLoading) && children}
                                {loading && <Spinner type={SpinnerType.BUTTON}/>}
                            </button>
                            <div slot="content">{tooltip}</div>
                        </Tooltip>
                    ) : (
                            <button type={type} className={className} {...restProps}>
                            {(!loading || !onlyLoading) && children}
                            {loading && <Spinner type={SpinnerType.BUTTON}/>}
                        </button>
                    )}
            </>
        );
    }
;

export default Button;
