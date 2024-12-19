using Application.Models.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IDataRepository
{
    public IEnumerable<Board> GetDomainModels();
    Player? GetUserByUsername(string username);
    Player AddPlayer(Player player);
}