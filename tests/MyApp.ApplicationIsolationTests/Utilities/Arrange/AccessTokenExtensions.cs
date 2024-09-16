using System.Security.Cryptography;
using MyApp.Server.Infrastructure;
using MyApp.Server.Infrastructure.Auth;

namespace MyApp.ApplicationIsolationTests.Utilities.Arrange;

public static class AccessTokenExtensions
{
    public static string ArrangeExpiredAccessToken(this IServiceProvider serviceProvider)
    {
        var authSettings = serviceProvider.GetRequiredService<AuthSettings>();
        var clockMock = new Mock<IClock>();
        var jwtGenerator = new JwtGenerator(authSettings, clockMock.Object);
        clockMock.Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow.AddMinutes(-authSettings.Jwt.AccessTokenExpirationMinutes - 1));
        var accessToken = jwtGenerator.CreateAccessToken(TestUser.Id, TestUser.Username, TestUser.Email);
        return accessToken;
    }

    public static string ArrangeExpiredRefreshToken(this IServiceProvider serviceProvider, int refreshTokenVersion)
    {
        var authSettings = serviceProvider.GetRequiredService<AuthSettings>();
        var clockMock = new Mock<IClock>();
        var jwtGenerator = new JwtGenerator(authSettings, clockMock.Object);
        clockMock.Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow.AddDays(-authSettings.Jwt.RefreshTokenExpirationDays - 1));
        var accessToken = jwtGenerator.CreateRefreshToken(TestUser.Id, TestUser.Username, TestUser.Email, refreshTokenVersion);
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
