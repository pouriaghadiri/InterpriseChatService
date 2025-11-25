# Authorization Policy Update Summary

This document summarizes all authorization policy updates applied to controllers based on the permission naming convention.

## Naming Convention Format

```
PERM_[Entity*]_[SubEntity]_[Action*]_[Scope*+]_[Role*+]
```

- `*` = Required
- `*+` = One of them is required (either Scope OR Role, but not both)

---

## Updated Controllers

### 1. AuthController

| Endpoint | Old Policy | New Policy | Notes |
|----------|------------|------------|-------|
| `POST /register` | `PERM_Register_User` | `PERM_User_Create_Admin` | Admin creates users |
| `PUT /change-password` | `[Authorize]` (generic) | `PERM_Password_Change_My` | User changes own password |
| `POST /logout` | `[Authorize]` (generic) | `PERM_Logout` | Logout action |
| `GET /profile` | `PERM_Review_My_Profile` | `PERM_Profile_View_My` | View own profile |
| `PUT /profile` | `PERM_Update_My_Profile` | `PERM_Profile_Update_My` | Update own profile |

**Endpoints with `[AllowAnonymous]` (unchanged):**
- `POST /login` - Public login
- `POST /refresh-token` - Public token refresh
- `POST /forgot-password` - Public password reset request
- `POST /reset-password` - Public password reset
- `POST /verify-email` - Public email verification
- `POST /resend-verification` - Public verification resend

---

### 2. UserController

| Endpoint | Old Policy | New Policy | Notes |
|----------|------------|------------|-------|
| `PUT /update-profile` | None | `PERM_Profile_Update_My` | Update own profile |
| `GET /by-email` | None | `PERM_User_View_All` | View any user by email |

---

### 3. DepartmentController

| Endpoint | Old Policy | New Policy | Notes |
|----------|------------|------------|-------|
| `POST /create` | None | `PERM_Department_Create_Admin` | Admin creates department |
| `GET /{id}` | None | `PERM_Department_View_All` | View any department |
| `GET /all` | `PERM_Department_All_View` | `PERM_Department_View_All` | View all departments (fixed format) |
| `PUT /update` | None | `PERM_Department_Update_Admin` | Admin updates department |
| `DELETE /{id}` | None | `PERM_Department_Delete_Admin` | Admin deletes department |

---

### 4. RoleController

| Endpoint | Old Policy | New Policy | Notes |
|----------|------------|------------|-------|
| `POST /create` | None | `PERM_Role_Create_Admin` | Admin creates role |
| `GET /{id}` | None | `PERM_Role_View_All` | View any role |
| `GET /all` | None | `PERM_Role_View_All` | View all roles |
| `PUT /update` | None | `PERM_Role_Update_Admin` | Admin updates role |
| `DELETE /{id}` | None | `PERM_Role_Delete_Admin` | Admin deletes role |

---

### 5. PermissionController

| Endpoint | Old Policy | New Policy | Notes |
|----------|------------|------------|-------|
| `POST /create` | None | `PERM_Permission_Create_Admin` | Admin creates permission |
| `GET /{id}` | None | `PERM_Permission_View_All` | View any permission |
| `GET /all` | None | `PERM_Permission_View_All` | View all permissions |
| `PUT /update` | None | `PERM_Permission_Update_Admin` | Admin updates permission |
| `DELETE /{id}` | None | `PERM_Permission_Delete_Admin` | Admin deletes permission |
| `POST /assign-to-role` | None | `PERM_Role_Permission_Assign_Admin` | Admin assigns permission to role |
| `POST /assign-to-user` | None | `PERM_User_Permission_Assign_Admin` | Admin assigns permission to user |
| `GET /role/{roleId}/{departmentId}` | None | `PERM_Role_Permission_View_All` | View role permissions |
| `GET /user/{userId}/{departmentId}` | None | `PERM_User_Permission_View_All` | View user permissions |

---

### 6. TestController

No changes - Test controller remains public (no authorization required).

---

## Policy Categories

### Authentication & Profile Policies
- `PERM_User_Create_Admin` - Admin creates users
- `PERM_Password_Change_My` - User changes own password
- `PERM_Logout` - Logout action
- `PERM_Profile_View_My` - View own profile
- `PERM_Profile_Update_My` - Update own profile

### User Management Policies
- `PERM_User_View_All` - View any user

### Department Management Policies
- `PERM_Department_Create_Admin` - Admin creates department
- `PERM_Department_View_All` - View all departments
- `PERM_Department_Update_Admin` - Admin updates department
- `PERM_Department_Delete_Admin` - Admin deletes department

### Role Management Policies
- `PERM_Role_Create_Admin` - Admin creates role
- `PERM_Role_View_All` - View all roles
- `PERM_Role_Update_Admin` - Admin updates role
- `PERM_Role_Delete_Admin` - Admin deletes role

### Permission Management Policies
- `PERM_Permission_Create_Admin` - Admin creates permission
- `PERM_Permission_View_All` - View all permissions
- `PERM_Permission_Update_Admin` - Admin updates permission
- `PERM_Permission_Delete_Admin` - Admin deletes permission

### Permission Assignment Policies
- `PERM_Role_Permission_Assign_Admin` - Admin assigns permission to role
- `PERM_User_Permission_Assign_Admin` - Admin assigns permission to user
- `PERM_Role_Permission_View_All` - View role permissions
- `PERM_User_Permission_View_All` - View user permissions

---

## Next Steps

1. **Update Permission Database**: Ensure all these policies exist in the database with matching permission names (without `PERM_` prefix).

2. **Update Permission Seeding**: If you have seed data, update it to include all new policies.

3. **Update Role Assignments**: Assign appropriate permissions to roles (Admin, Manager, etc.).

4. **Testing**: Test all endpoints to ensure authorization works correctly.

5. **Documentation**: Update API documentation to reflect the new permission requirements.

---

## Notes

- All policies follow the naming convention: `PERM_[Entity]_[Action]_[Scope/Role]`
- Policies with `_Admin` suffix require Admin role
- Policies with `_My` suffix allow users to access their own resources
- Policies with `_All` suffix allow viewing all resources
- SubEntity is used when actions involve relationships (e.g., `Role_Permission_Assign`)

---

## Migration Checklist

- [x] Updated AuthController policies
- [x] Updated UserController policies
- [x] Updated DepartmentController policies
- [x] Updated RoleController policies
- [x] Updated PermissionController policies
- [ ] Update permission database records
- [ ] Update permission seed data
- [ ] Assign permissions to roles
- [ ] Test all endpoints
- [ ] Update API documentation

