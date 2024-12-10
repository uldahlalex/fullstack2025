using core;
// using Infrastructure.Data

public class Repo() : IRepoLogic
{
    public IEnumerable<MyDomainModel> GetDomainModels()
    {
        return null;// ctx.YourEntities.ToList();
    }
}