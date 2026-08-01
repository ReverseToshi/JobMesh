using Microsoft.EntityFrameworkCore;
using JobMesh.Api.Models;

namespace JobMesh.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs => Set<Job>();

    public DbSet<User> Users => Set<User>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);


    modelBuilder.Entity<Job>(entity =>
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id);

        entity.Property(e => e.UserId).IsRequired();
        entity.Property(e => e.Type).IsRequired();
        entity.Property(e => e.Status).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
    });

    modelBuilder.Entity<User>(entity =>
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id);

        entity.Property(e => e.Username).IsRequired();
        entity.Property(e => e.PasswordHash).IsRequired();
        entity.Property(e => e.Role)
            .IsRequired()
            .HasDefaultValue("User");
    });
}

}