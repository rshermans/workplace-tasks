import React from 'react';
import { useAuth } from '../auth/AuthContext';
import { Link } from 'react-router-dom';

export const Header: React.FC = () => {
    const { user, logout } = useAuth();

    const getRoleBadgeClass = () => {
        switch (user?.role) {
            case 'Admin': return 'badge badge-admin';
            case 'Manager': return 'badge badge-manager';
            case 'Member': return 'badge badge-member';
            default: return 'badge';
        }
    };

    const getUserInitials = () => {
        if (!user?.email) return '?';
        return user.email.charAt(0).toUpperCase();
    };

    return (
        <header className="app-header">
            <div className="header-brand">
                <svg className="header-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2" />
                    <rect x="9" y="3" width="6" height="4" rx="1" />
                    <path d="M9 12l2 2 4-4" />
                </svg>
                <h1>WorkPlace Tasks</h1>
            </div>

            <div className="header-nav" style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-md)' }}>
                {user?.role === 'Admin' && (
                    <Link to="/users" className="btn btn-ghost btn-sm" style={{ textDecoration: 'none' }}>
                        👥 Users
                    </Link>
                )}
            </div>

            <div className="header-user">
                <div className="user-avatar">
                    {getUserInitials()}
                </div>
                <div className="user-info">
                    <span className="user-email">{user?.email}</span>
                    <span className={getRoleBadgeClass()}>{user?.role}</span>
                </div>
                <button onClick={logout} className="btn btn-ghost btn-sm">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <path d="M9 21H5a2 2 0 01-2-2V5a2 2 0 012-2h4" />
                        <polyline points="16,17 21,12 16,7" />
                        <line x1="21" y1="12" x2="9" y2="12" />
                    </svg>
                    Logout
                </button>
            </div>

            <style>{`
                .app-header {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    padding: var(--space-md) var(--space-lg);
                    background: var(--bg-secondary);
                    border-bottom: 1px solid var(--border-color);
                    margin-bottom: var(--space-xl);
                    border-radius: var(--radius-lg);
                }

                .header-brand {
                    display: flex;
                    align-items: center;
                    gap: var(--space-md);
                }

                .header-brand h1 {
                    margin: 0;
                    font-size: var(--font-size-xl);
                    background: linear-gradient(135deg, var(--color-primary), #a78bfa);
                    -webkit-background-clip: text;
                    -webkit-text-fill-color: transparent;
                    background-clip: text;
                }

                .header-icon {
                    width: 32px;
                    height: 32px;
                    color: var(--color-primary);
                }

                .header-user {
                    display: flex;
                    align-items: center;
                    gap: var(--space-md);
                }

                .user-avatar {
                    width: 40px;
                    height: 40px;
                    border-radius: 50%;
                    background: linear-gradient(135deg, var(--color-primary), #a78bfa);
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    font-weight: 600;
                    font-size: var(--font-size-lg);
                    color: white;
                }

                .user-info {
                    display: flex;
                    flex-direction: column;
                    align-items: flex-end;
                    gap: var(--space-xs);
                }

                .user-email {
                    font-size: var(--font-size-sm);
                    color: var(--text-secondary);
                }

                @media (max-width: 640px) {
                    .app-header {
                        flex-direction: column;
                        gap: var(--space-md);
                    }
                    
                    .user-email {
                        display: none;
                    }
                }
            `}</style>
        </header>
    );
};
