using infrastructure;
using service.Interfaces.Infrastructure.Data;

// using Infrastructure.Data

namespace Infrastructure.Repositories.Postgresql.Data;

public class Repo(MyDbContext ctx) : IDataRepository
{
    public IEnumerable<Board> GetDomainModels()
    {
        return ctx.Boards.ToList();
    }
}