using System;

namespace UserManagement.Domain;

// (Role ? Menu ? Action)
public sealed class Permission
{
    public long RoleId { get; private set; }
    public long MenuId { get; private set; }
    public PermittedActionKeyEnum ActionKeyEnum { get; private set; }
    public bool Allowed { get; private set; }

    // Optional navigation
    public Role? Role { get; private set; }
    public Menu? Menu { get; private set; }
    public PermittedAction? Action { get; private set; }

    private Permission() { }

    public Permission(long roleId, long menuId, PermittedActionKeyEnum actionKeyEnum, bool allowed)
    {
        RoleId = roleId;
        MenuId = menuId;
        ActionKeyEnum = actionKeyEnum;
        Allowed = allowed;
    }
}
