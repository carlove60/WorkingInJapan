namespace Eeckhoven.Models;

public class SectionModel
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
    public List<RemarkModel> Remarks { get; set; }
}