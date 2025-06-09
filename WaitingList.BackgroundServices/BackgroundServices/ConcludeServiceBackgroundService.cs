using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WaitingList.Database.Database;
using WaitingList.SseManager.Managers;
using WaitingListBackend.Extensions;
using WaitingListBackend.Interfaces;

namespace WaitingList.BackgroundServices.BackgroundServices;

/// <summary>
/// Background service responsible for processing and finalizing the parties.
/// This service continuously polls the application database for pending tasks related
/// to waiting list entities, marks the required changes, and saves updates to the database.
/// </summary>
public class ConcludeServiceBackgroundService(
    ILogger<ConcludeServiceBackgroundService> logger,
    IServiceScopeFactory scopeFactory, 
    SseChannelManager sseChannelManager)
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
                var waitingListService = scope.ServiceProvider.GetRequiredService<IWaitingListService>();
                var sseMessageManager = new SseMessageManager(sseChannelManager, logger);
                await ConcludeService(sseMessageManager, dbContext, waitingListService, logger, stoppingToken);
                await NotifyNextPartyForCheckIn(sseMessageManager, dbContext, logger, sseChannelManager, stoppingToken, waitingListService);
                sseMessageManager.SendMessagesAndClear();

            }
            catch (Exception exception)
            {
                logger.LogError(backgroundServiceName, exception);
            }

            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
        
        logger.LogInformation($"{backgroundServiceName} stopping.");
    }

    private async Task NotifyNextPartyForCheckIn(SseMessageManager sseMessageManager, ApplicationDbContext dbContext, ILogger<ConcludeServiceBackgroundService> logger, SseChannelManager sseChannelManager, CancellationToken stoppingToken, IWaitingListService waitingListService)
    {
        var waitingLists = dbContext.WaitingLists
            .Include((wL) => wL.Parties
                .Where((p) => p.CheckedIn == false)).ToList();
        foreach (var waitingList in waitingLists) 
        { 
            var waitingListDto = waitingListService.GetWaitingList(waitingList.Name).Records.First();
            var nextParty = waitingListDto.NextPartyToCheckIn;
            if (nextParty == null)
            {
                continue;
            }
                    
            if (nextParty.CanCheckIn) 
            { 
                sseMessageManager.AddParty(nextParty);
                logger.LogInformation($"{nextParty.Name} added to waiting list.");
            }
        }
                
        await dbContext.SaveChangesAsync(stoppingToken);
        sseMessageManager.SendMessagesAndClear();
    }

    private async Task ConcludeService(SseMessageManager sseMessageManager, ApplicationDbContext dbContext, IWaitingListService waitingListService, ILogger<ConcludeServiceBackgroundService> logger, CancellationToken stoppingToken)
    {
        logger.LogInformation($"Doing background work...");
        var result =
            dbContext.WaitingLists.Select((x) => x)
                .Include(waitingListEntity => waitingListEntity
                    .Parties.Where((p) => p.ServiceEndedAt == null && p.ServiceStartedAt != null)).ToList();
        foreach (var waitingList in result)
        {
            var parties = waitingList.Parties;
            foreach (var party in parties)
            {
                logger.LogInformation(
                    $"Checking if service has been concluded for party {party.Name} on waiting list {waitingList.Name}");
                var timeOfService = party.Size * Constants.TimeForServicePerPerson;
                if (party.ServiceStartedAt?.AddSeconds(timeOfService) < DateTime.Now)
                {
                    party.ServiceEndedAt = DateTime.Now;
                    sseMessageManager.AddParty(party.ToDto());
                }
            }
            var updatedWaitingList = waitingListService.GetWaitingList(waitingList.Name).Records.First();
            sseMessageManager.AddWaitingList(updatedWaitingList);
        }
        await dbContext.SaveChangesAsync(stoppingToken);
        logger.LogInformation($"Conclude services done.");
    }
}