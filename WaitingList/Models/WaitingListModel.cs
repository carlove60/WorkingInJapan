namespace WaitingList.Models;

public class WaitingListModel
{
    public List<PartyModel> Parties { get; set; } = new List<PartyModel>();
    public string Name { get; set; }
    
    public int TotalSeatsAvailable { get; set; }
    
    public int TimeForService { get; set; }
    public Guid Id { get; set; }
}