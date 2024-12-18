using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces.Infrastructure.Data;
using Application.Models;
using Application.Models.Dtos;
using Application.Models.Entities;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Options;

namespace Application.Services;

public interface ISecurityService
{
    public string HashPassword(string password);
    public bool VerifyPassword(string password, string hashedPassword);
    public string GenerateSalt();
    public string GenerateJwt(JwtClaims claims);
    public AuthResponseDto Login(AuthRequestDto dto);
    public AuthResponseDto Register(AuthRequestDto dto);
    public void VerifyJwt(string jwt);
}

public class SecurityService(IOptionsMonitor<AppOptions> optionsMonitor, IDataRepository repository) : ISecurityService
{
    public AuthResponseDto Login(AuthRequestDto dto)
    {
        var player = repository.GetUserByUsername(dto.Username) ??
                     throw new Exception("Could not get user by username");
        if (!VerifyPassword(dto.Password + player.Salt, player.Hash)) throw new Exception("Invalid password");
        return new AuthResponseDto() {
            Jwt = GenerateJwt(new JwtClaims
        {
            Id = player.Id.ToString(),
            Username = player.FullName,
            Role = player.Role,
            Email = player.Email,
            Exp = DateTimeOffset.UtcNow.AddHours(1000).ToUnixTimeSeconds().ToString()
        })};
    }

    public AuthResponseDto Register(AuthRequestDto dto)
    {
        var player = repository.GetUserByUsername(dto.Username);
        if (player is not null) throw new Exception("User already exists");
        var salt = GenerateSalt();
        var hash = HashPassword(dto.Password + salt);
        var insertedPlayer = repository.AddPlayer(new Player
        {
            FullName = dto.Username,
            Email = dto.Username,
            Role = "user",
            Salt = salt,
            Hash = hash
        });
        return new AuthResponseDto()
        {
            Jwt = GenerateJwt(new JwtClaims
            {
                Id = insertedPlayer.Id.ToString(),
                Username = insertedPlayer.FullName,
                Role = insertedPlayer.Role,
                Email = insertedPlayer.Email,
                Exp = DateTimeOffset.UtcNow.AddHours(1000).ToUnixTimeSeconds().ToString()
            })
        };
    }

    public string HashPassword(string password)
    {
        using var sha512 = SHA512.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha512.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return HashPassword(password) == hashedPassword;
    }

    public string GenerateSalt()
    {
        return Guid.NewGuid().ToString();
    }

    public string GenerateJwt(JwtClaims claims)
    {
        var tokenBuilder = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(optionsMonitor.CurrentValue.JwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder())
            .WithJsonSerializer(new JsonNetSerializer());

        foreach (var claim in claims.GetType().GetProperties())
            tokenBuilder.AddClaim(claim.Name, claim.GetValue(claims)!.ToString());
        return tokenBuilder.Encode();
    }

    public void VerifyJwt(string jwt)
    {
        var token = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm()) // Add this
            .WithSecret(optionsMonitor.CurrentValue.JwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder()) // Add this
            .WithJsonSerializer(new JsonNetSerializer()) // Add this
            .MustVerifySignature()
            .Decode<JwtClaims>(jwt);

        if (DateTimeOffset.FromUnixTimeSeconds(long.Parse(token.Exp)) < DateTimeOffset.UtcNow)
            throw new AuthenticationException("Token expired");
    }
}