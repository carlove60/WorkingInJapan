using System.ComponentModel.DataAnnotations;

namespace WaitingListBackend.Entities;

public class PartyEntity
{
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Range(1, 10)]
    public int Size { get; set; }
    
    public DateTime? ServiceEndedAt { get; set; }
    
    public DateTime? ServiceStartedAt { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public bool CheckedIn { get; set; }
    
    [Required]
    public string SessionId { get; set; } 
    
    [Required]
    public Guid WaitingListId { get; set; }
    
    [Required]
    public WaitingListEntity WaitingListEntity { get; set; }
}