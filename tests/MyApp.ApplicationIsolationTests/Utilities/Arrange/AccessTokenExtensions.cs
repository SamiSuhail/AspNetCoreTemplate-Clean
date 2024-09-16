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
}
