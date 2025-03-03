using Application.Models.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IDataRepository
{
    User GetUserOrNull(string username);
    User AddUser(User user);
}