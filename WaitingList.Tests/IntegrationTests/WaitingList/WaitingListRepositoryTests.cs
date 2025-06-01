using WaitingListBackend.Entities;

namespace WaitingListTests.IntegrationTests.WaitingList;

[TestClass]
public class WaitingListRepositoryTests : TestBase
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
        var result = WaitingListRepository.GetWaitingList("Test Waiting List");
        var waitingList = result.Records.FirstOrDefault();
        Assert.IsNotNull(waitingList);
        Assert.IsTrue(waitingList.Name == "Test Waiting List");;
    }
}