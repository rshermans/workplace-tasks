import React, { useState } from 'react';
import { useAuth } from '../auth/AuthContext';
import { useNavigate } from 'react-router-dom';
import { LoadingSpinner } from '../components/LoadingSpinner';

export const LoginPage: React.FC = () => {
    const { login } = useAuth();
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsLoading(true);
        setError('');

        try {
            await login(email, password);
            navigate('/tasks');
        } catch (err) {
            setError('Invalid email or password');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="login-container">
            <div className="login-card">
                <div className="login-header">
                    <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5">
                        <path d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2" />
                        <rect x="9" y="3" width="6" height="4" rx="1" />
                        <path d="M9 12l2 2 4-4" />
                    </svg>
                    <h1>WorkPlace Tasks</h1>
                    <p>Sign in to manage your tasks</p>
                </div>

                {error && (
                    <div className="login-error">
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                            <circle cx="12" cy="12" r="10" />
                            <line x1="15" y1="9" x2="9" y2="15" />
                            <line x1="9" y1="9" x2="15" y2="15" />
                        </svg>
                        {error}
                    </div>
                )}

                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label className="form-label" htmlFor="email">Email</label>
                        <input
                            id="email"
                            className="input"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="Enter your email..."
                            required
                            disabled={isLoading}
                        />
                    </div>
                    <div className="form-group">
                        <label className="form-label" htmlFor="password">Password</label>
                        <input
                            id="password"
                            className="input"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Enter your password..."
                            required
                            disabled={isLoading}
                        />
                    </div>
                    <button
                        type="submit"
                        className="btn btn-primary btn-lg"
                        disabled={isLoading}
                        style={{ width: '100%', marginTop: 'var(--space-md)' }}
                    >
                        {isLoading ? (
                            <>
                                <LoadingSpinner size="sm" />
                                Signing in...
                            </>
                        ) : (
                            'Sign In'
                        )}
                    </button>
                </form>

                <div className="login-credentials">
                    <h4>Demo Credentials</h4>
                    <div className="credentials-list">
                        <div className="credential-item">
                            <span className="badge badge-admin">Admin</span>
                            <code>admin@example.com</code>
                        </div>
                        <div className="credential-item">
                            <span className="badge badge-manager">Manager</span>
                            <code>manager@example.com</code>
                        </div>
                        <div className="credential-item">
                            <span className="badge badge-member">Member</span>
                            <code>member@example.com</code>
                        </div>
                        <p className="password-hint">Password: <code>Password123!</code></p>
                    </div>
                </div>
            </div>

            <style>{`
                .login-container {
                    min-height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    padding: var(--space-lg);
                }

                .login-card {
                    width: 100%;
                    max-width: 420px;
                    background: var(--bg-secondary);
                    border: 1px solid var(--border-color);
                    border-radius: var(--radius-lg);
                    padding: var(--space-xl);
                    box-shadow: var(--shadow-lg);
                }

                .login-header {
                    text-align: center;
                    margin-bottom: var(--space-xl);
                }

                .login-header svg {
                    color: var(--color-primary);
                    margin-bottom: var(--space-md);
                }

                .login-header h1 {
                    margin: 0 0 var(--space-xs);
                    font-size: var(--font-size-2xl);
                    background: linear-gradient(135deg, var(--color-primary), #a78bfa);
                    -webkit-background-clip: text;
                    -webkit-text-fill-color: transparent;
                    background-clip: text;
                }

                .login-header p {
                    margin: 0;
                    color: var(--text-muted);
                }

                .login-error {
                    display: flex;
                    align-items: center;
                    gap: var(--space-sm);
                    background: rgba(239, 68, 68, 0.1);
                    color: var(--color-danger);
                    padding: var(--space-md);
                    border-radius: var(--radius-md);
                    margin-bottom: var(--space-lg);
                    font-size: var(--font-size-sm);
                }

                .login-credentials {
                    margin-top: var(--space-xl);
                    padding-top: var(--space-lg);
                    border-top: 1px solid var(--border-color);
                }

                .login-credentials h4 {
                    margin: 0 0 var(--space-md);
                    font-size: var(--font-size-sm);
                    color: var(--text-muted);
                    text-transform: uppercase;
                    letter-spacing: 0.05em;
                }

                .credentials-list {
                    display: flex;
                    flex-direction: column;
                    gap: var(--space-sm);
                }

                .credential-item {
                    display: flex;
                    align-items: center;
                    gap: var(--space-sm);
                }

                .credential-item code {
                    font-size: var(--font-size-sm);
                    color: var(--text-secondary);
                }

                .password-hint {
                    margin-top: var(--space-md);
                    font-size: var(--font-size-sm);
                    color: var(--text-muted);
                }

                .password-hint code {
                    color: var(--text-secondary);
                    background: var(--bg-input);
                    padding: var(--space-xs);
                    border-radius: var(--radius-sm);
                }
            `}</style>
        </div>
    );
};
