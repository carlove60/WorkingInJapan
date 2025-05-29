using WaitingList.Database;
using WaitingList.Models;

namespace WaitingList.BackgroundServices;

public class EnsureBackgroundExistsBackgroundService : BackgroundService
{
    private readonly ILogger<EnsureBackgroundExistsBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly int _timeForService = 3;
    private readonly int _totalSeatsAvailable = 10;


    public EnsureBackgroundExistsBackgroundService(ILogger<EnsureBackgroundExistsBackgroundService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var backgroundServiceName = nameof(EnsureBackgroundExistsBackgroundService);
        _logger.LogInformation($"{backgroundServiceName} running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (dbContext == null)
            {
                _logger.LogInformation($"Unexpected null: {nameof(dbContext)}");
                return Task.CompletedTask;
            }

            _logger.LogInformation($"Doing background work...");
            var result =
                dbContext.WaitingLists.FirstOrDefault((wl) => wl.Name == Constants.DefaultWaitingListName);
            if (result != null)
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation($"Creating WaitingList: {Constants.DefaultWaitingListName}");
            var waitingList = new WaitingListModel
            {
                Name = Constants.DefaultWaitingListName, TimeForService = _timeForService,
                TotalSeatsAvailable = _totalSeatsAvailable
            };
            dbContext.Add(waitingList);
            dbContext.SaveChanges();
            _logger.LogInformation($"{backgroundServiceName} stopping.");
        }
        return Task.CompletedTask;
    }
}