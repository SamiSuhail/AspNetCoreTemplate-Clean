using MyApp.Application.Infrastructure.Abstractions.Auth;

namespace MyApp.InfrastructureTests.Tests;

public class AuthTests : BaseTest
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IJwtReader _jwtReader;

    public AuthTests(TestFixture fixture) : base(fixture)
    {
        var sp = new ServiceCollection()
            .AddSingleton<IClock, Clock>()
            .AddCustomAuth(Configuration)
            .BuildServiceProvider();

        _jwtGenerator = sp.GetRequiredService<IJwtGenerator>();
        _jwtReader = sp.GetRequiredService<IJwtReader>();
    }

    [Fact]
    public void WhenGeneratingAccessToken_ThenTokenIsValid()
    {
        // Arrange
        const int UserId = 1;
        const string Username = nameof(Username);
        const string Email = nameof(Email);

        // Act
        var token = _jwtGenerator.CreateAccessToken(UserId, Username, Email);

        // Assert
        var user = _jwtReader.ReadAccessToken(token);
        using var _ = new AssertionScope();
        user.Id.Should().Be(UserId);
        user.Username.Should().Be(Username);
        user.Email.Should().Be(Email);
    }

    [Fact]
    public void WhenGeneratingRefreshToken_ThenTokenIsValid()
    {
        // Arrange
        const int UserId = 1;
        const string Username = nameof(Username);
        const string Email = nameof(Email);
        const int Version = 2;

        // Act
        var token = _jwtGenerator.CreateRefreshToken(UserId, Username, Email, Version);

        // Assert
        var refreshToken = _jwtReader.ReadRefreshToken(token);
        refreshToken.Should().NotBeNull();
        using var _ = new AssertionScope();
        refreshToken!.UserId.Should().Be(UserId);
        refreshToken.Username.Should().Be(Username);
        refreshToken.Email.Should().Be(Email);
        refreshToken.Version.Should().Be(Version);
    }
}
