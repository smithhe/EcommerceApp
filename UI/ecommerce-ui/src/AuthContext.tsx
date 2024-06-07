import React, {createContext, ReactNode, useContext, useEffect, useState} from 'react';
import {jwtDecode} from "jwt-decode";

interface AuthContextType {
    checkAuth: () => boolean;
    claims: TokenClaims | undefined;
    isAuthenticated: boolean;
}

interface TokenClaims {
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": string;
    FirstName: string;
    LastName: string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [claims, setClaims] = useState<TokenClaims>();
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

    const checkAuth = () => {
        const token = localStorage.getItem('authToken');

        if (token != null) {
            setIsAuthenticated(true);
        }
        else {
            setIsAuthenticated(false);
        }

        const decodedClaims = decodeToken(token);

        if (decodedClaims) {
            setClaims(decodedClaims);
        }

        if (token != null)
        {
            return true;
        }
        else {
            return false;
        }
    };

    const decodeToken = (token: string | null): TokenClaims | null => {
        if (token == null) return null;

        try {
            return jwtDecode(token) as TokenClaims;
        } catch (error) {
            console.error("Invalid token:", error);
            return null;
        }
    };

    useEffect(() => {
        checkAuth();
    }, []);

    return (
        <AuthContext.Provider value={{ checkAuth, claims, isAuthenticated }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = (): AuthContextType => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};