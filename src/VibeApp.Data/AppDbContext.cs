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
    }
}

