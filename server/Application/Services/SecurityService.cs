using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models;
using Application.Models.Dtos;
using Application.Models.Entities;
using Application.Models.Enums;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Options;

namespace Application.Services;

public interface ISecurityService
{
    public string HashPassword(string password);
    public void VerifyPasswordOrThrow(string password, string hashedPassword);
    public string GenerateSalt();
    public string GenerateJwt(JwtClaims claims);
    public AuthResponseDto Login(AuthRequestDto dto);
    public AuthResponseDto Register(AuthRequestDto dto);
    public void VerifyJwtOrThrow(string jwt);
}

public class SecurityService(IOptionsMonitor<AppOptions> optionsMonitor, IDataRepository repository) : ISecurityService
{
    public AuthResponseDto Login(AuthRequestDto dto)
    {
        var player = repository.GetUserByUsernameOrThrow(dto.Username);
        VerifyPasswordOrThrow(dto.Password + player.Salt, player.Hash);
        return new AuthResponseDto
        {
            Jwt = GenerateJwt(new JwtClaims
            {
                Id = player.Id.ToString(),
                Username = player.FullName,
                Role = player.Role,
                Email = player.Email,
                Exp = DateTimeOffset.UtcNow.AddHours(1000).ToUnixTimeSeconds().ToString()
            })
        };
    }

    public AuthResponseDto Register(AuthRequestDto dto)
    {
        var player = repository.GetUserByUsernameOrThrow(dto.Username);
        if (player is not null) throw new ValidationException("User already exists");
        var salt = GenerateSalt();
        var hash = HashPassword(dto.Password + salt);
        var insertedPlayer = repository.AddPlayer(new Player
        {
            FullName = dto.Username,
            Email = dto.Username,
            Role = Roles.User.ToString(),
            Salt = salt,
            Hash = hash
        });
        return new AuthResponseDto
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

    public void VerifyPasswordOrThrow(string password, string hashedPassword)
    {
        if (HashPassword(password) != hashedPassword)
            throw new AuthenticationException("Invalid login");

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

    public void VerifyJwtOrThrow(string jwt)
    {
        var token = new JwtBuilder()
            .WithAlgorithm(new HMACSHA512Algorithm())
            .WithSecret(optionsMonitor.CurrentValue.JwtSecret)
            .WithUrlEncoder(new JwtBase64UrlEncoder()) 
            .WithJsonSerializer(new JsonNetSerializer()) 
            .MustVerifySignature()
            .Decode<JwtClaims>(jwt);

        if (DateTimeOffset.FromUnixTimeSeconds(long.Parse(token.Exp)) < DateTimeOffset.UtcNow)
            throw new AuthenticationException("Token expired");
    }
}