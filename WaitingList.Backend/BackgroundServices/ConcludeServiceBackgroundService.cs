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


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var backgroundServiceName = nameof(ConcludeServiceBackgroundService);
        logger.LogInformation($"{backgroundServiceName} running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            logger.LogInformation($"Doing background work...");
            var result =
                dbContext.WaitingLists.Select((x) => x);

            foreach (var waitingList in result.Include(waitingListEntity => waitingListEntity.Parties))
            {
                var parties = waitingList.Parties.Where((p) => p.ServiceStartedAt != null && p.ServiceEndedAt == null);
                foreach (var party in parties)
                {
                    logger.LogInformation($"Checking if service has been concluded for party {party.Name} on waiting list {waitingList.Name}");
                    var timeOfService = party.Size * Constants.TimeForServicePerPerson;
                    if (party.ServiceStartedAt?.AddSeconds(timeOfService) < DateTime.Now)
                    {
                        party.ServiceEndedAt = DateTime.Now;
                    }
                }

            }
            
            dbContext.SaveChanges();
            logger.LogInformation($"{backgroundServiceName} stopping.");
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }
}