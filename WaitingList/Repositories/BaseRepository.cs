using WaitingList.Database;

namespace WaitingList.Repositories;

public class BaseRepository
{
    protected readonly ApplicationDbContext _applicationDbContext;
    public BaseRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
}