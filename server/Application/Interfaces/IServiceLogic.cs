using service.Models;

namespace service.Interfaces;

public interface IServiceLogic
{
    public IEnumerable<Board> GetDomainModels();
}