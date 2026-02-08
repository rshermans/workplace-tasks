# 📜 API Contract

This document specifies the Request/Response contract for the WorkPlace Tasks API.

**Base URL**: `http://localhost:5000/api` (or `https://localhost:5001/api`)

---

## 🔐 Authentication

### POST `/auth/login`
Authenticates a user and returns a session token.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbG...",
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "role": "Admin|Manager|Member"
  }
}
```

---

## 📋 Tasks

### GET `/tasks`
Returns all tasks visible to the current user.

**Response (200 OK):**
```json
[
  {
    "id": "uuid",
    "title": "Task Title",
    "description": "Task Description",
    "status": "Pending|InProgress|Done",
    "createdAt": "2024-02-07T...",
    "createdByEmail": "owner@example.com"
  }
]
```

### POST `/tasks`
Creates a new task.

**Request:**
```json
{
  "title": "New Task",
  "description": "Optional details",
  "status": "Pending"
}
```

### PUT `/tasks/{id}`
Updates an existing task. Requires ownership or Manager/Admin role.

**Request:**
```json
{
  "title": "Updated Title",
  "description": "Updated Description",
  "status": "InProgress"
}
```

---

## 👥 Users (Admin Only)

### GET `/users`
Returns a list of all registered users.

### PUT `/users/{id}/role`
Updates a user's role.

**Request:**
```json
{
  "role": "Manager"
}
```

---

## ⚠️ Common Error Codes

| Status Code | Code | Meaning |
| :--- | :--- | :--- |
| `400` | `VALIDATION_ERROR` | Request body is missing required fields. |
| `401` | `AUTH_INVALID` | Invalid email or password. |
| `403` | `FORBIDDEN` | You don't have permission for this resource. |
| `404` | `NOT_FOUND` | The requested item does not exist. |
