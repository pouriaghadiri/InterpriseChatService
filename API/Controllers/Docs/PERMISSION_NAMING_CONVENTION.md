# Permission Naming Convention

## Format Structure

```
PERM_[Entity*]_[SubEntity]_[Action*]_[Scope*+]_[Role*+]
```

### Legend
- `*` = **Required**
- `*+` = **One of them is required** (either Scope OR Role, but not both)
- `_` = Separator between components

---

## Component Rules

### 1. Entity* (Required)
- **Primary resource/entity** being acted upon
- **PascalCase**, singular form
- **Examples**: `User`, `Role`, `Department`, `Permission`, `Profile`, `Password`

### 2. SubEntity (Optional)
- **Secondary entity** when action involves a relationship
- **PascalCase**, singular form
- **Examples**: `Role` (in `User_Role_Assign`), `Permission` (in `Role_Permission_Assign`)
- **Omit** if action is direct on the entity

### 3. Action* (Required)
- **Operation verb** describing what is being done
- **PascalCase**
- **Standard CRUD**: `Create`, `View`, `Update`, `Delete`
- **Special Actions**: `Assign`, `Approve`, `Change`, `Register`, `Logout`

### 4. Scope*+ (One of Scope or Role Required)
- **Ownership/access scope** for the action
- **PascalCase**
- **Common Values**: `My`, `All`, `Own`, `Department`
- **Use when**: Action is about ownership or data access scope

### 5. Role*+ (One of Scope or Role Required)
- **Role-based restriction** for the action
- **PascalCase**
- **Common Values**: `Admin`, `Manager`, `User`, `Guest`
- **Use when**: Action requires specific role regardless of ownership

---

## Examples

### Pattern 1: Entity + Action + Role
```
PERM_Role_Create_Admin
```
- **Entity**: `Role`
- **Action**: `Create`
- **Role**: `Admin`
- **Meaning**: Only Admin can create roles

### Pattern 2: Entity + SubEntity + Action + Role
```
PERM_User_Role_Assign_Admin
```
- **Entity**: `User`
- **SubEntity**: `Role`
- **Action**: `Assign`
- **Role**: `Admin`
- **Meaning**: Admin assigns roles to users

### Pattern 3: Entity + Action + Scope
```
PERM_Department_View_My
```
- **Entity**: `Department`
- **Action**: `View`
- **Scope**: `My`
- **Meaning**: View own department

### Pattern 4: Entity + Action + Scope (All)
```
PERM_User_View_All
```
- **Entity**: `User`
- **Action**: `View`
- **Scope**: `All`
- **Meaning**: View all users

### Pattern 5: Simple Action (Special Case)
```
PERM_Logout
```
- **Special case**: Simple action without entity/scope/role
- **Meaning**: Logout action

---

## Common Permission Patterns

### User Management
```csharp
PERM_User_Create_Admin              // Admin creates user
PERM_User_View_All                  // View all users
PERM_User_View_My                   // View own profile
PERM_User_Update_All                // Admin updates any user
PERM_User_Update_My                 // User updates own profile
PERM_User_Delete_Admin              // Admin deletes user
PERM_User_Role_Assign_Admin         // Admin assigns role to user
PERM_User_Role_Remove_Admin         // Admin removes role from user
PERM_User_Department_Assign_Admin   // Admin assigns department
```

### Profile Management
```csharp
PERM_Profile_View_My                // View own profile
PERM_Profile_Update_My              // Update own profile
PERM_Profile_View_All               // Admin views all profiles
PERM_Profile_Update_All             // Admin updates any profile
```

### Password Management
```csharp
PERM_Password_Change_My             // Change own password
PERM_Password_Reset_Admin           // Admin resets user password
PERM_Password_Change_All             // Admin changes any password
```

### Department Management
```csharp
PERM_Department_Create_Admin        // Admin creates department
PERM_Department_View_All            // View all departments
PERM_Department_View_My              // View own department
PERM_Department_Update_Admin        // Admin updates department
PERM_Department_Delete_Admin        // Admin deletes department
PERM_Department_User_View_All        // View all users in department
```

### Role Management
```csharp
PERM_Role_Create_Admin              // Admin creates role
PERM_Role_View_All                 // View all roles
PERM_Role_Update_Admin              // Admin updates role
PERM_Role_Delete_Admin             // Admin deletes role
PERM_Role_Permission_Assign_Admin   // Admin assigns permission to role
PERM_Role_Permission_Remove_Admin   // Admin removes permission from role
```

### Permission Management
```csharp
PERM_Permission_Create_Admin        // Admin creates permission
PERM_Permission_View_All           // View all permissions
PERM_Permission_Update_Admin        // Admin updates permission
PERM_Permission_Delete_Admin        // Admin deletes permission
```

### Authentication
```csharp
PERM_Logout                         // Logout (simple action)
```

---

## Decision Rules

### When to Use Scope vs Role

**Use Scope when:**
- Action is about ownership (`My`, `Own`)
- Action applies to all resources (`All`)
- Access is based on data ownership

**Use Role when:**
- Action requires specific role (`Admin`, `Manager`)
- Action is role-restricted regardless of ownership

### When to Include SubEntity

**Include SubEntity when:**
- Action involves relationship between entities
- Examples: `User_Role_Assign`, `Role_Permission_Assign`, `Department_User_View`

**Omit SubEntity when:**
- Action is direct on the entity
- Examples: `User_Create`, `Role_View`, `Department_Update`

---

## Validation Rules

1. ✅ **Entity** is always required
2. ✅ **Action** is always required
3. ✅ **Either Scope OR Role** is required (at least one, not both)
4. ✅ **SubEntity** is optional
5. ✅ Use **PascalCase** throughout
6. ✅ Keep names **concise and clear**

---

## Quick Reference Template

```
PERM_[Entity*]_[SubEntity]_[Action*]_[Scope*+ OR Role*+]
```

### Common Scope Values
- `My` - Own resource
- `All` - All resources
- `Own` - Same as My
- `Department` - Department-scoped

### Common Role Values
- `Admin` - Administrator
- `Manager` - Manager
- `User` - Regular user
- `Guest` - Guest user

### Common Actions
- `Create`, `View`, `Update`, `Delete` (CRUD)
- `Assign`, `Remove`, `Approve`, `Reject`
- `Change`, `Reset`, `Register`, `Logout`

---

## Current Policy Mappings

| Current Policy | Recommended Format | Notes |
|----------------|-------------------|-------|
| `PERM_Register_User` | `PERM_User_Create_Admin` | Admin creates users |
| `PERM_Change_Password` | `PERM_Password_Change_My` | User changes own password |
| `PERM_Logout` | `PERM_Logout` | Keep as-is (simple action) |
| `PERM_Review_My_Profile` | `PERM_Profile_View_My` | View own profile |
| `PERM_Update_My_Profile` | `PERM_Profile_Update_My` | Update own profile |
| `PERM_Department_All_View` | `PERM_Department_View_All` | View all departments |

---

## Notes

- **Policy names are case-sensitive** - always use PascalCase
- **Permission names in database** should match policy names (without `PERM_` prefix)
- **One permission per policy** - each policy maps to one permission check
- **Be consistent** - use the same pattern for similar operations

