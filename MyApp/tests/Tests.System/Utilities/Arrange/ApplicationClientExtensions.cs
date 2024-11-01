using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ConfirmUserRegistration;
using MyApp.Presentation.Interfaces.Http.Commands.Infra.CreateInstance;

namespace MyApp.Tests.System.Utilities.Arrange;

public static class ApplicationClientExtensions
{
    public static async Task<string> ArrangeRandomInstance(this IApplicationAdminClient client)
    {
        var instanceName = RandomData.InstanceName;
        var createInstanceRequest = new CreateInstanceRequest(instanceName, IsCleanupEnabled: true);
        var createInstanceResponse = await client.CreateInstance(createInstanceRequest);
        createInstanceResponse.AssertSuccess();
        return instanceName;
    }

    public static async Task<UserCredentials> ArrangeRandomConfirmedUser(this IApplicationClient client, string instanceName)
    {
        var email = GlobalContext.EmailSettings.Users.Default.EmailAddress;
        var registerRequest = RandomRequests.Register with
        {
            Email = email,
        };
        var registerResponse = await client.Register(registerRequest, instanceName);
        registerResponse.AssertSuccess();

        var code = await GlobalContext.EmailProvider
            .GetUserConfirmationCode(registerRequest.Username);
        var confirmUserRequest = new ConfirmUserRegistrationRequest(code);
        var confirmUserResponse = await client.ConfirmUserRegistration(confirmUserRequest);
        confirmUserResponse.AssertSuccess();

        return (registerRequest.Username, registerRequest.Password, email);
    }
}
