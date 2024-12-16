namespace Application.Services;

public interface ISecurityService
{
    public string HashPassword(string password);
    public bool VerifyPassword(string password, string hashedPassword);
    public string GenerateSalt();
    public string GenerateJwt(Dictionary<string, string> claims);
}

public class SecurityService : ISecurityService
{
    public string HashPassword(string password)
    {
        throw new NotImplementedException();
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        throw new NotImplementedException();
    }

    public string GenerateSalt()
    {
        throw new NotImplementedException();
    }

    public string GenerateJwt(Dictionary<string, string> claims)
    {
        throw new NotImplementedException();
    }
}