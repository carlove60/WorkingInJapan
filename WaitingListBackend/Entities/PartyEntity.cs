using System.ComponentModel.DataAnnotations;

namespace WaitingListBackend.Entities;

public class PartyEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    [Range(1, 10)]
    public int Size { get; set; }
    
    public DateTime? ServiceEndedAt { get; set; }
    
    public DateTime? ServiceStartedAt { get; set; }
    
    [Required]
    public WaitingListEntity WaitingListEntity { get; set; }
}