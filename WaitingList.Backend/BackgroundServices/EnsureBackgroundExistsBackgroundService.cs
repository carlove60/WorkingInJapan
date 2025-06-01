using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WaitingListBackend.Database;
using WaitingListBackend.Entities;

namespace WaitingListBackend.BackgroundServices;

public class EnsureBackgroundExistsBackgroundService(
    ILogger<EnsureBackgroundExistsBackgroundService> logger,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var backgroundServiceName = nameof(EnsureBackgroundExistsBackgroundService);
        logger.LogInformation($"{backgroundServiceName} started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            logger.LogInformation("Checking for default waiting list...");
            var result = dbContext.WaitingLists.FirstOrDefault(wl => wl.Name == Constants.DefaultWaitingListName);

            if (result == null)
            {
                logger.LogInformation($"Creating WaitingList: {Constants.DefaultWaitingListName}");
                var waitingList = new WaitingListEntity
                {
                    Name = Constants.DefaultWaitingListName,
                    TimeForService = Constants.TimeForServicePerPerson,
                    TotalSeats = Constants.TotalSeatsPerWaitingList
                };

                dbContext.Add(waitingList);
                dbContext.SaveChanges();
            }

            // Run every 3 seconds
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }

        logger.LogInformation($"{backgroundServiceName} stopping.");
    }
}