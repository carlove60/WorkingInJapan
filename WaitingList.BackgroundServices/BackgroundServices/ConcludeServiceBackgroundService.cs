using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WaitingList.Database.Database;

namespace WaitingList.BackgroundServices.BackgroundServices;

/// <summary>
/// Background service responsible for processing and finalizing the parties.
/// This service continuously polls the application database for pending tasks related
/// to waiting list entities, marks the required changes, and saves updates to the database.
/// </summary>
public class ConcludeServiceBackgroundService(
    ILogger<ConcludeServiceBackgroundService> logger,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    /// <summary>
    /// Executes the background task for concluding services for parties.
    /// This method performs continuous polling and updates the application database accordingly,
    /// while taking cancellation requests into account.
    /// </summary>
    /// <param name="stoppingToken">A token that signals the request to stop the background service.</param>
    /// <returns>A task that represents the asynchronous execution of the service.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var backgroundServiceName = nameof(ConcludeServiceBackgroundService);
        logger.LogInformation($"{backgroundServiceName} running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                logger.LogInformation($"Doing background work...");
                var result =
                    dbContext.WaitingLists.Select((x) => x);

                foreach (var waitingList in result.Include(waitingListEntity => waitingListEntity.Parties))
                {
                    var parties =
                        waitingList.Parties.Where((p) => p.ServiceStartedAt != null && p.ServiceEndedAt == null);
                    foreach (var party in parties)
                    {
                        logger.LogInformation(
                            $"Checking if service has been concluded for party {party.Name} on waiting list {waitingList.Name}");
                        var timeOfService = party.Size * Constants.TimeForServicePerPerson;
                        if (party.ServiceStartedAt?.AddSeconds(timeOfService) < DateTime.Now)
                        {
                            party.ServiceEndedAt = DateTime.Now;
                        }
                    }

                }

                dbContext.SaveChanges();
                logger.LogInformation($"{backgroundServiceName} stopping.");
            }
            catch (Exception exception)
            {
                logger.LogError(backgroundServiceName, exception);
            }

            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
        
        logger.LogInformation($"{backgroundServiceName} stopping.");
    }
}