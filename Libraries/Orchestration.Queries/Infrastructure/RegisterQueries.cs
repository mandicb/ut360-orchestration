
using Microsoft.Extensions.DependencyInjection;

namespace Orchestration.Queries.Infrastructure;

public static class RegisterQueries
{
    public static IServiceCollection RegisterQueryHandlers(
        this IServiceCollection services)
    {
        return services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterQueries).Assembly));
    }
}