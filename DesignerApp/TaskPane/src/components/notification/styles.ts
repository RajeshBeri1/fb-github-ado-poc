import styled from '@emotion/styled';

export const SnackbarStyles = styled.div`
    .notistack-SnackbarContainer {
        width: 75%;

        > div {
            width: 100%;
        }

        .notification {
            padding: 20px 20px 20px 60px;

            > omni-icon {
                top: 18px;
            }
            &.is-warning {
              padding: 20px 20px 20px 55px;
            }
            >omni-icon{
                width: 18px;
                height: 18px;
                top:23px;
            }
        }
    }
   
`;
