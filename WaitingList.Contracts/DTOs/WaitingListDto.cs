using System.ComponentModel.DataAnnotations;

namespace WaitingList.Contracts.DTOs;

public class WaitingListDto
{
    [Required]
    public string Name { get; set; }
    public Guid Id { get; set; }
    public int TotalSeats { get; set; }
    
    [Required]
    public int SeatsAvailable { get; set; }
    public List<PartyDto> Parties { get; set; } = new List<PartyDto>();
}