using MediatR;

namespace MyApp.Application.Interfaces.Commands.UserManagement.SignOutOnAllDevices;

public record SignOutOnAllDevicesRequest() : IRequest;
