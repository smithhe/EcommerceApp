import React, {createContext, ReactNode, useContext, useEffect, useState} from 'react';
import {jwtDecode} from "jwt-decode";
import authService from "./AuthService.ts";

interface AuthContextType {
    claims: TokenClaims | undefined;
    isAuthenticated: boolean;
    login: (userName: string, password: string) => Promise<any>;
    logout: (userName: string) => void;
}

interface TokenClaims {
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": string;
    FirstName: string;
    LastName: string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
    nbf: number;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [claims, setClaims] = useState<TokenClaims>();
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

    useEffect(() => {
        const token = localStorage.getItem('authToken');

        const date = new Date();
        const now_utc = Date.UTC(date.getUTCFullYear(), date.getUTCMonth(),
            date.getUTCDate(), date.getUTCHours(),
            date.getUTCMinutes(), date.getUTCSeconds());

        const currentTime = new Date(now_utc);

        const decodedClaims = decodeToken(token);

        if (decodedClaims == null) {
            setIsAuthenticated(false);
            return;
        }

        const expDate = new Date((decodedClaims.nbf * 1000));


        if (token != null && currentTime > expDate) {
            setIsAuthenticated(true);
        }
        else {
            setIsAuthenticated(false);
        }

        if (decodedClaims) {
            setClaims(decodedClaims);
        }
    }, []);

    const decodeToken = (token: string | null): TokenClaims | null => {
        if (token == null) return null;

        try {
            return jwtDecode(token) as TokenClaims;
        } catch (error) {
            console.error("Invalid token:", error);
            return null;
        }
    };

    const login = async (userName: string, password: string): Promise<any> => {
        const response = await authService.login(userName, password);

        if (response.success) {
            localStorage.setItem('authToken', response.token!);
            setIsAuthenticated(true);

            const decodedClaims = decodeToken(response.token!);

            if (decodedClaims) {
                setClaims(decodedClaims);
            }

            return {
                success: true
            };
        }

        return {
            success: false,
            message: response.message
        }
    }

    const logout = async (userName: string) => {
        await authService.logout(userName);

        localStorage.removeItem('authToken');
        setIsAuthenticated(false);
        setClaims(undefined);
    }



    return (
        <AuthContext.Provider value={{ claims, isAuthenticated, login, logout }}>
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