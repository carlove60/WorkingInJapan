using WaitingListBackend.Entities;

namespace WaitingListTests.IntegrationTests.WaitingList;

[TestClass]
public class RepositoryTests : TestBase
{
    [TestMethod]
    public void GivenPartySavedToRepository_WhenRetrievedBySessionId_ThenShouldReturnCorrectParty()
    {
        // Given
        var waitingListEntity = new WaitingListEntity
        {
            Name = Guid.NewGuid().ToString(),
            TotalSeats = 20
        };

        Context.WaitingLists.Add(waitingListEntity);
        Context.SaveChanges();

        var partyEntity = new PartyEntity
        {
            Name = Guid.NewGuid().ToString(),
            Size = 4,
            SessionId = Guid.NewGuid().ToString(),
            CreatedOn = DateTime.Now,
            WaitingListId = waitingListEntity.Id
        };


        // When
        var saveResult = PartyRepository.SaveParty(partyEntity);
        var retrieveResult = PartyRepository.GetParty(partyEntity.SessionId);

        // Then
        Assert.AreEqual(0, saveResult.Messages.Count); // Ensure no errors on save
        Assert.IsNotNull(retrieveResult);
        Assert.AreEqual(partyEntity.SessionId, retrieveResult.Records.First().SessionId);
        Assert.AreEqual(partyEntity.Name, retrieveResult.Records.First().Name);
    }

    [TestMethod]
    public void GivenWaitingListSavedToRepository_WhenRetrievedByName_ThenShouldReturnCorrectWaitingList()
    {
        // Given
        var waitingListEntity = new WaitingListEntity
        {
            Name = Guid.NewGuid().ToString(),
            TotalSeats = 15,
        };

        Context.WaitingLists.Add(waitingListEntity);
        Context.SaveChanges();


        // When
        var result = WaitingListRepository.GetWaitingList(waitingListEntity.Name, false);

        // Then
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Records.Count);
        Assert.AreEqual(waitingListEntity.Name, result.Records.First().Name);
    }
    

    [TestMethod]
    public void GivenNonexistentWaitingList_WhenRetrieved_ThenShouldReturnErrorMessage()
    {
        // Given

        // When
        var result = WaitingListRepository.GetWaitingList("Nonexistent List", false);

        // Then
        Assert.AreEqual(0, result.Records.Count);
        Assert.AreEqual(1, result.Messages.Count);
        Assert.AreEqual("Nonexistent List not found", result.Messages.First().Message);
    }

    [TestMethod]
    public void GivenPartySavedToRepository_WhenRemoved_ThenShouldNotBeRetrievable()
    {
        // Given
        var waitingListEntity = new WaitingListEntity
        {
            Name = Guid.NewGuid().ToString(),
            TotalSeats = 20
        };

        Context.WaitingLists.Add(waitingListEntity);
        Context.SaveChanges();

        var partyEntity = new PartyEntity
        {
            Name = Guid.NewGuid().ToString(),
            Size = 5,
            SessionId = Guid.NewGuid().ToString(),
            CreatedOn = DateTime.Now,
            WaitingListId = waitingListEntity.Id
        };

        PartyRepository.SaveParty(partyEntity);

        // When
        PartyRepository.RemoveParty(partyEntity);
        var retrieveResult = PartyRepository.GetParty(partyEntity.SessionId);

        // Then
        Assert.AreEqual(0, retrieveResult.Records.Count); // Party should no longer exist
    }
}