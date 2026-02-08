import React, { useState, useEffect } from 'react';
import { TaskItem } from '../types';
import http from '../api/http';
import { LoadingSpinner } from './LoadingSpinner';

interface EditTaskModalProps {
    task: TaskItem;
    onClose: () => void;
    onSuccess: () => void;
}

type TaskStatus = 'Pending' | 'InProgress' | 'Done';

export const EditTaskModal: React.FC<EditTaskModalProps> = ({
    task,
    onClose,
    onSuccess
}) => {
    const [title, setTitle] = useState(task.title);
    const [description, setDescription] = useState(task.description || '');
    const [status, setStatus] = useState<TaskStatus>(task.status);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Close on Escape key
    useEffect(() => {
        const handleEscape = (e: KeyboardEvent) => {
            if (e.key === 'Escape') onClose();
        };
        window.addEventListener('keydown', handleEscape);
        return () => window.removeEventListener('keydown', handleEscape);
    }, [onClose]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsSubmitting(true);
        setError(null);

        try {
            await http.put(`/tasks/${task.id}`, {
                title,
                description,
                status
            });
            onSuccess();
            onClose();
        } catch (err: any) {
            if (err.response?.status === 403) {
                setError('You do not have permission to edit this task.');
            } else {
                setError(err.response?.data?.message || 'Failed to update task');
            }
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleOverlayClick = (e: React.MouseEvent) => {
        if (e.target === e.currentTarget) {
            onClose();
        }
    };

    return (
        <div className="modal-overlay" onClick={handleOverlayClick}>
            <div className="modal fade-in">
                <div className="modal-header">
                    <h2>Edit Task</h2>
                    <button className="modal-close" onClick={onClose}>×</button>
                </div>

                {error && (
                    <div className="modal-error">{error}</div>
                )}

                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label className="form-label" htmlFor="edit-title">Title *</label>
                        <input
                            id="edit-title"
                            className="input"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                            required
                            disabled={isSubmitting}
                        />
                    </div>

                    <div className="form-group">
                        <label className="form-label" htmlFor="edit-description">Description</label>
                        <textarea
                            id="edit-description"
                            className="textarea"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            disabled={isSubmitting}
                        />
                    </div>

                    <div className="form-group">
                        <label className="form-label" htmlFor="edit-status">Status</label>
                        <div className="status-options">
                            {(['Pending', 'InProgress', 'Done'] as TaskStatus[]).map((s) => (
                                <label
                                    key={s}
                                    className={`status-option ${status === s ? 'active' : ''} status-${s.toLowerCase()}`}
                                >
                                    <input
                                        type="radio"
                                        name="status"
                                        value={s}
                                        checked={status === s}
                                        onChange={() => setStatus(s)}
                                        disabled={isSubmitting}
                                    />
                                    {s === 'InProgress' ? 'In Progress' : s}
                                </label>
                            ))}
                        </div>
                    </div>

                    <div className="modal-actions">
                        <button
                            type="button"
                            className="btn btn-ghost"
                            onClick={onClose}
                            disabled={isSubmitting}
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            className="btn btn-primary"
                            disabled={isSubmitting || !title.trim()}
                        >
                            {isSubmitting ? (
                                <>
                                    <LoadingSpinner size="sm" />
                                    Saving...
                                </>
                            ) : (
                                'Save Changes'
                            )}
                        </button>
                    </div>
                </form>

                <style>{`
                    .modal h2 {
                        margin: 0;
                        font-size: var(--font-size-xl);
                    }

                    .modal-error {
                        background: rgba(239, 68, 68, 0.1);
                        color: var(--color-danger);
                        padding: var(--space-md);
                        border-radius: var(--radius-md);
                        margin-bottom: var(--space-lg);
                        font-size: var(--font-size-sm);
                    }

                    .status-options {
                        display: flex;
                        gap: var(--space-sm);
                    }

                    .status-option {
                        flex: 1;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        padding: var(--space-sm) var(--space-md);
                        border: 1px solid var(--border-color);
                        border-radius: var(--radius-md);
                        cursor: pointer;
                        transition: all var(--transition-base);
                        font-size: var(--font-size-sm);
                    }

                    .status-option input {
                        display: none;
                    }

                    .status-option:hover {
                        border-color: var(--color-primary);
                    }

                    .status-option.active.status-pending {
                        background: rgba(100, 116, 139, 0.2);
                        border-color: var(--text-muted);
                        color: var(--text-secondary);
                    }

                    .status-option.active.status-inprogress {
                        background: rgba(245, 158, 11, 0.15);
                        border-color: var(--color-warning);
                        color: var(--color-warning);
                    }

                    .status-option.active.status-done {
                        background: rgba(16, 185, 129, 0.15);
                        border-color: var(--color-success);
                        color: var(--color-success);
                    }

                    .modal-actions {
                        display: flex;
                        justify-content: flex-end;
                        gap: var(--space-md);
                        margin-top: var(--space-xl);
                        padding-top: var(--space-lg);
                        border-top: 1px solid var(--border-color);
                    }
                `}</style>
            </div>
        </div>
    );
};
