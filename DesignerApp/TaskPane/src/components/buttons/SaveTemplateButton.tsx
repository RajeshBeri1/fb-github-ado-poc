import React, {JSX, MouseEvent, PropsWithChildren } from 'react';
import classNames from 'clsx';

import Link from '../Link';
import Spinner, { SpinnerType } from '../spinner/Spinner';
import '../../pages/common-styles.css';

export type TButtonProps = Partial<
    PropsWithChildren<{
        to?: string;
        vars?: { [key: string]: string };
        className?: string;
        disabled?: boolean;
        loading?: boolean;
        onlyLoading?: boolean;
        type?: 'submit' | 'reset' | 'button';
        onClick?: (event: MouseEvent<HTMLButtonElement | HTMLAnchorElement>) => void;
    }>
>;

const Button = ({
    to,
    vars,
    className,
    loading = false,
    onlyLoading = false,
    children,
    type = 'button',
    ...restProps
}: TButtonProps): JSX.Element => {
    const finalClassName = classNames(' flex gap-1', className);

    return (
        <>
            {to ? (
                <Link
                    to={to}
                    vars={vars}
                    className={finalClassName}
                    {...restProps}>
                    {(!loading || !onlyLoading) && children}
                    {loading && <Spinner type={SpinnerType.BUTTON} />}
                </Link>
            ) : (
                <button type={type} className={finalClassName} {...restProps}>
                    {(!loading || !onlyLoading) && children}
                    {loading && <Spinner type={SpinnerType.BUTTON} />}
                </button>
            )}
        </>
    );
};

export default Button;
