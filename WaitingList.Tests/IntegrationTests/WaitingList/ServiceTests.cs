using WaitingList.Contracts.DTOs;
using WaitingListBackend.Entities;

namespace WaitingListTests.IntegrationTests.WaitingList;

[TestClass]
public class ServiceTests : TestBase
{
    [TestMethod]
    public void GivenValidWaitingListName_WhenGetWaitingList_ThenReturnsWaitingList()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        CreateTestWaitingList(waitingListName, 10);

        // When
        var result = WaitingListService.GetWaitingList(waitingListName);

        // Then
        Assert.AreEqual(1, result.Records.Count);
        Assert.AreEqual(waitingListName, result.Records.First().Name);
    }

    [TestMethod]
    public void GivenNonExistentWaitingListName_WhenGetWaitingList_ThenReturnsError()
    {
        // When
        var result = WaitingListService.GetWaitingList("NonExistentList");

        // Then
        Assert.AreEqual(0, result.Records.Count);
        Assert.AreEqual(1, result.Messages.Count);
    }

    [TestMethod]
    public void GivenValidParty_WhenAddPartyToWaitingList_ThenPartyIsAdded()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        CreateTestWaitingList(waitingListName, 10);
        
        var partyDto = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = "TestParty",
            Size = 4,
            SessionId = Guid.NewGuid().ToString()
        };

        // When
        var result = WaitingListService.AddPartyToWaitingList(partyDto);

        // Then
        Assert.AreEqual(0, result.Messages.Count);
        Assert.AreEqual(1, result.Records.Count);
        Assert.IsNotNull(result.Records.First().AddedParty);
    }

    [TestMethod]
    public void GivenPartyExceedingSeatCapacity_WhenAddPartyToWaitingList_ThenReturnsError()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        var totalSeats = 5;
        CreateTestWaitingList(waitingListName, totalSeats);
        
        var partyDto = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = "BigParty",
            Size = totalSeats + 1,
            SessionId = Guid.NewGuid().ToString()
        };

        // When
        var result = WaitingListService.AddPartyToWaitingList(partyDto);

        // Then
        Assert.AreEqual(1, result.Messages.Count);
        Assert.IsTrue(result.Messages.First().Message.Contains("seat"));
    }

    [TestMethod]
    public void GivenMultiplePartiesAddedToWaitingList_WhenCapacityIsReached_ThenRejectsAdditionalParties()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        var totalSeats = 10;
        CreateTestWaitingList(waitingListName, totalSeats);
        
        // Add parties to fill the waiting list
        for (int i = 0; i < 2; i++)
        {
            var partyDto = new PartyDto
            {
                WaitingListName = waitingListName,
                Name = $"Party{i}",
                Size = 5, // Total seats will be filled with 2 parties
                SessionId = Guid.NewGuid().ToString()
            };
            WaitingListService.AddPartyToWaitingList(partyDto);
        }
        
        // Try to add one more party
        var additionalParty = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = "ExtraParty",
            Size = 1,
            SessionId = Guid.NewGuid().ToString()
        };

        // When
        var result = WaitingListService.AddPartyToWaitingList(additionalParty);

        // Then
        Assert.AreEqual(1, result.Messages.Count);
        Assert.IsTrue(result.Messages.First().Message.Contains("seat"));
    }

    [TestMethod]
    public void GivenPartyAlreadyOnWaitingList_WhenAddingSamePartyAgain_ThenReturnsExistingParty()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        CreateTestWaitingList(waitingListName, 10);
        
        var sessionId = Guid.NewGuid().ToString();
        var partyDto = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = "RepeatedParty",
            Size = 3,
            SessionId = sessionId
        };
        
        // Add the party first time
        WaitingListService.AddPartyToWaitingList(partyDto);

        // When - Add the same party again
        var result = WaitingListService.AddPartyToWaitingList(partyDto);

        // Then
        Assert.AreEqual(0, result.Messages.Count);
        Assert.AreEqual(sessionId, result.Records.First().AddedParty.SessionId);
    }
    
    [TestMethod]
    public void GivenValidSessionId_WhenGetParty_ThenReturnsParty()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        CreateTestWaitingList(waitingListName, 10);
        
        var sessionId = Guid.NewGuid().ToString();
        var partyDto = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = "PartyToRetrieve",
            Size = 4,
            SessionId = sessionId
        };
        
        WaitingListService.AddPartyToWaitingList(partyDto);

        // When
        var result = PartyService.GetParty(sessionId);

        // Then
        Assert.AreEqual(0, result.Messages.Count);
        Assert.AreEqual(1, result.Records.Count);
        Assert.AreEqual(sessionId, result.Records.First().SessionId);
        Assert.AreEqual(partyDto.Name, result.Records.First().Name);
    }

    [TestMethod]
    public void GivenInvalidSessionId_WhenGetParty_ThenReturnsError()
    {
        // When
        var result = PartyService.GetParty("NonExistentSessionId");

        // Then
        Assert.AreEqual(0, result.Records.Count);
        Assert.AreEqual(0, result.Messages.Count);
    }

    [TestMethod]
    public void GivenValidSessionId_WhenCheckIn_ThenMarksPartyAsCheckedIn()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        CreateTestWaitingList(waitingListName, 10);
        
        var sessionId = Guid.NewGuid().ToString();
        var partyDto = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = "CheckInParty",
            Size = 3,
            SessionId = sessionId
        };
        
        WaitingListService.AddPartyToWaitingList(partyDto);

        // When
        var result = PartyService.CheckIn(sessionId);

        // Then
        Assert.AreEqual(1, result.Messages.Count);
        Assert.AreEqual("Party checked in successfully.", result.Messages.Single().Message);
        Assert.AreEqual(1, result.Records.Count);
        Assert.IsTrue(result.Records.First().CheckedIn);
    }

    [TestMethod]
    public void GivenInvalidSessionId_WhenCheckIn_ThenReturnsError()
    {
        // When
        var result = PartyService.CheckIn("NonExistentSessionId");

        // Then
        Assert.AreEqual(0, result.Records.Count);
        Assert.AreEqual(1, result.Messages.Count);
    }

    [TestMethod]
    public void GivenAlreadyCheckedInParty_WhenCheckIn_ThenStillReturnsSuccessWithoutMessage()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        CreateTestWaitingList(waitingListName, 10);
        
        var sessionId = Guid.NewGuid().ToString();
        var partyDto = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = Guid.NewGuid().ToString(),
            Size = 2,
            SessionId = sessionId
        };
        
        WaitingListService.AddPartyToWaitingList(partyDto);
        var firstCheckIn = PartyService.CheckIn(sessionId); // First check-in

        // When
        var result = PartyService.CheckIn(sessionId); // Second check-in

        // Then
        Assert.AreEqual(1, firstCheckIn.Messages.Count);
        Assert.AreEqual("Party checked in successfully.", firstCheckIn.Messages.Single().Message);
        Assert.AreEqual(0, result.Messages.Count);
        Assert.AreEqual(1, result.Records.Count);
        Assert.IsTrue(result.Records.First().CheckedIn);
    }
    
    [TestMethod]
    public void GivenTwoParties_WhenRetrieved_ThenOnlyTheFirstOneCanBeCheckedIn()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        CreateTestWaitingList(waitingListName, 10);
        
        var sessionId1 = Guid.NewGuid().ToString();
        var sessionId2 = Guid.NewGuid().ToString();
        var partyDto = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = Guid.NewGuid().ToString(),
            Size = 2,
            SessionId = sessionId1
        };
        
        var partyDto2 = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = Guid.NewGuid().ToString(),
            Size = 2,
            SessionId = sessionId2
        };
        
        WaitingListService.AddPartyToWaitingList(partyDto);
        WaitingListService.AddPartyToWaitingList(partyDto2);
        
        // When
        var firstParty = PartyService.GetParty(sessionId1).Records.FirstOrDefault();
        var secondParty = PartyService.GetParty(sessionId2).Records.FirstOrDefault();

        // Then
        Assert.IsNotNull(firstParty);
        Assert.IsNotNull(secondParty);
        Assert.IsTrue(firstParty.CanCheckIn);
        Assert.IsFalse(secondParty.CanCheckIn);
    }
    
    [TestMethod]
    public void GivenTwoParties_WhenOneIsSetToCheckedInAndInService_ThenOnlyTheSecondOneCanNotBeCheckedIn()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        CreateTestWaitingList(waitingListName, 10);
        
        var sessionId1 = Guid.NewGuid().ToString();
        var sessionId2 = Guid.NewGuid().ToString();
        var partyDto = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = Guid.NewGuid().ToString(),
            Size = 10,
            SessionId = sessionId1
        };
        
        var partyDto2 = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = Guid.NewGuid().ToString(),
            Size = 2,
            SessionId = sessionId2
        };
        
        // When
        WaitingListService.AddPartyToWaitingList(partyDto);
        PartyService.CheckIn(partyDto.SessionId);
        WaitingListService.AddPartyToWaitingList(partyDto2);
        var firstParty = PartyService.GetParty(sessionId1).Records.FirstOrDefault();
        var secondParty = PartyService.GetParty(sessionId2).Records.FirstOrDefault();

        // Then
        Assert.IsNotNull(firstParty);
        Assert.IsNotNull(secondParty);
        Assert.IsTrue(firstParty.CheckedIn);
        Assert.IsFalse(secondParty.CanCheckIn);
    }

    [TestMethod]
    public void GivenTwoPartiesOnWaitingList_WhenOneIsCheckedIn_ThenOtherRemainsNotCheckedIn()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        CreateTestWaitingList(waitingListName, 20);
        
        var firstSessionId = Guid.NewGuid().ToString();
        var firstPartyDto = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = "FirstParty",
            Size = 3,
            SessionId = firstSessionId
        };
        
        var secondSessionId = Guid.NewGuid().ToString();
        var secondPartyDto = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = "SecondParty",
            Size = 4,
            SessionId = secondSessionId
        };
        
        WaitingListService.AddPartyToWaitingList(firstPartyDto);
        WaitingListService.AddPartyToWaitingList(secondPartyDto);

        // When
        PartyService.CheckIn(firstSessionId); // Check in first party
        var firstPartyResult = PartyService.GetParty(firstSessionId);
        var secondPartyResult = PartyService.GetParty(secondSessionId);

        // Then
        Assert.IsTrue(firstPartyResult.Records.First().CheckedIn);
        Assert.IsFalse(secondPartyResult.Records.First().CheckedIn);
    }

    [TestMethod]
    public void GivenTwoPartiesWithDifferentCreationTimes_WhenGetWaitingList_ThenPartiesAreOrderedByCreationTime()
    {
        // Given
        var waitingListName = Guid.NewGuid().ToString();
        CreateTestWaitingList(waitingListName, 20);

        var firstParty = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = "FirstParty",
            Size = 2,
            SessionId = Guid.NewGuid().ToString()
        };
        WaitingListService.AddPartyToWaitingList(firstParty);

        var secondParty = new PartyDto
        {
            WaitingListName = waitingListName,
            Name = "SecondParty",
            Size = 3,
            SessionId = Guid.NewGuid().ToString()
        };
        WaitingListService.AddPartyToWaitingList(secondParty);

        // When
        var result = WaitingListService.GetWaitingList(waitingListName);
        var parties = result.Records.First().Parties;

        // Then
        Assert.AreEqual(2, parties.Count);
        Assert.AreEqual("FirstParty", parties[0].Name);
        Assert.AreEqual("SecondParty", parties[1].Name);
    }

    private WaitingListEntity CreateTestWaitingList(string name, int totalSeats)
    {
        var waitingList = new WaitingListEntity
        {
            Name = name,
            TotalSeats = totalSeats,
            TimeForService = 3
        };
        Context.WaitingLists.Add(waitingList);
        Context.SaveChanges();
        return waitingList;
    }
}