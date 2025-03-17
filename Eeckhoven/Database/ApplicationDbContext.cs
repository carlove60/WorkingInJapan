using Eeckhoven.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Eeckhoven.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public new DbSet<UserModel> Users { get; set; }
    public DbSet<ResumeModel> Resumes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        CreateTables(modelBuilder);
        SetOneOnOneRelations(modelBuilder);
        SetLimits(modelBuilder);
        SetDefaults(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
    
    private static void CreateTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>().ToTable("Users");
        modelBuilder.Entity<ResumeModel>().ToTable("Resumes");
    }

    private static void SetLimits(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>().Property((u) => u.Nationality).HasMaxLength(100);
        modelBuilder.Entity<UserModel>().Property((u) => u.FirstName).HasMaxLength(100);
        modelBuilder.Entity<UserModel>().Property((u) => u.LastName).HasMaxLength(100);
        modelBuilder.Entity<UserModel>().Property((u) => u.PhoneNumber).HasMaxLength(16);
        modelBuilder.Entity<UserModel>().Property((u) => u.ProfilePicture).HasMaxLength(256);
        modelBuilder.Entity<UserModel>().Property((u) => u.Role).HasMaxLength(20);
    }

    private static void SetDefaults(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>().Property((u) => u.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<ResumeModel>().Property((u) => u.Id).ValueGeneratedOnAdd();
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