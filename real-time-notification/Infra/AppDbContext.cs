using System;
using Microsoft.EntityFrameworkCore;
using real_time_notification.Domain.Entities;

namespace real_time_notification.Infra;

/// <summary>
/// Represents the database context for the application, providing access to the database
/// with entities such as Users, Monthly Income, and Monthly Spending.
/// </summary>
/// <remarks>
/// Configures database mappings and relationships for the application's entities.
/// Uses Entity Framework Core for ORM functionality and ensures proper
/// constraints, relationships, and property configurations for the entities specified.
/// </remarks>
public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Users = Set<User>();
    }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Password).IsRequired();
        });
    }

}
