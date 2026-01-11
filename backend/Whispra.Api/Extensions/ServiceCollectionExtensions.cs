using Whispra.Application.Interfaces.Repositories;
using Whispra.Application.Interfaces.Services;
using Whispra.Application.UseCases.Users.Register;
using Whispra.Infrastructure.Configuration;
using Whispra.Infrastructure.Persistence.MongoDB;
using Whispra.Infrastructure.Persistence.Repositories;
using Whispra.Infrastructure.Services;

namespace Whispra.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register use cases
        services.AddScoped<RegisterUserUseCase>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure MongoDB
        services.Configure<MongoDbSettings>(
            configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton<MongoDbContext>();

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Register services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }
}