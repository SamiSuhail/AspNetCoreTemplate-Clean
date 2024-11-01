namespace MyApp.Tests.System.Core;

public class AuthenticatedBaseTest(TestFixture fixture) : BaseTest(fixture)
{
    public string InstanceName { get; private set; } = default!;
    public UserCredentials UserCredentials { get; private set; }
    public IApplicationClient AppClient { get; private set; } = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        InstanceName = await AdminAppClient.ArrangeRandomInstance();
        UserCredentials = await UnauthorizedAppClient.ArrangeRandomConfirmedUser(InstanceName);
        var client = await ClientFactory.CreateClientForUser(UserCredentials, InstanceName);
        AppClient = client.ToApplicationClient();
    }
}
