using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WaitingListBackend.Database;
using WaitingListBackend.Entities;

namespace WaitingListBackend.BackgroundServices;

/// <summary>
/// Represents a background service that periodically checks for and deletes sessions
/// that have timed out. This service identifies sessions that have been inactive
/// for a defined duration and removes the associated entities from the waiting list.
/// </summary>
/// <remarks>
/// The service operates as a hosted background process. It uses dependency injection
/// to access the necessary services, such as logging and scoped dependencies. The service runs
/// in a loop, checking for timed-out sessions at regular intervals until the application
/// shuts down or cancellation is requested.
/// </remarks>
/// <param name="logger">An instance of <see cref="ILogger"/> used for logging information about the lifecycle and operations of the service.</param>
/// <param name="scopeFactory">A factory for creating service scopes that provide access to scoped services such as the database context.</param>
public class DeleteTimedOutSessionsBackgroundService(
    ILogger<EnsureBackgroundExistsBackgroundService> logger,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    /// <summary>
    /// Executes the background service logic that monitors and deletes timed-out sessions
    /// from the waiting list. This method runs continuously until the cancellation token is triggered.
    /// </summary>
    /// <param name="stoppingToken">
    /// A <see cref="CancellationToken"/> that is triggered when the background service is stopped.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the execution of the background service.
    /// </returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var backgroundServiceName = nameof(DeleteTimedOutSessionsBackgroundService);
        logger.LogInformation($"{backgroundServiceName} started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            logger.LogInformation("Checking for default waiting list...");
            var partiesOnWaitingList = dbContext.Parties.Where(party => party.ServiceStartedAt == null && party.CreatedOn.AddMinutes(Constants.TimeoutInMinutes) < DateTime.Now);
            
            var timedOutParties = new List<PartyEntity>();
            foreach (var party in partiesOnWaitingList)
            {
                timedOutParties.Add(party);
            }

            if (timedOutParties.Any())
            {
                logger.LogInformation($"Removing {timedOutParties.Count()} parties from waiting list.");
                dbContext.RemoveRange(timedOutParties);
                var result = dbContext.SaveChanges();
                logger.LogInformation($"Removed {result} parties from waiting list.");
            }

            // Run every second
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }

        logger.LogInformation($"{backgroundServiceName} stopping.");
    }
}