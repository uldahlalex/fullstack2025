namespace Application.Models.Dtos.Websocket;

public interface IClientWantsToChangePreferences
{
    double TemperatureThreshold { get; }
}