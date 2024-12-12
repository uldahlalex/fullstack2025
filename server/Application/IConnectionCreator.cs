namespace service;

public interface IConnectionCreator
{
    IConnection Create(object nativeConnection);
}