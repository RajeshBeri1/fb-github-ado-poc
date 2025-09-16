import styled from '@emotion/styled';

export const SpinnerStyles = styled.div<{ size: number }>`
    display: flex;
    justify-content: center;
    ${({ size }: { size: number }): string => `
        omni-loading-indicator {
            width: ${size}px;
        }
    `};
`;
