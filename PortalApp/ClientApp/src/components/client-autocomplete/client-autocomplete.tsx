import * as React from 'react';
import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';
import {
    ClientApi,
    OmniClientInfoDTO,
    OmniClientSearchDTO,
} from '@omniflow/omni-webapi';
import TextField from '@mui/material/TextField';
import Autocomplete from '@mui/material/Autocomplete';
import CircularProgress from '@mui/material/CircularProgress';
import { plainToClass } from 'class-transformer';

export interface IClientAutoCompleteProps {
    handleClientAutoCompleteSelect: (x: any) => void;
}

export const ClientAutoComplete: React.FC<IClientAutoCompleteProps> = ({
    handleClientAutoCompleteSelect: handleClientSelect,
}) => {
    const [open, setOpen] = React.useState(false);
    const [options, setOptions] = React.useState<OmniClientInfoDTO[]>([]);
    const loading = open && options.length === 0;

    const loadClients = (searchText?: string, start = 0, count = 100) => {
        const clientApi = new ClientApi(WEBAPI_CONFIGURATION);
        clientApi
            .clientList(
                plainToClass(OmniClientSearchDTO, {
                    SearchText: searchText,
                    Start: start,
                    Count: count,
                    OrderAscending: true,
                    OrderBy: null,
                })
            )
            .then((x) => {
                setOptions([...(x.data.Items || [])]);
            });
    };
    React.useEffect(() => {
        let active = true;

        if (!loading) {
            return undefined;
        }
        loadClients();

        return () => {
            // eslint-disable-next-line @typescript-eslint/no-unused-vars
            active = false;
        };
    }, [loading]);

    React.useEffect(() => {
        if (!open) {
            setOptions([]);
        }
    }, [open]);

    return (
        <div>
            <Autocomplete
                id="client-autocomplete"
                sx={{ width: 300 }}
                open={open}
                onOpen={() => {
                    setOpen(true);
                }}
                onClose={() => {
                    setOpen(false);
                }}
                onInputChange={(event, newInputValue) => {
                    loadClients(newInputValue);
                    console.log(newInputValue);
                    handleClientSelect(newInputValue);
                }}
                filterOptions={(x) => x}
                isOptionEqualToValue={(option, value) =>
                    option?.Client?.Name === value?.Client?.Name
                }
                getOptionLabel={(option) => option?.Client?.Name || ''}
                options={options}
                loading={loading}
                renderInput={(params) => (
                    <TextField
                        {...params}
                        label="Client Search"
                        InputProps={{
                            ...params.InputProps,
                            endAdornment: (
                                <React.Fragment>
                                    {loading ? (
                                        <CircularProgress
                                            color="inherit"
                                            size={20}
                                        />
                                    ) : null}
                                    {params.InputProps.endAdornment}
                                </React.Fragment>
                            ),
                        }}
                    />
                )}
            />
        </div>
    );
};

export default ClientAutoComplete;

// export interface IClientListProps {
//   handleClientSelect: (x: any) => void
// }

// export interface IClientListState {
//     clients: ClientInfoListDTO
// }
// const ClientList:React.FC<IClientListProps> = ({handleClientSelect: handleClientSelect}) => {

//   return (
//     <ClientAutoComplete handleClientAutoCompleteSelect={(x) => handleClientSelect(x)} />
//   )

// }
// export default ClientList;
