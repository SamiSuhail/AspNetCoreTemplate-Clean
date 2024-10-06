using System.Security.Cryptography;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Infrastructure.Auth;

namespace MyApp.Tests.Integration.Utilities.Arrange;

public static class AccessTokenExtensions
{
    public static string ArrangeExpiredAccessToken(this IServiceProvider serviceProvider, TestUser user)
    {
        var authSettings = serviceProvider.GetRequiredService<AuthSettings>();
        var clockMock = new Mock<IClock>();
        var jwtGenerator = new JwtGenerator(authSettings, clockMock.Object);
        clockMock.Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow.AddMinutes(-authSettings.Jwt.AccessTokenExpirationMinutes - 1));
        var accessToken = jwtGenerator.CreateAccessToken(user.Entity.Id, user.Entity.Username, user.Entity.Email);
        return accessToken;
    }

    public static string ArrangeExpiredRefreshToken(this IServiceProvider serviceProvider, TestUser user)
    {
        var authSettings = serviceProvider.GetRequiredService<AuthSettings>();
        var clockMock = new Mock<IClock>();
        var jwtGenerator = new JwtGenerator(authSettings, clockMock.Object);
        clockMock.Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow.AddDays(-authSettings.Jwt.RefreshTokenExpirationDays - 1));
        var accessToken = jwtGenerator.CreateRefreshToken(user.Entity.Id, user.Entity.Username, user.Entity.Email, user.Entity.RefreshTokenVersion);
        return accessToken;
    }

    public static IJwtGenerator ArrangeJwtGeneratorWithInvalidPrivateKey(this IServiceProvider serviceProvider)
    {
        var clock = serviceProvider.GetRequiredService<IClock>();
        var authSettings = serviceProvider.GetRequiredService<AuthSettings>();
        var rsa = RSA.Create();
        var fakePrivateKey = rsa.ToXmlString(true);
        var fakeAuthSettings = new AuthSettings
        {
            Jwt = new()
            {
                PrivateKeyXml = fakePrivateKey,
                PublicKeyXml = authSettings.Jwt.PublicKeyXml,
                AccessTokenExpirationMinutes = authSettings.Jwt.AccessTokenExpirationMinutes,
                RefreshTokenExpirationDays = authSettings.Jwt.RefreshTokenExpirationDays,
            }
        };

        return new JwtGenerator(fakeAuthSettings, clock);
    }
}
