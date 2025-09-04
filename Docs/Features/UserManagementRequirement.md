# User Management Module — Requirements & Domain Model

## 1. Overview & Scope
The User Management module is responsible for managing users, their roles, and permissions within the ERP system. It enforces security and provides granular, menu-wise action permissions. This module does **not** manage warehouses or warehouse mappings directly, but may maintain a **projection table** for user-warehouse relationships for query and scoping purposes. The module must align with the Clean Architecture and microservices boundaries described in the project documentation.

---

## 2. Functional Requirements

### 2.1 User Management
- **Create User**: Add a new user with core profile information.
- **Edit User**: Update user details, including profile and roles.
- **Activate/Deactivate User**: Enable or disable user access.
- **Reset Password**: Allow admin to reset a user's password.
- **Force MFA**: Enable or require multi-factor authentication for a user.
- **Set Locale/Timezone**: Configure user locale and timezone preferences.

### 2.2 Role Management
- **View Roles**: List all roles (system and custom).
- **Create Custom Role**: Admins can define new roles with custom permissions.
- **Edit Role**: Update role details and permissions.
- **Assign/Unassign Roles to Users**: Map one or more roles to a user, with optional validity windows.
- **System Roles**: Predefined, non-editable roles (e.g., Sales Director, Account Manager, etc.).
- **Commission Roles**: Mark roles as commission-eligible and link to commission items (if applicable).

### 2.3 Permission & Access Control
- **Menu-wise, Action-level Permissions**: Assign permissions per role, per menu, per action (e.g., Sales Order ▸ Create/Edit/Delete).
- **Permission Matrix UI**: Admins can manage permissions in a matrix view.
- **Permission Enforcement**: Hide or disable UI actions the user lacks permission for; enforce on API level.
- **Permission Feedback**: Show clear feedback if an action is blocked (e.g., missing EDIT on Sales Orders).

### 2.4 User-Warehouse Projection Table
- **Purpose**: Maintain a denormalized, read-optimized projection of user-warehouse relationships for scoping and query purposes.
- **Source**: Populated and updated via integration events/messages from the Warehouse microservice.
- **No Foreign Keys**: No direct foreign key constraints to the warehouse table; only store warehouse IDs and metadata as received.
- **Fields**:
  - **UserId** (Guid)
  - **WarehouseId** (Guid)
  - **IsDefault** (bool, only one true per user)
  - **WarehouseCode** (string, as received)
  - **WarehouseName** (string, as received)
  - **Status** (string, as received)
  - **OtherMetadata** (jsonb, optional)
- **Usage**: Used for filtering, scoping, and permission checks in this service; not a source of truth for warehouse data.

### 2.5 UX & API Requirements
- **Admin ▸ Users**: CRUD, role mapping, password/MFA management.
- **Admin ▸ Roles**: View system roles, manage custom roles, assign permissions.
- **Health & Audit**: Log all user/role/permission changes for audit.

---

## 3. Domain Model Structure

### 3.1 Entities & Relationships

#### User
```
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!; // unique
    public string? Phone { get; set; }
    public UserStatus Status { get; set; } // Active/Disabled
    public string? PasswordHash { get; set; } = null!;
    public DateTime? PasswordCreatedAt { get; set; } = null!;
    public DateTime? PasswordLastChangedAt { get; set; } = null!;
    public bool MfaEnabled { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public enum UserStatus
{
    Active,
    Disabled
}
```
#### Role
```
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!; // unique
    public RoleType Type { get; set; } // System/Custom
    public string? Description { get; set; }
    public bool IsCommissionRole { get; set; }
    public Guid? CommissionItemId { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}

public enum RoleType
{
    System,
    Custom
}
```

#### UserRole (Many-to-Many: User ↔ Role)
```
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime? EffectiveFrom { get; set; }
}
```
#### Menu
```
public class Menu
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!; // e.g., SALES_ORDER
    public string DisplayName { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
    public string Route { get; set; } = null!;
    public ICollection<Action> Actions { get; set; } = new List<Action>();
}
```
#### Action
```
public class PermittedAction
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!; // e.g., CREATE, EDIT
    public string DisplayName { get; set; } = null!;
}
```
#### Permission 
```
(Role ▸ Menu ▸ Action)public class Permission
{
    public Guid RoleId { get; set; }
    public Guid MenuId { get; set; }
    public Guid ActionId { get; set; }
    public bool Allowed { get; set; }
}
```
#### UserWarehouseProjection (Projection Table)
```
public class UserWarehouseProjection
{
    public Guid UserId { get; set; }
    public Guid WarehouseId { get; set; }
    public bool IsDefault { get; set; }
    public string WarehouseCode { get; set; } = null!;
    public string WarehouseName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? OtherMetadata { get; set; } // JSONB or similar
}
```
---

## 4. Non-Functional & Architectural Requirements
- **Layered Architecture**: Follow Clean Architecture boundaries (Domain, Application, Infrastructure, API).
- **CQRS**: Use MediatR for commands/queries in Application layer.
- **EF Core**: Each service has its own schema and migrations; no cross-schema FKs.
- **Security**: Enforce permissions at API and UI layers.
- **Auditability**: All changes to users, roles, permissions must be logged.
- **Extensibility**: Support for future custom attributes via JSONB if needed.
- **Projection Table**: Must be updated via integration events/messages from the Warehouse service.

---

## 5. Example Use Cases
- **User logs in**: System checks status, password, MFA, and loads effective roles/permissions.
- **Admin creates a new user**: Fills profile, assigns roles.
- **Admin creates a custom role**: Defines permissions matrix, marks as commission-eligible if needed.
- **User attempts unauthorized action**: System blocks and provides clear feedback.
- **Warehouse mapping changes in Warehouse service**: Projection table is updated via event to reflect new user-warehouse relationships.

---

## 6. Open Questions / To Be Clarified
- CommissionItem entity structure and integration (if not already defined).
- Cross-service user identity (if SSO or external auth is planned).
- Audit log implementation details (centralized or per service).
- Event contract and update strategy for user-warehouse projection table.

---

*This document is the authoritative requirements and domain model specification for the User Management module. All implementation, API, and database design must conform to this specification and the project’s architectural guidelines.*
