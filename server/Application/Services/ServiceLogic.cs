using service.Interfaces;
using service.Models;

namespace service.Services;

public class ServiceLogic(IRepoLogic repo) : IServiceLogic
{
    public IEnumerable<Board> GetDomainModels()
    {
        return repo.GetDomainModels();
    }
}