using core;
using infrastructure;

// using Infrastructure.Data

public class Repo(MyDbContext ctx) : IRepoLogic
{
    public IEnumerable<Board> GetDomainModels()
    {
        return ctx.Boards.ToList();
    }
}