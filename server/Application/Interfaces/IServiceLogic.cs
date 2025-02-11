using Application.Models;
using Application.Models.Entities;

namespace Application.Interfaces;

public interface IServiceLogic
{
    public IEnumerable<Board> GetDomainModels(JwtClaims claims);
    public void Broadcast(object message, params string[] topics);

    public void Publish();
    object ChangePreferences(double temperatureThreshold); //todo request object instead of tech based dto
}