﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MyApp.Application.Infrastructure.Abstractions.Auth;

namespace MyApp.Infrastructure.Auth;

public class JwtReader(AuthSettings settings) : IJwtReader
{
    public AccessToken ReadAccessToken(string jwtBearer)
    {
        var (identity, ex) = ReadTokenClaims(settings.Jwt.PublicKeyXml, jwtBearer);

        if (identity == null)
            throw new InvalidOperationException("No identity could be generated with the provided access token.", ex);

        return new(
            Id: int.Parse(GetClaim(identity, JwtClaims.UserId)),
            Username: GetClaim(identity, JwtClaims.Username),
            Email: GetClaim(identity, JwtClaims.Email)
            );
    }

    public RefreshToken? ReadRefreshToken(string jwtBearer)
    {
        var (identity, _) = ReadTokenClaims(settings.Jwt.PublicKeyXml, jwtBearer);

        return identity == null 
            ? null
            : new(
            UserId: int.Parse(GetClaim(identity, JwtClaims.UserId)),
            Username: GetClaim(identity, JwtClaims.Username),
            Email: GetClaim(identity, JwtClaims.Email),
            Version: int.Parse(GetClaim(identity, JwtClaims.RefreshTokenVersion))
            );
    }

    private static (ClaimsIdentity?, Exception?) ReadTokenClaims(string publicKeyXml, string jwtBearer)
    {
        var tokenHandler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };
        var validationParameters = JwtValidationParametersProvider.Get(publicKeyXml);
        try
        {
            var principal = tokenHandler.ValidateToken(jwtBearer, validationParameters, out var _);
            return (principal.Identity as ClaimsIdentity, null);
        }
        catch (Exception ex)
        {
            return (null, ex);
        }
    }

    private static string GetClaim(ClaimsIdentity identity, string type)
    {
        var claim = identity.FindFirst(type)?.Value;
        ArgumentException.ThrowIfNullOrWhiteSpace(claim, nameof(claim));
        return claim;
    }
}
