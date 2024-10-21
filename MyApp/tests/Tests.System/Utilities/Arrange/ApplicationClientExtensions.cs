using MyApp.Application.Interfaces.Commands.Auth.Registration.ConfirmUserRegistration;
using MyApp.Application.Interfaces.Commands.Infra.CreateInstance;
using MyApp.Tests.Utilities.Utilities.Arrange;

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
        var registerRequest = RandomRequests.Register with
        {
            Email = GlobalContext.ServerSettings.AdminAuth.Email,
        };
        var registerResponse = await client.Register(registerRequest, instanceName);
        registerResponse.AssertSuccess();

        // get from email
        var code = await GlobalContext.EmailProvider
            .GetUserConfirmationCode(registerRequest.Email, registerRequest.Username);
        var confirmUserRequest = new ConfirmUserRegistrationRequest(code);
        var confirmUserResponse = await client.ConfirmUserRegistration(confirmUserRequest);
        confirmUserResponse.AssertSuccess();

        return (registerRequest.Username, registerRequest.Password, GlobalContext.ServerSettings.AdminAuth.Email);
    }
}
