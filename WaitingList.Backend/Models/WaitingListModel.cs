using WaitingListBackend.Entities;

namespace WaitingListBackend.Models;

public class WaitingListModel : WaitingListEntity
{
    public int AvailableSeats { get; set; } = 0;
}