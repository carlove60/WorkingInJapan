using Eeckhoven.Database;
using Eeckhoven.Models;

namespace Eeckhoven.Repositories;

public class LessonRepository : BaseRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public LessonRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public ResumeModel? GetLessonById(Guid id)
    {
        return _applicationDbContext.Resumes.FirstOrDefault((u) => u.Id == id);
    }
}