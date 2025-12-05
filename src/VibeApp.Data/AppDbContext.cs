using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VibeApp.Core.Entities;

namespace VibeApp.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<UserLookingFor> UserLookingFors => Set<UserLookingFor>();
    public DbSet<UserCustomArray1> UserCustomArray1s => Set<UserCustomArray1>();
    public DbSet<UserCustomArray2> UserCustomArray2s => Set<UserCustomArray2>();
    public DbSet<UserCustomArray3> UserCustomArray3s => Set<UserCustomArray3>();
    public DbSet<UserCustomArray4> UserCustomArray4s => Set<UserCustomArray4>();
    public DbSet<UserCustomArray5> UserCustomArray5s => Set<UserCustomArray5>();
    public DbSet<UserCustomArray6> UserCustomArray6s => Set<UserCustomArray6>();
    public DbSet<UserCustomArray7> UserCustomArray7s => Set<UserCustomArray7>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure UserProfile
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Telegram).HasMaxLength(100);
            entity.Property(e => e.LinkedIn).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Photo).HasMaxLength(500);
            entity.Property(e => e.StartupStage).HasMaxLength(100);
            entity.Property(e => e.StartupName).HasMaxLength(200);
            
            entity.HasMany(e => e.Skills)
                .WithOne(s => s.UserProfile)
                .HasForeignKey(s => s.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.LookingFor)
                .WithOne(l => l.UserProfile)
                .HasForeignKey(l => l.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.CustomArray1)
                .WithOne(c => c.UserProfile)
                .HasForeignKey(c => c.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.CustomArray2)
                .WithOne(c => c.UserProfile)
                .HasForeignKey(c => c.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.CustomArray3)
                .WithOne(c => c.UserProfile)
                .HasForeignKey(c => c.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.CustomArray4)
                .WithOne(c => c.UserProfile)
                .HasForeignKey(c => c.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.CustomArray5)
                .WithOne(c => c.UserProfile)
                .HasForeignKey(c => c.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.CustomArray6)
                .WithOne(c => c.UserProfile)
                .HasForeignKey(c => c.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.CustomArray7)
                .WithOne(c => c.UserProfile)
                .HasForeignKey(c => c.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Configure UserSkill
        modelBuilder.Entity<UserSkill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Skill).HasMaxLength(200).IsRequired();
        });
        
        // Configure UserLookingFor
        modelBuilder.Entity<UserLookingFor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LookingFor).HasMaxLength(200).IsRequired();
        });
        
        // Configure custom arrays
        ConfigureCustomArray<UserCustomArray1>(modelBuilder);
        ConfigureCustomArray<UserCustomArray2>(modelBuilder);
        ConfigureCustomArray<UserCustomArray3>(modelBuilder);
        ConfigureCustomArray<UserCustomArray4>(modelBuilder);
        ConfigureCustomArray<UserCustomArray5>(modelBuilder);
        ConfigureCustomArray<UserCustomArray6>(modelBuilder);
        ConfigureCustomArray<UserCustomArray7>(modelBuilder);
    }
    
    private void ConfigureCustomArray<T>(ModelBuilder modelBuilder) where T : class, IEntity
    {
        modelBuilder.Entity<T>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property("Value").HasMaxLength(500);
        });
    }
}

