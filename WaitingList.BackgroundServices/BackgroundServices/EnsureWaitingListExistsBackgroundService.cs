using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WaitingList.Database.Database;
using WaitingList.Database.Entities;

namespace WaitingList.BackgroundServices.BackgroundServices;

/// <summary>
/// Represents a background service responsible for ensuring the existence
/// of a default waiting list in the application's database. This service
/// runs periodically to check for and create the default waiting list if
/// it does not exist.
/// </summary>
public class EnsureWaitingListExistsBackgroundService(
    ILogger<EnsureWaitingListExistsBackgroundService> logger,
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
        var backgroundServiceName = nameof(EnsureWaitingListExistsBackgroundService);
        logger.LogInformation($"{backgroundServiceName} started.");
        WaitingListEntity? waitingListEntity = null;
        while (!stoppingToken.IsCancellationRequested && waitingListEntity == null)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                waitingListEntity = dbContext.WaitingLists.Include((wl) => wl.Parties).AsNoTracking()
                    .FirstOrDefault(wl => wl.Name == Constants.DefaultWaitingListName);
                
                    logger.LogInformation("Checking whether default waiting list exists...");
                    if (waitingListEntity == null)
                    {
                        logger.LogInformation($"Creating WaitingList: {Constants.DefaultWaitingListName}");
                        waitingListEntity = new WaitingListEntity
                        {
                            Name = Constants.DefaultWaitingListName,
                            TimeForService = Constants.TimeForServicePerPerson,
                            TotalSeats = Constants.TotalSeatsPerWaitingList,
                        };
                        logger.LogInformation($"WaitingList: {Constants.DefaultWaitingListName} created.");
                        dbContext.Add(waitingListEntity);
                    }
                    else
                    {
                        dbContext.RemoveRange(waitingListEntity.Parties);
                        logger.LogInformation($"WaitingList: {waitingListEntity.Name} cleared.");
                        ;
                    }

                    dbContext.SaveChanges();
            }
            catch (Exception exception)
            {
                logger.LogInformation(backgroundServiceName, exception.Message);
            }
        }

        // Run every 3 seconds
        logger.LogInformation($"{backgroundServiceName} stopping.");
        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
    }
}