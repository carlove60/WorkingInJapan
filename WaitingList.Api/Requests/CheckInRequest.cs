using System.ComponentModel.DataAnnotations;
using WaitingList.Contracts.DTOs;

namespace WaitingList.Requests;

public class CheckInRequest
{
    [Required]
    public PartyDto Party { get; set; }
}