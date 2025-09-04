using System;

namespace UserManagement.Domain;

public sealed class PermittedAction
{
    public long Id { get; private set; }
    public PermittedActionKeyEnum KeyEnum { get; private set; } // e.g., CREATE, EDIT
    public string DisplayName { get; private set; } = default!;

    private PermittedAction() { }

    public PermittedAction(PermittedActionKeyEnum keyEnum, string displayName)
    {
        KeyEnum = keyEnum;
        DisplayName = displayName;
    }
}
