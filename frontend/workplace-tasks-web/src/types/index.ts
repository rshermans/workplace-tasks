export interface User {
    email: string;
    role: 'Admin' | 'Manager' | 'Member';
}

export interface TaskItem {
    id: string;
    title: string;
    description: string;
    status: 'Pending' | 'InProgress' | 'Done';
    createdAt: string;
    createdByUserId: string;
    createdByEmail: string;
}

export interface LoginResponse {
    token: string;
    role: string;
    email: string;
}
