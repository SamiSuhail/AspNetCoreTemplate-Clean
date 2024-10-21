using MyApp.Application.Interfaces.Commands.Auth.Login;
using MyApp.Application.Interfaces.Commands.Auth.Registration.Register;
using MyApp.Application.Interfaces.Commands.Infra.CreateInstance;
using MyApp.Application.Interfaces.Commands.UserManagement.EmailUpdate.ChangeEmail;

namespace MyApp.Tests.Utilities.Utilities.Arrange;

public static class RandomRequests
{
    public static RegisterRequest Register => new(RandomData.Email, RandomData.Username, RandomData.Password);
    public static LoginRequest Login => new(RandomData.Username, RandomData.Password, []);

    public static ChangeEmailRequest ChangeEmail => new(RandomData.Email);

    public static CreateInstanceRequest CreateInstance => new(RandomData.InstanceName, IsCleanupEnabled: false);
}
