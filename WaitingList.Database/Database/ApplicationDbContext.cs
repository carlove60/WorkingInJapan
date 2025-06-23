using Microsoft.EntityFrameworkCore;
using WaitingList.Database.Entities;

namespace WaitingList.Database.Database;

/// <summary>
/// Represents the database context for the application. This class is responsible for managing
/// the database interactions, including defining the entity sets, configuring model relationships,
/// and applying constraints and defaults. It extends the Entity Framework Core DbContext class.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    /// <summary>
    /// Represents the database set containing <see cref="PartyEntity"/> objects.
    /// This property provides access to the Parties table in the database.
    /// </summary>
    /// <remarks>
    /// This property is used to perform CRUD operations on the Parties data and is typically
    /// accessed through the Entity Framework Core DbContext.
    /// </remarks>
    public DbSet<PartyEntity> Parties { get; set; }

    /// <summary>
    /// Represents the database set containing <see cref="WaitingListEntity"/> objects.
    /// This property provides access to the WaitingLists table in the database.
    /// </summary>
    /// <remarks>
    /// This property is used for performing CRUD operations on the WaitingLists data and is commonly
    /// accessed through the Entity Framework Core DbContext in the application.
    /// </remarks>
    public DbSet<WaitingListEntity> WaitingLists { get; set; }

    /// <summary>
    /// Configures the model relationships, tables, properties, constraints, and defaults for the database context.
    /// This method is called when the model for a derived context is being created. The resulting model may be cached
    /// and re-used for subsequent instances of the context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for the database context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        CreateTables(modelBuilder);
        SetRelations(modelBuilder);
        SetLimits(modelBuilder);
        SetDefaults(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Maps the entity types to their respective database tables in the model.
    /// This method is responsible for defining the table names for the entities within the database.
    /// Done for naming
    /// </summary>
    /// <param name="modelBuilder">The builder used to configure and map the entity types to database tables.</param>
    private static void CreateTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyEntity>().ToTable("Parties");
        modelBuilder.Entity<WaitingListEntity>().ToTable("WaitingLists");
    }

    /// <summary>
    /// Configures the property constraints and limitations for the entities in the database model.
    /// This method ensures required fields and unique constraints are properly set for the database schema.
    /// </summary>
    /// <param name="modelBuilder">The model builder used for defining the entity framework model configuration.</param>
    private static void SetLimits(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyEntity>().Property((u) => u.Size).IsRequired();
        modelBuilder.Entity<PartyEntity>().Property((u) => u.Name).IsRequired();
        modelBuilder.Entity<WaitingListEntity>().Property((u) => u.Name).IsRequired();
        modelBuilder.Entity<WaitingListEntity>()
            .HasIndex(w => w.Name)
            .IsUnique();

    }

    /// <summary>
    /// Configures the default values and behaviors for entities within the database context.
    /// This includes setting default values, constraints, and other default behaviors for specific properties
    /// of the entities in the model.
    /// </summary>
    /// <param name="modelBuilder">The builder used to define and configure entity sets and relationships within the database context.</param>
    private static void SetDefaults(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyEntity>().Property((u) => u.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<PartyEntity>().Property((u) => u.CheckedIn).HasDefaultValue("false");
        modelBuilder.Entity<PartyEntity>().Property((u) => u.CreatedOn).HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        modelBuilder.Entity<WaitingListEntity>().Property((u) => u.Id).ValueGeneratedOnAdd();
    }

    /// <summary>
    /// Configures the relationships between entities in the database model, including defining foreign key constraints
    /// and navigation properties. This method is used to establish associations between entities as part of the
    /// model configuration process.
    /// </summary>
    /// <param name="modelBuilder">The builder used to define the relationships in the entity model.</param>
    private static void SetRelations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyEntity>()
            .HasOne(p => p.WaitingListEntity)
            .WithMany(w => w.Parties)
            .HasForeignKey(p => p.WaitingListId);
    }
}