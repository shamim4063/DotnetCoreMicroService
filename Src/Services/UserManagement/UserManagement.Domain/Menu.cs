using System;
using System.Collections.Generic;

namespace UserManagement.Domain;

public sealed class Menu
{
    public long Id { get; private set; }
    public string Key { get; private set; } = default!; // e.g., SALES_ORDER
    public string DisplayName { get; private set; } = default!;
    public long? ParentId { get; private set; }
    public int SortOrder { get; private set; }
    public string Route { get; private set; } = default!;

    // Self-referencing hierarchy navigation
    public Menu? Parent { get; private set; }
    public ICollection<Menu> Children { get; private set; } = new List<Menu>();

    public ICollection<PermittedAction> Actions { get; private set; } = new List<PermittedAction>();

    private Menu() { }

    public Menu(string key, string displayName, string route, long? parentId = null, int sortOrder = 0)
    {
        Key = key;
        DisplayName = displayName;
        Route = route;
        ParentId = parentId;
        SortOrder = sortOrder;
    }
}
