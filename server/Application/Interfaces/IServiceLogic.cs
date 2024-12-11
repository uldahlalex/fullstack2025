using infrastructure;

namespace core;

public interface IServiceLogic
{
    public IEnumerable<Board> GetDomainModels();
}