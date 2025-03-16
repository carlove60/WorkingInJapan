namespace Eeckhoven.Models;

public class CurriculumModel
{
    public required Guid Id { get; set; }
    public required string Description { get; set; }
    public required List<ResumeModel> Lessons { get; set; }
}