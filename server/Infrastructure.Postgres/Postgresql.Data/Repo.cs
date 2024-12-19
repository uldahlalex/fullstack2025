using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Entities;
using Infrastructure.Postgres.Scaffolding;

namespace Infrastructure.Postgres.Postgresql.Data;

public class Repo(MyDbContext ctx) : IDataRepository
{
    public IEnumerable<Board> GetDomainModels()
    {
        return ctx.Boards.ToList();
    }

    public Player? GetUserByUsername(string username)
    {
        return ctx.Players.FirstOrDefault(p => p.FullName == username);
    }

    public Player AddPlayer(Player player)
    {
        ctx.Players.Add(player);
        ctx.SaveChanges();
        return player;
    }
}