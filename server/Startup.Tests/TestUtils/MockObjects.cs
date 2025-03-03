using Application.Models.Dtos;
using Application.Models.Entities;
using Application.Models.Enums;

namespace Startup.Tests.TestUtils;

public class MockObjects
{
    public const string TestUsername = "bob@bob.dk";
    public const string TestPassword = "asdASD123,-.";
    public const string TestSalt = "5cbd23b9-0cb4-4afe-8497-c81bc6691a42";

    public const string TestHash =
        "J4SHSN9SKisNBoijKZkNAA5GNWJlO/RNsiXWhoWq2lOpd7hBtmwnqb6bOcxxYP8tEvNRomJunrVkWKNa5W3lXg==";

    public static User GetUser(
        string? username = TestUsername,
        string? role = null,
        bool activated = true,
        DateTime? createdAt = null,
        string? email = TestUsername,
        string? salt = TestSalt,
        string? hash = TestHash
    )
    {
        return new User()
        {

            Role = role ?? Roles.User.ToString(),
            Salt = salt,
            Hash = hash
        };
    }

    public static AuthRequestDto GetAuthRequestDto(string? username = TestUsername, string? password = TestPassword)
    {
        return new AuthRequestDto
        {
            Email = username,
            Password = password
        };
    }
}