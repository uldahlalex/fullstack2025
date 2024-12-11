using infrastructure;

namespace core;

public interface IRepoLogic
{
    public IEnumerable<Board> GetDomainModels();

}