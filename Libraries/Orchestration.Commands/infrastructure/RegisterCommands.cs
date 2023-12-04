
using Microsoft.Extensions.DependencyInjection;

namespace Orchestration.Commands.infrastructure;

public static class RegisterCommands
{
    public static IServiceCollection RegisterCommandHandlers(
        this IServiceCollection services)
    {
        return services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterCommands).Assembly));
    }
}