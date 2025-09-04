using System;

namespace UserManagement.Domain;

public sealed class UserRole
{
    public long UserId { get; private set; }
    public long RoleId { get; private set; }
    public DateTime? EffectiveFrom { get; private set; }

    // Optional navigation properties for EF Core
    public User? User { get; private set; }
    public Role? Role { get; private set; }

    private UserRole() { }

    public UserRole(long userId, long roleId, DateTime? effectiveFrom = null)
    {
        UserId = userId;
        RoleId = roleId;
        EffectiveFrom = effectiveFrom;
    }
}
