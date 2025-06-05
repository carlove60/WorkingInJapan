using Microsoft.EntityFrameworkCore;
using WaitingList.Database.Database;
using WaitingListBackend.Interfaces;
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

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySQL($"server=db;uid=root;pwd=qh734hsr05;Database=WaitingListTest")
            .Options;

        Context = new ApplicationDbContext(options);
        if (!Context.Database.CanConnect())
        {
            Context.Database.EnsureCreated();
        }

        WaitingListRepository = new WaitingListRepository(Context);
        PartyRepository = new PartyRepository(Context);
        WaitingListService = new WaitingListService(WaitingListRepository, PartyRepository);
        PartyService = new PartyService(PartyRepository, WaitingListRepository);
    }
    

    public void Dispose()
    {
        Context.RemoveRange(Context.WaitingLists);
        Context.RemoveRange(Context.Parties);
        Context.Dispose();
    }
}