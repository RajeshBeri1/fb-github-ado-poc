import React, { Component, ErrorInfo, ReactNode } from 'react';

type State = {
    hasError: Error | null;
};

type Props = {
    children?: ReactNode;
};

export class ErrorBoundary extends Component<Props, State> {
    public state: State = {
        hasError: null,
    };

    private promiseRejectionHandler = (event: PromiseRejectionEvent): void => {
        this.setState({
            hasError: event.reason,
        });
    };

    private eventErrorHandler = (event: ErrorEvent): any => {
        this.setState({
            hasError: event.error,
        });
    };

    public static getDerivedStateFromError(error: Error): State {
        return { hasError: error };
    }

    componentDidMount(): void {
        window.addEventListener(
            'unhandledrejection',
            this.promiseRejectionHandler
        );
        window.addEventListener('error', this.eventErrorHandler);
    }

    componentWillUnmount(): void {
        window.removeEventListener(
            'unhandledrejection',
            this.promiseRejectionHandler
        );
        window.removeEventListener('error', this.eventErrorHandler);
    }

    componentDidCatch(error: Error, errorInfo: ErrorInfo): void {
        console.error('Uncaught error:', error, errorInfo);
    }

    render(): ReactNode {
        if (this.state.hasError) {
            return <h1>Sorry... there was an error!</h1>;
        }

        return this.props.children;
    }
}
