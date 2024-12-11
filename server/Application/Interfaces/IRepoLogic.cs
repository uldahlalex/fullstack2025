using service.Models;

namespace service.Interfaces;

public interface IRepoLogic
{
    public IEnumerable<Board> GetDomainModels();
}