namespace WaitingList.Database.Entities;

/// <summary>
/// Represents a waiting list entity which contains details about parties,
/// the waiting list name, total available seats, time for service,
/// and a unique identifier.
/// </summary>
public class WaitingListEntity
{
    public List<PartyEntity> Parties { get; set; } = new();
    public string Name { get; set; }
    public int TotalSeats { get; set; } = 0;
    public int TimeForService { get; set; }
    public Guid Id { get; set; }
}