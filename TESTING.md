# 🧪 Testing Strategy

Ensuring the reliability of our Role-Based Access Control (RBAC) and core business logic is a top priority. This document outlines our testing approach.

## 🏗️ Test Pyramid

Our strategy focuses on high-impact areas:

1.  **Unit Tests (Security)**: Full coverage of the `TaskAuthorizationPolicy`. We test every combination of **User Role** (Admin, Manager, Member) + **Ownership** (Owner vs. Non-Owner).
2.  **Integration Tests (API)**: Validating the full HTTP cycle. We ensure endpoints correctly return:
    *   `401 Unauthorized` (Missing/Invalid token)
    *   `403 Forbidden` (Insufficient permissions)
    *   `201 Created` / `200 OK` (Success paths)

---

## 🏃 How to Run Tests

### Backend
Backend tests are written in **xUnit**.

```bash
cd backend/WorkplaceTasks.Tests
dotnet test
```

### Coverage Targets
| Category | Target | Status |
| :--- | :--- | :--- |
| **Policies (RBAC)** | 100% | ✅ Essential |
| **Services** | 85%+ | 📈 High |
| **Controllers** | 70%+ | 🆗 Moderate |

---

## 🛡️ Key Scenarios Tested

### 🔐 RBAC Enforcement
*   **Admin**: Verified to have "God Mode" (Can View, Edit, and Delete ANY task).
*   **Manager**: Verified to view/edit all, but can ONLY delete tasks they created.
*   **Member**: Verified to have isolated workspace (Can ONLY view/edit/delete their own tasks).

### ⚓ Robustness
*   **Not Found**: Proper `404` handling for deleted or non-existent GUIDs.
*   **Validation**: Data annotations ensure `400 Bad Request` if titles are empty or too long.
*   **Self-Management**: Admins are prevented from deleting their own accounts.

---

## 🔮 Future Roadmap
*   **Playwright**: Automated E2E flows mirroring the user manual.
*   **Load Testing**: using `k6` to verify connection pooling under stress.
