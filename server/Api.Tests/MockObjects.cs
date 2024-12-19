using Application.Models.Dtos;
using Application.Models.Entities;
using Application.Models.Enums;
using Npgsql.Replication.PgOutput.Messages;

namespace Api.Tests;

public class MockObjects
{
    public const string TestUsername = "bob@bob.dk";
    public const string TestPassword = "asdASD123,-.";
    public const string TestSalt = "5cbd23b9-0cb4-4afe-8497-c81bc6691a42";

    public const string TestHash =
        "J4SHSN9SKisNBoijKZkNAA5GNWJlO/RNsiXWhoWq2lOpd7hBtmwnqb6bOcxxYP8tEvNRomJunrVkWKNa5W3lXg==";
    
    public static Player GetPlayer(
        string? username = TestUsername, 
        string? role = null,
        bool activated = true,
        DateTime? createdAt = null,
        string? email = TestUsername,
        string? salt = TestSalt,
        string? hash = TestHash
        )
    {
        return new Player
        {
            FullName = username,
            Activated = activated,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            Email = username,
            Role = role ?? Roles.User.ToString(),
            Salt = salt,
            Hash = hash
        };
    }

    public static AuthRequestDto GetAuthRequestDto(string? username = TestUsername, string? password = TestPassword)
    {
        return new AuthRequestDto()
        {
            Username = username,
            Password = password
        };
    }
}