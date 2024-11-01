using FluentAssertions;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Tests.Utilities.Assert;

public static class AssertionExtensions
{
    public static void AssertValidConfirmationCode(this string code)
    {
        code.Should().HaveLength(BaseConfirmationConstants.CodeLength);
        code.All(char.IsNumber).Should().BeTrue();
    }
}
