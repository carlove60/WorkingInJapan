using WaitingList.Models;
using WaitingList.Repositories;
using WaitingList.Tests;

namespace WaitingListTests;

[TestClass]
public sealed class WaitingListRepositoryTests : TestBase
{
    [TestMethod]
    public void GivenWaitingListExists_WhenRetrievingByName_ThenReturnsCorrectWaitingList
        ()
    {
        var waitingListRepository = new WaitingListRepository(base.Context);
        Context.WaitingLists.Add(new WaitingListEntity
        {
            Name = "Test Waiting List"
        });
        Context.SaveChanges();
        var waitingList = waitingListRepository.GetWaitingList("Test Waiting List");
        Assert.IsTrue(waitingList.Records.First().Name == "Test Waiting List");;
    }
}