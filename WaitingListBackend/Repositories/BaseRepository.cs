using WaitingListBackend.Database;

namespace WaitingListBackend.Repositories;

internal class BaseRepository
{
    protected readonly ApplicationDbContext _applicationDbContext;
    public BaseRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
}