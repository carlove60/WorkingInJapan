using Microsoft.EntityFrameworkCore;
using WaitingList.Database;
using WaitingList.Entities; // Adjust namespace to where your DbContext is
using WaitingList.Models;

namespace WaitingList.Tests;

public abstract class TestBase : IDisposable
{
    protected readonly ApplicationDbContext Context;
    
    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySQL($"server=127.0.0.1;uid=root;pwd=qh734hsr05;Database=WaitingListTest")
            .Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();
        AddTestData();

    }


    protected void AddTestData()
    {
        // Add some test parties
        var parties = new[]
        {
            new PartyEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Party 1",
                Size = 4,
            },
            new PartyEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Party 2",
                Size = 2,
            }
        };

        var waitingList = new WaitingListEntity
        {
            Name = "WaitingList"
        };

        Context.WaitingLists.Add(waitingList);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}