import { DefaultButton } from '@fluentui/react';
import * as React from 'react';

export interface HeaderProps {
    title: string;
    logo: string;
    message: string;
}

export default class Test extends React.Component {
    render() {
        return (
            <section className="ms-welcome__header ms-bgColor-neutralLighter ms-u-fadeIn500">
                <h1 className="ms-fontSize-su ms-fontWeight-light ms-fontColor-neutralPrimary">
                    {}
                </h1>
                <DefaultButton
                    className="ms-welcome__action"
                    iconProps={{ iconName: 'ChevronRight' }}>
                    Run
                </DefaultButton>
            </section>
        );
    }
}
