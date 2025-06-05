using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WaitingList.Database.Database;

/// <summary>
/// Factory class for creating instances of the <see cref="ApplicationDbContext"/> class at design time.
/// This class implements the <see cref="IDesignTimeDbContextFactory{TContext}"/> interface to allow
/// Entity Framework Core tools to create the database context when executing migrations or other design-time tasks.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Creates a new instance of the <see cref="ApplicationDbContext"/> class using the provided arguments.
    /// This method is typically used by Entity Framework Core tools to configure and instantiate the
    /// database context at design time or runtime.
    /// </summary>
    /// <param name="args">
    /// An array of command-line arguments. These arguments can be used to dynamically configure
    /// the database connection or other settings during the creation of the context.
    /// </param>
    /// <returns>
    /// Returns a new instance of the <see cref="ApplicationDbContext"/> initialized with the
    /// appropriate configuration and options.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("MySqlConnection");
        
        optionsBuilder.UseMySQL((string)(connectionString ?? throw new InvalidOperationException(
            "Connection string 'MySqlConnection' not found.")));
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}