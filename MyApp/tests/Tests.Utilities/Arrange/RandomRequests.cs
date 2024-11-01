using MyApp.Presentation.Interfaces.Http.Commands.Auth.Login;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.Register;
using MyApp.Presentation.Interfaces.Http.Commands.Infra.CreateInstance;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ChangeEmail;

namespace MyApp.Tests.Utilities.Arrange;

public static class RandomRequests
{
    public static RegisterRequest Register => new(RandomData.Email, RandomData.Username, RandomData.Password);
    public static LoginRequest Login => new(RandomData.Username, RandomData.Password, []);

    public static ChangeEmailRequest ChangeEmail => new(RandomData.Email);

    public static CreateInstanceRequest CreateInstance => new(RandomData.InstanceName, IsCleanupEnabled: false);
}
