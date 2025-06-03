using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WaitingList.Database.Database;
using WaitingListBackend.Entities;

namespace WaitingList.BackgroundServices.BackgroundServices;

/// <summary>
/// Represents a background service responsible for ensuring the existence
/// of a default waiting list in the application's database. This service
/// runs periodically to check for and create the default waiting list if
/// it does not exist.
/// </summary>
public class EnsureBackgroundExistsBackgroundService(
    ILogger<EnsureBackgroundExistsBackgroundService> logger,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    /// <summary>
    /// Executes the background service operation. Periodically ensures the
    /// existence of a default waiting list in the application's database.
    /// If the list does not exist, it creates a new default waiting list.
    /// Otherwise, it clears the existing list of parties.
    /// </summary>
    /// <param name="stoppingToken">A token used to signal cancellation of the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var backgroundServiceName = nameof(EnsureBackgroundExistsBackgroundService);
        logger.LogInformation($"{backgroundServiceName} started.");

        if (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                logger.LogInformation("Checking for default waiting list...");
                var waitingListEntity = dbContext.WaitingLists.Include((wL) => wL.Parties)
                    .FirstOrDefault(wl => wl.Name == Constants.DefaultWaitingListName);

                if (waitingListEntity == null)
                {
                    logger.LogInformation($"Creating WaitingList: {Constants.DefaultWaitingListName}");
                    waitingListEntity = new WaitingListEntity
                    {
                        Name = Constants.DefaultWaitingListName,
                        TimeForService = Constants.TimeForServicePerPerson,
                        TotalSeats = Constants.TotalSeatsPerWaitingList
                    };

                    dbContext.Add(waitingListEntity);
                }
                else
                {
                    dbContext.RemoveRange(waitingListEntity.Parties);
                }

                dbContext.SaveChanges();
            }
            catch (Exception exception)
            {
                logger.LogError(backgroundServiceName, exception);           
            }

            // Run every 3 seconds
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }

        logger.LogInformation($"{backgroundServiceName} stopping.");
    }
}