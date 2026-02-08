import React from 'react';
import { BrowserRouter, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { AuthProvider, useAuth } from './auth/AuthContext';
import { LoginPage } from './pages/LoginPage';
import { TasksPage } from './pages/TasksPage';
import { UsersPage } from './pages/UsersPage';

const PrivateRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { token, isLoading } = useAuth();
    const location = useLocation();

    if (isLoading) return <div>Loading...</div>;

    if (!token) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    return <>{children}</>;
};

function App() {
    return (
        <BrowserRouter>
            <AuthProvider>
                <Routes>
                    <Route path="/login" element={<LoginPage />} />
                    <Route path="/tasks" element={
                        <PrivateRoute>
                            <TasksPage />
                        </PrivateRoute>
                    }
                    />
                    <Route path="/users" element={
                        <PrivateRoute>
                            <UsersPage />
                        </PrivateRoute>
                    }
                    />
                    <Route path="/" element={<Navigate to="/tasks" />} />
                    <Route path="*" element={<Navigate to="/tasks" />} />
                </Routes>
            </AuthProvider>
        </BrowserRouter>
    );
}

export default App;

