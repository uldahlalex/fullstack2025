using Application.Models.Entities;

namespace Application.Interfaces.Infrastructure.Data;

public interface IDataRepository
{
    public IEnumerable<Board> GetDomainModels();
}