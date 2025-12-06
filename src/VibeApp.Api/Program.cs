using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VibeApp.Core.Extensions;
using VibeApp.Data;
using VibeApp.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle circular references in JSON serialization
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();

// Configure Antiforgery for production (Render.com HTTPS)
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax; // Allows POST from same origin
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Works on HTTP (dev) and HTTPS (prod)
    options.Cookie.HttpOnly = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VibeApp API", Version = "v1" });
    
    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Data layer services (includes DbContext)
builder.Services.AddDataServices(builder.Configuration);

// Add Authentication scheme explicitly for API controllers
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
});

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
{
    // Password settings - simplified for demo/hackathon
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    
    // User settings
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add Core layer services (business logic)
builder.Services.AddCoreServices();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    
    // Cookie settings for production compatibility
    options.Cookie.SameSite = SameSiteMode.Lax; // Allows cookies on form POST
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Works on HTTP (dev) and HTTPS (prod)
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    
    // Handle API requests differently - return 401 instead of redirect
    options.Events.OnRedirectToLogin = context =>
    {
        // If this is an API request (starts with /api/), return 401 instead of redirecting
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
        
        // For Razor Pages, do the default redirect
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
    
    options.Events.OnRedirectToAccessDenied = context =>
    {
        // If this is an API request, return 403 instead of redirecting
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
        
        // For Razor Pages, do the default redirect
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

// Add health checks
builder.Services.AddHealthChecks();

// Add CORS for frontend development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowFrontend");
}

app.UseHttpsRedirection();

// Serve default files (index.html)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapHealthChecks("/health");

// SPA fallback - serve Vue app for all non-API routes
app.MapFallbackToFile("index.html");

// Apply migrations on startup (for render.com)
if (!app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetService<AppDbContext>();
    if (db != null)
    {
        try
        {
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}

app.Run();

