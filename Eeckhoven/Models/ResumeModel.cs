using Eeckhoven.Repositories;

namespace Eeckhoven.Models;

public class ResumeModel
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public UserModel User { get; set; }
    
    public Guid UserId { get; set; }
}