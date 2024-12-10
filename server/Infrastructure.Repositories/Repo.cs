using core;
using Infrastructure.Data;

public class Repo(ApplicationDbContext ctx) : IRepoLogic
{
    public IEnumerable<MyDomainModel> GetDomainModels()
    {
        return ctx.YourEntities.ToList();
    }
}