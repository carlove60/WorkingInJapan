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
/// A background service responsible for notifying the next party in the queue for check-in.
/// This service periodically executes logic to determine the next party that can check-in,
/// leveraging dependencies to manage notifications via server-sent events and party-specific operations.
/// </summary>
public class NotifyNextPartyForCheckInBackgroundServer(
    ILogger<NotifyNextPartyForCheckInBackgroundServer> logger,
    IServiceScopeFactory scopeFactory,
    SseChannelManager sseChannelManager)
    : BackgroundService 
{
    /// <summary>
    /// Executes the background service logic for notifying the next party in the queue for check-in.
    /// This method runs continuously until the operation is canceled, performing notification tasks
    /// and updating relevant data in the database at regular intervals.
    /// </summary>
    /// <param name="stoppingToken">A token that indicates when the background operation should stop execution.</param>
    /// <returns>A task that represents the asynchronous execution of the background service operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var backgroundServiceName = nameof(NotifyNextPartyForCheckInBackgroundServer);
        logger.LogInformation($"{backgroundServiceName} running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation($"Doing background work...");
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var waitingLists = dbContext.WaitingLists
                    .Include((wL) => wL.Parties
                        .Where((p) => p.ServiceStartedAt == null));
                var sseMessageManager = new SseMessageManager(sseChannelManager, logger);
                var partyService = scope.ServiceProvider.GetRequiredService<IPartyService>();
                foreach (var waitingList in waitingLists) 
                { 
                    var nextParty = partyService.GetNextPartyToCheckIn(waitingList.Parties);
                    if (nextParty == null)
                    {
                        continue;
                    }
                    
                    var partyCanCheckInResult = partyService.CanCheckIn(nextParty.SessionId); 
                    var canCheckIn = partyCanCheckInResult.Records.First(); 
                    if (canCheckIn) 
                    { 
                        var nextPartyDto = nextParty.ToDto(); 
                        nextPartyDto.CanCheckIn = canCheckIn; 
                        sseMessageManager.AddParty(nextParty.ToDto());
                        logger.LogInformation($"{nextPartyDto.Name} messaged.");
                    }
                    break;
                }
                
                await dbContext.SaveChangesAsync(stoppingToken);
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
}