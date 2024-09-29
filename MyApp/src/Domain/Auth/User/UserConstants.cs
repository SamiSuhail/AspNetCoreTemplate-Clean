namespace MyApp.Domain.Auth.User;

public static class UserConstants
{
    public const int UsernameMinLength = 6;
    public const int UsernameMaxLength = 30;
    public const int PasswordMinLength = 10;
    public const int PasswordMaxLength = 128;
    public const int PasswordHashLength = 60;
    public const int EmailMaxLength = 320;
}
