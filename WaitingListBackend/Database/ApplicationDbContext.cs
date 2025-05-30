using Microsoft.EntityFrameworkCore;
using WaitingListBackend.Entities;

namespace WaitingListBackend.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<PartyEntity> Parties { get; set; }
    public DbSet<WaitingListEntity> WaitingLists { get; set; }
    
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
        modelBuilder.Entity<PartyEntity>().ToTable("Parties");
    }

    private static void SetLimits(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyEntity>().Property((u) => u.Size).IsRequired();
        modelBuilder.Entity<PartyEntity>().Property((u) => u.Name).IsRequired();
        modelBuilder.Entity<WaitingListEntity>().Property((u) => u.Name).IsRequired();
        modelBuilder.Entity<WaitingListEntity>()
            .HasIndex(w => w.Name)
            .IsUnique();

    }

    private static void SetDefaults(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyEntity>().Property((u) => u.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<WaitingListEntity>().Property((u) => u.Id).ValueGeneratedOnAdd();
    }

    private static void SetOneOnOneRelations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyEntity>()
            .HasOne(psc => psc.WaitingListEntity);

        modelBuilder.Entity<WaitingListEntity>()
            .HasMany(u => u.Parties);
    }
}