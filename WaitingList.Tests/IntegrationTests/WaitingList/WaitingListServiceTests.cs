using WaitingList.Contracts.DTOs;
using WaitingListBackend.Entities;
using WaitingListBackend.Enums;

namespace WaitingListTests.IntegrationTests.WaitingList;

[TestClass]
public class WaitingListServiceTests : TestBase
{
    [TestMethod]
    public void GivenWaitingListService_WhenNoPartiesAdded_ThenSeatsAvailableReturnsTotalSeats
        ()
    {
        Context.WaitingLists.Add(new WaitingListEntity
        {
            Name = "Test Waiting List"
        });
        Context.SaveChanges();
        var waitingListResult = WaitingListService.GetWaitingList("Test Waiting List");
        var waitingList = waitingListResult.Records.First();
        Assert.AreEqual(waitingList.SeatsAvailable, waitingList.TotalSeats);
    }
    
    [TestMethod]
    public void GivenWaitingListService_WhenPartiesAdded_ThenSeatsAvailableShouldReduceBySize
        ()
    {
        Context.WaitingLists.Add(new WaitingListEntity
        {
            Name = "Test Waiting List",
            TotalSeats = 10
        });
        Context.SaveChanges();
        var party = new PartyDto
        {
            Name = "Test Party",
            Size = 4,
            WaitingListName = "Test Waiting List"
        };
        
        WaitingListService.AddPartyToWaitingList(party);
        var waitingListResult = WaitingListService.GetWaitingList("Test Waiting List");
        var waitingList = waitingListResult.Records.First();
        var expectedSize = waitingList.TotalSeats - party.Size;
        Assert.AreEqual(expectedSize, waitingList.SeatsAvailable);
    }
    
    [TestMethod]
    public void GivenWaitingListService_WhenAddingASizeBiggerThanTotalSeats_ThenReturnAnError
        ()
    {
        var entity = new WaitingListEntity
        {
            Name = "Test Waiting List",
            TotalSeats = 10,
            Parties = new List<PartyEntity>()
        };
        entity.Parties.Add(new()
        {
            Name = "Test Party",
            Size = 10,
        });
        Context.WaitingLists.Add(entity);
        Context.SaveChanges();
        var party = new PartyDto
        {
            Name = "Test Party 2",
            Size = 4,
            WaitingListName = "Test Waiting List"
        };
        
        var result = WaitingListService.AddPartyToWaitingList(party);
        Assert.AreEqual(0, result.Records.Count);
        Assert.AreEqual(1, result.Messages.Count);
        Assert.AreEqual("There are only 0 seat(s) left on the waiting list. Please try again later.",
            result.Messages.Single().Message);
        Assert.AreEqual(MessageType.Error, result.Messages.Single().Type);
    }
}