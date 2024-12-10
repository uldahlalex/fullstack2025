using core;

namespace service;

public class ServiceLogic(IRepoLogic repo) : IServiceLogic
{
    public IEnumerable<MyDomainModel> GetDomainModels()
    {
        return repo.GetDomainModels();
    }
}
