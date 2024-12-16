namespace Application;

public interface IConnectionCreator
{
    IConnection Create(object nativeConnection);
}