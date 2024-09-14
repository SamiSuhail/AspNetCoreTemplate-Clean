namespace MyApp.Server.Domain.Auth.User;

public static class UserPasswordHelper
{
    public static string Hash(this string password)
        => BC.EnhancedHashPassword(password);

    public static bool Verify(this string password, string passwordHash)
        => BC.EnhancedVerify(password, passwordHash);
}
