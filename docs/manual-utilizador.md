# User Manual & Wireframes

This document details the expected user experience and interface behavior.

## Application Flow

### Screen 0 — Login
The entry point for all users.
*   **Header**: `[ WorkPlace Tasks ]`
*   **Input**: `Email` (e.g., admin@example.com)
*   **Input**: `Password` (e.g., Password123!)
*   **Action**: `(Login)` Button
*   **Feedback**: Display error message on 401 (Invalid credentials).

### Screen 1 — Tasks Dashboard (Main)
The central hub for task management.

#### Header
*   Title: `WorkPlace Tasks`
*   Role Indicator: `Role: [Admin|Manager|Member]`
*   Action: `(Logout)`

#### Left Panel (Create Task)
A persistent form for quick task creation.
*   **Title**: Text Input
*   **Description**: Text Area
*   **Status**: Dropdown (`Pending` [Default] | `InProgress` | `Done`)
*   **Action**: `(Create Task)`
    *   **Loading**: Disable button + spinner.
    *   **Success**: Clear form + refresh list.
    *   **Error**: Show toast message.

#### Right Panel (Task List)
A list of all tasks visible to the user.
*   **Filters** (Bonus): `Status: All | Pending | InProgress | Done`
*   **Task Card**:
    *   **Content**: Title | Status Badge | Created At | Created By
    *   **Actions**:
        *   `(Edit)`: Visible if user has `Update` permission on this task.
        *   `(Delete)`: Visible if user has `Delete` permission on this task.

### Modal — Edit Task
Appears when clicking `(Edit)`.
*   **Title**: Text Input (Editable)
*   **Description**: Text Area (Editable)
*   **Status**: Dropdown (Editable)
*   **Actions**:
    *   `(Save)`: Calls `PUT /tasks/{id}`.
    *   `(Cancel)`: Closes modal without saving.

## UX Principles
*   **Minimal & Clear**: No unnecessary elements. Focus on the core actions.
*   **Visual RBAC**: Users should only see buttons they can click.
*   **Feedback Loop**: Always show loading states and meaningful error messages.
