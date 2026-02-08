import http from './http';
import { TaskItem } from '../types';

export interface PagedResponse<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

export interface TaskQueryParams {
    page?: number;
    pageSize?: number;
    status?: string;
}

export const taskApi = {
    getAll: () => http.get<TaskItem[]>('/tasks').then(res => res.data),

    getPaged: (params: TaskQueryParams = {}) => {
        const queryParams = new URLSearchParams();
        if (params.page) queryParams.append('page', params.page.toString());
        if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString());
        if (params.status) queryParams.append('status', params.status);

        return http.get<PagedResponse<TaskItem>>(`/tasks/paged?${queryParams.toString()}`).then(res => res.data);
    },

    getById: (id: string) => http.get<TaskItem>(`/tasks/${id}`).then(res => res.data),

    create: (task: Partial<TaskItem>) => http.post<TaskItem>('/tasks', task).then(res => res.data),

    update: (id: string, task: Partial<TaskItem>) => http.put<TaskItem>(`/tasks/${id}`, task).then(res => res.data),

    delete: (id: string) => http.delete(`/tasks/${id}`),
};

export const userApi = {
    getAll: () => http.get('/users').then(res => res.data),

    create: (data: { email: string; password: string; role: string }) =>
        http.post('/users', data).then(res => res.data),

    updateRole: (id: string, role: string) => http.put(`/users/${id}/role`, { role }).then(res => res.data),

    delete: (id: string) => http.delete(`/users/${id}`),
};

