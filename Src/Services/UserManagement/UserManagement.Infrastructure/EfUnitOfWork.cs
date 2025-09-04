using UserManagement.Application.Abstractions;

namespace UserManagement.Infrastructure;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly UserDbContext _db;
    public EfUnitOfWork(UserDbContext db) => _db = db;

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        _db.SaveChangesAsync(ct);
}
