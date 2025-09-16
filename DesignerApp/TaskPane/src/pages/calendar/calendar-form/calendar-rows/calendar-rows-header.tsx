import React from 'react';

interface ICalendarRowsHeaderProps {
    headers: string[];
}

export const CalendarRowsHeader: React.FC<ICalendarRowsHeaderProps> = ({
    headers,
}) => {
    return (
        <thead>
            <tr>
                {headers.map((header) => (
                    <td key={header}>{header}</td>
                ))}
            </tr>
        </thead>
    );
};
