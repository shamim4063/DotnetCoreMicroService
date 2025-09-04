using System;
using System.Collections.Generic;

namespace UserManagement.Domain;

public sealed class User
{
    public long Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Phone { get; private set; }
    public UserStatusEnum Status { get; private set; } = UserStatusEnum.Active;
    public string? PasswordHash { get; private set; }
    public DateTime? PasswordCreatedAt { get; private set; }
    public DateTime? PasswordLastChangedAt { get; private set; }
    public bool MfaEnabled { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    private User() { }

    public User(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public void Activate() => Status = UserStatusEnum.Active;
    public void Deactivate() => Status = UserStatusEnum.Disabled;

    public void EnableMfa() => MfaEnabled = true;
    public void DisableMfa() => MfaEnabled = false;

    public void SetPassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        var now = DateTime.UtcNow;
        PasswordLastChangedAt = now;
        PasswordCreatedAt ??= now;
    }
}
