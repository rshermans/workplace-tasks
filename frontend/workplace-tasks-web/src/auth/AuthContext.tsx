import React, { createContext, useContext, useEffect, useState } from 'react';
import http from '../api/http';
import { LoginResponse, User } from '../types';

interface AuthContextType {
    user: User | null;
    token: string | null;
    login: (email: string, password: string) => Promise<void>;
    logout: () => void;
    isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<User | null>(null);
    const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        if (token) {
            // Decode token or restore user from storage. 
            // For simplicity, we store role/email in localstorage too or just decode JWT here.
            // Let's assume we stored them or decode them.
            const role = localStorage.getItem('role') as any;
            const email = localStorage.getItem('email') as string;
            if (role && email) {
                setUser({ role, email });
            }
        }
        setIsLoading(false);
    }, [token]);

    const login = async (email: string, password: string) => {
        const { data } = await http.post<LoginResponse>('/auth/login', { email, password });
        localStorage.setItem('token', data.token);
        localStorage.setItem('role', data.role);
        localStorage.setItem('email', data.email);
        setToken(data.token);
        setUser({ role: data.role as any, email: data.email });
    };

    const logout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('role');
        localStorage.removeItem('email');
        setToken(null);
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, token, login, logout, isLoading }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) throw new Error('useAuth must be used within AuthProvider');
    return context;
};
