import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from "../../AuthContext.tsx";
import authService from "../../AuthService.ts";

const Logout: React.FC = () => {
    const { claims } = useAuth();
    const navigate = useNavigate();

    useEffect( () => {
        const logoutCall = async () => {
            await authService.logout(claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '');
        }

        logoutCall();
        navigate('/');
    }, []);

    return (
        <div>
            <p>Logging out...</p>
        </div>
    );
};

export default Logout;
