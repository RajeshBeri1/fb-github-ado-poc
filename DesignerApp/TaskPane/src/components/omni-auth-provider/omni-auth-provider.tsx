import React, { useEffect } from 'react';
import { OMNI_AUTH_CONFIG } from '../../config/webapi.config';
import { useAuth } from './use-auth';

const OmniAuthProvider = () => {

    const auth = useAuth();

    useEffect(() => {

        if (!auth.isAuthenticated) {
            window.location.href = OMNI_AUTH_CONFIG.issuer;
        }

    }, []);

    return null;
};

export default OmniAuthProvider;