# RBAC Test Matrix

This document serves as the single source of truth for Role-Based Access Control verification.

## Definitions

*   **Own**: The task's `CreatedBy` field matches the current user's ID.
*   **Other**: The task's `CreatedBy` field does NOT match the current user's ID.

## Permission Table

| Action | API Endpoint | Admin | Manager | Member |
| :--- | :--- | :--- | :--- | :--- |
| **List Tasks** | `GET /tasks` | ✅ All | ✅ All | ✅ All (Read Only) |
| **Create Task**| `POST /tasks` | ✅ | ✅ | ✅ |
| **View Details**| `GET /tasks/{id}` | ✅ | ✅ | ✅ |
| **Edit Task** | `PUT /tasks/{id}` | ✅ Any | ✅ Any | ✅ **Own Only** <br> ❌ Other (403) |
| **Delete Task**| `DELETE /tasks/{id}` | ✅ Any | ✅ **Own Only** <br> ❌ Other (403) | ❌ **Forbidden** (403) |

## Test Cases (Integration)

### 1. Admin Power
- [ ] Admin creates task A.
- [ ] Admin deletes task B (created by Manager). -> **204 OK**
- [ ] Admin edits task C (created by Member). -> **200 OK**

### 2. Manager Limits
- [ ] Manager creates task M.
- [ ] Manager deletes task M. -> **204 OK**
- [ ] Manager tries to delete task A (created by Admin). -> **403 Forbidden**
- [ ] Manager edits task C (created by Member). -> **200 OK**

### 3. Member Restrictions
- [ ] Member creates task U.
- [ ] Member updates task U status. -> **200 OK**
- [ ] Member tries to update task M (created by Manager). -> **403 Forbidden**
- [ ] Member tries to delete task U. -> **403 Forbidden** (Members cannot delete, ever).
