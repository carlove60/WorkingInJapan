using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
        SetRelations(modelBuilder);
        SetLimits(modelBuilder);
        SetDefaults(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
    
    private static void CreateTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyEntity>().ToTable("Parties");
        modelBuilder.Entity<WaitingListEntity>().ToTable("WaitingLists");
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
        modelBuilder.Entity<PartyEntity>().Property((u) => u.CheckedIn).HasDefaultValue("false");
        modelBuilder.Entity<PartyEntity>().Property((u) => u.CreatedOn).HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<WaitingListEntity>().Property((u) => u.Id).ValueGeneratedOnAdd();
    }

    private static void SetRelations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyEntity>()
            .HasOne(p => p.WaitingListEntity)
            .WithMany(w => w.Parties)
            .HasForeignKey(p => p.WaitingListId);
    }
}