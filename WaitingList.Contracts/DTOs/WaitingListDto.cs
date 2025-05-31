using System.ComponentModel.DataAnnotations;

namespace WaitingList.DTO;

public class WaitingListDto
{
    public string Name { get; set; }
    public Guid Id { get; set; }
    public int TotalSeats { get; set; }
    public int TotalSeatsAvailable { get; set; }
    public List<PartyDto> Parties { get; set; } = new List<PartyDto>();
}