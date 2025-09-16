import React from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, useLocation } from 'react-router';

import { useAuth } from '../../components/auth-guard/auth-guard';
import ModalWrapper from '../../components/modal/modal';

export interface ILoginProps {
    handleChange?: (x: any) => void;
}

const Login = () => {
    React.useEffect(() => {
        const subscription = watch((value, { name, type }) => {
            // console.log(value, name, type);
        });
        return () => subscription.unsubscribe();
    }, []);

    const {
        handleSubmit,
        watch,
        register,
        formState: { errors },
    } = useForm({
        mode: 'onChange',
        reValidateMode: 'onChange',
        defaultValues: {
            UserName: 'Wendy.Lator',
            Password: 'Trans4mation',
        },
    });

    let navigate = useNavigate();
    let location = useLocation();
    let auth = useAuth();

    let { from } = location.state || { from: { pathname: '/' } };
    let login = (data) => {
        auth.signin(data, () => {
            navigate(from);
        });
    };
    return (
        <ModalWrapper
            header={<div>Login</div>}
            controls={false}
            body={
                <form
                    className="row container"
                    onSubmit={handleSubmit((data) => login(data))}>
                    <div className="d-flex flex-column col-12">
                        <label className="text-truncate">Username</label>
                        <input
                            className=""
                            type="text"
                            {...register('UserName')}></input>
                    </div>
                    <div className="d-flex flex-column col-12">
                        <label className="text-truncate">Password</label>
                        <input
                            type="password"
                            {...register('Password')}></input>
                    </div>
                    <div className="d-flex flex-column col-12 mt-4">
                        <button type="submit">
                            Login
                        </button>
                    </div>
                </form>
            }
        />
    );
};

export default Login;
