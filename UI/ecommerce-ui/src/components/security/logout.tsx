import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from "../../AuthContext.tsx";
//import authService from "../../AuthService.ts";

const Logout = () => {
    const { claims, logout } = useAuth();
    const navigate = useNavigate();

    useEffect( () => {
        const logoutCall = async () => {
            await logout(claims?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '');
        }

        logoutCall().then(() => {
            navigate('/');
        });
    });

    return (
        <div>
            <p>Logging out...</p>
        </div>
    );
};

export default Logout;
