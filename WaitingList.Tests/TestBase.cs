using Microsoft.EntityFrameworkCore;
using WaitingList.Database.Database;
using WaitingList.SseManager.Managers;
using WaitingListBackend.Interfaces;
using WaitingListBackend.Repositories;
using WaitingListBackend.Services;

namespace WaitingListTests;

public class TestBase : IDisposable
{
    protected readonly ApplicationDbContext Context;
    protected readonly IWaitingListService WaitingListService;
    protected readonly IWaitingListRepository WaitingListRepository;
    protected readonly IPartyRepository PartyRepository;
    protected readonly IPartyService PartyService;
    protected readonly SseChannelManager SseChannelManager;

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySQL($"server=127.0.0.1;uid=root;pwd=qh734hsr05;Database=WaitingListTest")
            .Options;

        Context = new ApplicationDbContext(options);
        if (!Context.Database.CanConnect())
        {
            Context.Database.EnsureCreated();
        }
        SseChannelManager = new SseChannelManager();
        WaitingListRepository = new WaitingListRepository(Context, SseChannelManager);
        PartyRepository = new PartyRepository(Context, SseChannelManager);
        PartyService = new PartyService(PartyRepository, WaitingListRepository, SseChannelManager);
        WaitingListService = new WaitingListService(WaitingListRepository, PartyRepository, PartyService, SseChannelManager);
    }
    

    public void Dispose()
    {
        Context.RemoveRange(Context.WaitingLists);
        Context.RemoveRange(Context.Parties);
        Context.Dispose();
    }
}