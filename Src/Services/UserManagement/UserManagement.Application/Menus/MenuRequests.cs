using MediatR;

namespace UserManagement.Application.Menus;

public sealed record GetMenuById(long Id) : IRequest<MenuDto?>;
public sealed record ListMenus(int Skip, int Take) : IRequest<IReadOnlyList<MenuDto>>;
public sealed record CreateMenu(string Key, string DisplayName, string Route, long? ParentId, int SortOrder) : IRequest<long>;
public sealed record UpdateMenu(long Id, string Key, string DisplayName, string Route, long? ParentId, int SortOrder) : IRequest;
public sealed record DeleteMenu(long Id) : IRequest;
