using Microsoft.Extensions.DependencyInjection;
using VibeApp.Core.Gateways;
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
        // Register gateways
        services.AddSingleton<IOpenAIGateway, OpenAIGateway>();

        // Register business logic services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IUserProfileParsingService, UserProfileParsingService>();
        services.AddScoped<IUserProfileEmbeddingService, UserProfileEmbeddingService>();
        services.AddScoped<IUserMatchingEmbeddingService, UserMatchingEmbeddingService>();
        services.AddScoped<IRagSearchService, RagSearchService>();
        services.AddScoped<IEmbeddingQueueService, EmbeddingQueueService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IMatchSummaryService, MatchSummaryService>();

        // Register background services
        services.AddHostedService<EmbeddingProcessingService>();

        return services;
    }
}

