import React, { useEffect, useState } from 'react';
import { TaskItem } from '../types';
import { useAuth } from '../auth/AuthContext';
import { Header } from '../components/Header';
import { TaskCard } from '../components/TaskCard';
import { TaskForm } from '../components/TaskForm';
import { LoadingSpinner } from '../components/LoadingSpinner';
import { EditTaskModal } from '../components/EditTaskModal';
import { taskApi } from '../api/taskApi';

type TaskStatus = 'Pending' | 'InProgress' | 'Done' | '';

export const TasksPage: React.FC = () => {
    const { user, token } = useAuth();
    const [tasks, setTasks] = useState<TaskItem[]>([]);
    const [userId, setUserId] = useState<string>('');
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [editingTask, setEditingTask] = useState<TaskItem | null>(null);
    const [isFormOpen, setIsFormOpen] = useState(false);

    // Pagination state
    const [page, setPage] = useState(1);
    const [pageSize] = useState(10);
    const [totalPages, setTotalPages] = useState(1);
    const [totalCount, setTotalCount] = useState(0);

    // Filter state
    const [statusFilter, setStatusFilter] = useState<TaskStatus>('');
    const [ownerFilter, setOwnerFilter] = useState<'all' | 'mine'>('all');

    useEffect(() => {
        if (token) {
            try {
                const payload = JSON.parse(atob(token.split('.')[1]));
                // .NET uses ClaimTypes.NameIdentifier which maps to this URI in JWT
                const nameIdClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
                const extractedUserId = payload[nameIdClaim] || payload.sub || payload.nameid || '';
                setUserId(extractedUserId);
                console.log('Extracted userId from token:', extractedUserId); // Debug log
            } catch (e) {
                console.error("Failed to parse token");
            }
        }
    }, [token]);

    useEffect(() => {
        fetchTasks();
    }, [page, statusFilter]);

    const fetchTasks = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const response = await taskApi.getPaged({
                page,
                pageSize,
                status: statusFilter || undefined
            });
            setTasks(response.items);
            setTotalPages(response.totalPages);
            setTotalCount(response.totalCount);
        } catch (err) {
            setError('Ainda não temos tarefa'); // Set friendly message on error as requested
            console.error('Failed to fetch tasks', err);
        } finally {
            setIsLoading(false);
        }
    };

    const handleDelete = async (id: string) => {
        if (!confirm('Are you sure you want to delete this task?')) return;
        try {
            await taskApi.delete(id);
            fetchTasks();
        } catch (error: any) {
            if (error.response?.status === 403) {
                alert("Forbidden: You cannot delete this task.");
            } else {
                alert("Error deleting task.");
            }
        }
    };

    const handleEdit = (task: TaskItem) => {
        setEditingTask(task);
    };

    const handleCloseModal = () => {
        setEditingTask(null);
    };

    const handleEditSuccess = () => {
        fetchTasks();
    };

    const handleStatusFilterChange = (status: TaskStatus) => {
        setStatusFilter(status);
        setPage(1); // Reset to first page when filter changes
    };

    const handlePreviousPage = () => {
        if (page > 1) setPage(page - 1);
    };

    const handleNextPage = () => {
        if (page < totalPages) setPage(page + 1);
    };

    // RBAC Logic - Helper for case-insensitive owner check
    const isTaskOwner = (task: TaskItem) => {
        if (!userId || !task.createdByUserId) return false;
        return task.createdByUserId.toLowerCase() === userId.toLowerCase();
    };

    const canDelete = (task: TaskItem) => {
        if (!user) return false;
        if (user.role === 'Admin') return true;
        if (user.role === 'Manager') return isTaskOwner(task);
        if (user.role === 'Member') return isTaskOwner(task);
        return false;
    };

    const canEdit = (task: TaskItem) => {
        if (!user) return false;
        if (user.role === 'Admin') return true;
        if (user.role === 'Manager') return true;
        if (user.role === 'Member') return isTaskOwner(task);
        return false;
    };

    // Client-side owner filter (pagination is server-side, but owner filter is client-side for now)
    const filteredTasks = ownerFilter === 'all'
        ? tasks
        : tasks.filter(t => isTaskOwner(t));

    return (
        <div className="container" style={{
            paddingTop: 'var(--space-xl)',
            paddingBottom: 0,
            display: 'flex',
            flexDirection: 'column',
            height: 'calc(100vh - var(--space-xl))'
        }}>
            <Header />

            {/* Toggle Button for Create Form */}
            {!isFormOpen && (
                <button
                    className="btn btn-primary btn-lg"
                    onClick={() => setIsFormOpen(true)}
                    style={{
                        width: '100%',
                        marginBottom: 'var(--space-lg)'
                    }}
                >
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <line x1="12" y1="5" x2="12" y2="19" />
                        <line x1="5" y1="12" x2="19" y2="12" />
                    </svg>
                    New Task
                </button>
            )}

            {/* Collapsible Form */}
            {isFormOpen && (
                <div style={{ marginBottom: 'var(--space-lg)', position: 'relative' }}>
                    <button
                        className="btn btn-sm btn-ghost"
                        onClick={() => setIsFormOpen(false)}
                        style={{
                            position: 'absolute',
                            top: 'var(--space-md)',
                            right: 'var(--space-md)',
                            zIndex: 10
                        }}
                    >
                        ✕
                    </button>
                    <TaskForm onSuccess={() => {
                        fetchTasks();
                        setIsFormOpen(false);
                    }} isOpen={isFormOpen} />
                </div>
            )}

            {/* Filters Section - Sticky */}
            <div className="filters-section" style={{
                position: 'sticky',
                top: 0,
                zIndex: 100,
                background: 'var(--bg-primary)',
                paddingTop: 'var(--space-sm)',
                paddingBottom: 'var(--space-md)',
                marginBottom: 'var(--space-md)',
                display: 'flex',
                flexWrap: 'wrap',
                gap: 'var(--space-md)',
                alignItems: 'center',
                justifyContent: 'space-between',
                borderBottom: '1px solid var(--border-color)'
            }}>
                <div style={{ display: 'flex', gap: 'var(--space-md)', alignItems: 'center' }}>
                    <h2 style={{ margin: 0, fontSize: 'var(--font-size-lg)' }}>Tasks</h2>

                    {/* Status Filter Dropdown */}
                    <select
                        value={statusFilter}
                        onChange={(e) => handleStatusFilterChange(e.target.value as TaskStatus)}
                        className="select"
                        style={{
                            padding: 'var(--space-xs) var(--space-sm)',
                            borderRadius: 'var(--radius-md)',
                            border: '1px solid var(--border-color)',
                            background: 'var(--bg-secondary)',
                            color: 'var(--text-primary)',
                            cursor: 'pointer'
                        }}
                    >
                        <option value="">All Status</option>
                        <option value="Pending">Pending</option>
                        <option value="InProgress">In Progress</option>
                        <option value="Done">Done</option>
                    </select>
                </div>

                {/* Owner Filter */}
                <div className="filter-group" style={{
                    display: 'flex',
                    gap: 'var(--space-xs)',
                    background: 'var(--bg-secondary)',
                    padding: 'var(--space-xs)',
                    borderRadius: 'var(--radius-md)',
                    border: '1px solid var(--border-color)'
                }}>
                    <button
                        className={`btn btn-sm ${ownerFilter === 'all' ? 'btn-primary' : 'btn-ghost'}`}
                        onClick={() => setOwnerFilter('all')}
                        style={{ border: 'none' }}
                    >
                        All Tasks
                    </button>
                    <button
                        className={`btn btn-sm ${ownerFilter === 'mine' ? 'btn-primary' : 'btn-ghost'}`}
                        onClick={() => setOwnerFilter('mine')}
                        style={{ border: 'none' }}
                    >
                        My Tasks
                    </button>
                </div>
            </div>

            {/* Tasks List - Scrollable Container */}
            <div style={{
                flex: 1,
                overflowY: 'auto',
                paddingRight: 'var(--space-xs)',
                marginBottom: 'var(--space-md)'
            }}>
                {isLoading ? (
                    <LoadingSpinner size="lg" message="Loading tasks..." />
                ) : (error || filteredTasks.length === 0) ? (
                    <div className="empty-state">
                        <svg width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1">
                            <path d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2" />
                            <rect x="9" y="3" width="6" height="4" rx="1" />
                        </svg>
                        <p>
                            {ownerFilter === 'mine'
                                ? "Você ainda não criou nenhuma tarefa."
                                : "Ainda não temos tarefa."}
                        </p>
                        {error && (
                            <button
                                className="btn btn-sm btn-ghost"
                                onClick={fetchTasks}
                                style={{ marginTop: 'var(--space-sm)', fontSize: 'var(--font-size-xs)', opacity: 0.5 }}
                            >
                                Tentar novamente
                            </button>
                        )}
                    </div>
                ) : (
                    <div className="task-list">
                        {filteredTasks.map(task => (
                            <TaskCard
                                key={task.id}
                                task={task}
                                canEdit={canEdit(task)}
                                canDelete={canDelete(task)}
                                isOwner={isTaskOwner(task)}
                                onEdit={() => handleEdit(task)}
                                onDelete={() => handleDelete(task.id)}
                            />
                        ))}
                    </div>
                )}
            </div>

            {/* Pagination Controls - Sticky */}
            {!isLoading && !error && totalCount > 0 && (
                <div className="pagination" style={{
                    position: 'sticky',
                    bottom: 0,
                    zIndex: 100,
                    display: 'flex',
                    justifyContent: 'center',
                    alignItems: 'center',
                    gap: 'var(--space-md)',
                    padding: 'var(--space-md)',
                    background: 'var(--bg-secondary)',
                    borderRadius: 'var(--radius-md)',
                    border: '1px solid var(--border-color)',
                    borderTop: '2px solid var(--border-color)',
                    marginTop: 0
                }}>
                    <button
                        className="btn btn-sm btn-ghost"
                        onClick={handlePreviousPage}
                        disabled={page === 1}
                        style={{ opacity: page === 1 ? 0.5 : 1 }}
                    >
                        ← Anterior
                    </button>

                    <span style={{ color: 'var(--text-secondary)' }}>
                        Página <strong>{page}</strong> de <strong>{totalPages}</strong>
                        {' '}({totalCount} tarefas)
                    </span>

                    <button
                        className="btn btn-sm btn-ghost"
                        onClick={handleNextPage}
                        disabled={page >= totalPages}
                        style={{ opacity: page >= totalPages ? 0.5 : 1 }}
                    >
                        Próxima →
                    </button>
                </div>
            )}

            {/* Edit Modal */}
            {editingTask && (
                <EditTaskModal
                    task={editingTask}
                    onClose={handleCloseModal}
                    onSuccess={handleEditSuccess}
                />
            )}
        </div>
    );
};
