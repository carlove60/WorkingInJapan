namespace WaitingListBackend.Entities;

public class WaitingListEntity
{
    public List<PartyEntity> Parties { get; set; } = new List<PartyEntity>();
    public string Name { get; set; }
    
    public int TotalSeatsAvailable { get; set; }
    
    public int TimeForService { get; set; }
    public Guid Id { get; set; }
}