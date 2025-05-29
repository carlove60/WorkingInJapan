using Microsoft.EntityFrameworkCore;
using WaitingList.Models;

namespace WaitingList.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<PartyModel> Parties { get; set; }
    public DbSet<WaitingListModel> WaitingLists { get; set; }
    
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
        modelBuilder.Entity<PartyModel>().ToTable("Parties");
    }

    private static void SetLimits(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyModel>().Property((u) => u.Size).IsRequired();
        modelBuilder.Entity<PartyModel>().Property((u) => u.Name).IsRequired();
        modelBuilder.Entity<WaitingListModel>().Property((u) => u.Name).IsRequired();
        modelBuilder.Entity<WaitingListModel>()
            .HasIndex(w => w.Name)
            .IsUnique();

    }

    private static void SetDefaults(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyModel>().Property((u) => u.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<WaitingListModel>().Property((u) => u.Id).ValueGeneratedOnAdd();
    }

    private static void SetOneOnOneRelations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyModel>()
            .HasOne(psc => psc.WaitingListModel);

        modelBuilder.Entity<WaitingListModel>()
            .HasMany(u => u.Parties);
    }
}