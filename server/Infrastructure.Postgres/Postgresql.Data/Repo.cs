using infrastructure;
using service.Interfaces;
using service.Models;

// using Infrastructure.Data

namespace Infrastructure.Repositories.Postgresql.Data;

public class Repo(MyDbContext ctx) : IDataRepository
{
    public IEnumerable<Board> GetDomainModels()
    {
        return ctx.Boards.ToList();
    }
    
}