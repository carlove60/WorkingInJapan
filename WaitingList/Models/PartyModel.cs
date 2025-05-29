using System.ComponentModel.DataAnnotations;

namespace WaitingList.Models;

public class PartyModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    [Range(1, 10)]
    public int Size { get; set; }
    public bool IsReady { get; set; }
    
    public bool IsServiceEnded { get; set; }
    
    public WaitingListModel WaitingListModel { get; set; }
}