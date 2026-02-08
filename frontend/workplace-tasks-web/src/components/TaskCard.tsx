import React from 'react';
import { TaskItem } from '../types';

interface TaskCardProps {
    task: TaskItem;
    canEdit: boolean;
    canDelete: boolean;
    onEdit: () => void;
    onDelete: () => void;
    isOwner: boolean;
}

export const TaskCard: React.FC<TaskCardProps> = ({
    task,
    canEdit,
    canDelete,
    onEdit,
    onDelete,
    isOwner
}) => {
    const getStatusBadgeClass = () => {
        switch (task.status) {
            case 'Done': return 'badge badge-done';
            case 'InProgress': return 'badge badge-inprogress';
            default: return 'badge badge-pending';
        }
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString('pt-PT', {
            day: '2-digit',
            month: 'short',
            year: 'numeric'
        });
    };

    return (
        <div className="task-card fade-in">
            <div className="task-card-header">
                <h3 className="task-title">{task.title}</h3>
                <span className={getStatusBadgeClass()}>{task.status}</span>
            </div>

            {task.description && (
                <p className="task-description">{task.description}</p>
            )}

            <div className="task-meta">
                <div className="task-meta-item">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <circle cx="12" cy="7" r="4" />
                        <path d="M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2" />
                    </svg>
                    <span>{task.createdByEmail}</span>
                    {isOwner && <span className="owner-tag">You</span>}
                </div>
                <div className="task-meta-item">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <rect x="3" y="4" width="18" height="18" rx="2" />
                        <line x1="16" y1="2" x2="16" y2="6" />
                        <line x1="8" y1="2" x2="8" y2="6" />
                        <line x1="3" y1="10" x2="21" y2="10" />
                    </svg>
                    <span>{formatDate(task.createdAt)}</span>
                </div>
            </div>

            <div className="task-actions">
                <button
                    className={`btn btn-sm ${canEdit ? 'btn-ghost' : 'btn-disabled'}`}
                    onClick={canEdit ? onEdit : undefined}
                    title={canEdit ? "Edit Task" : "Permissions required to edit this task"}
                    style={!canEdit ? { opacity: 0.4, cursor: 'not-allowed' } : {}}
                >
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7" />
                        <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z" />
                    </svg>
                    Edit
                </button>
                <button
                    className={`btn btn-sm ${canDelete ? 'btn-danger' : 'btn-disabled'}`}
                    onClick={canDelete ? onDelete : undefined}
                    title={canDelete ? "Delete Task" : "Only the creator or an Admin can delete this task"}
                    style={!canDelete ? { opacity: 0.4, cursor: 'not-allowed' } : {}}
                >
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <polyline points="3,6 5,6 21,6" />
                        <path d="M19 6v14a2 2 0 01-2 2H7a2 2 0 01-2-2V6m3 0V4a2 2 0 012-2h4a2 2 0 012 2v2" />
                    </svg>
                    Delete
                </button>
                {!canEdit && !canDelete && (
                    <span
                        className="btn btn-ghost btn-sm"
                        title="This will be in the next version"
                        style={{ opacity: 0.5, borderStyle: 'dashed' }}
                    >
                        Locked
                    </span>
                )}
            </div>

            <style>{`
                .task-card {
                    background: var(--bg-card);
                    border: 1px solid var(--border-color);
                    border-radius: var(--radius-lg);
                    padding: var(--space-lg);
                    margin-bottom: var(--space-md);
                    transition: all var(--transition-base);
                }

                .task-card:hover {
                    border-color: var(--color-primary);
                    box-shadow: var(--shadow-md);
                    transform: translateY(-2px);
                }

                .task-card-header {
                    display: flex;
                    justify-content: space-between;
                    align-items: flex-start;
                    gap: var(--space-md);
                    margin-bottom: var(--space-sm);
                }

                .task-title {
                    margin: 0;
                    font-size: var(--font-size-lg);
                    color: var(--text-primary);
                }

                .task-description {
                    color: var(--text-secondary);
                    font-size: var(--font-size-sm);
                    margin-bottom: var(--space-md);
                    line-height: 1.5;
                }

                .task-meta {
                    display: flex;
                    flex-wrap: wrap;
                    gap: var(--space-lg);
                    margin-bottom: var(--space-md);
                }

                .task-meta-item {
                    display: flex;
                    align-items: center;
                    gap: var(--space-xs);
                    font-size: var(--font-size-sm);
                    color: var(--text-muted);
                }

                .task-meta-item svg {
                    opacity: 0.7;
                }

                .owner-tag {
                    background: var(--color-primary);
                    color: white;
                    padding: 2px 6px;
                    border-radius: var(--radius-sm);
                    font-size: 0.70rem;
                    font-weight: 700;
                    text-transform: uppercase;
                    margin-left: var(--space-xs);
                    letter-spacing: 0.05em;
                }

                .task-actions {
                    display: flex;
                    gap: var(--space-sm);
                    padding-top: var(--space-md);
                    border-top: 1px solid var(--border-color);
                }
            `}</style>
        </div>
    );
};
