using Application.Interfaces.Infrastructure.Postgres;
using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Postgresql.Data;

public class Repo(MyDbContext ctx) : IDataRepository
{
    public User? GetUserOrNull(string email)
    {
        return ctx.Users.FirstOrDefault(u => u.Email == email);
    }

    public User AddUser(User user)
    {
        ctx.Users.Add(user);
        ctx.SaveChanges();
        return user;
    }

    public Devicelog AddMetric(Devicelog eventDtoData)
    {
        ctx.Devicelogs.Add(eventDtoData);
        ctx.SaveChanges();
        return eventDtoData;
    }

    public List<Devicelog> GetAllMetrics()
    {
        return ctx.Devicelogs.ToList();
    }

    public void ClearMetrics()
    {
        ctx.Devicelogs.Where(d => true).ExecuteDelete();
    }
}