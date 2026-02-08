# API Error Codes & Handling

All API errors follow a consistent envelope structure to simplify frontend handling and debugging.

## Error Response Format

```json
{
  "code": "string_error_code",
  "message": "Human readable message",
  "details": [ "validation_error_1", "validation_error_2" ],
  "traceId": "unique_request_id"
}
```

## Common Error Codes

| HTTP Status | Code | Meaning |
| :--- | :--- | :--- |
| **400 Bad Request** | `VALIDATION_ERROR` | Request body failed DTO validation rules. |
| **401 Unauthorized** | `AUTH_MISSING` | No JWT token provided in `Authorization` header. |
| **401 Unauthorized** | `AUTH_INVALID` | Token is expired or signature is invalid. |
| **403 Forbidden** | `ACCESS_DENIED` | User role does not have permission for this resource. |
| **404 Not Found** | `RESOURCE_NOT_FOUND` | The requested ID does not exist. |
| **500 Internal Server Error** | `INTERNAL_ERROR` | An unexpected server-side exception occurred. Check logs with `traceId`. |

## Handling Strategy (Frontend)

1.  **Interceptor**: Catch all 401/403 responses globally. Redirect to login on 401. Show toast on 403.
2.  **Form Errors**: Map `details` array to form fields for user feedback.
3.  **Generic**: Show `message` in a toast for other 400/500 errors.
