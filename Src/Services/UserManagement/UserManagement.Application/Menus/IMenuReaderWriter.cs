using System.Threading;
using System.Threading.Tasks;

namespace UserManagement.Application.Menus;

public interface IMenuReader
{
    Task<MenuDto?> ById(long id, CancellationToken ct);
    Task<IReadOnlyList<MenuDto>> List(int skip, int take, CancellationToken ct);
}

public interface IMenusWriter
{
    Task<long> Add(string key, string displayName, string route, long? parentId, int sortOrder, CancellationToken ct);
    Task Update(long id, string key, string displayName, string route, long? parentId, int sortOrder, CancellationToken ct);
    Task Delete(long id, CancellationToken ct);
}
