namespace MyApp.Tests.Integration.Utilities.Assert;

public static class AssertDateTimeExtensions
{
    public static void ShouldBeNow(this DateTime date, int precisionSeconds = 10)
        => date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(precisionSeconds));
}
