namespace WaitingList.Contracts.DTOs;

public class PartyDto
{
    public string WaitingListName { get; set; }
    public string? SessionId { get; set; }
    public string Name { get; set; }
    public int Size { get; set; }
    public bool CanCheckIn { get; set; }

    public bool CheckedIn { get; set; }
    
    public bool IsServiceDone { get; set; }
}