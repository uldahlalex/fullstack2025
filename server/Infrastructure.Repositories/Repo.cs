using infrastructure;
using service.Interfaces;
using service.Models;

// using Infrastructure.Data

namespace Infrastructure.Repositories;

public class Repo(MyDbContext ctx) : IRepoLogic
{
    public IEnumerable<Board> GetDomainModels()
    {
        return ctx.Boards.ToList();
    }
}