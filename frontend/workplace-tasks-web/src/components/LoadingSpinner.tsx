import React from 'react';

interface LoadingSpinnerProps {
    size?: 'sm' | 'md' | 'lg';
    message?: string;
}

export const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({
    size = 'md',
    message
}) => {
    const getSpinnerClass = () => {
        switch (size) {
            case 'sm': return 'spinner';
            case 'lg': return 'spinner spinner-lg';
            default: return 'spinner';
        }
    };

    if (message) {
        return (
            <div className="page-loading">
                <div className={getSpinnerClass()} />
                <span>{message}</span>
            </div>
        );
    }

    return <div className={getSpinnerClass()} />;
};
