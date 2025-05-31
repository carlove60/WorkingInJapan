using WaitingListBackend.Entities;

namespace WaitingListTests;

[TestClass]
public sealed class WaitingListRepositoryTests : TestBase
{
    [TestMethod]
    public void GivenWaitingListExists_WhenRetrievingByName_ThenReturnsCorrectWaitingList
        ()
    {
        Context.WaitingLists.Add(new WaitingListEntity
        {
            Name = "Test Waiting List"
        });
        Context.SaveChanges();
        var waitingList = WaitingListRepository.GetWaitingList("Test Waiting List");
        Assert.IsTrue(waitingList.Records.FirstOrDefault().Name == "Test Waiting List");;
    }
}