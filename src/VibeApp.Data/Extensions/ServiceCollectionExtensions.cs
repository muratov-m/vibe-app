using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VibeApp.Core.Interfaces;
using VibeApp.Data.Repositories;

namespace VibeApp.Data.Extensions;

/// <summary>
/// Extension methods for registering data layer services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configure PostgreSQL
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? Environment.GetEnvironmentVariable("DATABASE_URL");

        if (!string.IsNullOrEmpty(connectionString))
        {
            // Parse render.com DATABASE_URL format if needed
            if (connectionString.StartsWith("postgresql://") || connectionString.StartsWith("postgres://"))
            {
                try
                {
                    connectionString = ConvertPostgresUrl(connectionString);
                }
                catch (Exception ex)
                {
                    // Log error but continue - will fail later with more details
                    Console.WriteLine($"Error parsing DATABASE_URL: {ex.Message}");
                    Console.WriteLine($"DATABASE_URL format should be: postgresql://user:password@host:port/database");
                    throw new InvalidOperationException(
                        "Invalid DATABASE_URL format. Expected format: postgresql://user:password@host:port/database. " +
                        "In Render.com, use the 'Internal Database URL' not 'External Database URL'.", ex);
                }
            }
            
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }

    private static string ConvertPostgresUrl(string postgresUrl)
    {
        var uri = new Uri(postgresUrl);
        
        // Validate that we have all required components
        if (string.IsNullOrEmpty(uri.Host))
        {
            throw new ArgumentException("DATABASE_URL is missing hostname. Use Internal Database URL from Render.");
        }
        
        if (uri.Port <= 0)
        {
            throw new ArgumentException("DATABASE_URL is missing port. Use Internal Database URL from Render.");
        }
        
        if (string.IsNullOrEmpty(uri.UserInfo))
        {
            throw new ArgumentException("DATABASE_URL is missing user credentials.");
        }
        
        var userInfo = uri.UserInfo.Split(':');
        if (userInfo.Length != 2)
        {
            throw new ArgumentException("DATABASE_URL has invalid user credentials format.");
        }
        
        var database = uri.AbsolutePath.Trim('/');
        if (string.IsNullOrEmpty(database))
        {
            throw new ArgumentException("DATABASE_URL is missing database name.");
        }
        
        Console.WriteLine($"Parsed DATABASE_URL - Host: {uri.Host}, Port: {uri.Port}, Database: {database}, User: {userInfo[0]}");
        
        return $"Host={uri.Host};Port={uri.Port};Database={database};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
    }
}


