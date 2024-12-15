using infrastructure;

namespace service.Interfaces.Infrastructure.Data;

public interface IDataRepository
{
    public IEnumerable<Board> GetDomainModels();
}