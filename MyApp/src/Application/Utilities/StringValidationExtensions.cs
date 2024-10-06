using FluentValidation;
using MyApp.Utilities.Strings;

namespace MyApp.Application.Utilities;

public static class StringValidationExtensions
{
    public static IRuleBuilderOptions<T, string> NoWhitespaces<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(p => p.RemoveWhitespace().Equals(p))
            .WithMessage("{PropertyName} cannot contain whitespaces.");
    }
}
