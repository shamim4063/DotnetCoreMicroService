using System;
using System.Collections.Generic;

namespace UserManagement.Domain;

public sealed class Role
{
    public long Id { get; private set; }
    public string Name { get; private set; } = default!;
    public RoleTypeEnum Type { get; private set; } = RoleTypeEnum.Custom;
    public string? Description { get; private set; }
    public bool IsCommissionRole { get; private set; }
    public long? CommissionItemId { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<Permission> Permissions { get; private set; } = new List<Permission>();

    private Role() { }

    public Role(string name, RoleTypeEnum type = RoleTypeEnum.Custom, string? description = null, bool isCommissionRole = false, long? commissionItemId = null)
    {
        Name = name;
        Type = type;
        Description = description;
        IsCommissionRole = isCommissionRole;
        CommissionItemId = commissionItemId;
    }
}
