using Application.Models.Entities;

namespace Application.Interfaces.Infrastructure.Postgres;

public interface IDataRepository
{
    User? GetUserOrNull(string email);
    User AddUser(User user);
}