import * as React from 'react';
import { useEffect, useState } from 'react';
import { UserApi } from '@omniflow/omni-webapi';
import './toolbar.scss';
import { Link } from 'react-router';
import { WEBAPI_CONFIGURATION } from '../../config/webapi.config';
import ClientList from '../client-list/client-list';

interface ToolbarProps {
    callback: (newClientId: string) => void;
}

export default function Toolbar(props: ToolbarProps): React.JSX.Element {
    const [userName, setUserName] = useState<string>('...Loading');

    const onClientSelect = (newClientId: string): void => {
        props.callback(newClientId);
    };

    useEffect(() => {
        const userApi = new UserApi(WEBAPI_CONFIGURATION);
        userApi
            .userCurrent()
            .then((u) => {
                if (u.data.DisplayName) {
                    setUserName(u.data.DisplayName);
                } else {
                    setUserName('');
                }
            })
            .catch((error) => {
                console.log(error);
            });

        return () => {};
    }, []);

    return (
        <nav className="navbar navbar-expand-lg bg-light">
            <div className="container-fluid">
                <div className="collapse navbar-collapse" id="navbarNav">
                    <ul className="navbar-nav w-100">
                        <li className="nav-item">
                            <Link className="nav-link" to={'/'}>
                                <span className="fa fa-home"></span>
                            </Link>
                        </li>
                        <li className="nav-item">
                            <a href="#" className="nav-link">
                                Features
                            </a>
                        </li>
                        <li className="nav-item">
                            <a href="#" className="nav-link">
                                Flowchart Template
                            </a>
                        </li>
                        <li className="nav-item">
                            <a href="#" className="nav-link">
                                Data Builder
                            </a>
                        </li>
                        <li className="nav-item">
                            <a href="#" className="nav-link">
                                Flowchart Builder
                            </a>
                        </li>
                        <li className="nav-item">
                            <div className="nav-link">
                                <ClientList callback={onClientSelect} />
                            </div>
                        </li>
                        <li className="ms-auto ">
                            <a href="#" className="nav-link">
                                Administration
                            </a>
                        </li>
                        <li className="my-auto d-flex align-items-baseline">
                            <span className="fa fa-user mx-2"></span>
                            <span>{userName}</span>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    );
}
