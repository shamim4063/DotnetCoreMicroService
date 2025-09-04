namespace UserManagement.Application.Menus;

public sealed record MenuDto(
    long Id,
    string Key,
    string DisplayName,
    long? ParentId,
    int SortOrder,
    string Route);
