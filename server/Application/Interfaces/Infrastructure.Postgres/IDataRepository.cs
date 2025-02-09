using Application.Models.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IDataRepository
{
    public IEnumerable<Board> GetDomainModels();
    Player GetUserOrNull(string username);
    Player AddPlayer(Player player);
}