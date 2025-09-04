using Microsoft.Extensions.DependencyInjection;
using UserManagement.Application.Abstractions;
using UserManagement.Application.Menus;
using UserManagement.Infrastructure.Menus;

namespace UserManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUserManagementInfrastructure(this IServiceCollection services)
    {
        // Application abstractions -> EF Core implementations
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IMenuReader, MenuReader>();
        services.AddScoped<IMenusWriter, MenusWriter>();
        // If you introduce outbox/consumers, register them here as well.
        return services;
    }
}
