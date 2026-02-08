import React, { useEffect, useState } from 'react';
import { useAuth } from '../auth/AuthContext';
import { Header } from '../components/Header';
import { LoadingSpinner } from '../components/LoadingSpinner';
import { userApi } from '../api/taskApi';
import { useNavigate } from 'react-router-dom';

interface UserDto {
    id: string;
    email: string;
    role: string;
    taskCount: number;
}

interface NewUserForm {
    email: string;
    password: string;
    role: string;
}

const roles = ['Admin', 'Manager', 'Member'];

export const UsersPage: React.FC = () => {
    const { user } = useAuth();
    const navigate = useNavigate();
    const [users, setUsers] = useState<UserDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [updatingId, setUpdatingId] = useState<string | null>(null);

    // Add User Modal state
    const [isAddModalOpen, setIsAddModalOpen] = useState(false);
    const [isCreating, setIsCreating] = useState(false);
    const [createError, setCreateError] = useState<string | null>(null);
    const [newUser, setNewUser] = useState<NewUserForm>({
        email: '',
        password: '',
        role: 'Member'
    });

    useEffect(() => {
        // Only admins can access this page
        if (user?.role !== 'Admin') {
            navigate('/tasks');
            return;
        }
        fetchUsers();
    }, [user, navigate]);

    const fetchUsers = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const data = await userApi.getAll();
            setUsers(data);
        } catch (err: any) {
            if (err.response?.status === 403) {
                setError('Access denied. Admin only.');
            } else {
                setError('Failed to load users.');
            }
        } finally {
            setIsLoading(false);
        }
    };

    const handleRoleChange = async (userId: string, newRole: string) => {
        setUpdatingId(userId);
        try {
            await userApi.updateRole(userId, newRole);
            fetchUsers();
        } catch (err: any) {
            alert(err.response?.data?.message || 'Failed to update role');
        } finally {
            setUpdatingId(null);
        }
    };

    const handleDelete = async (userId: string, email: string) => {
        if (!confirm(`Are you sure you want to delete user "${email}"? This action cannot be undone.`)) {
            return;
        }
        try {
            await userApi.delete(userId);
            fetchUsers();
        } catch (err: any) {
            alert(err.response?.data?.message || 'Failed to delete user');
        }
    };

    const handleCreateUser = async (e: React.FormEvent) => {
        e.preventDefault();
        setCreateError(null);

        // Validation
        if (!newUser.email.trim()) {
            setCreateError('Email is required');
            return;
        }
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(newUser.email)) {
            setCreateError('Please enter a valid email address');
            return;
        }
        if (newUser.password.length < 6) {
            setCreateError('Password must be at least 6 characters');
            return;
        }

        setIsCreating(true);
        try {
            await userApi.create({
                email: newUser.email.trim(),
                password: newUser.password,
                role: newUser.role
            });
            // Reset form and close modal
            setNewUser({ email: '', password: '', role: 'Member' });
            setIsAddModalOpen(false);
            fetchUsers();
        } catch (err: any) {
            setCreateError(err.response?.data?.message || err.response?.data || 'Failed to create user');
        } finally {
            setIsCreating(false);
        }
    };

    const openAddModal = () => {
        setNewUser({ email: '', password: '', role: 'Member' });
        setCreateError(null);
        setIsAddModalOpen(true);
    };

    if (user?.role !== 'Admin') {
        return null;
    }

    return (
        <div className="container" style={{ paddingTop: 'var(--space-xl)', paddingBottom: 'var(--space-2xl)' }}>
            <Header />

            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 'var(--space-lg)' }}>
                <div>
                    <h2 style={{ fontSize: 'var(--font-size-xl)', marginBottom: 'var(--space-xs)' }}>
                        👥 User Management
                    </h2>
                    <p style={{ color: 'var(--text-secondary)', margin: 0 }}>
                        Manage user roles and permissions
                    </p>
                </div>
                <button className="btn btn-primary" onClick={openAddModal}>
                    + Add User
                </button>
            </div>

            {isLoading ? (
                <LoadingSpinner size="lg" message="Loading users..." />
            ) : error ? (
                <div className="empty-state">
                    <p style={{ color: 'var(--color-danger)' }}>{error}</p>
                    <button className="btn btn-primary" onClick={fetchUsers}>Retry</button>
                </div>
            ) : users.length === 0 ? (
                <div className="empty-state">
                    <p>No users found.</p>
                </div>
            ) : (
                <div className="users-list" style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-sm)' }}>
                    {users.map(u => (
                        <div
                            key={u.id}
                            className="card"
                            style={{
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'space-between',
                                padding: 'var(--space-md)',
                                background: 'var(--bg-secondary)',
                                borderRadius: 'var(--radius-md)',
                                border: '1px solid var(--border-color)'
                            }}
                        >
                            <div style={{ flex: 1 }}>
                                <div style={{ fontWeight: 600, marginBottom: 'var(--space-xs)' }}>
                                    {u.email}
                                </div>
                                <div style={{ fontSize: 'var(--font-size-sm)', color: 'var(--text-secondary)' }}>
                                    {u.taskCount} task{u.taskCount !== 1 ? 's' : ''} created
                                </div>
                            </div>

                            <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)' }}>
                                <select
                                    value={u.role}
                                    onChange={(e) => handleRoleChange(u.id, e.target.value)}
                                    disabled={updatingId === u.id}
                                    style={{
                                        padding: 'var(--space-xs) var(--space-sm)',
                                        borderRadius: 'var(--radius-md)',
                                        border: '1px solid var(--border-color)',
                                        background: 'var(--bg-primary)',
                                        color: 'var(--text-primary)',
                                        cursor: updatingId === u.id ? 'wait' : 'pointer'
                                    }}
                                >
                                    {roles.map(role => (
                                        <option key={role} value={role}>{role}</option>
                                    ))}
                                </select>

                                <button
                                    className="btn btn-sm btn-ghost"
                                    onClick={() => handleDelete(u.id, u.email)}
                                    style={{ color: 'var(--color-danger)' }}
                                    title="Delete user"
                                >
                                    🗑️
                                </button>
                            </div>
                        </div>
                    ))}
                </div>
            )}

            <div style={{ marginTop: 'var(--space-lg)', textAlign: 'center' }}>
                <button className="btn btn-ghost" onClick={() => navigate('/tasks')}>
                    ← Back to Tasks
                </button>
            </div>

            {/* Add User Modal */}
            {isAddModalOpen && (
                <div
                    className="modal-overlay"
                    style={{
                        position: 'fixed',
                        inset: 0,
                        background: 'rgba(0, 0, 0, 0.6)',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        zIndex: 1000
                    }}
                    onClick={() => setIsAddModalOpen(false)}
                >
                    <div
                        className="modal-content"
                        style={{
                            background: 'var(--bg-primary)',
                            borderRadius: 'var(--radius-lg)',
                            border: '1px solid var(--border-color)',
                            padding: 'var(--space-xl)',
                            width: '100%',
                            maxWidth: '400px',
                            boxShadow: '0 20px 40px rgba(0, 0, 0, 0.3)'
                        }}
                        onClick={(e) => e.stopPropagation()}
                    >
                        <h3 style={{ marginTop: 0, marginBottom: 'var(--space-lg)', fontSize: 'var(--font-size-lg)' }}>
                            ➕ Add New User
                        </h3>

                        <form onSubmit={handleCreateUser}>
                            <div style={{ marginBottom: 'var(--space-md)' }}>
                                <label style={{ display: 'block', marginBottom: 'var(--space-xs)', fontWeight: 500 }}>
                                    Email
                                </label>
                                <input
                                    type="email"
                                    value={newUser.email}
                                    onChange={(e) => setNewUser({ ...newUser, email: e.target.value })}
                                    placeholder="user@example.com"
                                    style={{
                                        width: '100%',
                                        padding: 'var(--space-sm)',
                                        borderRadius: 'var(--radius-md)',
                                        border: '1px solid var(--border-color)',
                                        background: 'var(--bg-secondary)',
                                        color: 'var(--text-primary)',
                                        boxSizing: 'border-box'
                                    }}
                                    autoFocus
                                />
                            </div>

                            <div style={{ marginBottom: 'var(--space-md)' }}>
                                <label style={{ display: 'block', marginBottom: 'var(--space-xs)', fontWeight: 500 }}>
                                    Password
                                </label>
                                <input
                                    type="password"
                                    value={newUser.password}
                                    onChange={(e) => setNewUser({ ...newUser, password: e.target.value })}
                                    placeholder="At least 6 characters"
                                    style={{
                                        width: '100%',
                                        padding: 'var(--space-sm)',
                                        borderRadius: 'var(--radius-md)',
                                        border: '1px solid var(--border-color)',
                                        background: 'var(--bg-secondary)',
                                        color: 'var(--text-primary)',
                                        boxSizing: 'border-box'
                                    }}
                                />
                            </div>

                            <div style={{ marginBottom: 'var(--space-lg)' }}>
                                <label style={{ display: 'block', marginBottom: 'var(--space-xs)', fontWeight: 500 }}>
                                    Role
                                </label>
                                <select
                                    value={newUser.role}
                                    onChange={(e) => setNewUser({ ...newUser, role: e.target.value })}
                                    style={{
                                        width: '100%',
                                        padding: 'var(--space-sm)',
                                        borderRadius: 'var(--radius-md)',
                                        border: '1px solid var(--border-color)',
                                        background: 'var(--bg-secondary)',
                                        color: 'var(--text-primary)',
                                        boxSizing: 'border-box'
                                    }}
                                >
                                    {roles.map(role => (
                                        <option key={role} value={role}>{role}</option>
                                    ))}
                                </select>
                            </div>

                            {createError && (
                                <div style={{
                                    color: 'var(--color-danger)',
                                    marginBottom: 'var(--space-md)',
                                    padding: 'var(--space-sm)',
                                    background: 'rgba(239, 68, 68, 0.1)',
                                    borderRadius: 'var(--radius-md)',
                                    fontSize: 'var(--font-size-sm)'
                                }}>
                                    {createError}
                                </div>
                            )}

                            <div style={{ display: 'flex', gap: 'var(--space-sm)', justifyContent: 'flex-end' }}>
                                <button
                                    type="button"
                                    className="btn btn-ghost"
                                    onClick={() => setIsAddModalOpen(false)}
                                    disabled={isCreating}
                                >
                                    Cancel
                                </button>
                                <button
                                    type="submit"
                                    className="btn btn-primary"
                                    disabled={isCreating}
                                >
                                    {isCreating ? 'Creating...' : 'Create User'}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};
