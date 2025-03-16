using Eeckhoven.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Eeckhoven.Database;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public new DbSet<UserModel> Users { get; set; }
    public DbSet<ResumeModel> Resumes { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        CreateTables(modelBuilder);
        SetOneOnOneRelations(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);


    }

    private static void CreateTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>().ToTable("Users");
        modelBuilder.Entity<ResumeModel>().ToTable("Resumes");
    }

    private static void SetOneOnOneRelations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>()
            .HasOne(psc => psc.Resume);
        
        modelBuilder.Entity<UserModel>()
            .HasOne(psc => psc.IdentityUser);

        modelBuilder.Entity<ResumeModel>()
            .HasOne(psc => psc.User);
    }
}