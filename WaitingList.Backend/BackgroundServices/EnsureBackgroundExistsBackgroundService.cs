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
    private readonly int _timeForService = 3;
    private readonly int _totalSeatsAvailable = 10;


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var backgroundServiceName = nameof(EnsureBackgroundExistsBackgroundService);
        logger.LogInformation($"{backgroundServiceName} running.");

        if (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            logger.LogInformation($"Doing background work...");
            var result =
                dbContext.WaitingLists.FirstOrDefault((wl) => wl.Name == Constants.DefaultWaitingListName);
            if (result != null)
            {
                return Task.CompletedTask;
            }

            logger.LogInformation($"Creating WaitingList.Api: {Constants.DefaultWaitingListName}");
            var waitingList = new WaitingListEntity
            {
                Name = Constants.DefaultWaitingListName, TimeForService = _timeForService,
                TotalSeats = _totalSeatsAvailable
            };
            dbContext.Add(waitingList);
            dbContext.SaveChanges();
            logger.LogInformation($"{backgroundServiceName} stopping.");
        }
        return Task.CompletedTask;
    }
}