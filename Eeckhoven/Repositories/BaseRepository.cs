using Eeckhoven.Database;

namespace Eeckhoven.Repositories;

public class BaseRepository
{
    protected readonly ApplicationDbContext ApplicationDbContext;
    public BaseRepository(ApplicationDbContext applicationDbContext)
    {
        ApplicationDbContext = applicationDbContext;
    }
}