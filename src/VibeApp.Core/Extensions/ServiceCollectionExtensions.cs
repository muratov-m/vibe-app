using Microsoft.Extensions.DependencyInjection;
using VibeApp.Core.Interfaces;
using VibeApp.Core.Services;

namespace VibeApp.Core.Extensions;

/// <summary>
/// Extension methods for registering core layer services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Register business logic services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IUserProfileEmbeddingService, UserProfileEmbeddingService>();

        return services;
    }
}

