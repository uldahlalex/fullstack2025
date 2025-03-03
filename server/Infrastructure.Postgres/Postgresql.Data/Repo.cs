using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace Infrastructure.Postgres.Postgresql.Data;

public class Repo(MyDbContext ctx) : IDataRepository
{
    public User GetUserOrNull(string username)
    {
        throw new NotImplementedException();
    }

    public User AddUser(User user)
    {
        throw new NotImplementedException();
    }
}