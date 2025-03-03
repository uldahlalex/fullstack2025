using Application.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Scaffolding;

public interface IMyDbContext
{
    DbSet<Device> Devices { get; set; }
    DbSet<Devicelog> Devicelogs { get; set; }
    DbSet<User> Users { get; set; }
}