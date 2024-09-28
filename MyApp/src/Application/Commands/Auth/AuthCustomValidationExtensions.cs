using FluentValidation;
using MyApp.Application.Utilities;
using static MyApp.Domain.Auth.User.UserConstants;

namespace MyApp.Application.Commands.Auth;

public static class AuthCustomValidationExtensions
{
    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotNull()
            .MinimumLength(PasswordMinLength)
            .MaximumLength(PasswordMaxLength)
            .NoWhitespaces();
    }

    public static IRuleBuilderOptions<T, string> Username<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotNull()
            .MinimumLength(UsernameMinLength)
            .MaximumLength(UsernameMaxLength)
            .NoWhitespaces();
    }
}
