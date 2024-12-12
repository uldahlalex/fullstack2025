using service.Models;

namespace service.Interfaces;

public interface IDataRepository
{
    public IEnumerable<Board> GetDomainModels();
}