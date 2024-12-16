namespace Application.Interfaces.Infrastructure.Broadcasting;

public interface IConnectionCreator
{
    IConnection Create(object nativeConnection);
}