namespace Application.Interfaces.Infrastructure.Broadcasting;

public interface IConnection
{
    Guid Id { get; }
    void Send(string jsonSerializedMessage);
}