namespace WaitingList.DTO;

public class PartyDto
{
    public string WaitingListName { get; set; }
    public Guid SessionId { get; set; }
    public string Name { get; set; }
    public int Size { get; set; }
}