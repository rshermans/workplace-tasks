import React, { useState } from 'react';
import http from '../api/http';
import { LoadingSpinner } from './LoadingSpinner';

interface TaskFormProps {
    onSuccess: () => void;
    isOpen?: boolean;
}

export const TaskForm: React.FC<TaskFormProps> = ({ onSuccess, isOpen = true }) => {
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsSubmitting(true);
        setError(null);

        try {
            await http.post('/tasks', { title, description });
            setTitle('');
            setDescription('');
            onSuccess();
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to create task');
        } finally {
            setIsSubmitting(false);
        }
    };

    if (!isOpen) {
        return null;
    }

    return (
        <div className="task-form-container fade-in">
            <div className="task-form-header">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <line x1="12" y1="5" x2="12" y2="19" />
                    <line x1="5" y1="12" x2="19" y2="12" />
                </svg>
                <h3>Create New Task</h3>
            </div>

            {error && (
                <div className="form-error">{error}</div>
            )}

            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label className="form-label" htmlFor="title">Title *</label>
                    <input
                        id="title"
                        className="input"
                        placeholder="Enter task title..."
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                        required
                        disabled={isSubmitting}
                    />
                </div>
                <div className="form-group">
                    <label className="form-label" htmlFor="description">Description</label>
                    <textarea
                        id="description"
                        className="textarea"
                        placeholder="Add a description (optional)..."
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        disabled={isSubmitting}
                    />
                </div>
                <button
                    type="submit"
                    className="btn btn-primary btn-lg"
                    disabled={isSubmitting || !title.trim()}
                    style={{ width: '100%' }}
                >
                    {isSubmitting ? (
                        <>
                            <LoadingSpinner size="sm" />
                            Creating...
                        </>
                    ) : (
                        <>
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                                <line x1="12" y1="5" x2="12" y2="19" />
                                <line x1="5" y1="12" x2="19" y2="12" />
                            </svg>
                            Create Task
                        </>
                    )}
                </button>
            </form>

            <style>{`
                .task-form-container {
                    background: var(--bg-secondary);
                    border: 1px solid var(--border-color);
                    border-radius: var(--radius-lg);
                    padding: var(--space-lg);
                    margin-bottom: var(--space-xl);
                }

                .task-form-header {
                    display: flex;
                    align-items: center;
                    gap: var(--space-sm);
                    margin-bottom: var(--space-lg);
                }

                .task-form-header h3 {
                    margin: 0;
                    color: var(--text-primary);
                }

                .task-form-header svg {
                    color: var(--color-primary);
                }

                .form-error {
                    background: rgba(239, 68, 68, 0.1);
                    color: var(--color-danger);
                    padding: var(--space-sm) var(--space-md);
                    border-radius: var(--radius-md);
                    margin-bottom: var(--space-md);
                    font-size: var(--font-size-sm);
                }
            `}</style>
        </div>
    );
};
