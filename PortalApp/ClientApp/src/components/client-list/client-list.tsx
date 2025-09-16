import React, { JSX } from 'react';
import { useEffect, useState } from 'react';
import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';
import {
    ClientApi,
    OmniClientInfoDTO,
    OmniClientSearchDTO,
} from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';

interface ClientAutoCompleteProps {
    callback: (x: any) => void;
}

export default function ClientList(
    props: ClientAutoCompleteProps
): JSX.Element {
    const [options, setOptions] = useState<OmniClientInfoDTO[]>([]);

    const loadClients = (
        searchText: string | null = null,
        start = 0,
        count = 100
    ): void => {
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

                if (x.data.Items && x.data.Items.length > 0) {
                    // Pass initially selected option to the parent component.
                    props.callback(x.data.Items[0].Id);
                }
            });
    };

    useEffect(() => {
        loadClients();

        return () => {};
    }, []);

    return (
        <div>
            <select
                className="select input"
                onChange={(x) => props.callback(x.target.value)}>
                {options &&
                    options.length > 0 &&
                    options.map((x, i) => (
                        <option key={i} value={x.Id || ''}>
                            {x.Client?.Name} - {x.Country}
                        </option>
                    ))}
            </select>
        </div>
    );
}
