using Microsoft.EntityFrameworkCore;
using WaitingListBackend.Interfaces;
using WaitingListBackend.Database;
using WaitingListBackend.Entities;
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

    public TestBase(IWaitingListRepository waitingListRepository, IWaitingListService waitingListService)
    {
        WaitingListRepository = waitingListRepository;
        WaitingListService = waitingListService;
    }

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySQL($"server=127.0.0.1;uid=root;pwd=qh734hsr05;Database=WaitingListTest")
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
        WaitingListRepository = new WaitingListRepository(Context);
        PartyRepository = new PartyRepository(Context);
        WaitingListService = new WaitingListService(WaitingListRepository, PartyRepository);
        PartyService = new PartyService(PartyRepository, WaitingListRepository);
    }
    

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}