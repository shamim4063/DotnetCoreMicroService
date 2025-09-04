using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Menus;
using UserManagement.Domain;

namespace UserManagement.Infrastructure.Menus;

internal sealed class MenuReader : IMenuReader
{
    private readonly UserDbContext _db;
    public MenuReader(UserDbContext db) => _db = db;

    public async Task<MenuDto?> ById(long id, CancellationToken ct)
    {
        return await _db.Menus
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new MenuDto(m.Id, m.Key, m.DisplayName, m.ParentId, m.SortOrder, m.Route))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<MenuDto>> List(int skip, int take, CancellationToken ct)
    {
        return await _db.Menus
            .AsNoTracking()
            .OrderBy(m => m.SortOrder).ThenBy(m => m.Id)
            .Skip(skip)
            .Take(take)
            .Select(m => new MenuDto(m.Id, m.Key, m.DisplayName, m.ParentId, m.SortOrder, m.Route))
            .ToListAsync(ct);
    }
}

internal sealed class MenusWriter : IMenusWriter
{
    private readonly UserDbContext _db;
    public MenusWriter(UserDbContext db) => _db = db;

    public async Task<long> Add(string key, string displayName, string route, long? parentId, int sortOrder, CancellationToken ct)
    {
        // basic invariants
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key is required", nameof(key));
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("DisplayName is required", nameof(displayName));
        if (string.IsNullOrWhiteSpace(route)) throw new ArgumentException("Route is required", nameof(route));

        var entity = new Menu(key, displayName, route, parentId, sortOrder);
        _db.Menus.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task Update(long id, string key, string displayName, string route, long? parentId, int sortOrder, CancellationToken ct)
    {
        var entity = await _db.Menus.FirstOrDefaultAsync(m => m.Id == id, ct);
        if (entity is null) throw new ArgumentException($"Menu {id} not found");

        // update via reflection-safe approach since setters are private; use EF Core tracked entity with property setters
        // Use EF to set values
        _db.Entry(entity).CurrentValues.SetValues(new { Key = key, DisplayName = displayName, Route = route, ParentId = parentId, SortOrder = sortOrder });
        await _db.SaveChangesAsync(ct);
    }

    public async Task Delete(long id, CancellationToken ct)
    {
        var entity = await _db.Menus.FirstOrDefaultAsync(m => m.Id == id, ct);
        if (entity is null) throw new ArgumentException($"Menu {id} not found");

        _db.Menus.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}
