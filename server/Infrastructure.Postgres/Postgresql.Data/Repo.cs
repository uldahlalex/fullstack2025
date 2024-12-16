using Application.Interfaces.Infrastructure.Data;
using Application.Models.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace Infrastructure.Postgres.Postgresql.Data;

public class Repo(MyDbContext ctx) : IDataRepository
{
    public IEnumerable<Board> GetDomainModels()
    {
        return ctx.Boards.ToList();
    }
}