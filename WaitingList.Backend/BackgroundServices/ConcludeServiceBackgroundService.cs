using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WaitingListBackend.Database;

namespace WaitingListBackend.BackgroundServices;

public class ConcludeServiceBackgroundService(
    
    ILogger<ConcludeServiceBackgroundService> logger,
    IServiceScopeFactory scopeFactory)
    : BackgroundService
{


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var backgroundServiceName = nameof(ConcludeServiceBackgroundService);
        logger.LogInformation($"{backgroundServiceName} running.");

        if (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            logger.LogInformation($"Doing background work...");
            var result =
                dbContext.WaitingLists.Select((x) => x);
            if (!result.Any())
            {
                return Task.CompletedTask;
            }

            foreach (var waitingList in result.Include(waitingListEntity => waitingListEntity.Parties))
            {
                var parties = waitingList.Parties.Where((p) => p.ServiceStartedAt != null && p.ServiceEndedAt == null);
                foreach (var party in parties)
                {
                    logger.LogInformation($"Checking if service has been concluded for party {party.Name} on waiting list {waitingList.Name}");
                    var timeOfService = party.Size * Constants.TimeForServicePerPerson;
                    if (timeOfService > Constants.TotalSeatsPerWaitingList)
                    {
                        party.ServiceEndedAt = DateTime.Now;
                    }
                }

            }
            
            dbContext.SaveChanges();
            logger.LogInformation($"{backgroundServiceName} stopping.");
        }
        return Task.CompletedTask;
    }
}