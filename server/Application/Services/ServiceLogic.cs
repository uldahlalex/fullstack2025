using core;
using infrastructure;

namespace service;

public class ServiceLogic(IRepoLogic repo) : IServiceLogic
{
    public IEnumerable<Board> GetDomainModels()
    {
        return repo.GetDomainModels();
    }
}
